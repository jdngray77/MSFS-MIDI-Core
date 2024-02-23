using System.Xml.Serialization;

namespace lib.Configuration
{
    public class SimConnectConfig
    {
        [XmlAttribute]
        public string ClientName { get; set; }

        [XmlAttribute]
        public string Location { get; set; } = "localhost";

        [XmlAttribute]
        public uint Port { get; set; } = 500;

        public override string ToString()
        {
            return $"ClientName: {ClientName}, Location: {Location}, Port: {Port}";
        }
    }
}