using System;
using System.Collections.Generic;

namespace QueueServiceAPI.Models
{
    public partial class Clients
    {
        public Clients()
        {
            Queues = new HashSet<Queues>();
        }

        public int Id { get; set; }
        public string Fio { get; set; }
        public DateTime Created { get; set; }

        public virtual ICollection<Queues> Queues { get; set; }
    }
}
