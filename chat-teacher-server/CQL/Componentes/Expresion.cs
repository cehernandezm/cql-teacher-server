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
         * Constructor de la clase para casteos explicitos tambien sirve para acceder a User Types,INCREMENTO Y DECREMENTO, IN:
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
                else if (op1.GetType() == typeof(Map) && op2.GetType() == typeof(Map))
                {
                    Mensaje ms = new Mensaje();
                    Map temp = (Map)op1;
                    Map temp2 = (Map)op2;
                    if (temp.id.Equals(temp2.id))return new Map(temp.id, new LinkedList<KeyValue>(temp.datos.Union(temp2.datos)));
  
                    else
                    {
                        mensajes.AddLast(ms.error("No coinciden los tipos de MAPS: " + temp.id + " con: " + temp2.id,linea1,columna1,"Semantico"));
                        return null;
                    }
                }
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
                    else if (op1.GetType() == typeof(InstanciaUserType) && op2.GetType() == typeof(InstanciaUserType)) return ((InstanciaUserType)op1).lista.Equals(((InstanciaUserType)op2).lista);
                    else if (op1.GetType() == typeof(Map) && op2.GetType() == typeof(Map)) return ((Map)op1).datos.Equals(((Map)op2).datos);
                    else if (op1.GetType() == typeof(List) && op2.GetType() == typeof(List)) return ((List)op1).lista.Equals(((List)op2).lista);
                    else if (op1.GetType() == typeof(Set) && op2.GetType() == typeof(Set)) return ((Set)op1).datos.Equals(((Set)op2).datos);
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
                    else if (op1.GetType() == typeof(InstanciaUserType) && op2.GetType() == typeof(InstanciaUserType)) return ! ((InstanciaUserType)op1).lista.Equals(((InstanciaUserType)op2).lista);
                    else if (op1.GetType() == typeof(Map) && op2.GetType() == typeof(Map)) return !((Map)op1).datos.Equals(((Map)op2).datos);
                    else if (op1.GetType() == typeof(List) && op2.GetType() == typeof(List)) return !((List)op1).lista.Equals(((List)op2).lista);
                    else if (op1.GetType() == typeof(Set) && op2.GetType() == typeof(Set)) return !((Set)op1).datos.Equals(((Set)op2).datos);
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
                        LinkedList<Atributo> lista = getAtributos(a, db,mensajes);
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
                        LinkedList<Atributo> listaA = getAtributos(a, db,mensajes);
                        if (listaA != null)
                        {
                            if (listaA.Count() == listaUser.Count())
                            {
                                LinkedList<Atributo> newLista = compareListas(listaA, listaUser, ts, user, ref baseD, mensajes, tsT);
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
                        else return null;
                        
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
            //-------------------------------------------------------- EXP IN EXP ------------------------------------------------------------------
            else if(operacion.Equals("IN") && op1 != null)
            {
                Mensaje ms = new Mensaje();
                object value = tsT.getValor(casteo);            
                if (!value.Equals("none"))
                {
                    LinkedList<object> listaValores = new LinkedList<object>();
                    if (op1.GetType() == typeof(List)) listaValores = ((List)op1).lista;
                    else if (op1.GetType() == typeof(Set)) listaValores = ((Set)op1).datos;
                    else
                    {
                        mensajes.AddLast(ms.error("La sentencia IN solo admite LIST O SET", linea1, columna1, "Semantico"));
                        return null;
                    }
                    return buscarValorIN(listaValores, value);
                }
                else mensajes.AddLast(ms.error("No se encuentra la columna: " + casteo,linea1,columna1,"Semantico"));
                return null;
            }
            //------------------------------------------------------ CREAR MAP -------------------------------------------------------------------
            else if (operacion.Equals("CREARMAP"))
            {
                Mensaje mensa = new Mensaje();
                if (tipo1.Equals("string") || tipo1.Equals("boolean") || tipo1.Equals("date") || tipo1.Equals("time") || tipo1.Equals("int") || tipo1.Equals("double"))
                {
                    string identificador = tipo1 + "," + tipo2;
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
            //-------------------------------------------------------- NEW LIST <TIPO> ---------------------------------------------------------
            else if (operacion.Equals("NEWLIST"))
            {
                return new List(valor.ToString(), new LinkedList<object>());
            }
            //-------------------------------------------------------- NEW SET <TIPO> ---------------------------------------------------------
            else if (operacion.Equals("NEWSET"))
            {
                return new Set(valor.ToString(), new LinkedList<object>());
            }
            //--------------------------------------------------------- LISTA LIST ------------------------------------------------------------------
            else if (operacion.Equals("LISTALIST"))
            {
                LinkedList<Expresion> lista = (LinkedList<Expresion>)valor;
                List list = getList(lista, ts, user, ref baseD, mensajes, tsT);
                if (list != null) return list;
                return null;
            }
            //--------------------------------------------------------- LISTA SET ------------------------------------------------------------------
            else if (operacion.Equals("LISTASET"))
            {
                LinkedList<Expresion> lista = (LinkedList<Expresion>)valor;
                List list = getList(lista, ts, user, ref baseD, mensajes, tsT);
                if (list != null)
                {
                    Set set = new Set(list.id, list.lista);
                    object valoresRepetidos = set.buscarRepetidos(mensajes,linea1,columna1);
                    if(valoresRepetidos!= null)
                    {
                        set.order();
                        return set;
                    }
                }
                return null;
            }
            //--------------------------------------------------------- EXPRESION . GET VALUE -----------------------------------------------------------
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
                        mensajes.AddLast(ms.error("No se encontro la key: " + op2, linea1, columna1, "Semantico"));
                        return null;
                    }
                    else if(op1.GetType() == typeof(List))
                    {
                        List temp = (List)op1;
                        if (op2 != null)
                        {
                            if (op2.GetType() == typeof(int))
                            {
                                int index = (int)op2;
                                if (index > -1)
                                {
                                    if (index < temp.lista.Count()) return temp.lista.ElementAt(index);
                                    else mensajes.AddLast(ms.error("El index supera el tamaño de la lista", linea1, columna1, "Semantico"));
                                }
                                else mensajes.AddLast(ms.error("El index debe ser positivo: " + index, linea1, columna1, "Semantico"));
                            }
                            else mensajes.AddLast(ms.error("El index debe ser tipo numerico no se reconoce: " + op2, linea1, columna1, "Semantico"));
                        }
                        else mensajes.AddLast(ms.error("No se permite un index null", linea1, columna1, "Semantico"));
                    }
                    else if (op1.GetType() == typeof(Set))
                    {
                        Set temp = (Set)op1;
                        if (op2 != null)
                        {
                            if (op2.GetType() == typeof(int))
                            {
                                int index = (int)op2;
                                if (index > -1)
                                {
                                    if (index < temp.datos.Count()) return temp.datos.ElementAt(index);
                                    else mensajes.AddLast(ms.error("El index supera el tamaño de la lista", linea1, columna1, "Semantico"));
                                }
                                else mensajes.AddLast(ms.error("El index debe ser positivo: " + index, linea1, columna1, "Semantico"));
                            }
                            else mensajes.AddLast(ms.error("El index debe ser tipo numerico no se reconoce: " + op2, linea1, columna1, "Semantico"));
                        }
                        else mensajes.AddLast(ms.error("No se permite un index null", linea1, columna1, "Semantico"));
                    }
                    else mensajes.AddLast(ms.error("No se reconoce este tipo de Collection: " + op1.ToString(), linea1, columna1, "Semantico"));
                }
                else mensajes.AddLast(ms.error("No se puede aplicar GET a un NULL", linea1, columna1, "Semantico"));
            }
            //-----------------------------------------------------------EXPRESION . SIZE ------------------------------------------------------
            else if (operacion.Equals("SIZE"))
            {
                Mensaje ms = new Mensaje();
                if(op1 != null)
                {
                    if (op1.GetType() == typeof(Map)) return ((Map)op1).datos.Count();
                    else if (op1.GetType() == typeof(List)) return ((List)op1).lista.Count();
                    else if (op1.GetType() == typeof(Set)) return ((Set)op1).datos.Count();
                    else mensajes.AddLast(ms.error("No se puede aplicar Size a un tipo no Collection: " + op1 ,linea1,columna1,"Semantico"));

                }
                mensajes.AddLast(ms.error("No se puede aplicar SIZE en null", linea1, columna1, "Semantico"));
                return null;
            }
            //--------------------------------------------------------- EXPRESION . CONTAINS VALUE -----------------------------------------------------------
            else if (operacion.Equals("CONTAINS"))
            {
                Mensaje ms = new Mensaje();
                if (op1 != null)
                {
                    if (op1.GetType() == typeof(Map))
                    {
                        Map temp = (Map)op1;
                        foreach (KeyValue keyValue in temp.datos)
                        {
                            if (keyValue.value.Equals(op2)) return true;
                        }
                        return false;
                    }
                    else if(op1.GetType() == typeof(List))
                    {
                        List temp = (List)op1;
                        foreach(object o in temp.lista)
                        {
                            if (o.Equals(op2)) return true;
                        }
                        return false;
                    }
                    else if (op1.GetType() == typeof(Set))
                    {
                        Set temp = (Set)op1;
                        foreach (object o in temp.datos)
                        {
                            if (o.Equals(op2)) return true;
                        }
                        return false;
                    }
                    else mensajes.AddLast(ms.error("No se reconoce este tipo de Collection: " + op1.ToString(), linea1, columna1, "Semantico"));
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



        private Boolean buscarValorIN(LinkedList<object> lista, object value)
        {
            foreach(object o in lista)
            {
                if (o.Equals(value)) return true;
            }
            return false;
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
        private List getList(LinkedList<Expresion> lista,TablaDeSimbolos ts, string user, ref string baseD,LinkedList<string> mensajes,TablaDeSimbolos tsT)
        {
            Mensaje ms = new Mensaje();
            string tipo = "none";
            LinkedList<object> valores = new LinkedList<object>();
            foreach(Expresion e in lista)
            {
                object res = (e == null) ? null : e.ejecutar(ts, user, ref baseD, mensajes, tsT);
                string tp = (getTipoValorSecundario(res, mensajes) == null) ? "null" : getTipoValorSecundario(res, mensajes);
                if (tipo.Equals("none")) tipo = tp;
                if (tipo.Equals(tp)) valores.AddLast(res);
                else
                {
                    if (tp.Equals("null"))
                    {
                        if (tipo.Equals("int") || tipo.Equals("double") || tipo.Equals("map") || tipo.Equals("list") || tipo.Equals("boolean")){
                            mensajes.AddLast(ms.error("No puede existir un valor null con el tipo: " + tipo, linea1, columna1, "Semantico"));
                            return null;
                        }
                        else if(tipo.Equals("string") || tipo.Equals("date") || tipo.Equals("time")) valores.AddLast(tp);
                        else valores.AddLast(new InstanciaUserType(tipo,new LinkedList<Atributo>()));  
                    }
                    else
                    {
                        mensajes.AddLast(ms.error("No coincide el tipo: " + tipo + " con : " + tp, linea1, columna1, "Semantico"));
                        return null;
                    }
                    
                    
                }
                 
            }
            List list = new List(tipo, valores);
            return list;
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
                if (tipoKey != null)
                {
                    LinkedList<KeyValue> temporal = new LinkedList<KeyValue>();
                    foreach (object o in listado)
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
                        return new Map(tipoKey + "," + tipoValor, temporal);
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
                else if (valor.GetType() == typeof(Map)) return "map<" +((Map)valor).id + ">";
                else if (valor.GetType() == typeof(List)) return "list<"+ ((List)valor).id + ">";
                else if (valor.GetType() == typeof(Set)) return "set<" + ((Set)valor).id + ">";
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
        private LinkedList<Atributo> getAtributos(User_Types u, BaseDeDatos bd, LinkedList<string> mensajes)
        {
            Mensaje ms = new Mensaje();
            LinkedList<Atributo> at = new LinkedList<Atributo>();
            foreach (Attrs a in u.type)
            {
                System.Diagnostics.Debug.WriteLine("Este es el tipo: " + a.type);
                Atributo att;
                if (a.type.ToLower().Equals("string")) att = new Atributo(a.name.ToLower(), null, "string");
                else if (a.type.ToLower().Equals("date")) att = new Atributo(a.name.ToLower(), null, "date");
                else if (a.type.ToLower().Equals("time")) att = new Atributo(a.name.ToLower(), null, "time");
                else if (a.type.ToLower().Equals("int")) att = new Atributo(a.name.ToLower(), 0, "int");
                else if (a.type.ToLower().Equals("double")) att = new Atributo(a.name.ToLower(), 0.0, "double");
                else if (a.type.ToLower().Equals("boolean")) att = new Atributo(a.name.ToLower(), false, "boolean");
                else if (a.type.ToLower().Contains("list"))
                {
                    string tipo = a.type.ToLower().TrimStart('l').TrimStart('i').TrimStart('s').TrimStart('t').TrimStart('<').TrimEnd('>');
                    att = new Atributo(a.name.ToLower(), new List(tipo, new LinkedList<object>()), "list");
                }
                else if (a.type.ToLower().Contains("set"))
                {
                    string tipo = a.type.ToLower().TrimStart('s').TrimStart('e').TrimStart('t').TrimStart('<').TrimEnd('>');
                    att = new Atributo(a.name.ToLower(), new Set(tipo, new LinkedList<object>()), "set");
                }
                else if (a.type.ToLower().Contains("map"))
                {
                    string tipo = a.type.ToLower().TrimStart('m').TrimStart('a').TrimStart('p').TrimStart('<').TrimEnd('>');
                    string tipoKey = tipo.Split(new[] { ',' }, 2)[0];
                    string tipoValue = tipo.Split(new[] { ',' }, 2)[1];
                    if (cheackPrimaryKey(tipoKey)) att = new Atributo(a.name.ToLower(), new Map(tipoKey + "," + tipoValue, new LinkedList<KeyValue>()), "map");
                    else
                    {
                        mensajes.AddLast(ms.error("El tipo map no puede tener este tipo: " + tipoKey + " como tipo de key", linea1, columna1, "Semantico"));
                        return null;
                    }
                }
                else
                {
                    User_Types temp = TablaBaseDeDatos.getUserTypeV(a.type.ToLower(), bd);

                    if (temp != null)
                    {
                        LinkedList<Atributo> tempA = getAtributos(temp, bd,mensajes);
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
            Mensaje ms = new Mensaje();
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
                    else if (at.tipo.Equals("list") && (operador1.GetType() == typeof(List)))
                    {
                        List actual = (List)at.valor;
                        List temp = (List)operador1;
                        if (actual.id.Equals(temp.id)) listaAtributo.AddLast(new Atributo(at.nombre, temp, "list"));
                        else
                        {
                            mensaje.AddLast(ms.error("No coinciden los tipos de las listas: " + actual.id + " con: " + temp.id, linea1, columna1, "Semantico"));
                            return null;
                        }
                    }
                    else if (at.tipo.Equals("set") && (operador1.GetType() == typeof(Set)))
                    {
                        Set actual = (Set)at.valor;
                        Set temp = (Set)operador1;
                        if (actual.id.Equals(temp.id)) listaAtributo.AddLast(new Atributo(at.nombre, temp, "set"));
                        else
                        {
                            mensaje.AddLast(ms.error("No coinciden los tipos de las Set: " + actual.id + " con: " + temp.id, linea1, columna1, "Semantico"));
                            return null;
                        }
                    }
                    else if (at.tipo.Equals("map") && (operador1.GetType() == typeof(Map)))
                    {
                        Map actual = (Map)at.valor;
                        Map temp = (Map)operador1;
                        if (actual.id.Equals(temp.id)) listaAtributo.AddLast(new Atributo(at.nombre, temp, "map"));
                        else
                        {
                            mensaje.AddLast(ms.error("No coinciden los tipos de los MAP: " + actual.id + " con: " + temp.id, linea1, columna1, "Semantico"));
                            return null;
                        }
                    }
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
                    if(at.tipo.Equals("string") || at.tipo.Equals("date") || at.tipo.Equals("time")) listaAtributo.AddLast(new Atributo(at.nombre, null, at.tipo));
                    else if (at.tipo.Contains("map") || at.tipo.Equals("int") || at.tipo.Equals("double") || at.tipo.Contains("list") || at.tipo.Equals("boolean") || at.tipo.Contains("set"))
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


        /*
         * METODOQ QUE OBTENDRA EL VALOR DE LA KEY PRINCIPAL PARA MAPS
         * @param {tipo} tipo de key
         * @return false si hay un error | true si todo esta correcto
         */

        private Boolean cheackPrimaryKey(string tipo)
        {
            if (!tipo.Equals("string") && !tipo.Equals("int") && !tipo.Equals("double") && !tipo.Equals("boolean") && !tipo.Equals("date") && !tipo.Equals("time")) return false;
            return true;
        }
    }


}
