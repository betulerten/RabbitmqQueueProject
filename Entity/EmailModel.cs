using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class EmailModel
    {
        public int Id { get; set; }
        public string Header { get; set; }
        public string Email { get; set; }
        public string Body { get; set; }
    }
}
