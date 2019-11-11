using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QueueServiceAdmin.Main
{
    /// <summary>
    /// Класс содержит методы взятия клиентов из очереди
    /// </summary>
    public static class QueueWebMethods
    {
        /// <summary>
        /// метод отправляет запрос на сервер, чтобы забрать клиента из неконкурирующей очереди
        /// </summary>
        /// <param name="address">Адрес сервера</param>
        /// <param name="emplname">ФИО сотрудника</param>
        public static async void GetNextAsync(string address, string emplname)
        {
            using (var client = new MyWebClient())
            {
                var response = await client.Post(
                                remoteAddress: new Uri($"{address}/api/queues/next"),
                                httpContent: MyWebClient.BuildJSONContent($"1"));
                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show("\r\nПроизошла ошибка, повторите попытку позже!", $"Сотрудник {emplname}");
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var queueRecord = JsonConvert.DeserializeObject<QueueRecord>(responseContent);
                    MessageBox.Show($"Вы успешно забрали клиента \"{queueRecord.Fio}\" из неконкурирующей очереди", $"Сотрудник {emplname}");
                }
            }
        }

        /// <summary>
        /// метод отправляет запрос на сервер, чтобы забрать клиента из конкурирующей очереди
        /// </summary>
        /// <param name="address">Адрес сервера</param>
        /// <param name="emplname">ФИО сотрудника</param>
        /// <param name="queueRecord">Объект представляющий информацию о взятом клиенте</param>
        public static async void GetConcurAsync(string address, string emplname, QueueRecord queueRecord)
        {
            using (var client = new MyWebClient())
            {
                var response = await client.Post(
                                remoteAddress: new Uri($"{address}/api/queues/{queueRecord.RecId}"),
                                httpContent: MyWebClient.BuildJSONContent($"1"));
                if (!response.IsSuccessStatusCode)
                    MessageBox.Show("\r\nПроизошла ошибка, повторите попытку позже!", $"Сотрудник {emplname}");
                else
                    MessageBox.Show($"Вы успешно забрали клиента \"{queueRecord.Fio}\" из конкурирующей очереди", $"Сотрудник {emplname}");
            }
        }
    }
}
