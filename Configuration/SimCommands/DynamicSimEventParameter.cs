using System.Xml.Serialization;

namespace lib.Configuration.SimCommands
{
    public class DynamicSimEventParameter
    {
        [XmlAttribute]
        public byte MinimumInputValue { get; set; } = 1;

        [XmlAttribute]
        public byte MaximumInputValue { get; set; } = 127;

        [XmlAttribute]
        public uint MinimumOutputValue { get; set; }

        [XmlAttribute]
        public uint MaximumOutputValue { get; set; }

        public override string ToString()
        {
            return $"Min: {MinimumInputValue}, Max: {MaximumInputValue}, MinOut: {MinimumOutputValue}, MaxOut: {MaximumOutputValue}";
        }
    }
}
