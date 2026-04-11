using System.Xml.Serialization;

namespace Celeste.BGMPlayer
{
    public class BGMTrack
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("asset")]
        public string Asset { get; set; }
    }
}