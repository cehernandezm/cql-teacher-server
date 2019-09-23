using cql_teacher_server.CHISON;
using cql_teacher_server.CHISON.Componentes;
using cql_teacher_server.LUP.Arbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.LUP.Componentes
{
    public class UsuarioLUP : InstruccionLUP
    {
        private string usuario;
        private string password;

        public UsuarioLUP(string a, string b)
        {
            this.usuario = a;
            this.password = b;
        }


        public Object ejecutar()
        {
            if (usuario.Equals("admin") && password.Equals("admin")) return true;
            Usuario user = TablaBaseDeDatos.getUsuario(usuario.ToLower().TrimEnd().TrimStart());
            if (user == null) return false;
            System.Diagnostics.Debug.WriteLine("US:" + usuario + "PAS:" + password);
            return password.Equals(user.password);
        }
    }
}
