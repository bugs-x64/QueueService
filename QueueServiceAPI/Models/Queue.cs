using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueueServiceAPI.Models
{
    /// <summary>
    /// Модель записи в очередь.
    /// </summary>
    public class Queue
    {
        /// <summary>
        /// Идентификатор записи.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Конкурирующая очередь.
        /// </summary>
        public bool Competing { get; set; }

        /// <summary>
        /// Запись отработана.
        /// </summary>
        public bool Handled { get; set; }

        /// <summary>
        /// Идентификатор клиента.
        /// </summary>
        [ForeignKey("Client")]
        public int Clientid { get; set; }

        /// <summary>
        /// Идентификатор сотрудника.
        /// </summary>
        [ForeignKey("Employee")]
        public int Employeeid { get; set; }

        /// <summary>
        /// Дата создания записи.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Клиент.
        /// </summary>
        public virtual Client Client { get; set; }

        /// <summary>
        /// Сотрудник.
        /// </summary>
        public virtual Employee Employee { get; set; }

        public DateTime Modified { get; set; }
    }
}
