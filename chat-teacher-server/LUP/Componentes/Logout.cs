using cql_teacher_server.CHISON;
using cql_teacher_server.CHISON.Componentes;
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
            if (usuario.Equals("admin")) return true;
            Usuario user = TablaBaseDeDatos.getUsuario(usuario.ToLower().TrimEnd().TrimStart());
            if(user != null)
            {
                TablaBaseDeDatos.deleteMine(usuario);
                return true;
            }
            return false;
        }
    }
}
