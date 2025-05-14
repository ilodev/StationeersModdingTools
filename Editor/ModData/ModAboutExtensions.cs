using System.IO;
using System.Xml.Serialization;

namespace ilodev.stationeersmods.tools.moddata
{
    public static class ModAboutExtensions
    {
        public static void Save(this ModAbout modAbout, string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ModAbout));
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                serializer.Serialize(stream, modAbout);
            }
        }
    }
}
