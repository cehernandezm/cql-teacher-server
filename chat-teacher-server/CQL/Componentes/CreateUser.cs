using cql_teacher_server.CHISON;
using cql_teacher_server.CHISON.Componentes;
using cql_teacher_server.CQL.Arbol;
using cql_teacher_server.Herramientas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes
{
    public class CreateUser : InstruccionCQL
    {
        string id { set; get; }
        string password { set; get; }
        int l { set; get; }
        int c { set; get; }

        /*
         * Constructor de la clase 
         * @id nombre del usuario
         * @password password del usuario
         * @l linea del id
         * @c columna del id
         */
        public CreateUser(string id, string password, int l, int c)
        {
            this.id = id;
            this.password = password;
            this.l = l;
            this.c = c;
        }


        /*
        * Constructor de la clase
        * @id nombre de la tabla
        * @l linea del id
        * @c columna del id
        */

        public object ejecutar(TablaDeSimbolos ts, string user, ref string baseD, LinkedList<string> mensajes)
        {
            Mensaje mensa = new Mensaje();

            if (user.Equals("admin"))
            {
                Usuario usuario = TablaBaseDeDatos.getUsuario(id);
                if (usuario == null)
                {
                    usuario = new Usuario(id, password, new LinkedList<string>());
                    TablaBaseDeDatos.listaUsuario.AddLast(usuario);
                    mensajes.AddLast(mensa.message("El usuario: " + id + " se creo exitosamente"));
                    return "";
                }
                else mensajes.AddLast(mensa.error("El usuario: " + id + " ya existe", l, c, "Semantico"));
            }
            else mensajes.AddLast(mensa.error("Solo un admin puede crear usuarios, el usuario: " + user + " no es admin", l, c, "Semantico"));




            return null;
        }
    }
}
