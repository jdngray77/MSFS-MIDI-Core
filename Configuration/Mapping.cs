using lib.Configuration.SimCommands;
using System.Xml.Serialization;

namespace lib.Configuration
{
    public class Mapping
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public int MidiTrigger { get; set; }

        [XmlElement]
        public SimCommand Result { get; set; }

        public override string ToString()
        {
            return $"[{Name}] {MidiTrigger} -> {Result}";
        }
    }
}
    