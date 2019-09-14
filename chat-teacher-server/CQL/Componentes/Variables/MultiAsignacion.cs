using cql_teacher_server.CQL.Arbol;
using cql_teacher_server.Herramientas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes.Variables
{
    public class MultiAsignacion : InstruccionCQL
    {
        LinkedList<string> listaId { set; get; }
        Expresion expresion { set; get; }
        int l { set; get; }
        int c { set; get; }

        /*
         * Constructor de la clase
         * @param {listaId} ID's a los que se le asignara las variables
         * @param {expresion} listado de valores a asignar
         * @param {l} linea del id
         * @param {c} columna del id
         */
        public MultiAsignacion(LinkedList<string> listaId, Expresion expresion, int l, int c)
        {
            this.listaId = listaId;
            this.expresion = expresion;
            this.l = l;
            this.c = c;
        }
        /*
       * Metodo de la implementacion
       * @ts tabla de simbolos global
       * @user usuario que ejecuta la accion
       * @baseD base de datos donde estamos ejecutando todo
       * @mensajes linkedlist con la salida deseada
       */
        public object ejecutar(TablaDeSimbolos ts, string user, ref string baseD, LinkedList<string> mensajes, TablaDeSimbolos tsT)
        {
            Mensaje ms = new Mensaje();
            object valores = (expresion == null) ? null : expresion.ejecutar(ts, user, ref baseD, mensajes, tsT);
            if (valores != null)
            {
                if (valores.GetType() == typeof(LinkedList<object>))
                {
                    LinkedList<object> lista = (LinkedList<object>)valores;
                    if (lista.Count() == listaId.Count())
                    {
                        for (int i = 0; i < lista.Count(); i++)
                        {
                            string tipo = ts.getTipo(listaId.ElementAt(i)).ToLower().TrimEnd().TrimStart();
                            if (!tipo.Equals("none"))
                            {
                                object res = checkValues(lista.ElementAt(i), tipo, mensajes, ts, listaId.ElementAt(i));
                                if (res == null) return null;
                            }
                            else
                            {
                                mensajes.AddLast(ms.error("La variable: " + listaId.ElementAt(i) + " no existe en este ambito", l, c, "Semantico"));
                                return null;
                            }
                            
                        }
                        return "";
                    }
                    else mensajes.AddLast(ms.error("No coincide la cantidad de valores con la cantidad de variables",l,c,"Semantico"));
                }
                else mensajes.AddLast(ms.error("No se puede asignar este valor: " + valores + " a la lista de ID'S",l,c,"Semantico"));
            }
            else mensajes.AddLast(ms.error("No se puede asignar un valor null a la lista de ID's",l,c,"Semantico"));
            return null;
        }


        /*
          * METODO QUE VERIFICA LOS VALORES A GUARDAR
          * @param {op1} valor a guardar
          * @param {tipo} tipo de variable
          * @param {a} expresion original
          * @param {mensajes} output
          * @param {ts} tabla de variables
          */
        private object checkValues(object op1, string tipo, LinkedList<string> mensajes, TablaDeSimbolos ts, string id)
        {
            Mensaje mensa = new Mensaje();


            if (op1 != null)
            {
                if (op1.GetType() == typeof(string) && tipo.Equals("string")) ts.setValor(id, (string)op1);
                else if (op1.GetType() == typeof(int) && tipo.Equals("int")) ts.setValor(id, (int)op1);
                else if (op1.GetType() == typeof(int) && tipo.Equals("double")) ts.setValor(id, Convert.ToInt32((Double)op1));
                else if (op1.GetType() == typeof(Double) && tipo.Equals("double")) ts.setValor(id, (Double)op1);
                else if (op1.GetType() == typeof(Double) && tipo.Equals("int")) ts.setValor(id, Convert.ToDouble((int)op1));
                else if (op1.GetType() == typeof(Boolean) && tipo.Equals("boolean")) ts.setValor(id, (Boolean)op1);
                else if (op1.GetType() == typeof(DateTime) && tipo.Equals("date")) ts.setValor(id, (DateTime)op1);
                else if (op1.GetType() == typeof(TimeSpan) && tipo.Equals("time")) ts.setValor(id, (TimeSpan)op1);
                else if (op1.GetType() == typeof(Map) && tipo.Equals("map"))
                {
                    Map temp = (Map)op1;
                    Map valor = (Map)ts.getValor(id);
                    if (valor.id.Equals(temp.id) || valor.id.Equals("none"))
                    {
                        ts.setValor(id, temp);
                        return "";
                    }
                    else
                    {
                        mensajes.AddLast(mensa.error("No coincide los tipos: " + valor.id + " con: " + temp.id, l, c, "Semantico"));
                        return null;
                    }

                }
                else if (op1.GetType() == typeof(List) && tipo.Equals("list"))
                {
                    List temp = (List)op1;

                    List valor = (List)ts.getValor(id);
                    if (valor.id.Equals(temp.id) || valor.id.Equals("none"))
                    {
                        ts.setValor(id, temp);
                        return "";
                    }
                    else
                    {
                        mensajes.AddLast(mensa.error("No coincide los tipos: " + valor.id + " con: " + temp.id, l, c, "Semantico"));
                        return null;
                    }
                }
                else if (tipo.Equals("set") && op1.GetType() == typeof(Set))
                {
                    Set original = (Set)ts.getValor(id);
                    Set temp = (Set)op1;
                    if (original.id.Equals(temp.id) || original.id.Equals("none"))
                    {
                        object resp = temp.buscarRepetidos(mensajes, l, c);
                        if (resp == null) return null;
                        temp.order();
                        ts.setValor(id, temp);
                    }
                    else
                    {
                        mensajes.AddLast(mensa.error("No coincide los tipos: " + original.id + " con: " + temp.id, l, c, "Semantico"));
                        return null;
                    }
                }
                else if (op1.GetType() == typeof(InstanciaUserType))
                {
                    InstanciaUserType temp = (InstanciaUserType)op1;
                    if (tipo.Equals(temp.tipo.ToLower()) || temp.tipo.Equals("none"))
                    {
                        ts.setValor(id, temp);
                        return "";
                    }
                    else
                    {
                        mensajes.AddLast(mensa.error("No se le puede asignar a la variable: " + id + " el valor: " + op1, l, c, "Semantico"));
                        return null;
                    }

                }
                else
                {
                    mensajes.AddLast(mensa.error("No se le puede asignar a la variable: " + id + " el valor: " + op1, l, c, "Semantico"));
                    return null;

                }
                return "";
            }
            else
            {
                if (tipo.Equals("string") || tipo.Equals("date") || tipo.Equals("time")) ts.setValor(id, null);
                else if(tipo.Equals("int") || tipo.Equals("double") || tipo.Equals("boolean") || tipo.Equals("map") || tipo.Equals("list") || tipo.Equals("set")){
                    mensajes.AddLast(mensa.error("No se le puede asignar a la variable: " + id + " el valor: null", l, c, "Semantico"));
                    return null;
                }
                else ts.setValor(id, new InstanciaUserType(tipo, null));
                return "";
            }

        }

    }
}
