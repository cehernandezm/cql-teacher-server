using cql_teacher_server.CQL.Arbol;
using cql_teacher_server.CHISON.Componentes;
using cql_teacher_server.Herramientas;
using System;
using cql_teacher_server.CHISON;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cql_teacher_server.CQL.Componentes.Try_Catch;

namespace cql_teacher_server.CQL.Componentes
{
    public class Asignacion : InstruccionCQL
    {
        string id { set; get; }
        int l { set; get; }
        int c { set; get; }

        Expresion a { set; get; }

        Expresion atributo { set; get; }

        string operacion { set; get; }

        public TablaDeSimbolos tPadre { set; get; }

        /*
         * Constructor de la clase que asigna valores a una id en especifico
         * @id el nombre de la varibale
         * @l linea del nombre de la variable
         * @c columna del nombre de la variable
         * @a valor a guardar
         * @operacion que se realizara
         */
        public Asignacion(string id, int l, int c, Expresion a, string operacion)
        {
            this.id = id;
            this.l = l;
            this.c = c;
            this.a = a;
            this.operacion = operacion;
            this.tPadre = null;
        }


        /*
        * Constructor de la clase que asigna valores a una id en especifico
        * @id el nombre de la varibale
        * @l linea del nombre de la variable
        * @c columna del nombre de la variable
        * @a valor a guardar
        * @operacion que se realizara
        */
        public Asignacion(string id, int l, int c, Expresion a, Expresion b, string operacion)
        {
            this.id = id;
            this.l = l;
            this.c = c;
            this.a = b;
            this.atributo = a;
            this.operacion = operacion;
            this.tPadre = null;
        }



        /*
         * Metodo que se implementa de la clase padre
         * @ts tabla de simbolos
         * @user usuario que ejecuta la accion
         * @baseD base de datos en la que se esta trabajando
         * @mensajes respuesta de errores o mensajes de salida
         */

        public object ejecutar(TablaDeSimbolos ts,Ambito ambito, TablaDeSimbolos tsT)
        {

            Mensaje mensa = new Mensaje();
            TablaDeSimbolos tablaT;
            LinkedList<string> mensajes = ambito.mensajes;
            if (tPadre != null) tablaT = tPadre;
            else tablaT = ts;


