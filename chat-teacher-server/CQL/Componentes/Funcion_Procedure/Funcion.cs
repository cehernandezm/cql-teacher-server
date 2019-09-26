using cql_teacher_server.CHISON;
using cql_teacher_server.CQL.Arbol;
using cql_teacher_server.CQL.Componentes.Cursor;
using cql_teacher_server.Herramientas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes
{
    public class Funcion : InstruccionCQL
    {
        string tipo { set; get; }
        public string id { set; get; }
        public string identificador { set; get; }
        public LinkedList<InstruccionCQL> parametros { set; get; }
        public LinkedList<Expresion> valores { set; get; }
        LinkedList<InstruccionCQL> cuerpo { set; get; }
        public int l { set; get; }
        public int c { set; get; }

        /*
         * Constructor de la clase
         * @param {id} nombre de la funcion
         * @param {parametros} parametros de la funcion
         * @param {cuerpo} instrucciones a ejecutar
         * @param {l} linea del id
         * @param {c} columnad el id
         */
        public Funcion(string tipo,string id, LinkedList<InstruccionCQL> parametros, LinkedList<InstruccionCQL> cuerpo, int l, int c)
        {
            this.tipo = tipo;
            this.id = id;
            this.parametros = parametros;
            this.cuerpo = cuerpo;
            this.l = l;
            this.c = c;
            generarId();
        }



      
        /*
         * Metodo de la implementacion
         * @ts tabla de simbolos global
         * @user usuario que ejecuta la accion
         * @baseD base de datos donde estamos ejecutando todo
         * @mensajes linkedlist con la salida deseada
         */
        public object ejecutar(TablaDeSimbolos ts, Ambito ambito, TablaDeSimbolos tsT)
        {
            Mensaje ms = new Mensaje();
            TablaDeSimbolos newAmbito = new TablaDeSimbolos();
            foreach(Simbolo s in ambito.tablaPadre)
            {
                newAmbito.AddLast(s);
            }

            if (parametros.Count() == valores.Count())
            {
                //-------------------------------------- CREACION Y ASIGNACION DE PARAMETROS -----------------------------------------------------
                for (int i = 0; i < parametros.Count(); i++)
                {
                    Declaracion d = (Declaracion)parametros.ElementAt(i);
                    d.parametro = true;
                    object rd = d.ejecutar(newAmbito, ambito, tsT);
                    if (rd == null) return null;
                    Asignacion a = new Asignacion(d.id, l, c, valores.ElementAt(i), "ASIGNACION");
                    a.tPadre = ts;
                    object ra = a.ejecutar(newAmbito, ambito, tsT);
                    if (ra == null) return null;
                }
                //---------------------------------------- INSTRUCCIONES DE LA FUNCION -----------------------------------------------------------
                foreach(InstruccionCQL ins in cuerpo)
                {
                    object r = ins.ejecutar(newAmbito, ambito, tsT);
                    if (r == null)
                    {
                        ambito.mensajes.AddLast(ms.error("Error en la Funcion: " + id, l, c, "Semantico"));
                        return null;
                    }
                    else if (r.GetType() == typeof(Retorno))
                    {
                        object re = ((Retorno)r).valor;
                        if (re != null)
                        {

                            if (re.GetType() == typeof(int) && tipo.Equals("int")) return (int)re;
                            else if (re.GetType() == typeof(string) && tipo.Equals("string")) return (String)re;
                            else if (re.GetType() == typeof(Boolean) && tipo.Equals("boolean")) return (Boolean)re;
                            else if (re.GetType() == typeof(double) && tipo.Equals("double")) return (Double)re;
                            else if (re.GetType() == typeof(DateTime) && tipo.Equals("date")) return (DateTime)re;
                            else if (re.GetType() == typeof(TimeSpan) && tipo.Equals("time")) return (TimeSpan)re;
                            else if (re.GetType() == typeof(Map) && tipo.Equals("map")) return (Map)re;
                            else if (re.GetType() == typeof(List) && tipo.Equals("list")) return (List)re;
                            else if (re.GetType() == typeof(Set) && tipo.Equals("set")) return (Set)re;
                            else if (re.GetType() == typeof(TypeCursor) && tipo.Equals("cursor")) return (TypeCursor)re;
                            else if (re.GetType() == typeof(InstanciaUserType))
                            {
                                InstanciaUserType temp = (InstanciaUserType)re;
                                if (temp.tipo.Equals(tipo)) return temp;
                                else ambito.mensajes.AddLast(ms.error("No coincide el tipo de USERTYPE", l, c, "Semantico"));

                            }
                            else ambito.mensajes.AddLast(ms.error("No coincide el tipo: " + tipo + " con el valor: " + re, l, c, "Semantico"));
                            return null;
                        }
                    }
                }
                ambito.mensajes.AddLast(ms.error("La funcion no posee ningun return", l, c, "Semantico"));
                return null;
            }
            else ambito.mensajes.AddLast(ms.error("No coinciden el numero de parametros con el numero de valores", l, c, "Semantico"));

            return null;
        }


        /*
       * METODO QUE GENERA UN IDENTIFICADOR UNICO
       */

        public void generarId()
        {
            string identi = id;
            foreach (Declaracion d in parametros)
            {
                identi += "_" + d.tipo;
            }
            identificador = identi;
        }

    }
}
