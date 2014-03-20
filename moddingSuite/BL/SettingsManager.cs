using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using moddingSuite.Model.Settings;

namespace moddingSuite.BL
{
    public static class SettingsManager
    {
        public static readonly string SettingsPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "edataFileManager", "settings.xml");

        //private static Settings LastLoadedSettings { get; set; }

        public static Settings Load()
        {
            var settings = new Settings();

            if (!File.Exists(SettingsPath))
                return settings;

            var serializer = new XmlSerializer(typeof (Settings));
            using (var fs = new FileStream(SettingsPath, FileMode.Open))
            {
                try
                {
                    settings = serializer.Deserialize(fs) as Settings;

                    //LastLoadedSettings = settings;
                }
                catch (InvalidOperationException ex)
                {
                    Trace.TraceError(string.Format("Error while loading Settings: {0}", ex));
                }
            }

            return settings;
        }

        public static bool Save(Settings settingsToSave)
        {
            if (settingsToSave == null)
                return false;

            string dir = Path.GetDirectoryName(SettingsPath);

            if (dir != null && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            try
            {
                using (FileStream fs = File.Create(SettingsPath))
                {
                    var serializer = new XmlSerializer(typeof (Settings));

                    serializer.Serialize(fs, settingsToSave);

                    fs.Flush();
                }
            }
            catch (UnauthorizedAccessException uaex)
            {
                Trace.TraceError("Error while saving settings: {0}", uaex);
                return false;
            }
            catch (IOException ioex)
            {
                Trace.TraceError("Error while saving settings: {0}", ioex);
                return false;
            }

            return true;
        }
    }
}