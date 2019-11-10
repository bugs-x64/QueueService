using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueServiceAdmin
{
    public class QueueRecord
    {
        public int RecId { get; set; }
        public string Fio { get; set; }
        public bool Competing { get; set; }
    }
}
