using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace QueueServiceAdmin
{
    class Settings
    {
        public string ServerAddress { get; private set; }
        public int SleepTime { get; private set; }

        public void Load()
        {
            var settings = XDocument.Load("settings.xml").Root;
            ServerAddress = settings.Element("server").Attribute("address").Value;
            SleepTime = Convert.ToInt32(settings.Element("sleep").Attribute("seconds").Value);
        }
    }
}
