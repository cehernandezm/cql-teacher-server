using cql_teacher_server.CHISON;
using cql_teacher_server.CHISON.Componentes;
using cql_teacher_server.CQL.Arbol;
using cql_teacher_server.CQL.Componentes.Cursor;
using cql_teacher_server.CQL.Componentes.Try_Catch;
using cql_teacher_server.Herramientas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes
{
    public class Declaracion : InstruccionCQL
    {
         public string tipo { set; get; }
        Expresion valor { set; get; }

        public string id { set; get; }

        int l { set; get; }
        int c { set; get; }

        public Boolean parametro { set; get; }

        /*
         * Constructor de la clase
         * @tipo es el tipo de variable
         * @valor es el valor a guardar
         * @id de la variable
         * @l linea de codigo
         * @c columna de codigo
         */
        public Declaracion(string tipo, Expresion valor, string id, int l, int c)
        {
            this.tipo = tipo;
            this.valor = valor;
            this.id = id;
            this.l = l;
            this.c = c;
            this.parametro = false;
        }

        /*
         * Metodo de la implementacion
         * @ts tabla de simbolos global
         * @user usuario que ejecuta la accion
         * @baseD base de datos donde estamos ejecutando todo
         * @mensajes linkedlist con la salida deseada
         */
        public object ejecutar(TablaDeSimbolos ts, Ambito ambito,TablaDeSimbolos tsT)
        {
            Mensaje mensa = new Mensaje();
            object a = (valor == null) ? null : valor.ejecutar(ts,ambito,tsT);
            LinkedList<string> mensajes = ambito.mensajes;
            object res = ts.getValor(id);
            string baseD = ambito.baseD;
            string existe = (res == null) ? "si" : res.ToString();
            if (parametro)
            {
                ts.AddLast(new Simbolo(tipo, id));
                if (tipo.Equals("list")) ts.setValor(id, new List("none", new LinkedList<object>()));
                else if (tipo.Equals("set")) ts.setValor(id, new Set("none", new LinkedList<object>()));
                else if (tipo.Equals("map")) ts.setValor(id, new Map("none", new LinkedList<KeyValue>()));
                else if (tipo.Equals("string") || tipo.Equals("int") || tipo.Equals("double") || tipo.Equals("date") || tipo.Equals("time") || tipo.Equals("boolean") || tipo.Equals("cursor")) { }
                else ts.setValor(id, new InstanciaUserType("none", new LinkedList<Atributo>()));

                return "";
            }
            else if (existe.Equals("none"))
            {
                if(valor == null)
                {
                    if (tipo.Equals("int"))
                    {
                        ts.AddLast(new Simbolo(tipo, id));
                        ts.setValor(id, 0);
                    }
                    else if (tipo.Equals("double"))
                    {
                        ts.AddLast(new Simbolo(tipo, id));
                        ts.setValor(id, 0.0);
                    }
                    else if (tipo.Equals("boolean"))
                    {
                        ts.AddLast(new Simbolo(tipo, id));
                        ts.setValor(id, false);
                    }
                    else if (tipo.Equals("string") || tipo.Equals("date") || tipo.Equals("time"))
                    {
                        ts.AddLast(new Simbolo(tipo, id));
                        ts.setValor(id, null);
                        return "";
                    }
                    else if (tipo.Equals("map"))
                    {
                        mensajes.AddLast(mensa.error("El tipo MAP necesita ser instanciado", l, c, "Semantico"));
                        return null;
                    }
                    else if (tipo.Equals("list"))
                    {
                        mensajes.AddLast(mensa.error("El tipo LIST necesita ser instanciado", l, c, "Semantico"));
                        return null;
                    }
                    else if (tipo.Equals("set"))
                    {
                        mensajes.AddLast(mensa.error("El tipo SET necesita ser instanciado", l, c, "Semantico"));
                        return null;
                    }
                    else if (tipo.Equals("cursor"))
                    {
                        mensajes.AddLast(mensa.error("El tipo CURSOR necesita ser instanciado", l, c, "Semantico"));
                        return null;
                    }
                    else
                    {
                        BaseDeDatos bd = TablaBaseDeDatos.getBase(baseD);
                        if(bd != null)
                        {
                            if (TablaBaseDeDatos.getUserType(tipo.ToLower(), bd))
                            {
                                ts.AddLast(new Simbolo(tipo, id));
                                ts.setValor(id, new InstanciaUserType(tipo,null));
                            }
                            else
                            {
                                mensajes.AddLast(mensa.error("El tipo " + tipo + " no es un userType en esta base de datos: " + baseD, l, c, "Semantico"));
                                return null;
                            }

                        }
                        return "";
                        
                    }
                   
                }
                else
                {
                    if ( a != null)
                    {

                   
                        if(tipo.Equals("string") && a.GetType() == typeof(string))
                        {
                            ts.AddLast(new Simbolo(tipo, id));
                            ts.setValor(id, (string)a);
                        }
                        else if (tipo.Equals("int") && a.GetType() == typeof(int))
                        {
                            ts.AddLast(new Simbolo(tipo, id));
                            ts.setValor(id, (int)a);
                        }
                        else if (tipo.Equals("int") && a.GetType() == typeof(Double))
                        {
                            ts.AddLast(new Simbolo(tipo, id));
                            ts.setValor(id, Convert.ToInt32((Double)a));
                        }
                        else if (tipo.Equals("double") && a.GetType() == typeof(Double))
                        {
                            ts.AddLast(new Simbolo(tipo, id));
                            ts.setValor(id, (Double)a);
                        }
                        else if (tipo.Equals("double") && a.GetType() == typeof(int))
                        {
                            ts.AddLast(new Simbolo(tipo, id));
                            ts.setValor(id, Convert.ToDouble((int)a));
                        }
                        else if (tipo.Equals("boolean") && a.GetType() == typeof(Boolean))
                        {
                            ts.AddLast(new Simbolo(tipo, id));
                            ts.setValor(id, (Boolean)a);
                        }
                        else if (tipo.Equals("date") && a.GetType() == typeof(DateTime))
                        {
                            ts.AddLast(new Simbolo(tipo, id));
                            ts.setValor(id, (DateTime)a);
                        }
                        else if (tipo.Equals("time") && a.GetType() == typeof(TimeSpan))
                        {
                            ts.AddLast(new Simbolo(tipo, id));
                            ts.setValor(id, (TimeSpan)a);
                        }
                        else if (tipo.Equals("map") && a.GetType() == typeof(Map))
                        {
                            ts.AddLast(new Simbolo(tipo, id));
                            ts.setValor(id, (Map)a);
                        }
                        else if (tipo.Equals("list") && a.GetType() == typeof(List))
                        {
                            ts.AddLast(new Simbolo(tipo, id));
                            ts.setValor(id, (List)a);
                        }
                        else if (tipo.Equals("set") && a.GetType() == typeof(Set))
                        {
                            ts.AddLast(new Simbolo(tipo, id));
                            Set temp = (Set)a;
                            object resp = temp.buscarRepetidos(mensajes,l,c);
                            if (resp == null) return null;
                            temp.order();
                            ts.setValor(id, temp);
                        }
                        else if (tipo.Equals("cursor") && a.GetType() == typeof(TypeCursor))
                        {
                            ts.AddLast(new Simbolo(tipo, id));
                            ts.setValor(id, (TypeCursor)a);
                        }
                        else if (a.GetType() == typeof(InstanciaUserType))
                        {
                            InstanciaUserType ins = (InstanciaUserType)a;
                            if (tipo.Equals(ins.tipo.ToLower()))
                            {
                                ts.AddLast(new Simbolo(tipo, id));
                                ts.setValor(id, ins);
                            }
                            else
                            {
                                Mensaje me = new Mensaje();
                                mensajes.AddLast(me.error("La variable " + id + " es de Tipo: " + tipo + " no se puede instanciar al tipo: " + ins.tipo, l, c, "Semantico"));
                            }
                            
                        }
                        else
                        {
                            Mensaje me = new Mensaje();
                            mensajes.AddLast(me.error("La variable " + id + " no se le puede asignar este valor " + a.ToString(), l, c, "Semantico"));
                            return null;
                        }
                        return "";
                    }
                    else
                    {
                        if (tipo.Equals("string") || tipo.Equals("date") || tipo.Equals("time"))
                        {
                            ts.AddLast(new Simbolo(tipo, id));
                            ts.setValor(id, null);
                        }
                        else if (tipo.Equals("int") || tipo.Equals("double") || tipo.Equals("map") || tipo.Equals("set") || tipo.Equals("list") || tipo.Equals("cursor") || tipo.Equals("boolean"))
                        {
                            mensajes.AddLast(mensa.error("No se le puede asignar un valor null al tipo: " + tipo, l, c, "Semantico"));
                            return null;
                        }
                        else
                        {
                            ts.AddLast(new Simbolo(tipo, id));
                            ts.setValor(id, new InstanciaUserType(tipo,null));
                        }
                        return "";
                    }
                   
                }
            }
            else
            {
                Mensaje me = new Mensaje();
                ambito.listadoExcepciones.AddLast(new Excepcion("objectalreadyexists", "La variable " + id + " ya existe en este ambito"));
                mensajes.AddLast(me.error("La variable " + id + " ya existe en este ambito",l,c,"Semantico"));
            }

            return null;
        }
    }
}
