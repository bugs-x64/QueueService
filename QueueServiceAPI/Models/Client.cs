using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QueueServiceAPI.Models
{
    /// <summary>
    /// Модель клиента.
    /// </summary>
    public partial class Client
    {
        public Client()
        {
            Queues = new HashSet<Queue>();
        }
        
        /// <summary>
        /// Идентификатор записи.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Фамилия Имя Отчество.
        /// </summary>
        public string Fio { get; set; }

        /// <summary>
        /// Дата создания.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Дата модификации.
        /// </summary>
        public DateTime Modified { get; set; }

        /// <summary>
        /// Записи в очередь.
        /// </summary>
        public virtual ICollection<Queue> Queues { get; set; }
    }
}
