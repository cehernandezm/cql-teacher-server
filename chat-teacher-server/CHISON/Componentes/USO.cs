using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CHISON.Componentes
{
    public class USO
    {
        public string bd { set; get; }
        public string nombre { set; get; }

        public USO(string bd, string nombre)
        {
            this.bd = bd;
            this.nombre = nombre;
        }
    }
}
