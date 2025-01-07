using System.Configuration;

public static class ConfigHelper
{
    public static string GetAppSetting(string key)
    {
        return ConfigurationManager.AppSettings[key];
    }
}
