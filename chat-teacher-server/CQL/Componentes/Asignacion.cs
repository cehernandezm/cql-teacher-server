using cql_teacher_server.CQL.Arbol;
using cql_teacher_server.Herramientas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes
{
    public class Asignacion : InstruccionCQL
    {
        string id { set; get; }
        int l { set; get; }
        int c { set; get; }

        Expresion a { set; get; }


        /*
         * Constructor de la clase que asigna valores a una id en especifico
         * @id el nombre de la varibale
         * @l linea del nombre de la variable
         * @c columna del nombre de la variable
         * @a valor a guardar
         */
        public Asignacion(string id, int l, int c, Expresion a)
        {
            this.id = id;
            this.l = l;
            this.c = c;
            this.a = a;
        }



        /*
         * Metodo que se implementa de la clase padre
         * @ts tabla de simbolos
         * @user usuario que ejecuta la accion
         * @baseD base de datos en la que se esta trabajando
         * @mensajes respuesta de errores o mensajes de salida
         */

        public object ejecutar(TablaDeSimbolos ts, string user, ref string baseD, LinkedList<string> mensajes)
        {
            Mensaje mensa = new Mensaje();
            object op1 = (a == null) ? null : a.ejecutar(ts, user, ref baseD, mensajes);
            string tipo = ts.getTipo(id);
            tipo = tipo.ToLower().TrimEnd().TrimStart();
            if (!tipo.Equals("none"))
            {
                if( a == null)
                {
                    if (tipo.Equals("string") || tipo.Equals("date") || tipo.Equals("time")) ts.setValor(id, null);
                    else if(!tipo.Equals("int") || !tipo.Equals("boolean") || !tipo.Equals("double"))
                    {
                        InstanciaUserType temp = new InstanciaUserType(tipo,null);
                        ts.setValor(id, temp);
                    }
                    else
                    {
                        mensajes.AddLast(mensa.error("No se le puede asignar a la variable: " + id + " el valor: null" , l, c, "Semantico"));
                        return null;

                    }
                }
                else
                {
                    if(op1 != null)
                    {
                        if (op1.GetType() == typeof(string) && tipo.Equals("string")) ts.setValor(id, (string)op1);
                        else if (op1.GetType() == typeof(int) && tipo.Equals("int")) ts.setValor(id, (int)op1);
                        else if (op1.GetType() == typeof(int) && tipo.Equals("double")) ts.setValor(id, Convert.ToInt32((Double)op1));
                        else if (op1.GetType() == typeof(Double) && tipo.Equals("double")) ts.setValor(id, (Double)op1);
                        else if (op1.GetType() == typeof(Double) && tipo.Equals("int")) ts.setValor(id, Convert.ToDouble((int)op1));
                        else if (op1.GetType() == typeof(Boolean) && tipo.Equals("boolean")) ts.setValor(id, (Boolean)op1);
                        else if (op1.GetType() == typeof(DateTime) && tipo.Equals("date")) ts.setValor(id, (DateTime)op1);
                        else if (op1.GetType() == typeof(TimeSpan) && tipo.Equals("time")) ts.setValor(id, (TimeSpan)op1);
                        else if (op1.GetType() == typeof(InstanciaUserType))
                        {
                            InstanciaUserType temp = (InstanciaUserType)op1;
                            if (tipo.Equals(temp.tipo.ToLower()))
                            {
                                ts.setValor(id, temp);
                                return null;
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
                    }
                }
            }
            else mensajes.AddLast(mensa.error("La variable: " + id + " no existe en este ambito", l, c, "Semantico"));
            return null;
        }
    }
}
