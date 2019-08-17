using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CHISON.Componentes
{
    public class Attrs
    {
        public string name { set; get; }
        public string type { set; get; }

        public Attrs(string name, string type)
        {
            this.name = name;
            this.type = type;
        }
    }
}
