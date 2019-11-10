using System;
using System.Collections.Generic;

namespace QueueServiceAPI
{
    public partial class Queues
    {
        public int Id { get; set; }
        public bool Competing { get; set; }
        public bool Handled { get; set; }
        public int Clientid { get; set; }
        public int Employeeid { get; set; }
        public DateTime Created { get; set; }

        public virtual Clients Client { get; set; }
        public virtual Employees Employee { get; set; }
    }
}
