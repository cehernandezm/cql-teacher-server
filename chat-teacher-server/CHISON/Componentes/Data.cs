using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CHISON.Componentes
{
    public class Data
    {
       public LinkedList<Atributo> valores { set; get; }

        public Data(LinkedList<Atributo> valores)
        {
            this.valores = valores;
        }
    }
}
