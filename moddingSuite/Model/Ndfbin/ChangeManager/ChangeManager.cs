using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using moddingSuite.ViewModel.Base;

namespace moddingSuite.Model.Ndfbin.ChangeManager
{
    public class ChangeManager
    {
        private readonly ObservableCollection<ChangeEntry> _changes = new ObservableCollection<ChangeEntry>();

        public ChangeManager()
        {
            RevertChange = new ActionCommand(ReverChangeExeCute, () => false);
        }

        public ObservableCollection<ChangeEntry> Changes
        {
            get { return _changes; }
        }

        public ICommand RevertChange { get; protected set; }

        public bool HasChanges
        {
            get { return Changes.Count > 0; }
        }

        protected void ReverChangeExeCute(object obj)
        {
            ICollectionView cv = CollectionViewSource.GetDefaultView(Changes);

            var item = cv.CurrentItem as ChangeEntry;

            if (item == null)
                return;

            //Changes.Remove(item);
            //item.ChangedValue.Value = NdfTypeManager.GetValue(item.OldValue, item.ChangedValue.Value.Type, item.ChangedValue.Manager, item.ChangedValue.Value.OffSet);
        }
    }
}