using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes
{
    public class Retorno
    {
        public object valor { set; get; }

        //------------------------------------------------- VALOR QUE ALMACENA DE UN RETURN ----------------------------------------------------------
        public Retorno(object valor)
        {
            this.valor = valor;
        }
    }
}
