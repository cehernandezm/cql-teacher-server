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
    public class EditUser: InstruccionCQL
    {
        string id { set; get; }
        string iddb { set; get; }
        int l { set; get; }
        int c { set; get; }
        string operacion { set; get; }

        /*
         * Constructor de la clase
         * @id es el id del usuario
         * @iddb es la base de datos
         * @l linea del id
         * @c columna del id
         * @operacion puede ser REVOKE O GRANT
         */
        public EditUser(string id, string iddb, int l, int c, string operacion)
        {
            this.id = id;
            this.iddb = iddb;
            this.l = l;
            this.c = c;
            this.operacion = operacion;
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
            BaseDeDatos db = TablaBaseDeDatos.getBase(iddb);
            Usuario us = TablaBaseDeDatos.getUsuario(user);
            if (db != null)
            {
                if (user.Equals("admin"))
                {

                    Usuario usuario2 = TablaBaseDeDatos.getUsuario(id);
                    if (usuario2 != null)
                    {
                        if (operacion.Equals("GRANT"))
                        {
                            usuario2.bases.AddLast(iddb);
                            mensajes.AddLast(mensa.message("Se le dio permiso al usuario: " + id + " sobre la DB: " + iddb));
                        }
                        else
                        {
                            usuario2.bases.Remove(iddb);
                            mensajes.AddLast(mensa.message("Se le quitaron permisos al usuario: " + id + " sobre la DB: " + iddb));
                        }

                        return "";
                    }
                    else mensajes.AddLast(mensa.error("El usuario " + id + " no existe", l, c, "Semantico"));
                }
                else
                {
                    if (us != null)
                    {
                        Boolean permiso = TablaBaseDeDatos.getPermiso(us, id);
                        if (permiso)
                        {
                            Usuario usuario2 = TablaBaseDeDatos.getUsuario(id);
                            if(usuario2 != null)
                            {
                                if (operacion.Equals("GRANT"))
                                {
                                    usuario2.bases.AddLast(iddb);
                                    mensajes.AddLast(mensa.message("Se le dio permiso al usuario: " + id + " sobre la DB: " + iddb));
                                }
                                else
                                {
                                    usuario2.bases.Remove(iddb);
                                    mensajes.AddLast(mensa.message("Se le quitaron permisos al usuario: " + id + " sobre la DB: " + iddb));
                                }
                                
                                return "";
                            }
                            else mensajes.AddLast(mensa.error("El usuario " + id + " no existe", l, c, "Semantico"));
                        }
                        else mensajes.AddLast(mensa.error("El usuario " + user + " no tiene permisos sobre esta base de datos", l, c, "Semantico"));
                    }
                    else  mensajes.AddLast(mensa.error("El usuario " + user + " no existe", l, c, "Semantico"));
                    
                }

            }
            else mensajes.AddLast(mensa.error("La base de datos ha eliminar: " + id + " no existe", l, c, "Semantico"));
            return null;
        }
    }
}
