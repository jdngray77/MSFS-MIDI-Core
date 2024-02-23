using System.Xml.Serialization;

namespace lib.Configuration.SimCommands
{
    public class SimCommand
    {
        [XmlElement]
        public SimEvent SimEvent { get; set; }

        // TODO other types of commands.

        public override string ToString()
        {
            return $"{SimEvent}";
        }
    }
}
