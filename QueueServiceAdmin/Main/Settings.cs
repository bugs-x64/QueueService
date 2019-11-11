using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace QueueServiceAdmin.Main
{
    class Settings
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
            var settings = XDocument.Load("settings.xml").Root;
            ServerAddress = settings.Element("server").Attribute("address").Value;
        }
    }
}
