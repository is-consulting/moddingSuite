using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moddingSuite.Test
{
    public abstract class BaseTests
    {
        public TestContext TestContext { get; set; }
        protected string RedDragonGameDataPath => TestContext.Properties["reddragonData"] as string;
        protected string AirLandGameDataPath => TestContext.Properties["airlandData"] as string;
        protected string AirLandUserDataPath => TestContext.Properties["airlandUserPath"] as string;
        protected string RedDragonLinuxGameDataPath => TestContext.Properties["linuxReddragonData"] as string;
    }
}
