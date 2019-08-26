using cql_teacher_server.CQL.Arbol;
using cql_teacher_server.Herramientas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes
{
    public class Case : InstruccionCQL
    {
        Expresion condicion { set; get; }

        LinkedList<InstruccionCQL> cuerpo { set; get; }

        int l { set; get; }
        int c { set; get; }

        public Boolean flag { set; get; }

        public Boolean isDefault { set; get; }


        /*
         * Constructor de la clase si es un CASE
         * @condicion Boolean True o false
         * @cuerpo InstruccionesCQL a ejecutar
         * @l linea de la condicion
         * @c columna de la condicion
         */
        public Case(Expresion condicion, LinkedList<InstruccionCQL> cuerpo, int l, int c)
        {
            this.condicion = condicion;
            this.cuerpo = cuerpo;
            this.l = l;
            this.c = c;
            this.isDefault = false;
        }

        /*
         * Constructor de la clase si es un DEFAULT
         * @cuerpo InstruccionesCQL a ejecutar
         * @l linea de la condicion
         * @c columna de la condicion
         */
        public Case(LinkedList<InstruccionCQL> cuerpo, int l, int c)
        {
            this.cuerpo = cuerpo;
            this.l = l;
            this.c = c;
            this.condicion = null;
            this.isDefault = true;
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
            if(condicion == null)
            {
                TablaDeSimbolos ambitoLocal = new TablaDeSimbolos();
                foreach (Simbolo s in ts)
                {
                    ambitoLocal.AddLast(s);
                }
                foreach (InstruccionCQL i in cuerpo)
                {
                    object r = i.ejecutar(ambitoLocal, user, ref baseD, mensajes);
                    if (r == null) return r;
                }
                return "";
            }
            else
            {
                Mensaje mensa = new Mensaje();
                object a = condicion.ejecutar(ts, user, ref baseD, mensajes);
                if(a != null)
                {
                    if(a.GetType() == typeof(Boolean))
                    {
                        flag = (Boolean)a;
                        if ((Boolean)a)
                        {
                            TablaDeSimbolos ambitoLocal = new TablaDeSimbolos();
                            foreach (Simbolo s in ts)
                            {
                                ambitoLocal.AddLast(s);
                            }
                            foreach (InstruccionCQL i in cuerpo)
                            {
                                object r = i.ejecutar(ambitoLocal, user, ref baseD, mensajes);
                                if (r == null) return r;
                            }
                        }
                    }
                    else
                    {
                        mensajes.AddLast(mensa.error("La condicion tiene que ser de tipo Bool: " + a.ToString(), l, c, "Semantico"));
                        return null;
                    }
                }
                else
                {
                    mensajes.AddLast(mensa.error("No se aceptan condiciones null ", l, c, "Semantico"));
                    return null;
                }
                return "";
            }
        }
    }
}
