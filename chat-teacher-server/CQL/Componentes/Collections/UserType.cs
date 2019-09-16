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
    public class UserType : InstruccionCQL
    {
        string id { set; get; }
        LinkedList<Attrs> lista { set; get; }

        int linea { set; get; }
        int columna { set; get; }

        Boolean flag { set; get; }

        /*
         * Constructor de la clase
         * @id nombre de la variable
         * @lista un LinkedList con los atributos del objeto
         * @linea linea del identificador
         * @columna columna del identificador
         */
        public UserType(string id, LinkedList<Attrs> lista, int linea, int columna, Boolean flag)
        {
            this.id = id;
            this.id = this.id.ToLower().TrimEnd().TrimStart();
            this.lista = lista;
            this.linea = linea;
            this.columna = columna;
            this.flag = flag;
        }

        /*
         * Metodo del padre
         * @ts tabla de simbolos global
         * @user usuario que esta ejecutando la accion
         * @baseD base de datos donde se esta ejecutando la accion
         * @mensajes respuesta por parte de la accion
         */
        public object ejecutar(TablaDeSimbolos ts,Ambito ambito, TablaDeSimbolos tsT)
        {
            Mensaje mensa = new Mensaje();
            BaseDeDatos db = TablaBaseDeDatos.getBase(ambito.baseD);
            if (db != null)
            {
                if (ambito.usuario.Equals("admin"))
                {
                    if (!TablaBaseDeDatos.getUserType(id, db))
                    {
                        LinkedList<Attrs> newL = newLista(lista, ambito.mensajes, db);
                        if (lista.Count() == newL.Count())
                        {
                            User_Types u = new User_Types(id, newL);
                            db.objetos.user_types.AddLast(u);
                        }

                        return "";

                    }
                    else
                    {
                        if (!flag)
                        {
                            ambito.listadoExcepciones.AddLast(new Excepcion("typealreadyexists", "El USER TYPE : " + id + " ya existe en la base de datos : " + ambito.baseD));
                            ambito.mensajes.AddLast(mensa.error("El USER TYPE : " + id + " ya existe en la base de datos : " + ambito.baseD, linea, columna, "Semantico"));
                        }
                        else return "";
                    }
                }
                else
                {
                    Usuario us = TablaBaseDeDatos.getUsuario(ambito.usuario);
                    if (us != null)
                    {
                        if (TablaBaseDeDatos.getPermiso(us, ambito.baseD))
                        {
                            if (!TablaBaseDeDatos.getUserType(id, db))
                            {
                                LinkedList<Attrs> newL = newLista(lista, ambito.mensajes, db);
                                if (lista.Count() == newL.Count())
                                {
                                    User_Types u = new User_Types(id, newL);
                                    db.objetos.user_types.AddLast(u);
                                }

                                return "";

                            }
                            else
                            {
                                if (!flag)
                                {
                                    ambito.listadoExcepciones.AddLast(new Excepcion("typealreadyexists", "El USER TYPE : " + id + " ya existe en la base de datos : " + ambito.baseD));
                                    ambito.mensajes.AddLast(mensa.error("El USER TYPE : " + id + " ya existe en la base de datos : " + ambito.baseD, linea, columna, "Semantico"));
                                }
                                else return "";
                            }
                        }
                        else ambito.mensajes.AddLast(mensa.error("El usuario : " + ambito.usuario + " no tiene permisos sobre la base de datos : " + ambito.baseD, linea, columna, "Semantico"));

                    }
                    else ambito.mensajes.AddLast(mensa.error("El usuario : " + ambito.usuario + " no existe ", linea, columna, "Semantico"));
                }
                


            }
            else ambito.mensajes.AddLast(mensa.error("La base de datos : " + ambito.baseD + " no existe o seleccione una antes Comando (USE) " , linea, columna , "Semantico"));

            return null;
        }

        private LinkedList<Attrs> newLista(LinkedList<Attrs> oldList , LinkedList<string> mensajes, BaseDeDatos db)
        {
            LinkedList<Attrs> newL = new LinkedList<Attrs>();
            foreach(Attrs a in oldList)
            {
                if (a.type.Equals("string") || a.type.Equals("int") || a.type.Equals("double") || a.type.Equals("boolean") || a.type.Equals("map")
                    || a.type.Equals("date") || a.type.Equals("time")) newL.AddLast(a);
                else if (a.type.ToLower().Contains("list") || a.type.ToLower().Contains("map") || a.type.ToLower().Contains("set")) newL.AddLast(a);
                else if (TablaBaseDeDatos.getUserType(a.type.ToLower(), db)) newL.AddLast(a);
                else
                {
                    Mensaje mensa = new Mensaje();
                    mensajes.AddLast(mensa.error("No se reconoce el tipo " + a.type + " en el USER TYPE : " + id, linea, columna, "Semantico"));
                }
            }

            return newL;

        }

    }
}
