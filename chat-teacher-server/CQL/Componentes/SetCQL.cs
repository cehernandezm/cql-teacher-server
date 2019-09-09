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

        public Expresion accesoUS { set; get; }

        public string operacion { set; get; }

        public int l { set; get; }
        public int c { set; get; }

        public Expresion key { get; set; }

        /*
       * Constructor de la clase
       * @campo es el nombre del campo
       * @valor es el valor nuevo a asignar
       * @l linea del campo
       * @c columna del campo
       */

        public SetCQL(string campo, Expresion valor, string operacion, int l, int c)
        {
            this.campo = campo;
            this.valor = valor;
            this.operacion = operacion;
            this.l = l;
            this.c = c;
        }

        /*
         * Constructor de la clase
         * @campo es el nombre del campo
         * @valor es el valor nuevo a asignar
         * @accesoUS es el parametro para MAP O USERTYPE,LIST, SET
         * @l linea del campo
         * @c columna del campo
         */
        public SetCQL(string campo, Expresion valor, Expresion accesoUS, string operacion, int l, int c)
        {
            this.campo = campo;
            this.valor = valor;
            this.accesoUS = accesoUS;
            this.operacion = operacion;
            this.l = l;
            this.c = c;
        }

        /*
         * Constructor de la clase
         * @campo es el nombre del campo
         * @valor es el valor nuevo a asignar
         * @accesoUS es el parametro para MAP O USERTYPE,LIST, SET
         * @l linea del campo
         * @c columna del campo
         */
        public SetCQL(Expresion key, Expresion valor, Expresion accesoUS, string operacion, int l, int c)
        {
            this.key = key;
            this.valor = valor;
            this.accesoUS = accesoUS;
            this.operacion = operacion;
            this.l = l;
            this.c = c;
        }
    }
}
