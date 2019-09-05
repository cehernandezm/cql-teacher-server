using cql_teacher_server.CQL.Arbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cql_teacher_server.Herramientas;
using System.Globalization;
using cql_teacher_server.CHISON.Componentes;
using cql_teacher_server.CHISON;

namespace cql_teacher_server.CQL.Componentes
{
    public class Expresion : InstruccionCQL
    {
        Expresion a { set; get; }

        Expresion b { set; get; }

        Expresion condicion { set; get; }

        Object valor { set; get; }

        string operacion { set; get; }

        int linea1 { set; get; }
        int columna1 { set; get; }

        string casteo { set; get; }

        string tipoA { set; get; }

        LinkedList<Expresion> listaUser { set; get; }
        string idAs { set; get; }
  
        string tipo1 { set; get; }

        string tipo2 { set; get; }


        /*
         * Constructor para instanciar map
         * @param {operacion} = CREAR MAP
         * @param {linea1} linea de los tipos
         * @param {columna} columna de los tipos
         * @param {tipo1} tipo de la key
         * @param {tipo2} tipo del valor
         */
        public Expresion(string operacion, int linea1, int columna1, string tipo1, string tipo2)
        {
            this.operacion = operacion;
            this.linea1 = linea1;
            this.columna1 = columna1;
            this.tipo1 = tipo1;
            this.tipo2 = tipo2;
        }



        /* 
         * Constructor de la clase para casteos explicitos tambien sirve para acceder a User Types,INCREMENTO Y DECREMENTO:
         * CADENA,ENTERO,DECIMAL,FECHA,HORA,BOOLEAN
         * @a expresion a evalular
         * @casteo a que se quiere convertir
         * @operacion tipo de operacion
         * @l1 linea del operador izquierdo
         * @c1 columna del operador izquierdo
         */
        public Expresion(Expresion a, string operacion, int linea1, int columna1, string casteo)
        {
            this.a = a;
            this.operacion = operacion;
            this.linea1 = linea1;
            this.columna1 = columna1;
            this.casteo = casteo;
        }


        /*
         * Constructor de la clase para una operacion binario:
         * SUMA,RESTA,MULTIPLICACION,DIVISION,MODULAR,MENOR,MAYOR,MENORIGUAL,MAYORIGUAL,IGUAL,POTENCIA, GETVALUE(map)
         * @a operador izquierdo
         * @b operador derecho
         * @operacion tipo de operacion.
         * @l1 linea del operador izquierdo
         * @c1 columna del operador izquierdo
         */
        public Expresion(Expresion a, Expresion b, string operacion, int l1, int c1)
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
         * Constructor de la clase para valores puntuales o listado de map:
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
         * Constructor de la clase para instacias de user types
         * @operacion el tipo de operacion que se realizara
         * @linea1 es la linea del id
         * @columna1 el la columna del id
         * @tipoB es el tipo de Instancia
         */

        public Expresion(string operacion, int linea1, int columna1, string tipoB)
        {
            this.operacion = operacion;
            this.linea1 = linea1;
            this.columna1 = columna1;
            this.tipoA = tipoB;
        }

        /*
         * Constructor de la clase para asignar valores a un usertype 
         * @operacion el tipo de operacion que se realizara
         * @linea1 es la linea del id
         * @columna es la columna del id
         * @lista es una lista de valores a asignar
         * @idAs es la referencia a que USER TYPE nos referimos
         */


        public Expresion(string operacion, int linea1, int columna1, LinkedList<Expresion> lista, string idAs)
        {
            this.operacion = operacion;
            this.linea1 = linea1;
            this.columna1 = columna1;
            this.listaUser = lista;
            this.idAs = idAs;
        }

        /*
         * Constructor de la clase para resolver operacion ternaria
         * @a Expresion si es verdadera
         * @b Expresion si es falsa
         * @condicion tiene que ser un boolean
         * @operacion el tipo de operacion que se realizara
         * @linea1 es la linea del id
         * @columna es la columna del id
         */

