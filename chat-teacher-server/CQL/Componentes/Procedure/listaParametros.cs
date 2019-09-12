using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes.Procedure
{
    public class listaParametros
    {
        public LinkedList<Declaracion> lista { set; get; }

        /*
         * CONSTRUCTOR DE LA CLASE
         * @param {lista} lista de declaraciones para un procedure
         */
        public listaParametros(LinkedList<Declaracion> lista)
        {
            this.lista = lista;
        }
    }
}
