using cql_teacher_server.Herramientas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes
{
    public class Set
    {
        public string id { set; get; }
        public LinkedList<object> datos { set; get; }

        /*
         * Constructor de la clase
         * @param {id} tipo de set
         * @param {datos} linkedlist de datos
         */
        public Set(string id, LinkedList<object> datos)
        {
            this.id = id;
            this.datos = datos;
        }

        /*
         * METODO QUE ORDENARA LA LISTA 
         */
        public void order()
        {
            try
            {
                datos = new LinkedList<object>(datos.OrderBy(a => a));
            }
            catch (Exception) { }
        }

        /*
         * METODO QUE BUSCA DATOS REPETIDOS
         * @param {mensajes} output
         * @param {l} linea de la instruccion
         * @param {c} columna de la instruccion
         * @return null si hay error | "" si todo esta bien
         */

        public object buscarRepetidos(LinkedList<string> mensajes, int l, int c)
        {
            Mensaje ms = new Mensaje();
            foreach(object o in datos)
            {
                int i = 0;

                foreach(object oo in datos)
                {
                    if (o.Equals(oo)) i++;
                }


                if(i > 1)
                {
                    mensajes.AddLast(ms.error("Este valor esta repetido: " + o.ToString(),l,c,"Semantico"));
                    return null;
                }
            }
            return "";
        }
    }
}
