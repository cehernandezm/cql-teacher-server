using cql_teacher_server.CHISON;
using cql_teacher_server.CHISON.Componentes;
using cql_teacher_server.CQL.Arbol;
using cql_teacher_server.CQL.Componentes.Try_Catch;
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

        public object ejecutar(TablaDeSimbolos ts, Ambito ambito, TablaDeSimbolos tsT)
        {
            Mensaje mensa = new Mensaje();
            string user = ambito.usuario;
            string baseD = ambito.baseD;
            LinkedList<string> mensajes = ambito.mensajes;
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
                    else
                    {
                        ambito.listadoExcepciones.AddLast(new Excepcion("userdontexists", "El usuario " + id + " no existe"));
                        mensajes.AddLast(mensa.error("El usuario " + id + " no existe", l, c, "Semantico"));
                    }
                }
                else
                {
                    if (us != null)
                    {
                        Boolean permiso = TablaBaseDeDatos.getPermiso(us, iddb);
                        if (permiso)
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
                            else
                            {
                                ambito.listadoExcepciones.AddLast(new Excepcion("userdontexists", "El usuario " + id + " no existe"));
                                mensajes.AddLast(mensa.error("El usuario " + id + " no existe", l, c, "Semantico"));
                            }
                        }
                        else mensajes.AddLast(mensa.error("El usuario " + user + " no tiene permisos sobre esta base de datos", l, c, "Semantico"));
                    }
                    else  mensajes.AddLast(mensa.error("El usuario " + user + " no existe", l, c, "Semantico"));
                    
                }

            }
            else
            {
                ambito.listadoExcepciones.AddLast(new Excepcion("usedbexception", "No existe la base de datos: " + iddb + " o no se ha usado el comando use"));
                ambito.mensajes.AddLast(mensa.error("No existe la base de datos: " + iddb + " o no se ha usado el comando use", l, c, "Semantico"));
            }
            return null;
        }
    }
}
