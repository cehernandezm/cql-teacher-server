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

        /*
        * Constructor de la clase
        * @param {id} es el objeto de tipo Map al cual se le agregara un valor
        * @param {key} es la clave nueva a insertar
        * @param {valor} es el valor a almacenar
        * @param {l} linea del id
        * @param {c} columna del id
        */
        public SetMap(Expresion id, Expresion key, Expresion valor, int l, int c)
        {
            this.id = id;
            this.key = key;
            this.valor = valor;
            this.l = l;
            this.c = c;
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
                        string tV = (getTipoValorSecundario(vl, mensajes) == null) ? "null" : (getTipoValorSecundario(vl, mensajes));
                        foreach(KeyValue key in temp.datos)
                        {
                            if (key.key.Equals(ky))
                            {

                            }
                        }
                        mensajes.AddLast(ms.error("No se encontro la key: " + key, l, c, "Semantico"));
                    }
                    else mensajes.AddLast(ms.error("No se puede aplicar un insert en un tipo no map, no se reconoce: " + mp, l, c, "Semantico"));
                }
                else mensajes.AddLast(ms.error("La key no puede ser null", l, c, "Semantico"));
            }
            else mensajes.AddLast(ms.error("No se puede insertar en un null", l, c, "Semantico"));

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
