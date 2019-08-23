using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CHISON.Componentes
{
    public class User_Types
    {
        public string name { set; get;}
        public LinkedList<Attrs> type { set; get; }



        public User_Types(string name, LinkedList<Attrs> type)
        {
            this.name = name;
            this.type = type;
        }
    }
}
