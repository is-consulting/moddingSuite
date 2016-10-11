using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using moddingSuite.BL.Scenario;
using moddingSuite.Model.Edata;
using moddingSuite.Model.Mesh;
using moddingSuite.Model.Scenario;
using moddingSuite.View.DialogProvider;
using moddingSuite.ViewModel.Base;
using moddingSuite.ViewModel.Edata;
using moddingSuite.ViewModel.Ndf;
using ZoneEditor;
using moddingSuite.ZoneEditor;

namespace moddingSuite.ViewModel.Scenario
{
    public class ScenarioEditorViewModel: ViewModelBase
    {
        private ScenarioFile _scenarioFile;
        private string _statusText;

        public ICommand EditGameModeLogicCommand { get; protected  set; }
        public ICommand ZoneEditorCommand { get; protected set; }
        public ICommand SaveCommand { get; protected set; }

        protected EdataFileViewModel EdataFileViewModel { get; set; }
        protected EdataContentFile OwnerFile { get; set; }
        private ZoneEditorData zoneEditor;
        public string StatusText
        {
            get { return _statusText; }
            set
            {
                _statusText = value;
                OnPropertyChanged(() => StatusText);
            }
        }

        public ScenarioFile ScenarioFile
        {
            get { return _scenarioFile; }
            set { _scenarioFile = value; OnPropertyChanged("ScenarioFile"); }
        }


        public ScenarioEditorViewModel(EdataContentFile file, EdataFileViewModel ownerVm)
        {
            OwnerFile = file;
            EdataFileViewModel = ownerVm;

            var reader = new ScenarioReader();
            ScenarioFile = reader.Read(ownerVm.EdataManager.GetRawData(file));

            EditGameModeLogicCommand = new ActionCommand(EditGameModeLogicExecute);
            ZoneEditorCommand = new ActionCommand(ZoneEditorExecute);
            SaveCommand = new ActionCommand(SaveExecute);
        }
        private void ZoneEditorExecute(object obj)
        {
            zoneEditor=new ZoneEditorData(ScenarioFile,OwnerFile.Path);
            Console.WriteLine("Launch Editor");
        }
        private void EditGameModeLogicExecute(object obj)
        {
            var ndfEditor = new NdfEditorMainViewModel(ScenarioFile.NdfBinary);

            DialogProvider.ProvideView(ndfEditor, this);
        }

        private void SaveExecute(object obj)
        {
            if (zoneEditor != null)
            {
                zoneEditor.Save();
            }
            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
            Action<string> report = msg => StatusText = msg;

            var s = new Task(() =>
                {
                    try
                    {
                        dispatcher.Invoke(() => IsUIBusy = true);
                        dispatcher.Invoke(report, string.Format("Saving back changes..."));

                        var writer = new ScenarioWriter();
                        byte[] newFile = writer.Write(ScenarioFile);
                        dispatcher.Invoke(report, string.Format("Recompiling of {0} finished! ", EdataFileViewModel.EdataManager.FilePath));

                        EdataFileViewModel.EdataManager.ReplaceFile(OwnerFile, newFile);
                        dispatcher.Invoke(report, "Replacing new File in edata finished!");

                        EdataFileViewModel.LoadFile(EdataFileViewModel.LoadedFile);

                        EdataContentFile newOwen = EdataFileViewModel.EdataManager.Files.Single(x => x.Path == OwnerFile.Path);

                        OwnerFile = newOwen;
                        dispatcher.Invoke(report, string.Format("Saving of changes finished! {0}", EdataFileViewModel.EdataManager.FilePath));
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError(string.Format("Error while saving scenario file: {0}", ex));
                        dispatcher.Invoke(report, "Saving interrupted - Did you start Wargame before I was ready?");
                    }
                    finally
                    {
                        dispatcher.Invoke(() => IsUIBusy = false);
                    }
                });
            s.Start();
        }
    }
}
