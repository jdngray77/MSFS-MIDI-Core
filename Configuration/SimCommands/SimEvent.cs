using System.Xml.Serialization;

namespace lib.Configuration.SimCommands
{
    public class SimEvent
    {
        [XmlAttribute]
        public string Command { get; set; }

        [XmlElement]
        public SimEventParameter Parameter { get; set; }

        public override string ToString()
        {
            if (Parameter == null)
            {
                return $"{Command} (w/ no parameter)";
            }

            return $"{Command} ({Parameter})";
        }
    }
}
