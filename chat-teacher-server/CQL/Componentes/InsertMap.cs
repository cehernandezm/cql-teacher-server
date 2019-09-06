using cql_teacher_server.CQL.Arbol;
using cql_teacher_server.Herramientas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes
{
    public class InsertMap : InstruccionCQL
    {
        Expresion id { set; get; }
        Expresion key { set; get; }
        Expresion valor { set; get; }
        int l { set; get; }
        int c { set; get; }
        string operacion { set; get; }




        /*
         * Constructor de la clase
         * @param {id} es el objeto de tipo Map al cual se le agregara un valor
         * @param {key} es la clave nueva a insertar
         * @param {valor} es el valor a almacenar
         * @param {l} linea del id
         * @param {c} columna del id
         */
        public InsertMap(Expresion id, Expresion key, Expresion valor, int l, int c, string operacion)
        {
            this.id = id;
            this.key = key;
            this.valor = valor;
            this.l = l;
            this.c = c;
            this.operacion = operacion;
        }

        /*
          * Constructor de la clase padre
          * @ts tabla de simbolos padre
          * @user usuario que esta ejecutando las acciones
          * @baseD string por referencia de que base de datos estamos trabajando
          * @mensajes el output de la ejecucion
        */
        public object ejecutar(TablaDeSimbolos ts, string user, ref string baseD, LinkedList<string> mensajes, TablaDeSimbolos tsT)
        {
            Mensaje ms = new Mensaje();
            object mp = (id == null) ? null : id.ejecutar(ts, user, ref baseD, mensajes, tsT);
            object ky = (key == null) ? null : key.ejecutar(ts, user, ref baseD, mensajes, tsT);
            object vl = (valor == null) ? null : valor.ejecutar(ts, user, ref baseD, mensajes, tsT);
            if (mp != null)
            {
                if (ky != null)
                {
                    if (operacion.Equals("MAP"))
                    {
                        if (mp.GetType() == typeof(Map))
                        {
                            Map temp = (Map)mp;
                            string tK = (getTipoValorPrimario(ky, mensajes) == null) ? "null" : (getTipoValorPrimario(ky, mensajes));
                            string tV = (getTipoValorSecundario(vl, mensajes) == null) ? "null" : (getTipoValorSecundario(vl, mensajes));
                            if (temp.id.Equals(tK + "/" + tV))
                            {
                                if (!searchKey(temp.datos, ky, mensajes))
                                {
                                    temp.datos.AddLast(new KeyValue(ky, vl));
                                    return "";
                                }
                            }
                            else if (tV.Equals("null"))
                            {
                                string tipo2 = temp.id.Split("/")[1];
                                if (!tipo2.Equals("int") && !tipo2.Equals("double") && !tipo2.Equals("boolean") && !tipo2.Equals("map"))
                                {
                                    if (!searchKey(temp.datos, ky, mensajes))
                                    {
                                        if (tipo2.Equals("string") || tipo2.Equals("date") || tipo2.Equals("time")) temp.datos.AddLast(new KeyValue(ky, vl));
                                        else temp.datos.AddLast(new KeyValue(ky, new InstanciaUserType(tipo2, null)));
                                        return "";
                                    }
                                }
                                else mensajes.AddLast(ms.error("No se puede guardar null el tipo: " + tipo2, l, c, "Semantico"));
                            }
                            else mensajes.AddLast(ms.error("No coinciden los tipo: " + temp.id + " con: " + tK + "/" + tV, l, c, "Semantico"));
                        }
                        else mensajes.AddLast(ms.error("No se puede aplicar un insert en un tipo no map, no se reconoce: " + mp, l, c, "Semantico"));
                    }
                    else if (operacion.Equals("LIST"))
                    {
                        if(mp.GetType() == typeof(List))
                        {
                            List temp = (List)mp;
                            string tV = (getTipoValorSecundario(ky, mensajes) == null) ? "null" : (getTipoValorSecundario(ky, mensajes));
                            if (tV.Equals(temp.id))
                            {
                                temp.lista.AddLast(ky);
                                return "";
                            }
                            else if (tV.Equals("null"))
                            {
                                if(!temp.id.Equals("int") && !temp.id.Equals("double") && !temp.id.Equals("boolean") && !temp.id.Equals("map") && !temp.id.Equals("list"))
                                {
                                    if(temp.id.Equals("string") || temp.id.Equals("date") || temp.id.Equals("time"))
                                    {
                                        temp.lista.AddLast(ky);
                                        return "";
                                    }
                                    temp.lista.AddLast(new InstanciaUserType(temp.id, null));
                                    return "";
                                }
                                else mensajes.AddLast(ms.error("No se puede asignar un valor null a un : " + temp.id, l, c, "Semantico"));
                            }
                            else mensajes.AddLast(ms.error("Los tipos : " + temp.id + " no coincide con: " + tV,l,c,"Semantico"));
                        }
                        else mensajes.AddLast(ms.error("No se puede aplicar un insert en un tipo no LIST, no se reconoce: " + mp, l, c, "Semantico"));
                    }
                    
                }
                else mensajes.AddLast(ms.error("La key no puede ser null", l, c, "Semantico"));
            }
            else mensajes.AddLast(ms.error("No se puede insertar en un null", l, c, "Semantico"));

            return null;
        }

        /*
         * METODO QUE BUSCARA LA NUEVA KEY EN LA LISTA
         * @param {lista} datos que se encuentran en el map
         * @param {key} el valor de la nueva key
         * @param {mensajes} output
         * @return true si encuentra otra key | false si no encuentra ninguna key
         */
        private Boolean searchKey(LinkedList<KeyValue> lista,object key, LinkedList<string> mensajes)
        {
            Mensaje ms = new Mensaje();
            foreach(KeyValue keyValue in lista)
            {
                if (keyValue.key.Equals(key))
                {
                    mensajes.AddLast(ms.error("El valor: " + key + " ya se encuentra en este MAP", l, c, "Semantico"));
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
            if (valor != null)
            {
                if (valor.GetType() == typeof(string)) return "string";
                else if (valor.GetType() == typeof(int)) return "int";
                else if (valor.GetType() == typeof(double)) return "double";
                else if (valor.GetType() == typeof(Boolean)) return "boolean";
                else if (valor.GetType() == typeof(DateTime)) return "date";
                else if (valor.GetType() == typeof(TimeSpan)) return "time";
                else
                {
                    mensajes.AddLast(ms.error("No se acepta este tipo de valor como clave primaria : " + valor, l, c, "Semantico"));
                    return null;
                }
            }
            mensajes.AddLast(ms.error("No se aceptan valores nulos como parte de una key", l, c, "Semantico"));
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
                    mensajes.AddLast(ms.error("No se acepta este tipo de valor como clave Secundaria : " + valor, l, c, "Semantico"));
                    return null;
                }
            }
            return null;
        }

        
    }
}
