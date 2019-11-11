using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueServiceAPI.Models
{
    /// <summary>
    /// Класс представляет запись в очереди
    /// </summary>
    public class QueueRecord
    {
        /// <summary>
        /// Номер записи
        /// </summary>
        public int RecId { get; set; }
        /// <summary>
        /// ФИО клиента
        /// </summary>
        public string Fio { get; set; }
        /// <summary>
        /// Конкурирующая очередь
        /// </summary>
        public bool Competing { get; set; }
    }
}
