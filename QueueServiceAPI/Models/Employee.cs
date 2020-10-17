using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QueueServiceAPI.Models
{
    /// <summary>
    /// Сотрудники.
    /// </summary>
    public partial class Employee
    {
        public Employee()
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
        /// Записи в очередь, к которым относится сотрудник.
        /// </summary>
        public virtual ICollection<Queue> Queues { get; set; }
    }
}
