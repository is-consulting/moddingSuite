using System.Collections.Generic;
using System.Linq;

namespace moddingSuite.ViewModel.Ndf
{
    public class NdfScriptableClassList
    {
        private readonly IList<NdfClassViewModel> Classes;

        public NdfScriptableClassList(IList<NdfClassViewModel> classes)
        {
            Classes = classes;
        }

        /// <summary>
        /// Easy class lookup by name for scripts.
        /// </summary>
        /// <returns></returns>
        public NdfClassViewModel this[string name] => Classes.FirstOrDefault(cls => cls.Name == name);
    }
}
