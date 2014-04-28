using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using moddingSuite.ViewModel.Base;

namespace moddingSuite.Model.Ndfbin.ChangeManager
{
    public class NdfChangeManager
    {
        private readonly ObservableCollection<ChangeEntryBase> _changes = new ObservableCollection<ChangeEntryBase>();

        public NdfChangeManager()
        {
        }

        public ObservableCollection<ChangeEntryBase> Changes
        {
            get { return _changes; }
        }

        public bool HasChanges
        {
            get { return Changes.Count > 0; }
        }

        public void AddChange(ChangeEntryBase change)
        {
            Changes.Add(change);
        }
    }
}