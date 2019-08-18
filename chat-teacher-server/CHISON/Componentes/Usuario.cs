using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CHISON.Componentes
{
    public class Usuario
    {
        public string nombre { set; get; }
        public string password { set; get; }

        public LinkedList<String> bases { set; get; }

        public Usuario(string nombre, string password, LinkedList<string> bases)
        {
            this.nombre = nombre;
            this.password = password;
            this.bases = bases;
        }


        
    }
}
