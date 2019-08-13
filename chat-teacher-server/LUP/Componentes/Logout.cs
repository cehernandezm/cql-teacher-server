using cql_teacher_server.LUP.Arbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.LUP.Componentes
{
    class Logout : InstruccionLUP
    {
        private string usuario;

        public Logout(string usuario)
        {
            this.usuario = usuario;
        }

        public object ejecutar()
        {
            return usuario == "admin";
        }
    }
}
