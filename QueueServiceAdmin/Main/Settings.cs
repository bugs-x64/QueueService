using System.Xml.Linq;

namespace QueueServiceAdmin.Main
{
    internal class Settings
    {
        /// <summary>
        /// Адрес сервера с данными
        /// </summary>
        public string ServerAddress { get; private set; }

        /// <summary>
        /// Загрузка настроек из XML файла
        /// </summary>
        public void LoadXML()
        {
            XElement settings = XDocument.Load("settings.xml").Root;
            ServerAddress = settings.Element("server").Attribute("address").Value;
        }
    }
}
