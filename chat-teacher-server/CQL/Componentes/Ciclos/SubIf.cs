using cql_teacher_server.CQL.Arbol;
using cql_teacher_server.CQL.Componentes.Ciclos;
using cql_teacher_server.Herramientas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes 
{

    public class SubIf : InstruccionCQL
    {
        Expresion condicion { set; get; }
        LinkedList<InstruccionCQL> cuerpo { set; get; }

        public Boolean flagElse { set; get; }

        public Boolean flagCondicion { set; get; }

        int l { set; get; }

        int c { set; get; }

        /*
         * Constructor de la clase que crea un if,elseif
         * @condicion es la condicion  a evalular
         * @cuerpo es lo que se ejecutara dentro del else if
         * @l linea de la condicion
         * @c columna de la condicion
         */

        public SubIf(Expresion condicion, LinkedList<InstruccionCQL> cuerpo, int l, int c)
        {
            this.condicion = condicion;
            this.cuerpo = cuerpo;
            this.l = l;
            this.c = c;
            this.flagElse = false;
        }

        /*
         * Constructor de la clase que crea un else
         * @cuerpo es lo que se ejecutara dentro del else if
         * @l linea de la condicion
         * @c columna de la condicion
         */
        public SubIf(LinkedList<InstruccionCQL> cuerpo, int l, int c)
        {
            this.cuerpo = cuerpo;
            this.l = l;
            this.c = c;
            this.flagElse = true;
            this.condicion = null;
        }
        /*
         * Constructor de la clase padre
         * @ts tabla de simbolos padre
         * @user usuario que esta ejecutando las acciones
         * @baseD string por referencia de que base de datos estamos trabajando
         * @mensajes el output de la ejecucion
         */
        public object ejecutar(TablaDeSimbolos ts, Ambito ambito, TablaDeSimbolos tsT)
        {
            flagCondicion = false;
            object a = (condicion == null) ? null : condicion.ejecutar(ts,ambito, tsT);

            Mensaje men = new Mensaje();
            //----------------------------------------------------------- ELSE -----------------------------------------------------------
            if (condicion == null)
            {
                TablaDeSimbolos ambitoLocal = new TablaDeSimbolos();
                foreach (Simbolo s in ts)
                {
                    ambitoLocal.AddLast(s);
                }
                TablaDeSimbolos tablaTemp = new TablaDeSimbolos();
                foreach (Simbolo s in tsT)
                {
                    tablaTemp.AddLast(s);
                }
                foreach (InstruccionCQL i in cuerpo)
                {
                    object r = i.ejecutar(ambitoLocal,ambito, tablaTemp);
                    if (r == null)
                    {
                        System.Diagnostics.Debug.WriteLine("ACCION: " + i.GetType());
                        return r;
                    }
                    else if (r.GetType() == typeof(Retorno)) return r;
                }
                return "";
            }
            //--------------------------------------------------------------------------------- IF ELSE IF 
            else
            {
                if (a != null)
                {
                    if (a.GetType() == typeof(Boolean))
                    {
                        flagCondicion = (Boolean)a;
                        if (flagCondicion)
                        {
                            TablaDeSimbolos ambitoLocal = new TablaDeSimbolos();
                            foreach (Simbolo s in ts)
                            {
                                ambitoLocal.AddLast(s);
                            }
                            TablaDeSimbolos tablaTemp = new TablaDeSimbolos();
                            foreach (Simbolo s in tsT)
                            {
                                tablaTemp.AddLast(s);
                            }
                            //------------------------------------------------- INSTRUCCIONES DEL IF -----------------------------------------------
                            foreach (InstruccionCQL i in cuerpo)
                            {
                                object r = i.ejecutar(ambitoLocal, ambito,tablaTemp);
                                if (r == null) return r;
                                else if (r.GetType() == typeof(Retorno)) return (Retorno)r;
                                else if (i.GetType() == typeof(Continue)) return i;
                            }
                        }
                        return "";
                    }
                    else ambito.mensajes.AddLast(men.error("La condicion tiene que ser de tipo boolean no se reconoce: " + a.ToString(), l, c, "Semantico"));
                }
                else ambito.mensajes.AddLast(men.error("No se acepta un null como condicion", l, c, "Semantico"));
            }
            

            return null;
        }
    }
}
