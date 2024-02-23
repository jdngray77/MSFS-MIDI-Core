using System;
using System.Xml.Serialization;

namespace lib.Configuration
{
    [XmlRoot("Device")]
    public class Device
    {

        [XmlAttribute("Name")]
        public string ProductName { get; set; }

        [XmlArray("Mappings")]
        [XmlArrayItem("Mapping")]
        public Mapping[] Mappings { get; set; }

        public override string ToString()
        {
            return $"Device: {ProductName}," +
                Environment.NewLine +
                $" Mappings:" +
                Environment.NewLine +
                $" {String.Join<Mapping>(Environment.NewLine, Mappings)}";
        }
    }
}
