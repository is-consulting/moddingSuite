using moddingSuite.ViewModel.Base;

namespace moddingSuite.ViewModel.Media
{
    public class MoviePlaybackViewModel : ViewModelBase
    {
        private string _file;

        public MoviePlaybackViewModel(string file)
        {
            _file = file;
        }

        public string File
        {
            get { return _file; }
            set
            {
                _file = value;
                OnPropertyChanged(() => File);
            }
        }
    }
}