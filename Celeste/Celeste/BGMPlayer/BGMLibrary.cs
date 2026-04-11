using System.Collections.Generic;
using System.Xml.Serialization;

namespace Celeste.BGMPlayer
{
    [XmlRoot("BGMLibrary")]
    public class BGMLibrary
    {
        [XmlElement("Track")]
        public List<BGMTrack> Tracks { get; set; } = new();
    }
}