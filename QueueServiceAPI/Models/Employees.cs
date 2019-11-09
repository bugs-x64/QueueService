using System;
using System.Collections.Generic;

namespace QueueServiceAPI
{
    public partial class Employees
    {
        public Employees()
        {
            Queues = new HashSet<Queues>();
        }

        public int Id { get; set; }
        public string Fio { get; set; }
        public DateTime Created { get; set; }

        public virtual ICollection<Queues> Queues { get; set; }
    }
}
