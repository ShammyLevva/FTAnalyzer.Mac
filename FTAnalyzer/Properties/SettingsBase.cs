using System;
namespace FTAnalyzer.Properties
{
	public static class SettingsBase
	{
		public static void Save()
        { 
        }

        public static object Load(object settingsClass)
        {
            return settingsClass;
        }
	}
}
