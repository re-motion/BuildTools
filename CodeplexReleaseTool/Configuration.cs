using System;
using System.Configuration;

namespace CodeplexReleaseTool
{
  public class Configuration : ConfigurationSection
  {
    private static Configuration _current;

    public static Configuration Current
    {
      get
      {
        if (_current == null)
          _current = (Configuration) ConfigurationManager.GetSection ("codeplexReleaseToolSettings");
        return _current;
      }
    }

    [ConfigurationProperty ("url", IsRequired = true)]
    public string Url
    {
      get { return (string) this["url"]; }
    }
   
  }
}