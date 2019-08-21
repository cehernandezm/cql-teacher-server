using cql_teacher_server.CQL.Arbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cql_teacher_server.Herramientas;

namespace cql_teacher_server.CQL.Componentes
{
    public class Expresion : InstruccionCQL
    {
        Expresion a { set; get; }
        Expresion b { set; get; }

        Object valor { set; get; }

        string operacion { set; get; }

        int linea1 { set; get; }
        int columna1 { set; get; }

        /*
         * Constructor de la clase para una operacion binario:
         * SUMA,RESTA,MULTIPLICACION,DIVISION,MODULAR,MENOR,MAYOR,MENORIGUAL,MAYORIGUAL,IGUAL,POTENCIA
         * @a operador izquierdo
         * @b operador derecho
         * @operacion tipo de operacion.
         * @l1 linea del operador izquierdo
         * @c1 columna del operador izquierdo
         */
        public Expresion(Expresion a, Expresion b, string operacion,int l1 , int c1)
        {
            this.a = a;
            this.b = b;
            this.operacion = operacion;
            this.linea1 = l1;
            this.columna1 = c1;
        }

        /* 
         * Constructor de la clase para operaciones unarias:
         * NEGACION,NEGATIVO
         * @a operador izquierdo
         * @operacion tipo de operacion
         * @l1 linea del operador izquierdo
         * @c1 columna del operador izquierdo
         */
        public Expresion(Expresion a, string operacion, int l1, int c1)
        {
            this.a = a;
            this.operacion = operacion;
            this.linea1 = l1;
            this.columna1 = c1;
        }
        /* 
         * Constructor de la clase para valores puntuales:
         * CADENA,ENTERO,DECIMAL,FECHA,HORA,BOOLEAN
         * @valor operador izquierdo
         * @operacion tipo de operacion
         * @l1 linea del operador izquierdo
         * @c1 columna del operador izquierdo
         */
        public Expresion(object valor, string operacion, int linea1, int columna1)
        {
            this.valor = valor;
            this.operacion = operacion;
            this.linea1 = linea1;
            this.columna1 = columna1;
        }


        /*
         * Metodo de la implementacion de la clase InstruccionCQL
         * @ts Tabla de simbolos global
         * @user usuario que ejecuta la accion
         * @baseD base de datos en la que se realizara la accion, es pasada por referencia
         */

        public object ejecutar(TablaDeSimbolos ts, string user, ref string baseD,LinkedList<string> mensajes)
        {
            object op1 = (a == null) ? null : a.ejecutar(ts, user, ref baseD,mensajes);
            object op2 = (b == null) ? null : b.ejecutar(ts, user, ref baseD,mensajes);

            //-------------------------------------------------- TIPO DE OPERACION ------------------------------------------------------------
            //---------------------------------------------------------------------------------------------------------------------------------

