using cql_teacher_server.CQL.Arbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes.Procedure
{
    public class listaParametros
    {
        public LinkedList<InstruccionCQL> lista { set; get; }

        /*
         * CONSTRUCTOR DE LA CLASE
         * @param {lista} lista de declaraciones para un procedure
         */
        public listaParametros(LinkedList<InstruccionCQL> lista)
        {
            this.lista = lista;
        }
    }
}
