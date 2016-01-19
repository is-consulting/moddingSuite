using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;
using moddingSuite.BL;
using moddingSuite.Model.Edata;
using moddingSuite.Model.Trad;
using moddingSuite.Util;
using moddingSuite.ViewModel.Base;
using moddingSuite.ViewModel.Edata;

namespace moddingSuite.ViewModel.Trad
{
    public class TradFileViewModel : ViewModelBase
    {
        private ObservableCollection<TradEntry> _entries;
        private ICollectionView _entriesCollectionView;
        private string _filterExpression = string.Empty;
        private string _titleText;

        public TradFileViewModel(EdataContentFile owner, EdataFileViewModel contentFile)
        {
            SaveCommand = new ActionCommand(SaveCommandExecute);
            CreateHashCommand = new ActionCommand(CreateHashExecute, CreateHashCanExecute);
            AddEntryCommand = new ActionCommand(AddEntryExecute);
            RemoveEntryCommand = new ActionCommand(RemoveEntryExecute);

            OwnerFile = owner;
            OwnerVm = contentFile;

            Manager = new TradManager(OwnerVm.EdataManager.GetRawData(OwnerFile));

            Entries = Manager.Entries;

            TitleText = string.Format("Dictionary editor [{0}]", OwnerFile.Path);
        }

        public TradManager Manager { get; protected set; }

        public EdataFileViewModel OwnerVm { get; protected set; }
        public EdataContentFile OwnerFile { get; protected set; }

        public ICommand SaveCommand { get; protected set; }
        public ICommand CreateHashCommand { get; protected set; }
        public ICommand AddEntryCommand { get; set; }
        public ICommand RemoveEntryCommand { get; set; }


        public ObservableCollection<TradEntry> Entries
        {
            get { return _entries; }
            set
            {
                _entries = value;
                OnPropertyChanged(() => Entries);
            }
        }

        public ICollectionView EntriesCollectionView
        {
            get
            {
                if (_entriesCollectionView == null)
                {
                    _entriesCollectionView = CollectionViewSource.GetDefaultView(Entries);
                    _entriesCollectionView.Filter = FilterEntries;
                }

                return _entriesCollectionView;
            }
        }

        public string FilterExpression
        {
            get { return _filterExpression; }
            set
            {
                _filterExpression = value;
                OnPropertyChanged(() => FilterExpression);
                EntriesCollectionView.Refresh();
            }
        }

        public string TitleText
        {
            get { return _titleText; }
            set
            {
                _titleText = value;
                OnPropertyChanged(() => TitleText);
            }
        }

        private bool CreateHashCanExecute()
        {
            var item = EntriesCollectionView.CurrentItem as TradEntry;

            if (item == null || !item.UserCreated)
                return false;

            return true;
        }

        private void CreateHashExecute(object obj)
        {
            var item = EntriesCollectionView.CurrentItem as TradEntry;

            if (item == null || !item.UserCreated)
                return;

            CalculateHash(item);
        }

        public static void CalculateHash(TradEntry item)
        {
            const string characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            var allowedChars = characters.ToCharArray().ToList();

            var wordToHash = new StringBuilder();

            foreach (char t in item.Content)
                if (allowedChars.Contains(t))
                    wordToHash.Append(t);

            var word = wordToHash.ToString();

            item.Hash = Utils.CreateLocalisationHash(word, word.Length);
        }

        private void SaveCommandExecute(object obj)
        {
            byte[] newFile = Manager.BuildTradFile();

            OwnerVm.EdataManager.ReplaceFile(OwnerFile, newFile);

            OwnerVm.LoadFile(OwnerVm.LoadedFile);

            EdataContentFile newOwen = OwnerVm.EdataManager.Files.Single(x => x.Path == OwnerFile.Path);

            OwnerFile = newOwen;
        }

        private bool FilterEntries(object obj)
        {
            var entr = obj as TradEntry;

            if (entr == null)
                return false;

            if (FilterExpression.Length < 2)
                return true;

            if (entr.Content.ToLower().Contains(FilterExpression.ToLower()) ||
                entr.HashView.ToLower().Contains(FilterExpression.ToLower()))
                return true;

            return false;
        }

        private void RemoveEntryExecute(object obj)
        {
            var entry = EntriesCollectionView.CurrentItem as TradEntry;

            if (entry == null)
                return;

            Entries.Remove(entry);
        }

        private void AddEntryExecute(object obj)
        {
            var newEntry = new TradEntry { Content = "New entry", UserCreated = true };

            //CalculateHash(newEntry);

            int newIndex;

            if (Entries.Count > 1)
                newIndex = Entries.Count - 1;
            else
                newIndex = Entries.Count;

            Entries.Insert(newIndex, newEntry);
        }
    }
}