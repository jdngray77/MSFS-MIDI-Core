using System;
using System.Linq;
using System.Xml.Serialization;

namespace lib.Configuration
{
    [XmlRoot("Config")]
    public class ConfigFile
    {
        [XmlElement]
        public SimConnectConfig SimConnectConfig { get; set; } = new SimConnectConfig();

        [XmlArray("Devices")]
        [XmlArrayItem("Device")]
        public Device[] Devices { get; set; }

        public override string ToString()
        {
            return $"SimConnectConfig: {SimConnectConfig}," +
                Environment.NewLine +
                $" Devices: " +
                Environment.NewLine +
                $"{String.Join<Device>(Environment.NewLine, Devices)}";
        }
    }
}
