using Microsoft.VisualStudio.TestTools.UnitTesting;

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
