using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes
{
    public class SetCQL
    {
        public string campo { set; get; }
        public Expresion valor { set; get; }

        public int l { set; get; }
        public int c { set; get; }


        /*
         * Constructor de la clase
         * @campo es el nombre del campo
         * @valor es el valor nuevo a asignar
         * @l linea del campo
         * @c columna del campo
         */

        public SetCQL(string campo, Expresion valor, int l, int c)
        {
            this.campo = campo;
            this.valor = valor;
            this.l = l;
            this.c = c;
        }
    }
}
