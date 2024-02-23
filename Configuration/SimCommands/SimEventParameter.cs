using System.Xml.Serialization;

namespace lib.Configuration.SimCommands
{
    public class SimEventParameter
    {
        [XmlElement]
        public DynamicSimEventParameter DynamicParameter { get; set; }

        [XmlElement]
        public StaticSimEventParameter StaticParameter { get; set; }

        public override string ToString()
        {
            if (DynamicParameter != null)
            {
                return $"Dynamic param : {DynamicParameter}";
            }
            else
            {
                return $"Static param : {StaticParameter}";
            }
        }
    }
}
