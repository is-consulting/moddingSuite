using moddingSuite.ViewModel.Base;
using System.Reflection;

namespace moddingSuite.ViewModel.About
{
    public class AboutViewModel : ViewModelBase
    {
        public string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString(); 
    }
}