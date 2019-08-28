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
    public class DropTable : InstruccionCQL
    {
        string id { set; get; }
        Boolean flag { set; get; }

        int l { set; get; }

        int c { set; get; }
        
        
        /*
         * Constructor de la clase
         * @id id de la tabla a eliminar
         * @flag es si esta el IF EXISTS
         * @l linea del id
         * @c columna del id
         */

        public DropTable(string id, bool flag, int l, int c)
        {
            this.id = id;
            this.flag = flag;
            this.l = l;
            this.c = c;
        }


        

        /*
          * Constructor de la clase padre
          * @ts tabla de simbolos padre
          * @user usuario que esta ejecutando las acciones
          * @baseD string por referencia de que base de datos estamos trabajando
          * @mensajes el output de la ejecucion
          */
        public object ejecutar(TablaDeSimbolos ts, string user, ref string baseD, LinkedList<string> mensajes)
        {
            Mensaje mensa = new Mensaje();
            BaseDeDatos db = TablaBaseDeDatos.getBase(baseD);
            if (db != null)
            {
                if (user.Equals("admin"))
                {

                }
                else
                {
                    Usuario usuario = TablaBaseDeDatos.getUsuario(user);
                    if(usuario != null)
                    {
                        Boolean permiso = TablaBaseDeDatos.getPermiso(usuario, baseD);
                        if (permiso)
                        {
                            Boolean enUso = TablaBaseDeDatos.getEnUso(baseD, user);
                            if (!enUso)
                            {
                                Tabla tabla = TablaBaseDeDatos.getTabla(db, id);
                                if(tabla != null)
                                {
                                    db.objetos.tablas.Remove(tabla);
                                    mensajes.AddLast(mensa.message("La tabla: " + id + " fue eliminada con exito"));
                                    return "";
                                }
                                else
                                {
                                    if (!flag) mensajes.AddLast(mensa.error("La tabla:" + id + " no existe en a DB: " + baseD, l, c, "Semantico"));
                                    else return "";
                                }
                            }
                            else mensajes.AddLast(mensa.error("La DB: " + baseD +  " esta siendo utilizada por otro usuario", l, c, "Semantico"));
                        }
                        else mensajes.AddLast(mensa.error("El usuario " + user + " no tiene permisos en la DB: " + baseD, l, c, "Semantico"));
                    }
                    else mensajes.AddLast(mensa.error("No existe el usuario: " + user, l, c, "Semantico"));
                }
            }
            else mensajes.AddLast(mensa.error("No existe la DB: " + baseD + " o no se ha usado el comando USE", l, c, "Semantico"));
            

            return null;
        }
    }
}
