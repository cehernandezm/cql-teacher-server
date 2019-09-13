using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes
{
    public class KeyValue
    {
        public object key { set; get; }
        public object value { set; get; }

        /*
         * CONSTRUCTOR DE LA CLASE
         * @param {key} key del objeto
         * @param {value} valor del objeto
         */
        public KeyValue(object key, object value)
        {
            this.key = key;
            this.value = value;
        }
    }
}
