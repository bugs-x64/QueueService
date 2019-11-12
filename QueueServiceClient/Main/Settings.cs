using System.Xml.Linq;

namespace QueueServiceClient.Main
{
    internal class Settings
    {
        public string ServerAddress { get; private set; }

        public void Load()
        {
            XElement settings = XDocument.Load("settings.xml").Root;
            ServerAddress = settings.Element("server").Attribute("address").Value;
        }
    }
}
