using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes
{
    public class Map
    {
        public string id { set; get; }
        public LinkedList<KeyValue> datos { set; get; }
    
        /*
         * CONSTRUCTOR DE LA CLASE
         * @param {id} sera la suma de sus valores si es none significa que solo ha sido declarado
         * @param {datos} lista de datos a almacenar
         */
        public Map(string id, LinkedList<KeyValue> datos)
        {
            this.id = id;
            this.datos = datos;
        }
    }
}
