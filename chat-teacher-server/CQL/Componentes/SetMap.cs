using cql_teacher_server.CQL.Arbol;
using cql_teacher_server.Herramientas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes
{
    public class SetMap : InstruccionCQL
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
        * @param {operacion} que tipo de objeto tiene que ser
        */
        public SetMap(Expresion id, Expresion key, Expresion valor, int l, int c, string operacion)
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

                    if (mp.GetType() == typeof(Map))
                    {
                        Map temp = (Map)mp;
                        string t2 = (temp.id.Split(new[] { ',' } , 2))[1];
                        string tV = (getTipoValorSecundario(vl, mensajes) == null) ? "null" : (getTipoValorSecundario(vl, mensajes));
                        foreach (KeyValue key in temp.datos)
                        {
                            if (key.key.Equals(ky))
                            {
                                if (t2.Equals(tV))
                                {
                                    key.value = vl;
                                    return "";
                                }
                                else if (tV.Equals("null"))
                                {
                                    if (!t2.Equals("int") && !t2.Equals("double") && !t2.Equals("boolean") && !t2.Equals("map") && !t2.Equals("list") && !t2.Equals("set"))
                                    {
                                        if (t2.Equals("string") || t2.Equals("date") || t2.Equals("time"))
                                        {
                                            key.value = vl;
                                            return "";
                                        }
                                        else
                                        {
                                            key.value = new InstanciaUserType(temp.id, null);
                                        }
                                    }
                                    else
                                    {
                                        mensajes.AddLast("El valor es de tipo " + t2 + " no es igual con: " + tV);
                                        return null;
                                    }
                                }
                            }
                        }
                        mensajes.AddLast(ms.error("No se encontro la key: " + ky, l, c, "Semantico"));
                    }
                    else if(mp.GetType() == typeof(List))
                    {
                        List temp = (List)mp;
                        if (ky.GetType() == typeof(int))
                        {
                            int index = (int)ky;
                            if (index > -1)
                            {
                                if (index < temp.lista.Count())
                                {
                                    string tipoValor = (getTipoValorSecundario(vl, mensajes) == null) ? "null" : getTipoValorSecundario(vl, mensajes);
                                    if (tipoValor.Equals(temp.id))
                                    {

                                        ChangeValueList(temp.lista, index, vl);
                                        return "";
                                    }
                                    else if (tipoValor.Equals("null"))
                                    {
                                        if (!temp.id.Equals("int") && !temp.id.Equals("double") && !temp.id.Equals("boolean") && !temp.id.Equals("map") && !temp.id.Equals("list") && !temp.id.Equals("set"))
                                        {
                                            if (temp.id.Equals("string") || temp.id.Equals("date") || temp.id.Equals("time"))
                                            {
                                                ChangeValueList(temp.lista, index, vl);
                                                return "";
                                            }
                                            else
                                            {
                                                ChangeValueList(temp.lista, index, new InstanciaUserType(temp.id, null));
                                                return "";
                                            }
                                        }
                                        else mensajes.AddLast(ms.error("No se puede asignar null al tipo: " + temp.id,l,c,"Semantico"));
                                    }
                                    else mensajes.AddLast(ms.error("No coinciden los tipos: " + temp.id + " con: " + tipoValor,l,c,"Semantico"));
                                    
                                }
                                else mensajes.AddLast(ms.error("El index es mayor al tamaño de la lista", l, c, "Semantico"));
                            }
                            else mensajes.AddLast(ms.error("Index tiene que ser mayor a 0 : " + index, l, c, "Semantico"));
                        }
                        else mensajes.AddLast(ms.error("Index tiene que ser de tipo numerico: " + ky, l, c, "Semantico"));
                    }
                    else if (mp.GetType() == typeof(Set))
                    {
                        Set temp = (Set)mp;
                        if (ky.GetType() == typeof(int))
                        {
                            int index = (int)ky;
                            if (index > -1)
                            {
                                if (index < temp.datos.Count())
                                {
                                    string tipoValor = (getTipoValorSecundario(vl, mensajes) == null) ? "null" : getTipoValorSecundario(vl, mensajes);
                                    if (tipoValor.Equals(temp.id))
                                    {
                                        object res = temp.buscarRepetidosPorValor(mensajes, l, c, vl);
                                        if (res == null) return null;
                                        ChangeValueList(temp.datos, index, vl);
                                        temp.order();
                                        return "";
                                    }
                                    else if (tipoValor.Equals("null"))
                                    {
                                        if (!temp.id.Equals("int") && !temp.id.Equals("double") && !temp.id.Equals("boolean") && !temp.id.Equals("map") && !temp.id.Equals("list") && !temp.id.Equals("set"))
                                        {
                                            if (temp.id.Equals("string") || temp.id.Equals("date") || temp.id.Equals("time"))
                                            {
                                                object res = temp.buscarRepetidosPorValor(mensajes, l, c, vl);
                                                if (res == null) return null;
                                                ChangeValueList(temp.datos, index, vl);
                                                temp.order();
                                                return "";
                                            }
                                            else
                                            {
                                                object res = temp.buscarRepetidosPorValor(mensajes, l, c, new InstanciaUserType(temp.id, null));
                                                if (res == null) return null;
                                                ChangeValueList(temp.datos, index, new InstanciaUserType(temp.id, null));
                                                return "";
                                            }
                                        }
                                        else mensajes.AddLast(ms.error("No se puede asignar null al tipo: " + temp.id, l, c, "Semantico"));
                                    }
                                    else mensajes.AddLast(ms.error("No coinciden los tipos: " + temp.id + " con: " + tipoValor, l, c, "Semantico"));

                                }
                                else mensajes.AddLast(ms.error("El index es mayor al tamaño de la lista", l, c, "Semantico"));
                            }
                            else mensajes.AddLast(ms.error("Index tiene que ser mayor a 0 : " + index, l, c, "Semantico"));
                        }
                        else mensajes.AddLast(ms.error("Index tiene que ser de tipo numerico: " + ky, l, c, "Semantico"));
                    }
                    else mensajes.AddLast(ms.error("No se puede aplicar un SET en un tipo no Collection, no se reconoce: " + mp, l, c, "Semantico"));



                }
                else mensajes.AddLast(ms.error("La key no puede ser null", l, c, "Semantico"));
            }
            else mensajes.AddLast(ms.error("No se puede insertar en un null", l, c, "Semantico"));

            return null;
        }

        /*
         * Metodo que cambia el valor de una lista de datos
         * @param {lista} lista de datos
         * @param {index}  posicion a cambiar
         * @param {valor} nuevo valor
         */
        private void ChangeValueList(LinkedList<object> lista, int index,object valor)
        {
            if(lista.Count() > 0)
            {
                var node = lista.First;
                int i = 0;
                while(node != null)
                {
                    var nodeNext = node.Next;
                    if (i == index) node.Value = valor;
                    i++;
                    node = nodeNext;
                }
            }
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
                else if (valor.GetType() == typeof(Map)) return "map<" +  ((Map)valor).id + ">";
                else if (valor.GetType() == typeof(List)) return "list<" + ((List)valor).id + ">";
                else if (valor.GetType() == typeof(Set)) return "set<" + ((Set)valor).id + ">";
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
