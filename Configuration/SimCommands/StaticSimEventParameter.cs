using System.Xml.Serialization;

namespace lib.Configuration.SimCommands
{
    public class StaticSimEventParameter
    {
        [XmlAttribute]
        public uint Value { get; set; }

        public override string ToString()
        {
            return $"{Value}";
        }
    }
}
