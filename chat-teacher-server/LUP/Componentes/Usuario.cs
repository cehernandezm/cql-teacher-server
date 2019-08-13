using chat_teacher_server.LUP.Arbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace chat_teacher_server.LUP.Componentes
{
    public class Usuario : InstruccionLUP
    {
        private string usuario;
        private string password;

        public Usuario(string a, string b)
        {
            this.usuario = a;
            this.password = b;
        }


        public Object ejecutar()
        {
            return usuario == "admin" && password == "admin";
        }
    }
}
