using cql_teacher_server.CQL.Arbol;
using cql_teacher_server.Herramientas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes
{
    public class Clear : InstruccionCQL
    {
        Expresion id { set; get; }
        int l { set; get; }
        int c { set; get; }

        /*
         * Constructor de la clase
         * @param {id} Tipo de expresion
         * @param {l} linea del id
         * @param {c} columna del id
         */
        public Clear(Expresion id, int l, int c)
        {
            this.id = id;
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
            if (mp != null)
            {

                if (mp.GetType() == typeof(Map))
                {
                    Map temp = (Map)mp;
                    temp.datos.Clear();
                    return "";
                }
                else if(mp.GetType() == typeof(List))
                {
                    List temp = (List)mp;
                    temp.lista.Clear();
                    return "";
                }
                else mensajes.AddLast(ms.error("No se puede aplicar un REMOVE en un tipo no Collection, no se reconoce: " + mp, l, c, "Semantico"));

            }
            else mensajes.AddLast(ms.error("No se puede insertar en un null", l, c, "Semantico"));

            return null;
        }
    }
}
