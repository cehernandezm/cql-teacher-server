using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes
{
    public class OrderBy
    {
        public string nombre { set; get; }
        public Boolean asc { set; get; }


        /*
         * CONSTRUCTOR DE LA CLASE
         * @param  {nombre} es la columna que se usara para ordernar
         * @param {asc} true = asc, false = desc
         */
        public OrderBy(string nombre, bool asc)
        {
            this.nombre = nombre;
            this.asc = asc;
        }
    }
}
