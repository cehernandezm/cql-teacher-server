using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes
{
    public class List
    {
        public string id { set; get; }
        public LinkedList<object> lista { set; get; }

        /*
         * Constructor de la clase
         * @param {id} tipo de lista
         * @param {lista} lista de valores
         */
        public List(string id, LinkedList<object> lista)
        {
            this.id = id;
            this.lista = lista;
        }
    }
}