            object op1 = (a == null) ? null : a.ejecutar(tablaT,ambito,tsT);
            object atri = (atributo == null) ? null : atributo.ejecutar(tablaT, ambito,tsT);
            //--------------------------------------------- REALIZA UNA ASIGNACION NORMAL---------------------------------------------------------------
            if (operacion.Equals("ASIGNACION"))
            {
                string tipo = ts.getTipo(id);
                tipo = tipo.ToLower().TrimEnd().TrimStart();
                if (!tipo.Equals("none"))
                {
                    return checkValues(op1, tipo, a, mensajes, ts);
                }
                else mensajes.AddLast(mensa.error("La variable: " + id + " no existe en este ambito", l, c, "Semantico"));
            }
            //----------------------------------------------- REALIZA LA ASIGNACION DE UN ATRIBUTO ------------------------------------------------------
            else if(operacion.Equals("ASIGNACIONA"))
            {
                if(atri != null)
                {
                    if(atri.GetType() == typeof(InstanciaUserType))
                    {
                        InstanciaUserType tempa = (InstanciaUserType)atri;
                        if(tempa.lista != null)
                        {
                            foreach (Atributo at in tempa.lista)
                            {
                                if (at.nombre.Equals(id))
                                {
                                    string tipo = at.tipo.ToLower();
                                    if (a == null)
                                    {
                                        if (tipo.Equals("string") || tipo.Equals("date") || tipo.Equals("time")) at.valor = null;
                                        else if (!tipo.Equals("int") && !tipo.Equals("boolean") && !tipo.Equals("double") && !tipo.Contains("map") && !tipo.Contains("list") && !tipo.Contains("set"))
                                        {
                                            InstanciaUserType temp = new InstanciaUserType(tipo, null);
                                            at.valor = temp;
                                        }
                                        else
                                        {
                                            mensajes.AddLast(mensa.error("No se le puede asignar al atributo: " + at.nombre + " el valor: null", l, c, "Semantico"));
                                            return null;

                                        }
                                        return "";
                                    }
                                    else
                                    {
                                        if (op1 != null)
                                        {

                                            if (op1.GetType() == typeof(string) && tipo.Equals("string")) at.valor = (string)op1;
                                            else if (op1.GetType() == typeof(int) && tipo.Equals("int")) at.valor = (int)op1;
                                            else if (op1.GetType() == typeof(int) && tipo.Equals("double")) at.valor = Convert.ToInt32((Double)op1);
                                            else if (op1.GetType() == typeof(Double) && tipo.Equals("double")) at.valor = (Double)op1;
                                            else if (op1.GetType() == typeof(Double) && tipo.Equals("int")) at.valor = Convert.ToDouble((int)op1);
                                            else if (op1.GetType() == typeof(Boolean) && tipo.Equals("boolean")) at.valor = (Boolean)op1;
                                            else if (op1.GetType() == typeof(DateTime) && tipo.Equals("date")) at.valor = (DateTime)op1;
                                            else if (op1.GetType() == typeof(TimeSpan) && tipo.Equals("time")) at.valor = (TimeSpan)op1;
                                            else if (op1.GetType() == typeof(Map) && tipo.Equals("map"))
                                            {
                                                Map temp = (Map)op1;
                                                Map valor = (Map)at.valor;
                                                if (valor.id.Equals(temp.id))
                                                {
                                                    at.valor = temp;
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
                                                List valor = (List)at.valor;
                                                if (valor.id.Equals(temp.id))
                                                {
                                                    at.valor = temp;
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
                                                Set original = (Set)at.valor;
                                                Set temp = (Set)op1;
                                                if (original.id.Equals(temp.id))
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
                                                if (tipo.Equals(temp.tipo.ToLower())) at.valor = temp;
                                                else
                                                {
                                                    mensajes.AddLast(mensa.error("No se le puede asignar al atributo " + at.nombre + " el valor: " + op1, l, c, "Semantico"));
                                                    return null;
                                                }

                                            }
                                            else
                                            {
                                                mensajes.AddLast(mensa.error("No se le puede asignar al atributo: " + at.nombre + " el valor: " + op1, l, c, "Semantico"));
                                                return null;

                                            }
                                            return "";
                                        }
                                        else
                                        {
                                            if (tipo.Equals("string") || tipo.Equals("date") || tipo.Equals("time")) at.valor = null;
                                            else if(tipo.Equals("int") || tipo.Equals("double") || tipo.Equals("boolean") || tipo.Equals("map") || tipo.Equals("list") || tipo.Equals("set"))
                                            {
                                                mensajes.AddLast(mensa.error("No se le puede asignar al atributo: " + at.nombre + " el valor: null" , l, c, "Semantico"));

                                                return null;
                                            }
                                            else
                                            {
                                                at.valor = null;
                                            }
                                            return "";
                                        }
                                    }
                                    return null;
                                }
                            }
                            ambito.listadoExcepciones.AddLast(new Excepcion("exception", "No existe el atributo:" + id));
                            mensajes.AddLast(mensa.error("No existe el atributo:" + id, l, c, "Semantico"));
                        }
                        
                        
                    }
                    else  mensajes.AddLast(mensa.error("Para acceder a un atributo se necesita que sea de tipo USERTYPE no se reconoce: " + atri.ToString(), l, c, "Semantico"));
                    
                    
                }
                ambito.listadoExcepciones.AddLast(new Excepcion("nullpointerexception", "No se puede asignar un valor a un null"));

            }

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
        private object checkValues(object op1, string tipo, object a, LinkedList<string> mensajes, TablaDeSimbolos ts)   
        {
            Mensaje mensa = new Mensaje();
            if (a == null)
            {
                if (tipo.Equals("string") || tipo.Equals("date") || tipo.Equals("time")) ts.setValor(id, null);
                else if (!tipo.Equals("int") && !tipo.Equals("boolean") && !tipo.Equals("double") && !tipo.Contains("map") && !tipo.Contains("list") && !tipo.Contains("set"))
                {
                    InstanciaUserType temp = new InstanciaUserType(tipo, null);
                    ts.setValor(id, temp);
                }
                else
                {
                    mensajes.AddLast(mensa.error("No se le puede asignar a la variable: " + id + " el valor: null", l, c, "Semantico"));
                    return null;

                }
                return "";
            }
            else
            {
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
                    else if(op1.GetType() == typeof(List) && tipo.Equals("list"))
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
                    else if(op1.GetType() == typeof(LinkedList<object>))
                    {
                        LinkedList<object> listaV = (LinkedList<object>)op1;
                        if (listaV.Count() == 1)return checkValues(listaV.ElementAt(0), tipo, "", mensajes, ts);
                        else mensajes.AddLast(mensa.error("La lista de valores es mayor a la lista de variables",l,c,"Semantico"));
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
                    else if (tipo.Equals("int") || tipo.Equals("double") || tipo.Equals("map") || tipo.Equals("set") || tipo.Equals("list") || tipo.Equals("cursor") || tipo.Equals("boolean"))
                    {
                        mensajes.AddLast(mensa.error("No se le puede asignar un valor null al tipo: " + tipo, l, c, "Semantico"));
                        return null;
                    }
                    else ts.setValor(id, new InstanciaUserType(tipo, null));
                    return "";
                }
            }
            return null;
        }
    }
}
