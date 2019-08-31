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
    public class Delete : InstruccionCQL
    {
        string id { set; get; }
        int l { set; get; }
        int c { set; get; }
        string operacion { set; get; }

        Expresion condicion { set; get; }

        /*
         * Constructor de la clase
         * @id nombre de la tabla
         * @l linea del id
         * @c columna del id
         * @operacion SIN WHERE
         */
        public Delete(string id, int l, int c, string operacion)
        {
            this.id = id;
            this.l = l;
            this.c = c;
            this.operacion = operacion;
        }
        /*
         * Constructor de la clase
         * @id nombre de la tabla
         * @l linea del id
         * @c columna del id
         * @operacion CON WHERE
         * @condicion expresion del Where
         */
        public Delete(string id, int l, int c, string operacion, Expresion condicion) : this(id, l, c, operacion)
        {
            this.condicion = condicion;
        }
        /*
              * Constructor de la clase padre
              * @ts tabla de simbolos padre
              * @user usuario que esta ejecutando las acciones
              * @baseD string por referencia de que base de datos estamos trabajando
              * @mensajes el output de la ejecucion
              */
        public object ejecutar(TablaDeSimbolos ts, string user, ref string baseD, LinkedList<string> mensajes, TablaDeSimbolos tsT)
        {
            Mensaje mensa = new Mensaje();
            BaseDeDatos db = TablaBaseDeDatos.getBase(baseD);
            Usuario us = TablaBaseDeDatos.getUsuario(user);
            if (db != null)
            {
                if (user.Equals("admin"))
                {
                    
                }
                else
                {
                    if (us != null)
                    {
                        Boolean permiso = TablaBaseDeDatos.getPermiso(us, baseD);
                        if (permiso)
                        {
                            Boolean enUso = TablaBaseDeDatos.getEnUso(baseD, user);
                            if (!enUso)
                            {
                                Tabla tabla = TablaBaseDeDatos.getTabla(db, id);
                                if (tabla != null)
                                {
                                    object res;
                                    if (operacion.Equals("NORMAL")) res = deleteALL(mensajes, tabla);
                                    else res = deleteSpecific(ts, user, ref baseD, mensajes, tabla);
                                    if (res != null) return res;
                                    
                                }
                                else mensajes.AddLast(mensa.error("La tabla: " + id + " no existe en la DB: " + baseD, l, c, "Semantico"));
                            }
                            else mensajes.AddLast(mensa.error("La DB: " + baseD + " ya esta siendo utilizada por alguien mas", l, c, "Semantico"));
                        }
                        else mensajes.AddLast(mensa.error("El usuario " + user + " no tiene permisos sobre esta base de datos", l, c, "Semantico"));
                    }
                    else mensajes.AddLast(mensa.error("El usuario " + user + " no existe", l, c, "Semantico"));

                }

            }
            else mensajes.AddLast(mensa.error("La base de datos ha eliminar: " + id + " no existe", l, c, "Semantico"));
            return null;
        }


        /*
         * Se encarga de eliminar todos los datos de una tabla 
         * @mensajes output
         * @t tabla en la que se esta trabajando
         */
        public object deleteALL(LinkedList<string> mensajes, Tabla t)
        {
            Mensaje mensa = new Mensaje();
            t.datos = new LinkedList<Data>();
            mensajes.AddLast(mensa.message("Se eliminaron todos los datos de la tabla: " + t.nombre));
            return "";
        }


        public object deleteSpecific(TablaDeSimbolos ts, string user, ref string baseD, LinkedList<string> mensajes,Tabla t)
        {
            Mensaje mensa = new Mensaje();
            if(t.datos.Count() > 0)
            {
                var node = t.datos.First;
                while(node != null)
                {
                    var nodeNext = node.Next;
                    Data data = (Data)node.Value;
                    TablaDeSimbolos tsT = new TablaDeSimbolos();
                    guardarTemp(data.valores, tsT);
                    object res = (condicion == null) ? null : condicion.ejecutar(ts, user, ref baseD, mensajes, tsT);
                    if (condicion != null)
                    {
                        if (res != null)
                        {
                            if (res.GetType() == typeof(Boolean))
                            {
                                if ((Boolean)res)
                                {
                                    t.datos.Remove(node);
                                    mensajes.AddLast(mensa.message("Se elimino el dato con exito"));
                                }
                            }
                            else
                            {
                                mensajes.AddLast(mensa.error("La condicion tiene que ser de tipo Boolean no se reconoce: " + res, l, c, "Semantico"));
                                return null;
                            }
                        }
                        else return null;
                    }
                    else
                    {
                        mensajes.AddLast(mensa.error("La condicion no puede ser null", l, c, "Semantico"));
                        return null;
                    }
                    node = nodeNext;
                }
            }
            return "";
        }


        /*
        * Metodo que se encargara de guardar los valores como variables temporales
        * @atributos es la data a guardar
        * @tsT es la tabla temporal
        */
        private void guardarTemp(LinkedList<Atributo> atributos, TablaDeSimbolos tsT)
        {
            foreach (Atributo atributo in atributos)
            {
                tsT.AddLast(new Simbolo(atributo.tipo, atributo.nombre));
                tsT.setValor(atributo.nombre, atributo.valor);
            }
        }

    }
}