            //-------------------------------------------------- SUMA -------------------------------------------------------------------------
            if (operacion.Equals("SUMA") && op1 != null && op2 != null)
            {
                if (op1.GetType() == typeof(int) && op2.GetType() == typeof(int)) return (int)op1 + (int)op2;
                else if (op1.GetType() == typeof(int) && op2.GetType() == typeof(Double)) return (Double)((int)op1 + (Double)op2);
                else if (op1.GetType() == typeof(Double) && op2.GetType() == typeof(int)) return (Double)((Double)op1 + (int)op2);
                else if (op1.GetType() == typeof(Double) && op2.GetType() == typeof(Double)) return (Double)((Double)op1 + (Double)op2);
                else if (op1.GetType() == typeof(string) && op2.GetType() == typeof(int)) return op1.ToString() + op2.ToString();
                else if (op1.GetType() == typeof(int) && op2.GetType() == typeof(string)) return op1.ToString() + op2.ToString();
                else if (op1.GetType() == typeof(string) && op2.GetType() == typeof(Double)) return op1.ToString() + op2.ToString();
                else if (op1.GetType() == typeof(Double) && op2.GetType() == typeof(string)) return op1.ToString() + op2.ToString();
                else if (op1.GetType() == typeof(string) && op2.GetType() == typeof(Boolean)) return op1.ToString() + op2.ToString();
                else if (op1.GetType() == typeof(Boolean) && op2.GetType() == typeof(string)) return op1.ToString() + op2.ToString();
                else if (op1.GetType() == typeof(string) && op2.GetType() == typeof(string)) return op1.ToString() + op2.ToString();
                else
                {
                    Mensaje mes = new Mensaje();
                    mensajes.AddLast(mes.error("No se puede sumar " + op1.ToString() + " con " + op2.ToString(), linea1, columna1));
                    return null;
                }
            }
            //--------------------------------------------------- RESTA ----------------------------------------------------------------------
            else if (operacion.Equals("RESTA") && op1 != null && op2 != null)
            {
                if (op1.GetType() == typeof(int) && op2.GetType() == typeof(int)) return (int)op1 - (int)op2;
                else if (op1.GetType() == typeof(int) && op2.GetType() == typeof(Double)) return (Double)((int)op1 - (Double)op2);
                else if (op1.GetType() == typeof(Double) && op2.GetType() == typeof(int)) return (Double)((Double)op1 - (int)op2);
                else if (op1.GetType() == typeof(Double) && op2.GetType() == typeof(Double)) return (Double)((Double)op1 - (Double)op2);
                else
                {
                    Mensaje mes = new Mensaje();
                    mensajes.AddLast(mes.error("No se puede restar " + op1.ToString() + " con " + op2.ToString(), linea1, columna1));
                    return null;
                }
            }
            //---------------------------------------------------- MULTIPLICACION --------------------------------------------------------------
            else if (operacion.Equals("MULTIPLICACION") && op1 != null && op2 != null)
            {
                if (op1.GetType() == typeof(int) && op2.GetType() == typeof(int)) return (int)op1 * (int)op2;
                else if (op1.GetType() == typeof(int) && op2.GetType() == typeof(Double)) return (Double)((int)op1 * (Double)op2);
                else if (op1.GetType() == typeof(Double) && op2.GetType() == typeof(int)) return (Double)((Double)op1 * (int)op2);
                else if (op1.GetType() == typeof(Double) && op2.GetType() == typeof(Double)) return (Double)((Double)op1 * (Double)op2);
                else
                {
                    Mensaje mes = new Mensaje();
                    mensajes.AddLast(mes.error("No se puede multiplicar " + op1.ToString() + " con " + op2.ToString(), linea1, columna1));
                    return null;
                }
            }
            //----------------------------------------------------- POTENCIA -------------------------------------------------------------------
            else if (operacion.Equals("POTENCIA") && op1 != null && op2 != null)
            {
                if (op1.GetType() == typeof(int) && op2.GetType() == typeof(int)) return (Double)(Math.Pow((int)op1, (int)op2));
                else if (op1.GetType() == typeof(int) && op2.GetType() == typeof(Double)) return (Double)(Math.Pow((int)op1, (Double)op2));
                else if (op1.GetType() == typeof(Double) && op2.GetType() == typeof(int)) return (Double)(Math.Pow((Double)op1, (int)op2));
                else if (op1.GetType() == typeof(Double) && op2.GetType() == typeof(Double)) return (Double)(Math.Pow((Double)op1, (Double)op2));
                else
                {
                    Mensaje mes = new Mensaje();
                    mensajes.AddLast(mes.error("No se puede obtener una potencia de " + op1.ToString() + " con " + op2.ToString(), linea1, columna1));
                    return null;
                }
            }
            //-------------------------------------------------------MODULO --------------------------------------------------------------------
            else if (operacion.Equals("MODULO") && op1 != null && op2 != null)
            {
                if (!op2.ToString().Equals("0"))
                {
                    if (op1.GetType() == typeof(int) && op2.GetType() == typeof(int)) return (int)op1 % (int)op2;
                    else if (op1.GetType() == typeof(int) && op2.GetType() == typeof(Double)) return (Double)((int)op1 % (Double)op2);
                    else if (op1.GetType() == typeof(Double) && op2.GetType() == typeof(int)) return (Double)((Double)op1 % (int)op2);
                    else if (op1.GetType() == typeof(Double) && op2.GetType() == typeof(Double)) return (Double)((Double)op1 % (Double)op2);
                    else
                    {
                        Mensaje mes = new Mensaje();
                        mensajes.AddLast(mes.error("No se puede obtener el modulo de  " + op1.ToString() + " con " + op2.ToString(), linea1, columna1));
                        return null;
                    }
                }
                else
                {
                    Mensaje mes = new Mensaje();
                    mensajes.AddLast(mes.error("No se puede obtener el modulo un divisor 0", linea1, columna1));
                    return null;
                }

            }
            //------------------------------------------------------DIVISION -------------------------------------------------------------------
            else if (operacion.Equals("DIVISION") && op1 != null && op2 != null)
            {
                if (!op2.ToString().Equals("0"))
                {
                    if (op1.GetType() == typeof(int) && op2.GetType() == typeof(int)) return (int)op1 / (int)op2;
                    else if (op1.GetType() == typeof(int) && op2.GetType() == typeof(Double)) return (Double)((int)op1 / (Double)op2);
                    else if (op1.GetType() == typeof(Double) && op2.GetType() == typeof(int)) return (Double)((Double)op1 / (int)op2);
                    else if (op1.GetType() == typeof(Double) && op2.GetType() == typeof(Double)) return (Double)((Double)op1 / (Double)op2);
                    else
                    {
                        Mensaje mes = new Mensaje();
                        mensajes.AddLast(mes.error("No se puede dividir  " + op1.ToString() + " con " + op2.ToString(), linea1, columna1));
                        return null;
                    }
                }
                else
                {
                    Mensaje mes = new Mensaje();
                    mensajes.AddLast(mes.error("No se puede dividir entre 0", linea1, columna1));
                    return null;
                }
            }
            //-----------------------------------------------------MAYOR------------------------------------------------------------------------
            else if (operacion.Equals("MAYOR") && op1 != null && op2 != null)
            {
                if (op1.GetType() == typeof(int) && op2.GetType() == typeof(int)) return (int)op1 > (int)op2;
                else if (op1.GetType() == typeof(int) && op2.GetType() == typeof(Double)) return (int)op1 > (Double)op2;
                else if (op1.GetType() == typeof(Double) && op2.GetType() == typeof(int)) return (Double)op1 > (int)op2;
                else if (op1.GetType() == typeof(Double) && op2.GetType() == typeof(Double)) return (Double)op1 > (Double)op2;
                else if (op1.GetType() == typeof(TimeSpan) && op2.GetType() == typeof(TimeSpan)) return (TimeSpan)op1 > (TimeSpan)op2;
                else if (op1.GetType() == typeof(DateTime) && op2.GetType() == typeof(DateTime)) return (DateTime)op1 > (DateTime)op2;
                else
                {
                    Mensaje mes = new Mensaje();
                    mensajes.AddLast(mes.error("No se puede conocer el mayor de   " + op1.ToString() + " con " + op2.ToString(), linea1, columna1));
                    return null;
                }
            }
            //-----------------------------------------------------MENOR------------------------------------------------------------------------
            else if (operacion.Equals("MENOR") && op1 != null && op2 != null)
            {
                if (op1.GetType() == typeof(int) && op2.GetType() == typeof(int)) return (int)op1 < (int)op2;
                else if (op1.GetType() == typeof(int) && op2.GetType() == typeof(Double)) return (int)op1 < (Double)op2;
                else if (op1.GetType() == typeof(Double) && op2.GetType() == typeof(int)) return (Double)op1 < (int)op2;
                else if (op1.GetType() == typeof(Double) && op2.GetType() == typeof(Double)) return (Double)op1 < (Double)op2;
                else if (op1.GetType() == typeof(TimeSpan) && op2.GetType() == typeof(TimeSpan)) return (TimeSpan)op1 < (TimeSpan)op2;
                else if (op1.GetType() == typeof(DateTime) && op2.GetType() == typeof(DateTime)) return (DateTime)op1 < (DateTime)op2;
                else
                {
                    Mensaje mes = new Mensaje();
                    mensajes.AddLast(mes.error("No se puede conocer el menor de   " + op1.ToString() + " con " + op2.ToString(), linea1, columna1));
                    return null;
                }
            }
            //-----------------------------------------------------MAYORIGUAL------------------------------------------------------------------------
            else if (operacion.Equals("MAYORIGUAL") && op1 != null && op2 != null)
            {
                if (op1.GetType() == typeof(int) && op2.GetType() == typeof(int)) return (int)op1 >= (int)op2;
                else if (op1.GetType() == typeof(int) && op2.GetType() == typeof(Double)) return (int)op1 >= (Double)op2;
                else if (op1.GetType() == typeof(Double) && op2.GetType() == typeof(int)) return (Double)op1 >= (int)op2;
                else if (op1.GetType() == typeof(Double) && op2.GetType() == typeof(Double)) return (Double)op1 >= (Double)op2;
                else if (op1.GetType() == typeof(TimeSpan) && op2.GetType() == typeof(TimeSpan)) return (TimeSpan)op1 >= (TimeSpan)op2;
                else if (op1.GetType() == typeof(DateTime) && op2.GetType() == typeof(DateTime)) return (DateTime)op1 >= (DateTime)op2;
                else
                {
                    Mensaje mes = new Mensaje();
                    mensajes.AddLast(mes.error("No se puede conocer el menor de   " + op1.ToString() + " con " + op2.ToString(), linea1, columna1));
                    return null;
                }
            }
            //------------------------------------------------- ENTERO -------------------------------------------------------------------------
            else if (operacion.Equals("ENTERO")) return Int32.Parse(valor.ToString());
            //------------------------------------------------- DOUBLE -------------------------------------------------------------------------
            else if (operacion.Equals("DECIMAL")) return Double.Parse(valor.ToString());
            //-------------------------------------------------- CADENA ------------------------------------------------------------------------
            else if (operacion.Equals("CADENA")) return valor.ToString();
            //--------------------------------------------------- BOOLEAN ----------------------------------------------------------------------
            else if (operacion.Equals("BOOLEAN")) return Boolean.Parse(valor.ToString());
            //---------------------------------------------------- HORA ------------------------------------------------------------------------
            else if (operacion.Equals("HORA")) return TimeSpan.Parse(valor.ToString());
            //-----------------------------------------------------FECHA------------------------------------------------------------------------
            else if (operacion.Equals("FECHA")) return DateTime.Parse(valor.ToString());
            return null;
        }
    }
}
