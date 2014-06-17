using System.Collections.Generic;

namespace moddingSuite.Model.Edata
{
    /// <summary>
    ///     struct dictGroupEntry {
    ///     DWORD groupId;
    ///     DWORD entrySize;
    ///     zstring name;
    ///     };
    /// </summary>
    public class EdataDir : EdataEntity
    {
        private readonly List<EdataDir> _children = new List<EdataDir>();
        private readonly List<EdataContentFile> _files = new List<EdataContentFile>();
        private EdataDir _parent;
        private int _subTreeSize;

        public EdataDir()
            : base(string.Empty)
        {
        }

        public EdataDir(string name, EdataDir parent = null)
            : base(name)
        {
            Parent = parent;
            if (parent != null)
                parent.Children.Add(this);
        }

        public int SubTreeSize
        {
            get { return _subTreeSize; }
            set
            {
                _subTreeSize = value;
                OnPropertyChanged("GroupId");
            }
        }

        public EdataDir Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
                OnPropertyChanged("Parent");
            }
        }

        public List<EdataDir> Children
        {
            get { return _children; }
        }

        public List<EdataContentFile> Files
        {
            get { return _files; }
        }
    }
}