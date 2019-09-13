using cql_teacher_server.CHISON.Componentes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes
{
    public class TablaSelect
    {
        public LinkedList<Columna> columnas { set; get; }
        public LinkedList<Data> datos { set; get; }

        /*
         * CONSTRUCTOR DE LA CLASE
         * @param columnas: sera la cabecera de la la consulta
         * @param datos: la informacion de la consulta
         */
        public TablaSelect(LinkedList<Columna> columnas, LinkedList<Data> datos)
        {
            this.columnas = columnas;
            this.datos = datos;
        }
    }
}
