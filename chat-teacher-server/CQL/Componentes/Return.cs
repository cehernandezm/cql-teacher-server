using cql_teacher_server.CQL.Arbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes
{
    public class Return : InstruccionCQL
    {
        Expresion valor { set; get; }

        /*
         * CONSTRUCTOR DE LA CLASE
         * @param {valor} valor para retornar
         */
        public Return(Expresion valor)
        {
            this.valor = valor;
        }

        /*
         * Metodo de la implementacion
         * @ts tabla de simbolos global
         * @user usuario que ejecuta la accion
         * @baseD base de datos donde estamos ejecutando todo
         * @mensajes linkedlist con la salida deseada
         */
        public object ejecutar(TablaDeSimbolos ts, string user, ref string baseD, LinkedList<string> mensajes, TablaDeSimbolos tsT)
        {
            object res = (valor == null) ? null : valor.ejecutar(ts, user, ref baseD, mensajes, tsT);
            return new Retorno(res);
        }
    }
}