        public Expresion(Expresion a, Expresion b, Expresion condicion, string operacion, int linea1, int columna1)
        {
            this.a = a;
            this.b = b;
            this.condicion = condicion;
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

        public object ejecutar(TablaDeSimbolos ts, string user, ref string baseD, LinkedList<string> mensajes, TablaDeSimbolos tsT)
        {
            object op1 = (a == null) ? null : a.ejecutar(ts, user, ref baseD, mensajes,tsT);
            object op2 = (b == null) ? null : b.ejecutar(ts, user, ref baseD, mensajes,tsT);

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
                    mensajes.AddLast(mes.error("No se puede sumar " + op1.ToString() + " con " + op2.ToString(), linea1, columna1, "Semantico"));
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
                    mensajes.AddLast(mes.error("No se puede restar " + op1.ToString() + " con " + op2.ToString(), linea1, columna1, "Semantico"));
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
                    mensajes.AddLast(mes.error("No se puede multiplicar " + op1.ToString() + " con " + op2.ToString(), linea1, columna1, "Semantico"));
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
                    mensajes.AddLast(mes.error("No se puede obtener una potencia de " + op1.ToString() + " con " + op2.ToString(), linea1, columna1, "Semantico"));
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
                        mensajes.AddLast(mes.error("No se puede obtener el modulo de  " + op1.ToString() + " con " + op2.ToString(), linea1, columna1, "Semantico"));
                        return null;
                    }
                }
                else
                {
                    Mensaje mes = new Mensaje();
                    mensajes.AddLast(mes.error("No se puede obtener el modulo un divisor 0", linea1, columna1, "Semantico"));
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
                        mensajes.AddLast(mes.error("No se puede dividir  " + op1.ToString() + " con " + op2.ToString(), linea1, columna1, "Semantico"));
                        return null;
                    }
                }
                else
                {
                    Mensaje mes = new Mensaje();
                    mensajes.AddLast(mes.error("No se puede dividir entre 0", linea1, columna1, "Semantico"));
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
                    mensajes.AddLast(mes.error("No se puede conocer el mayor de   " + op1.ToString() + " con " + op2.ToString(), linea1, columna1, "Semantico"));
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
                    mensajes.AddLast(mes.error("No se puede conocer el menor de   " + op1.ToString() + " con " + op2.ToString(), linea1, columna1, "Semantico"));
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
                    mensajes.AddLast(mes.error("No se puede conocer el mayor igual de   " + op1.ToString() + " con " + op2.ToString(), linea1, columna1, "Semantico"));
                    return null;
                }
            }
            //-----------------------------------------------------MENORIGUAL------------------------------------------------------------------------
            else if (operacion.Equals("MENORIGUAL") && op1 != null && op2 != null)
            {
                if (op1.GetType() == typeof(int) && op2.GetType() == typeof(int)) return (int)op1 <= (int)op2;
                else if (op1.GetType() == typeof(int) && op2.GetType() == typeof(Double)) return (int)op1 <= (Double)op2;
                else if (op1.GetType() == typeof(Double) && op2.GetType() == typeof(int)) return (Double)op1 <= (int)op2;
                else if (op1.GetType() == typeof(Double) && op2.GetType() == typeof(Double)) return (Double)op1 <= (Double)op2;
                else if (op1.GetType() == typeof(TimeSpan) && op2.GetType() == typeof(TimeSpan)) return (TimeSpan)op1 <= (TimeSpan)op2;
                else if (op1.GetType() == typeof(DateTime) && op2.GetType() == typeof(DateTime)) return (DateTime)op1 <= (DateTime)op2;
                else
                {
                    Mensaje mes = new Mensaje();
                    mensajes.AddLast(mes.error("No se puede conocer el menor igual de   " + op1.ToString() + " con " + op2.ToString(), linea1, columna1, "Semantico"));
                    return null;
                }
            }
            //-----------------------------------------------------IGUALIGUAL------------------------------------------------------------------------
            else if (operacion.Equals("IGUALIGUAL"))
            {
                if (op1 != null && op2 != null)
                {
                    if (op1.GetType() == typeof(int) && op2.GetType() == typeof(int)) return (int)op1 == (int)op2;
                    else if (op1.GetType() == typeof(int) && op2.GetType() == typeof(Double)) return (int)op1 == (Double)op2;
                    else if (op1.GetType() == typeof(Double) && op2.GetType() == typeof(int)) return (Double)op1 == (int)op2;
                    else if (op1.GetType() == typeof(Double) && op2.GetType() == typeof(Double)) return (Double)op1 == (Double)op2;
                    else if (op1.GetType() == typeof(TimeSpan) && op2.GetType() == typeof(TimeSpan)) return (TimeSpan)op1 == (TimeSpan)op2;
                    else if (op1.GetType() == typeof(DateTime) && op2.GetType() == typeof(DateTime)) return (DateTime)op1 == (DateTime)op2;
                    else if (op1.GetType() == typeof(string) && op2.GetType() == typeof(string)) return op1.ToString().Equals(op2.ToString());
                    else if (op1.GetType() == typeof(Boolean) && op2.GetType() == typeof(Boolean)) return (Boolean)op1 == (Boolean)op2;
                    else if (op1.GetType() == typeof(InstanciaUserType) && op2.GetType() == typeof(InstanciaUserType)) return ((InstanciaUserType)op1).tipo.Equals(((InstanciaUserType)op2).tipo);
                    else
                    {
                        Mensaje mes = new Mensaje();
                        mensajes.AddLast(mes.error("No se puede conocer si es igual   " + op1.ToString() + " con " + op2.ToString(), linea1, columna1, "Semantico"));
                        return null;
                    }
                }
                else if (op1 != null && op2 == null)
                {
                    if (op1.GetType() == typeof(InstanciaUserType) && op2 == null) return ((InstanciaUserType)op1).lista == null;
                    else if (op1.GetType() == typeof(string) && op2 == null) return (string)op1 == null;
                    else if (op1.GetType() == typeof(DateTime) && op2 == null) return (DateTime)op1 == null;
                    else if (op1.GetType() == typeof(TimeSpan) && op2 == null) return (TimeSpan)op1 == null;
                    else if (op1.GetType() == typeof(Map) && op2 == null) return ((Map)op1).datos == null;
                    else
                    {
                        Mensaje mes = new Mensaje();
                        mensajes.AddLast(mes.error("No se puede conocer si es igual   " + op1.ToString() + " con " + op2.ToString(), linea1, columna1, "Semantico"));
                        return null;
                    }
                   
                }
                else if (op2 != null && op1 == null)
                {
                    if (op2.GetType() == typeof(InstanciaUserType) && op1 == null) return ((InstanciaUserType)op2).lista == null;
                    else if (op2.GetType() == typeof(string) && op1 == null) return (string)op2 == null;
                    else if (op2.GetType() == typeof(DateTime) && op1 == null) return (DateTime)op2 == null;
                    else if (op2.GetType() == typeof(TimeSpan) && op1 == null) return (TimeSpan)op2 == null;
                    else if (op2.GetType() == typeof(Map) && op1 == null) return ((Map)op2).datos == null;
                    else
                    {
                        Mensaje mes = new Mensaje();
                        mensajes.AddLast(mes.error("No se puede conocer si es igual   " + op2.ToString() + " con " + op2.ToString(), linea1, columna1, "Semantico"));
                        return null;
                    }
                }
                else return true;
            }
            //-----------------------------------------------------DIFERENTE------------------------------------------------------------------------
            else if (operacion.Equals("DIFERENTE"))
            {
                if (op1 != null && op2 != null)
                {
                    if (op1.GetType() == typeof(int) && op2.GetType() == typeof(int)) return (int)op1 != (int)op2;
                    else if (op1.GetType() == typeof(int) && op2.GetType() == typeof(Double)) return (int)op1 != (Double)op2;
                    else if (op1.GetType() == typeof(Double) && op2.GetType() == typeof(int)) return (Double)op1 != (int)op2;
                    else if (op1.GetType() == typeof(Double) && op2.GetType() == typeof(Double)) return (Double)op1 != (Double)op2;
                    else if (op1.GetType() == typeof(TimeSpan) && op2.GetType() == typeof(TimeSpan)) return (TimeSpan)op1 != (TimeSpan)op2;
                    else if (op1.GetType() == typeof(DateTime) && op2.GetType() == typeof(DateTime)) return (DateTime)op1 != (DateTime)op2;
                    else if (op1.GetType() == typeof(string) && op2.GetType() == typeof(string)) return op1.ToString().Equals(op2.ToString());
                    else if (op1.GetType() == typeof(Boolean) && op2.GetType() == typeof(Boolean)) return (Boolean)op1 != (Boolean)op2;
                    else if (op1.GetType() == typeof(InstanciaUserType) && op2.GetType() == typeof(InstanciaUserType)) return !((InstanciaUserType)op1).tipo.Equals(((InstanciaUserType)op2).tipo);
                    else
                    {
                        Mensaje mes = new Mensaje();
                        mensajes.AddLast(mes.error("No se puede conocer si es diferente   " + op1.ToString() + " con " + op2.ToString(), linea1, columna1, "Semantico"));
                        return null;
                    }
                }
                else if (op1 != null && op2 == null)
                {
                    if (op1.GetType() == typeof(InstanciaUserType) && op2 == null) return ((InstanciaUserType)op1).lista != null;
                    else if (op1.GetType() == typeof(string) && op2 == null) return (string)op1 != null;
                    else if (op1.GetType() == typeof(DateTime) && op2 == null) return (DateTime)op1 != null;
                    else if (op1.GetType() == typeof(TimeSpan) && op2 == null) return (TimeSpan)op1 != null;
                    else if (op1.GetType() == typeof(Map) && op2 == null) return ((Map)op1).datos != null;
                    else
                    {
                        Mensaje mes = new Mensaje();
                        mensajes.AddLast(mes.error("No se puede conocer si es diferente   " + op1.ToString() + " con " + op2.ToString(), linea1, columna1, "Semantico"));
                        return null;
                    }
                }
                else if (op2 != null && op1 == null)
                {
                    if (op2.GetType() == typeof(InstanciaUserType) && op1 == null) return ((InstanciaUserType)op2).lista != null;
                    else if (op2.GetType() == typeof(string) && op1 == null) return (string)op2 != null;
                    else if (op2.GetType() == typeof(DateTime) && op1 == null) return (DateTime)op2 != null;
                    else if (op2.GetType() == typeof(TimeSpan) && op1 == null) return (TimeSpan)op2 != null;
                    else if (op2.GetType() == typeof(Map) && op1 == null) return ((Map)op2).datos != null;
                    else
                    {
                        Mensaje mes = new Mensaje();
                        mensajes.AddLast(mes.error("No se puede conocer si es diferente   " + op2.ToString() + " con " + op2.ToString(), linea1, columna1, "Semantico"));
                        return null;
                    }
                }
                else return false;
            }
            //----------------------------------------------------- OR -----------------------------------------------------------------------------
            else if (operacion.Equals("OR") && op1 != null && op2 != null)
            {
                if (op1.GetType() == typeof(Boolean) && op2.GetType() == typeof(Boolean)) return (Boolean)op1 || (Boolean)op2;
                else
                {
                    Mensaje mes = new Mensaje();
                    mensajes.AddLast(mes.error("No se puede aplicar (OR) " + op1.ToString() + " con " + op2.ToString(), linea1, columna1, "Semantico"));
                    return null;
                }
            }
            //----------------------------------------------------- AND -----------------------------------------------------------------------------
            else if (operacion.Equals("AND") && op1 != null && op2 != null)
            {
                if (op1.GetType() == typeof(Boolean) && op2.GetType() == typeof(Boolean)) return (Boolean)op1 && (Boolean)op2;
                else
                {
                    Mensaje mes = new Mensaje();
                    mensajes.AddLast(mes.error("No se puede aplicar (AND) " + op1.ToString() + " con " + op2.ToString(), linea1, columna1, "Semantico"));
                    return null;
                }
            }
            //----------------------------------------------------- XOR -----------------------------------------------------------------------------
            else if (operacion.Equals("XOR") && op1 != null && op2 != null)
            {
                if (op1.GetType() == typeof(Boolean) && op2.GetType() == typeof(Boolean))
                {
                    if ((Boolean)op1 && (Boolean)op2) return false;
                    else if (!(Boolean)op1 && !(Boolean)op2) return false;
                    else return true;
                }
                else
                {
                    Mensaje mes = new Mensaje();
                    mensajes.AddLast(mes.error("No se puede aplicar (XOR) " + op1.ToString() + " con " + op2.ToString(), linea1, columna1, "Semantico"));
                    return null;
                }
            }
            //------------------------------------------------------ NEGATIVO ( - NUMERO ) ----------------------------------------------------------
            else if (operacion.Equals("NEGATIVO") && op1 != null)
            {
                if (op1.GetType() == typeof(int)) return (int)op1 * -1;
                else if (op1.GetType() == typeof(Double)) return (Double)op1 * -1;
                else
                {
                    Mensaje mes = new Mensaje();
                    mensajes.AddLast(mes.error("No se puede convertir a negativo a  " + op1.ToString(), linea1, columna1, "Semantico"));
                    return null;
                }
            }
            //------------------------------------------------------ NEGATIVO ( - NUMERO ) ----------------------------------------------------------
            else if (operacion.Equals("NEGACION") && op1 != null)
            {
                if (op1.GetType() == typeof(Boolean)) return !(Boolean)op1;
                else
                {
                    Mensaje mes = new Mensaje();
                    mensajes.AddLast(mes.error("No se puede convertir a negar a  " + op1.ToString(), linea1, columna1, "Semantico"));
                    return null;
                }
            }
            //------------------------------------------------------- CONVERSION EXPLICITA --------------------------------------------------------
            else if (operacion.Equals("CONVERSION") && op1 != null)
            {
                if (casteo.Equals("string")) return op1.ToString();
                else if (op1.GetType() == typeof(string) && casteo.Equals("int"))
                {
                    Boolean salida = false;
                    int datoS = 0;
                    Boolean conver = Int32.TryParse(op1.ToString(), out datoS);
                    if (!conver)
                    {
                        Mensaje men = new Mensaje();
                        mensajes.AddLast(men.error("No se puede castear: " + op1 + " to (int)", linea1, columna1, "Semantico"));
                        return null;
                    }
                    else return datoS;
                }
                else if (op1.GetType() == typeof(string) && casteo.Equals("time"))
                {
                    try
                    {
                        TimeSpan result = TimeSpan.ParseExact(op1.ToString(), "hh\\:mm\\:ss", CultureInfo.InvariantCulture);
                        return result;
                    }
                    catch (Exception e)
                    {
                        Mensaje men = new Mensaje();
                        mensajes.AddLast(men.error("No se puede castear: " + op1 + " to (Time)", linea1, columna1, "Semantico"));
                        return null;
                    }


                }
                else if (op1.GetType() == typeof(string) && casteo.Equals("double"))
                {
                    Boolean salida = false;
                    Double result;

                    Boolean conver = Double.TryParse(op1.ToString(), out result);
                    if (!conver)
                    {
                        Mensaje men = new Mensaje();
                        mensajes.AddLast(men.error("No se puede castear: " + op1 + " to (Double)", linea1, columna1, "Semantico"));
                        return null;
                    }
                    else return result;
                }
                else if (op1.GetType() == typeof(string) && casteo.Equals("date"))
                {

                    try
                    {
                        DateTime result = DateTime.ParseExact(op1.ToString(), "yyyy\\-MM\\-dd", CultureInfo.InvariantCulture);
                        return result;
                    }
                    catch (Exception e)
                    {
                        Mensaje men = new Mensaje();
                        mensajes.AddLast(men.error("No se puede castear: " + op1 + " to (Date)", linea1, columna1, "Semantico"));
                        return null;
                    }

                }
                else
                {
                    Mensaje men = new Mensaje();
                    mensajes.AddLast(men.error("No se puede castear: " + op1 + " to (" + casteo + ")", linea1, columna1, "Semantico"));
                    return null;
                }
            }
            //-------------------------------------------------------- INSTANCIA -------------------------------------------------------------------
            else if (operacion.Equals("INSTANCIA"))
            {
                BaseDeDatos db = TablaBaseDeDatos.getBase(baseD);
                if (db != null)
                {
                    User_Types a = TablaBaseDeDatos.getUserTypeV(tipoA.ToLower(), db);
                    if (a != null)
                    {
                        LinkedList<Atributo> lista = getAtributos(a, db);
                        if (lista != null)
                        {
                            return new InstanciaUserType(tipoA, lista);
                        }
                        else return null;

                    }
                    else
                    {
                        Mensaje men = new Mensaje();
                        mensajes.AddLast(men.error("No existe el USER TYPE " + tipoA, linea1, columna1, "Semantico"));
                        return null;
                    }
                }
                else
                {
                    Mensaje men = new Mensaje();
                    mensajes.AddLast(men.error("No se puede acceder a la base de datos: " + baseD + " asegurese de usar el comando USE", linea1, columna1, "Semantico"));
                    return null;
                }
            }
            //--------------------------------------------------------- ACCESO A USER TYPES ------------------------------------------------------
            else if (operacion.Equals("ACCESOUSER") && op1 != null)
            {
                if (op1.GetType() == typeof(InstanciaUserType))
                {
                    InstanciaUserType temp = (InstanciaUserType)op1;
                    foreach (Atributo a in temp.lista)
                    {
                        if (a.nombre.Equals(casteo)) return a.valor;
                    }
                    Mensaje men = new Mensaje();
                    mensajes.AddLast(men.error("No se encontro el atributo: " + casteo, linea1, columna1, "Semantico"));
                    return null;
                }
                else
                {
                    Mensaje men = new Mensaje();
                    mensajes.AddLast(men.error("Se necesita un User Type para acceder a sus atributos  ", linea1, columna1, "Semantico"));
                    return null;
                }
            }
            //--------------------------------------------------------- OBTENER EL ATRIBUTO DE ALGUN USER TYPE ------------------------------------------------------
            else if (operacion.Equals("GETATRIBUTO") && op1 != null)
            {
                if (op1.GetType() == typeof(InstanciaUserType))
                {
                    InstanciaUserType temp = (InstanciaUserType)op1;
                    foreach (Atributo a in temp.lista)
                    {
                        if (a.nombre.Equals(casteo))
                        {
                            if(a.valor != null)
                            {
                                if (a.valor.GetType() == typeof(InstanciaUserType)) return (InstanciaUserType)a.valor;
                            }
                            return (InstanciaUserType)op1;
                        }
                    }
                    Mensaje men = new Mensaje();
                    mensajes.AddLast(men.error("No se encontro el atributo: " + casteo, linea1, columna1, "Semantico"));
                    return null;
                }
                else
                {
                    Mensaje men = new Mensaje();
                    mensajes.AddLast(men.error("Se necesita un User Type para acceder a sus atributos  ", linea1, columna1, "Semantico"));
                    return null;
                }
            }
            //------------------------------------------------------- ASIGNACION DE VALORES A UN USER TYPE ---------------------------------------
            else if (operacion.Equals("ASIGNACIONUSER"))
            {
                BaseDeDatos db = TablaBaseDeDatos.getBase(baseD);
                if(db != null)
                {
                    User_Types a = TablaBaseDeDatos.getUserTypeV(idAs.ToLower(), db);
                    if(a != null)
                    {
                        LinkedList<Atributo> listaA = getAtributos(a, db);
                        if (listaA.Count() == listaUser.Count())
                        {
                            LinkedList<Atributo> newLista = compareListas(listaA, listaUser, ts, user, ref baseD, mensajes,tsT);
                            if (newLista == null) return null;

                            InstanciaUserType ius = new InstanciaUserType(idAs.ToLower(), newLista);
                            return ius;
                        }
                        else
                        {
                            Mensaje men = new Mensaje();
                            mensajes.AddLast(men.error("La cantidad de atributos no con cuerda con la del user type" + idAs, linea1, columna1, "Semantico"));
                            return null;
                        }
                    }
                    else
                    {
                        Mensaje men = new Mensaje();
                        mensajes.AddLast(men.error("No existe el USER TYPE " + idAs, linea1, columna1, "Semantico"));
                        return null;
                    }
                }
                else
                {
                    Mensaje men = new Mensaje();
                    mensajes.AddLast(men.error("No se puede acceder a la base de datos: " + baseD + " asegurese de usar el comando USE", linea1, columna1, "Semantico"));
                    return null;
                }
            }
            //-------------------------------------------------------- INCREMENTO ---------------------------------------------------------------
            else if(operacion.Equals("INCREMENTO") && op1 != null)
            {
                if(op1.GetType() == typeof(int))
                {
                    int copia = (int)op1;
                    ts.setValor(casteo, (int)op1 + 1);
                    return copia;
                }
                else if (op1.GetType() == typeof(Double))
                {
                    Double copia = (Double)op1;
                    ts.setValor(casteo, (Double)op1 + 1);
                    return copia;
                }
                else
                {
                    Mensaje men = new Mensaje();
                    mensajes.AddLast(men.error("No se puede hacer un incremento en la variable: " + casteo + " porque no es de tipo entero", linea1, columna1, "Semantico"));
                    return null;
                }
            }
            //-------------------------------------------------------- DECREMENTO ---------------------------------------------------------------
            else if (operacion.Equals("DECREMENTO") && op1 != null)
            {
                if (op1.GetType() == typeof(int))
                {
                    int copia = (int)op1;
                    ts.setValor(casteo, (int)op1 - 1);
                    return copia;
                }
                else if (op1.GetType() == typeof(Double))
                {
                    Double copia = (Double)op1;
                    ts.setValor(casteo, (Double)op1 - 1);
                    return (copia);
                }
                else
                {
                    Mensaje men = new Mensaje();
                    mensajes.AddLast(men.error("No se puede hacer un decremento en la variable: " + casteo + " porque no es de tipo entero", linea1, columna1, "Semantico"));
                    return null;
                }
            }
            //------------------------------------------------------ CREAR MAP -------------------------------------------------------------------
            else if (operacion.Equals("CREARMAP"))
            {
                Mensaje mensa = new Mensaje();
                if (tipo1.Equals("string") || tipo1.Equals("boolean") || tipo1.Equals("date") || tipo1.Equals("time") || tipo1.Equals("int") || tipo1.Equals("double"))
                {
                    string identificador = tipo1 + "/" + tipo2;
                    Map map = new Map(identificador, new LinkedList<KeyValue>());
                    return map;
                }
                else mensajes.AddLast(mensa.error("El tipo de la key para un map tiene que ser primitivo",linea1,columna1,"Semantico"));
                return null;
            }
            //------------------------------------------------------- CREACION DE MAP POR LISTA DE VALORES---------------------------------------
            else if (operacion.Equals("LISTAMAP"))
            {

                LinkedList<object> lista = (LinkedList<object>)valor;
                Map map = getMap(lista, ts, user, ref baseD, mensajes, tsT);
                if (map != null) return map;
                return null;

            }
            //--------------------------------------------------------- MAP . GET VALUE -----------------------------------------------------------
            else if (operacion.Equals("GETMAP"))
            {
                Mensaje ms = new Mensaje();
                if (op1 != null)
                {
                    if (op1.GetType() == typeof(Map))
                    {
                        Map temp = (Map)op1;
                        foreach(KeyValue keyValue in temp.datos)
                        {
                            if (keyValue.key.Equals(op2)) return keyValue.value; 
                        }
                        return null;
                    }
                    else mensajes.AddLast(ms.error("No se reconoce este tipo de MAP: " + op1.ToString(), linea1, columna1, "Semantico"));
                }
                else mensajes.AddLast(ms.error("No se puede aplicar GET a un NULL", linea1, columna1, "Semantico"));
            }
            //--------------------------------------------------------- TERNARIO -----------------------------------------------------------------
            else if (operacion.Equals("TERNARIO"))
            {
                object con = (condicion == null) ? null : condicion.ejecutar(ts, user, ref baseD, mensajes,tsT);
                if(con != null)
                {
                    if(con.GetType() == typeof(Boolean))
                    {
                        if ((Boolean)con) return op1;
                        return op2;
                    }
                    else
                    {
                        Mensaje men = new Mensaje();
                        mensajes.AddLast(men.error("No se reconoce la condicion: " + con.ToString(), linea1, columna1, "Semantico"));
                        return null;
                    }
                }
                else
                {
                    Mensaje men = new Mensaje();
                    mensajes.AddLast(men.error("No se puede tener una condicion null", linea1, columna1, "Semantico"));
                    return null;
                }
            }
            //---------------------------------------------------COUNT ------------------------------------------------------------------------
            else if (operacion.Equals("COUNT"))
            {
                Select select = (Select)valor;
                object res = select.ejecutar(ts, user, ref baseD, mensajes, tsT);
                if(res != null)
                {
                    mensajes.RemoveLast();
                    TablaSelect tabla = (TablaSelect)res;
                    return tabla.datos.Count();
                }
                return null;
            }
            //---------------------------------------------------MIN ------------------------------------------------------------------------
            else if (operacion.Equals("MIN"))
            {
                Mensaje mensa = new Mensaje();
                Select select = (Select)valor;
                object res = select.ejecutar(ts, user, ref baseD, mensajes, tsT);
                if (res != null)
                {
                    mensajes.RemoveLast();
                    TablaSelect tabla = (TablaSelect)res;
                    if(tabla.datos.Count() > 0)
                    {
                        if (tabla.columnas.ElementAt(0).tipo.Equals("int") || tabla.columnas.ElementAt(0).tipo.Equals("double") || tabla.columnas.ElementAt(0).tipo.Equals("date")) return getMinimun(tabla.datos);
                        else mensajes.AddLast(mensa.error("MIN solo se aplica a valores numericos, no se reconoce el tipo: " + tabla.columnas.ElementAt(0).tipo, linea1, columna1, "Semantico"));
                    }
                    else mensajes.AddLast(mensa.error("La tabla tiene que tener almenos un registro", linea1, columna1, "Semantico"));

                }
                return null;
            }
            //---------------------------------------------------MAX ------------------------------------------------------------------------
            else if (operacion.Equals("MAX"))
            {
                Mensaje mensa = new Mensaje();
                Select select = (Select)valor;
                object res = select.ejecutar(ts, user, ref baseD, mensajes, tsT);
                if (res != null)
                {
                    mensajes.RemoveLast();
                    TablaSelect tabla = (TablaSelect)res;
                    if (tabla.datos.Count() > 0)
                    {
                        if (tabla.columnas.ElementAt(0).tipo.Equals("int") || tabla.columnas.ElementAt(0).tipo.Equals("double") || tabla.columnas.ElementAt(0).tipo.Equals("date")) return getMaximo(tabla.datos);
                        else mensajes.AddLast(mensa.error("MMAX  solo se aplica a valores numericos o tipo date, no se reconoce el tipo: " + tabla.columnas.ElementAt(0).tipo, linea1, columna1, "Semantico"));
                    }
                    else mensajes.AddLast(mensa.error("La tabla tiene que tener almenos un registro", linea1, columna1, "Semantico"));

                }
                return null;
            }
            //---------------------------------------------------SUM ------------------------------------------------------------------------
            else if (operacion.Equals("SUM"))
            {
                Mensaje mensa = new Mensaje();
                Select select = (Select)valor;
                object res = select.ejecutar(ts, user, ref baseD, mensajes, tsT);
                if (res != null)
                {
                    mensajes.RemoveLast();
                    TablaSelect tabla = (TablaSelect)res;
                    if (tabla.datos.Count() > 0)
                    {
                        if (tabla.columnas.ElementAt(0).tipo.Equals("int") || tabla.columnas.ElementAt(0).tipo.Equals("double")) return getSum(tabla.datos);
                        else mensajes.AddLast(mensa.error("SUM  solo se aplica a valores numericos, no se reconoce el tipo: " + tabla.columnas.ElementAt(0).tipo, linea1, columna1, "Semantico"));
                    }
                    else mensajes.AddLast(mensa.error("La tabla tiene que tener almenos un registro", linea1, columna1, "Semantico"));

                }
                return null;
            }
            //---------------------------------------------------SUM ------------------------------------------------------------------------
            else if (operacion.Equals("AVG"))
            {
                Mensaje mensa = new Mensaje();
                Select select = (Select)valor;
                object res = select.ejecutar(ts, user, ref baseD, mensajes, tsT);
                if (res != null)
                {
                    mensajes.RemoveLast();
                    TablaSelect tabla = (TablaSelect)res;
                    if (tabla.datos.Count() > 0)
                    {
                        if (tabla.columnas.ElementAt(0).tipo.Equals("int") || tabla.columnas.ElementAt(0).tipo.Equals("double")) return getPromedio(tabla.datos);
                        else mensajes.AddLast(mensa.error("AVG  solo se aplica a valores numericos, no se reconoce el tipo: " + tabla.columnas.ElementAt(0).tipo, linea1, columna1, "Semantico"));
                    }
                    else mensajes.AddLast(mensa.error("La tabla tiene que tener almenos un registro", linea1, columna1, "Semantico"));

                }
                return null;
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
            //----------------------------------------------------- ID --------------------------------------------------------------------------
            else if (operacion.Equals("ID"))
            {
                object a = ts.getValor(valor.ToString().TrimStart().TrimEnd());
                if (a == null) return null;
                else if (a.ToString().Equals("none"))
                {
                    Mensaje me = new Mensaje();
                    mensajes.AddLast(me.error("La variable " + valor + " no existe en este ambito", linea1, columna1, "Semantico"));
                    return null;
                }
                else
                {
                    return a;
                }
            }
            //----------------------------------------------------- ID --------------------------------------------------------------------------
            else if (operacion.Equals("IDTABLA"))
            {
                object a = tsT.getValor(valor.ToString().TrimStart().TrimEnd());
                if (a == null) return null;
                else if (a.ToString().Equals("none"))
                {
                    Mensaje me = new Mensaje();
                    mensajes.AddLast(me.error("el campo " + valor + " no existe en este ambito", linea1, columna1, "Semantico"));
                    return null;
                }
                else return a;
                
            }
            //----------------------------------------------------- NULL ------------------------------------------------------------------------
            return null;
        }

        /*
         * METODO ENCARGADO DE CONTROLAR LOS DATOS DEL LISTADO
         * @param {listado} linkedlist con los valores de la lista
         * @param {ts} tabla de simbolos
         * @param {user} usuario que ejecuta la accion
         * @ref param {baseD} base de datos donde se ejecutara todo
         * @param {mensajes} outout
         * @param {tsT} variables de una tabla CQL
         * @return map o null
         */

         private Map getMap(LinkedList<object> listado, TablaDeSimbolos ts, string user, ref string baseD, LinkedList<string> mensajes, TablaDeSimbolos tsT)
        {
            Mensaje ms = new Mensaje();
            if (listado.Count() > 0)
            {
                Expresion exp1 = (Expresion)((KeyValue)(listado.ElementAt(0))).key;
                Expresion exp2 = (Expresion)((KeyValue)(listado.ElementAt(0))).value;

                object op1 = (exp1 == null) ? null : exp1.ejecutar(ts, user, ref baseD, mensajes, tsT);
                object op2 = (exp2 == null) ? null : exp2.ejecutar(ts, user, ref baseD, mensajes, tsT);


                string tipoKey = getTipoValorPrimario(op1, mensajes);
                string tipoValor = (getTipoValorSecundario(op2, mensajes) == null) ? "null" : getTipoValorSecundario(op2, mensajes);
                if(tipoKey != null)
                {
                    LinkedList<KeyValue> temporal = new LinkedList<KeyValue>();
                    foreach(object o in listado)
                    {
                        exp1 = (Expresion)((KeyValue)o).key;
                        exp2 = (Expresion)((KeyValue)o).value;
                        op1 = (exp1 == null) ? null : exp1.ejecutar(ts, user, ref baseD, mensajes, tsT);
                        op2 = (exp2 == null) ? null : exp2.ejecutar(ts, user, ref baseD, mensajes, tsT);
                        temporal.AddLast(new KeyValue(op1, op2));
                    }
                    if (!verificarValoresRepetidos(temporal, mensajes))
                    {
                        if (!verificarTipos(tipoKey, ref tipoValor, mensajes, temporal)) return null;
                        return new Map(tipoKey + "/" + tipoValor, temporal);
                    }
                }
            }
            else mensajes.AddLast(ms.error("La lista de valores tiene que tener al menos un key : value", linea1, columna1, "Semantico"));
            return null;
        }


        private Boolean verificarTipos(string tipoKey,ref string tipoValor,LinkedList<string> mensajes, LinkedList<KeyValue> lista)
        {
            Mensaje ms = new Mensaje();
            foreach(KeyValue valor in lista)
            {

                string tk = getTipoValorPrimario(valor.key, mensajes);
                string tv = (getTipoValorSecundario(valor.value, mensajes) == null) ? "null" : getTipoValorSecundario(valor.value, mensajes);
                if (tipoValor.Equals("null") && (!tv.Equals("null") && !tv.Equals("int") && !tv.Equals("double") && !tv.Equals("boolean"))) tipoValor = tv;
                
                if (!(tk.Equals(tipoKey) && tv.Equals(tipoValor)))
                {
                    mensajes.AddLast(ms.error("Los datos no son homogeneos <" + tipoKey + "," + tipoValor + "> con <" + tk + "," + tv + ">" , linea1, columna1, "Semantico"));
                    return false;
                }
            }
            return true;
        }

        /*
         * METODO QUE BUSCARA VALORES REPETIDO O VALORES NULL EN LAS KEY
         * @param {lista} lista de valores a guardar
         * @param {mensajes} output
         * @return true si encuentra uno repetido o un null, falso si no encuentra ningun repetido
         */
        private Boolean verificarValoresRepetidos(LinkedList<KeyValue> lista, LinkedList<string> mensajes)
        {
            Mensaje ms = new Mensaje();
            foreach(KeyValue keyValue in lista)
            {
                int repetido = 0;
                if(keyValue.key != null)
                {
                    foreach(KeyValue key2 in lista)
                    {
                        if(key2.key != null)
                        {
                            if (key2.key.Equals(keyValue.key)) repetido++;
                        }
                        else
                        {
                            mensajes.AddLast(ms.error("Map no acepta valores nulos como key", linea1, columna1, "Semantico"));
                            return true;
                        }
                    }

                    if (repetido > 1)
                    {
                        mensajes.AddLast(ms.error("El valor: " + keyValue.key + " esta repetido en la lista", linea1, columna1, "Semantico"));
                        return true;
                    }
                }
                else
                {
                    mensajes.AddLast(ms.error("Map no acepta valores nulos como key", linea1, columna1, "Semantico"));
                    return true;
                }

            }
            return false;
        }

        /*
         * METODO QUE DEVUELVE EL VALOR PERMITIDO EN UNA KEY
         * @param {valor} valor a guardar
         * @return string con tipo o null si no existe ese tipo como key posible
         */
        private string getTipoValorPrimario(object valor, LinkedList<string> mensajes)
        {
            Mensaje ms = new Mensaje();
            if(valor != null)
            {
                if (valor.GetType() == typeof(string)) return "string";
                else if (valor.GetType() == typeof(int)) return "int";
                else if (valor.GetType() == typeof(double)) return "double";
                else if (valor.GetType() == typeof(Boolean)) return "boolean";
                else if (valor.GetType() == typeof(DateTime)) return "date";
                else if (valor.GetType() == typeof(TimeSpan)) return "time";
                else
                {
                    mensajes.AddLast(ms.error("No se acepta este tipo de valor como clave primaria : " + valor, linea1, columna1, "Semantico"));
                    return null;
                }
            }
            mensajes.AddLast(ms.error("No se aceptan valores nulos como parte de una key", linea1, columna1, "Semantico"));
            return null;
        }

        /*
         * METODO QUE DEVUELVE EL VALOR PERMITIDO EN UNA VALUE
         * @param {valor} valor a guardar
         * @return string con tipo o null si no existe ese tipo como key posible
         */
        private string getTipoValorSecundario(object valor, LinkedList<string> mensajes)
        {
            Mensaje ms = new Mensaje();
            if (valor != null)
            {
                if (valor.GetType() == typeof(string)) return "string";
                else if (valor.GetType() == typeof(int)) return "int";
                else if (valor.GetType() == typeof(double)) return "double";
                else if (valor.GetType() == typeof(Boolean)) return "boolean";
                else if (valor.GetType() == typeof(DateTime)) return "date";
                else if (valor.GetType() == typeof(TimeSpan)) return "time";
                else if (valor.GetType() == typeof(InstanciaUserType)) return ((InstanciaUserType)valor).tipo;
                else if (valor.GetType() == typeof(Map)) return ((Map)valor).id;
                else
                {
                    mensajes.AddLast(ms.error("No se acepta este tipo de valor como clave Secundaria : " + valor, linea1, columna1, "Semantico"));
                    return null;
                }
            }
            return null;
        }

        /*
         * Metodo que recorre un USER TYPE y crear una declaracion de cada atributo
         * @u User type que se recorrera
         * @bd base de datos donde se encuentra el user type
         */
        private LinkedList<Atributo> getAtributos(User_Types u, BaseDeDatos bd)
        {
            LinkedList<Atributo> at = new LinkedList<Atributo>();
            foreach(Attrs a in u.type)
            {
                Atributo att;
                if (a.type.ToLower().Equals("string")) att = new Atributo(a.name.ToLower(), null, "string");
                else if (a.type.ToLower().Equals("date")) att = new Atributo(a.name.ToLower(), null, "date");
                else if (a.type.ToLower().Equals("time")) att = new Atributo(a.name.ToLower(), null, "time");
                else if (a.type.ToLower().Equals("int")) att = new Atributo(a.name.ToLower(), 0, "int");
                else if (a.type.ToLower().Equals("double")) att = new Atributo(a.name.ToLower(), 0.0, "double");
                else if (a.type.ToLower().Equals("boolean")) att = new Atributo(a.name.ToLower(), false, "boolean");
                else
                {
                    User_Types temp = TablaBaseDeDatos.getUserTypeV(a.type.ToLower(), bd);

                    if (temp != null)
                    {
                        LinkedList<Atributo> tempA = getAtributos(temp, bd);
                        InstanciaUserType tm = new InstanciaUserType(a.type, tempA);
                        if (tempA != null)
                        {
                            att = new Atributo(a.name.ToLower(), tm, a.type.ToLower());
                        }
                        else return null;
                    }
                    else return null;
                }
                at.AddLast(att);
            }
            return at;
        }

        /*
         * Metodo que recorre la lista de expresiones y la lista de atributos y los compara
         * 
         */

        private LinkedList<Atributo> compareListas(LinkedList<Atributo> a , LinkedList<Expresion> e, TablaDeSimbolos ts, string user,ref string baseD, LinkedList<string> mensaje,TablaDeSimbolos tsT)
        {
            LinkedList<Atributo> listaAtributo = new LinkedList<Atributo>();
            for (int i = 0; i < a.Count(); i++)
            {
                Atributo at = a.ElementAt(i);
                object operador1 = e.ElementAt(i).ejecutar(ts, user, ref baseD, mensaje,tsT);
                if (operador1 != null)
                {
                    if (at.tipo.Equals("string") && (operador1.GetType() == typeof(string))) listaAtributo.AddLast(new Atributo(at.nombre, (string)operador1, "string"));
                    else if (at.tipo.Equals("int") && (operador1.GetType() == typeof(int))) listaAtributo.AddLast(new Atributo(at.nombre, (int)operador1, "int"));
                    else if (at.tipo.Equals("double") && (operador1.GetType() == typeof(Double))) listaAtributo.AddLast(new Atributo(at.nombre, (Double)operador1, "double"));
                    else if (at.tipo.Equals("boolean") && (operador1.GetType() == typeof(Boolean))) listaAtributo.AddLast(new Atributo(at.nombre, (Boolean)operador1, "boolean"));
                    else if (at.tipo.Equals("date") && (operador1.GetType() == typeof(DateTime))) listaAtributo.AddLast(new Atributo(at.nombre, (DateTime)operador1, "date"));
                    else if (at.tipo.Equals("time") && (operador1.GetType() == typeof(TimeSpan))) listaAtributo.AddLast(new Atributo(at.nombre, (TimeSpan)operador1, "time"));
                    else if (operador1.GetType() == typeof(InstanciaUserType))
                    {
                        InstanciaUserType temp = (InstanciaUserType)operador1;
                        if(at.tipo.Equals(temp.tipo.ToLower())) listaAtributo.AddLast(new Atributo(at.nombre, temp, temp.tipo.ToLower()));
                        else
                        {
                            Mensaje men = new Mensaje();
                            mensaje.AddLast(men.error("No conincide el tipo: " + at.tipo + " con el Tipo: " + temp.tipo, linea1, columna1, "Semantico"));
                            return null;
                        }
                    }
                    else
                    {
                        Mensaje men = new Mensaje();
                        mensaje.AddLast(men.error("No coincide el tipo: " + at.tipo + " con el valor: " + operador1, linea1, columna1, "Semantico"));
                        return null;
                    }
                }
                else
                {
                    if (at.tipo.Equals("string") || at.tipo.Equals("int") || at.tipo.Equals("double") || at.tipo.Equals("map") || at.tipo.Equals("boolean") || at.tipo.Equals("date") || at.tipo.Equals("time"))
                    {
                        Mensaje men = new Mensaje();
                        mensaje.AddLast(men.error("No coincide el tipo: " + at.tipo + " con el valor: null " , linea1, columna1, "Semantico"));
                        return null;
                    }
                    InstanciaUserType temp = new InstanciaUserType(at.tipo, null);
                    listaAtributo.AddLast(new Atributo(at.nombre, temp, temp.tipo.ToLower()));
                }
            }
            return listaAtributo;
        }


        /*
         * Metodo que obtiene el minimo de una columna
         * @param {datas} toda la informacion de la tabla
         * @return {int|double|date}
         */
        private object getMinimun(LinkedList<Data> datas)
        {
            object minimo = datas.ElementAt(0).valores.ElementAt(0).valor;
            foreach(Data data in datas)
            {
                Atributo atributo = data.valores.ElementAt(0);
                if (atributo.tipo.Equals("int")) { if ((int)atributo.valor < (int)minimo) minimo = (int)atributo.valor; }
                else if (atributo.tipo.Equals("double")) { if ((Double)atributo.valor < (Double)minimo) minimo = (Double)atributo.valor; }
                else if (atributo.tipo.Equals("date")) { if ((DateTime)atributo.valor < (DateTime)minimo) minimo = (DateTime)atributo.valor; }
            }
            return minimo;
        }

        /*
         * Metodo que obtiene el maximo de una columna
         * @param {datas} toda la informacion de la tabla
         * @return {int|double|date}
         */
        private object getMaximo(LinkedList<Data> datas)
        {
            object minimo = datas.ElementAt(0).valores.ElementAt(0).valor;
            foreach (Data data in datas)
            {
                Atributo atributo = data.valores.ElementAt(0);
                if (atributo.tipo.Equals("int")) { if ((int)atributo.valor > (int)minimo) minimo = (int)atributo.valor; }
                else if (atributo.tipo.Equals("double")) { if ((Double)atributo.valor > (Double)minimo) minimo = (Double)atributo.valor; }
                else if (atributo.tipo.Equals("date")) { if ((DateTime)atributo.valor > (DateTime)minimo) minimo = (DateTime)atributo.valor; }
            }
            return minimo;
        }

        /*
        * Metodo que obtiene la suma de una columna
        * @param {datas} toda la informacion de la tabla
        * @return {int|double}
        */
        private object getSum(LinkedList<Data> datas)
        {
            object minimo = datas.ElementAt(0).valores.ElementAt(0).valor;
            foreach (Data data in datas)
            {
                Atributo atributo = data.valores.ElementAt(0);
                if (atributo.tipo.Equals("int"))  minimo = (int)minimo + (int)atributo.valor; 
                else if (atributo.tipo.Equals("double")) minimo = (Double)minimo + (Double)atributo.valor; 
            }
            return minimo;
        }

        /*
        * Metodo que obtiene el promedio de una columna
        * @param {datas} toda la informacion de la tabla
        * @return {int|double}
        */
        private object getPromedio(LinkedList<Data> datas)
        {
            object sum = getSum(datas);
            if (sum.GetType() == typeof(int)) return ((int)sum / datas.Count());
            else return (Double)((Double)sum / datas.Count());
        }
    }


}
