using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using moddingSuite.BL;
using moddingSuite.BL.DDS;
using moddingSuite.Model.Edata;
using moddingSuite.Settings;
using moddingSuite.View.Common;
using moddingSuite.View.DialogProvider;
using moddingSuite.View.Ndfbin;
using moddingSuite.ViewModel.About;
using moddingSuite.ViewModel.Base;
using moddingSuite.ViewModel.Media;
using moddingSuite.ViewModel.Ndf;
using moddingSuite.ViewModel.Trad;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace moddingSuite.ViewModel.Edata
{
    public class EdataManagerViewModel : ViewModelBase
    {
        private readonly ObservableCollection<EdataFileViewModel> _openFiles =
            new ObservableCollection<EdataFileViewModel>();

        public EdataManagerViewModel()
        {
            InitializeCommands();

            Settings.Settings settings = SettingsManager.Load();

            foreach (string file in settings.LastOpenedFiles)
            {
                var fileInfo = new FileInfo(file);

                if (fileInfo.Exists)
                    AddFile(fileInfo.FullName);
            }

            //if (settings.LastHighlightedFileIndex <= OpenFiles.Count)
            //    CollectionViewSource.GetDefaultView(OpenFiles).MoveCurrentTo(OpenFiles.ElementAt(settings.LastHighlightedFileIndex));
            //else
            CollectionViewSource.GetDefaultView(OpenFiles).MoveCurrentToFirst();

            OpenFiles.CollectionChanged += OpenFilesCollectionChanged;
        }

        public ICommand ExportNdfCommand { get; set; }
        public ICommand ExportRawCommand { get; set; }
        public ICommand ReplaceRawCommand { get; set; }
        public ICommand ExportTextureCommand { get; set; }
        public ICommand ReplaceTextureCommand { get; set; }
        public ICommand OpenFileCommand { get; set; }
        public ICommand CloseFileCommand { get; set; }
        public ICommand ChangeExportPathCommand { get; set; }
        public ICommand ChangeWargamePathCommand { get; set; }
        public ICommand ViewContentCommand { get; set; }
        public ICommand ViewTradFileCommand { get; set; }
        public ICommand PlayMovieCommand { get; set; }
        public ICommand AboutUsCommand { get; set; }

        public ObservableCollection<EdataFileViewModel> OpenFiles
        {
            get { return _openFiles; }
        }

        protected void OpenFilesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Settings.Settings set = SettingsManager.Load();
            set.LastOpenedFiles.Clear();
            set.LastOpenedFiles.AddRange(OpenFiles.Select(x => x.LoadedFile).ToList());
            SettingsManager.Save(set);
        }

        public void AddFile(string path)
        {
            var vm = new EdataFileViewModel();

            vm.LoadFile(path);

            OpenFiles.Add(vm);
            CollectionViewSource.GetDefaultView(OpenFiles).MoveCurrentTo(vm);
        }

        public void CloseFile(EdataFileViewModel vm)
        {
            if (!OpenFiles.Contains(vm))
                return;

            OpenFiles.Remove(vm);
        }

        protected void InitializeCommands()
        {
            OpenFileCommand = new ActionCommand(OpenFileExecute);
            CloseFileCommand = new ActionCommand(CloseFileExecute);

            ChangeExportPathCommand = new ActionCommand(ChangeExportPathExecute);
            ChangeWargamePathCommand = new ActionCommand(ChangeWargamePathExecute);

            ExportNdfCommand = new ActionCommand(ExportNdfExecute, () => IsOfType(EdataFileType.Ndfbin));
            ExportRawCommand = new ActionCommand(ExportRawExecute);
            ReplaceRawCommand = new ActionCommand(ReplaceRawExecute);
            ExportTextureCommand = new ActionCommand(ExportTextureExecute);
            ReplaceTextureCommand = new ActionCommand(ReplaceTextureExecute);
            PlayMovieCommand = new ActionCommand(PlayMovieExecute);

            AboutUsCommand = new ActionCommand(AboutUsExecute);

            ViewTradFileCommand = new ActionCommand(ViewTradFileExecute, () => IsOfType(EdataFileType.Dictionary));
            ViewContentCommand = new ActionCommand(ViewContentExecute, () => IsOfType(EdataFileType.Ndfbin));
        }

        private void ReplaceRawExecute(object obj)
        {
            var vm = CollectionViewSource.GetDefaultView(OpenFiles).CurrentItem as EdataFileViewModel;

            if (vm == null)
                return;

            var file = vm.FilesCollectionView.CurrentItem as EdataContentFile;

            if (file == null)
                return;
            
            Settings.Settings settings = SettingsManager.Load();

            var openfDlg = new OpenFileDialog
            {
                //DefaultExt = ".*",
                Multiselect = false,
                Filter = "All files (*.*)|*.*"
            };

            if (File.Exists(settings.LastOpenFolder))
                openfDlg.InitialDirectory = settings.LastOpenFolder;

            if (openfDlg.ShowDialog().Value)
            {
                settings.LastOpenFolder = new FileInfo(openfDlg.FileName).DirectoryName;
                SettingsManager.Save(settings);

                byte[] replacefile = File.ReadAllBytes(openfDlg.FileName);

                vm.EdataManager.ReplaceFile(file, replacefile);
                vm.LoadFile(vm.LoadedFile);
            }
        }

        protected void ReplaceTextureExecute(object obj)
        {
            var vm = CollectionViewSource.GetDefaultView(OpenFiles).CurrentItem as EdataFileViewModel;

            if (vm == null)
                return;

            var tgvFile = vm.FilesCollectionView.CurrentItem as EdataContentFile;

            if (tgvFile == null)
                return;

            var mgr = new TgvManager();

            var data = tgvFile.Manager.GetRawData(tgvFile);

            //Util.Utils.SaveDebug("testooooor", data);

            var tgv = mgr.ReadFile(data);


            Settings.Settings settings = SettingsManager.Load();

            var openfDlg = new OpenFileDialog
            {
                DefaultExt = ".dds",
                Multiselect = false,
                Filter = "DDS files (.dds)|*.dds"
            };

            if (File.Exists(settings.LastOpenFolder))
                openfDlg.InitialDirectory = settings.LastOpenFolder;


            if (openfDlg.ShowDialog().Value)
            {
                settings.LastOpenFolder = new FileInfo(openfDlg.FileName).DirectoryName;
                SettingsManager.Save(settings);

                byte[] oldDds = File.ReadAllBytes(openfDlg.FileName);

                var reader = new DDSReader();
                var newDds = reader.ReadDDS(oldDds);

                var newTgv = mgr.CreateTgv(newDds, tgv.SourceChecksum, tgv.IsCompressed);

                //Util.Utils.SaveDebug(string.Format("created_tgv_{0}", DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ff")), newTgv);

                vm.EdataManager.ReplaceFile(tgvFile, newTgv);
                vm.LoadFile(vm.LoadedFile);
            }
        }

        protected void ExportTextureExecute(object obj)
        {
            var vm = CollectionViewSource.GetDefaultView(OpenFiles).CurrentItem as EdataFileViewModel;

            if (vm == null)
                return;

            var tgvFile = vm.FilesCollectionView.CurrentItem as EdataContentFile;

            if (tgvFile == null)
                return;

            Settings.Settings settings = SettingsManager.Load();

            var mgr = new TgvManager();
            var tgv = mgr.ReadFile(tgvFile.Manager.GetRawData(tgvFile));

            var writer = new DDSWriter();

            byte[] content = writer.CreateDDSFile(tgv);

            var f = new FileInfo(tgvFile.Path);

            using (var fs = new FileStream(Path.Combine(settings.SavePath, f.Name + ".dds"), FileMode.OpenOrCreate))
            {
                fs.Write(content, 0, content.Length);
                fs.Flush();
            }
        }

        protected bool IsOfType(EdataFileType type)
        {
            var vm = CollectionViewSource.GetDefaultView(OpenFiles).CurrentItem as EdataFileViewModel;

            if (vm == null)
                return false;

            var ndf = vm.FilesCollectionView.CurrentItem as EdataContentFile;

            if (ndf == null)
                return false;

            return ndf.FileType == type;
        }

        protected void ViewTradFileExecute(object obj)
        {
            var vm = CollectionViewSource.GetDefaultView(OpenFiles).CurrentItem as EdataFileViewModel;

            if (vm == null)
                return;

            var ndf = vm.FilesCollectionView.CurrentItem as EdataContentFile;

            if (ndf == null)
                return;

            var tradVm = new TradFileViewModel(ndf, vm);

            DialogProvider.ProvideView(tradVm, this);
        }

        protected void ViewContentExecute(object obj)
        {
            var vm = CollectionViewSource.GetDefaultView(OpenFiles).CurrentItem as EdataFileViewModel;

            if (vm == null)
                return;

            var ndf = vm.FilesCollectionView.CurrentItem as EdataContentFile;

            if (ndf == null)
                return;

            var detailsVm = new NdfEditorMainViewModel(ndf, vm);

            DialogProvider.ProvideView(detailsVm, this);
        }

        protected void ExportNdfExecute(object obj)
        {
            var vm = CollectionViewSource.GetDefaultView(OpenFiles).CurrentItem as EdataFileViewModel;

            if (vm == null)
                return;

            var ndf = vm.FilesCollectionView.CurrentItem as EdataContentFile;

            if (ndf == null)
                return;

            Settings.Settings settings = SettingsManager.Load();

            byte[] content = new NdfbinManager(ndf.Manager.GetRawData(ndf)).GetContent();

            var f = new FileInfo(ndf.Path);

            using (var fs = new FileStream(Path.Combine(settings.SavePath, f.Name), FileMode.OpenOrCreate))
            {
                fs.Write(content, 0, content.Length);
                fs.Flush();
            }
        }

        protected void ExportRawExecute(object obj)
        {
            var vm = CollectionViewSource.GetDefaultView(OpenFiles).CurrentItem as EdataFileViewModel;

            if (vm == null)
                return;

            var ndf = vm.FilesCollectionView.CurrentItem as EdataContentFile;

            if (ndf == null)
                return;

            Settings.Settings settings = SettingsManager.Load();

            byte[] buffer = vm.EdataManager.GetRawData(ndf);

            var f = new FileInfo(ndf.Path);

            using (var fs = new FileStream(Path.Combine(settings.SavePath, f.Name), FileMode.OpenOrCreate))
            {
                fs.Write(buffer, 0, buffer.Length);
                fs.Flush();
            }
        }

        protected void ExportAll()
        {
            //foreach (var file in Files)
            //{
            //    var f = new FileInfo(file.Path);

            //    var dirToCreate = Path.Combine("c:\\temp\\", f.DirectoryName);

            //    if (!Directory.Exists(dirToCreate))
            //        Directory.CreateDirectory(dirToCreate);

            //    var buffer = NdfManager.GetRawData(file);
            //    using (var fs = new FileStream(Path.Combine(dirToCreate, f.Name), FileMode.OpenOrCreate))
            //    {
            //        fs.Write(buffer, 0, buffer.Length);
            //        fs.Flush();
            //    }

            //}
        }

        protected void ChangeExportPathExecute(object obj)
        {
            Settings.Settings settings = SettingsManager.Load();

            var folderDlg = new FolderBrowserDialog
                                {
                                    SelectedPath = settings.SavePath,
                                    //RootFolder = Environment.SpecialFolder.MyComputer,
                                    ShowNewFolderButton = true,
                                };

            if (folderDlg.ShowDialog() == DialogResult.OK)
            {
                settings.SavePath = folderDlg.SelectedPath;
                SettingsManager.Save(settings);
            }
        }

        protected void ChangeWargamePathExecute(object obj)
        {
            Settings.Settings settings = SettingsManager.Load();

            var folderDlg = new FolderBrowserDialog
            {
                SelectedPath = settings.WargamePath,
                //RootFolder = Environment.SpecialFolder.MyComputer,
                ShowNewFolderButton = true,
            };

            if (folderDlg.ShowDialog() == DialogResult.OK)
            {
                settings.WargamePath = folderDlg.SelectedPath;
                SettingsManager.Save(settings);
            }
        }

        protected void OpenFileExecute(object obj)
        {
            Settings.Settings settings = SettingsManager.Load();

            var openfDlg = new OpenFileDialog
                               {
                                   DefaultExt = ".dat",
                                   Multiselect = true,
                                   Filter = "Edat (.dat)|*.dat|All Files|*.*"
                               };

            if (File.Exists(settings.LastOpenFolder))
                openfDlg.InitialDirectory = settings.LastOpenFolder;


            if (openfDlg.ShowDialog().Value)
            {
                settings.LastOpenFolder = new FileInfo(openfDlg.FileName).DirectoryName;
                SettingsManager.Save(settings);
                foreach (string fileName in openfDlg.FileNames)
                {
                    HandleNewFile(fileName);
                }
            }
        }

        private void HandleNewFile(string fileName)
        {
            byte[] headerBuffer;

            var type = EdataFileType.Unknown;

            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                headerBuffer = new byte[12];
                fs.Read(headerBuffer, 0, headerBuffer.Length);

                type = EdataManager.GetFileTypeFromHeaderData(headerBuffer);


                if (type == EdataFileType.Ndfbin)
                {
                    var buffer = new byte[fs.Length];

                    fs.Seek(0, SeekOrigin.Begin);
                    fs.Read(buffer, 0, buffer.Length);

                    var detailsVm = new NdfEditorMainViewModel(buffer);

                    var view = new NdfbinView { DataContext = detailsVm };

                    view.Show();
                }
            }

            if (type == EdataFileType.Package)
                AddFile(fileName);
        }

        protected void CloseFileExecute(object obj)
        {
            CloseFile(CollectionViewSource.GetDefaultView(OpenFiles).CurrentItem as EdataFileViewModel);
        }

        protected void PlayMovieExecute(object obj)
        {
            const string name = "temp.wmv";
            var vm = CollectionViewSource.GetDefaultView(OpenFiles).CurrentItem as EdataFileViewModel;

            if (vm == null)
                return;

            var ndf = vm.FilesCollectionView.CurrentItem as EdataContentFile;

            if (ndf == null)
                return;

            Settings.Settings settings = SettingsManager.Load();

            byte[] buffer = vm.EdataManager.GetRawData(ndf);

            //var f = new FileInfo(ndf.Path);

            using (var fs = new FileStream(Path.Combine(settings.SavePath, name), FileMode.OpenOrCreate))
            {
                fs.Write(buffer, 0, buffer.Length);
                fs.Flush();
            }

            var detailsVm = new MoviePlaybackViewModel(Path.Combine(settings.SavePath, name));

            var view = new MoviePlaybackView { DataContext = detailsVm };

            view.Show();
        }

        protected void AboutUsExecute(object obj)
        {
            DialogProvider.ProvideView(new AboutViewModel(), this);
        }
    }
}