using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes.Cursor
{
    public class TypeCursor
    {
        public TablaSelect tabla { set; get; }
        public Select consulta { set; get; }

        /*
         * CONSTRUCTOR DE LA CLASE
         * @param {tabla} lo que devuelve la consulta
         * @param {consulta} es la instruccion que se ejecutara con cada OPEN O CLOSE
         */
        public TypeCursor(TablaSelect tabla, Select consulta)
        {
            this.tabla = tabla;
            this.consulta = consulta;
        }
    }
}
