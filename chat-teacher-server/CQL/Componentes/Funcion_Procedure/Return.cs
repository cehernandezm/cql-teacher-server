using cql_teacher_server.CQL.Arbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes
{
    public class Return : InstruccionCQL
    {
        object valor { set; get; }

        /*
         * CONSTRUCTOR DE LA CLASE
         * @param {valor} valor para retornar
         */
        public Return(object valor)
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
        public object ejecutar(TablaDeSimbolos ts, Ambito ambito, TablaDeSimbolos tsT)
        {
            if( valor != null)
            {
                if(valor.GetType() == typeof(Expresion))
                {
                    object res = (valor == null) ? null : ((Expresion)valor).ejecutar(ts,ambito, tsT);
                    return new Retorno(res);
                }
                LinkedList<Expresion> listaExpresiones = (LinkedList<Expresion>)valor;
                LinkedList<object> valoresReturn = new LinkedList<object>();
                foreach(Expresion e in listaExpresiones)
                {
                    object res = (e == null) ? null : e.ejecutar(ts, ambito, tsT);
                    valoresReturn.AddLast(res);
                }
                return new Retorno(valoresReturn);
                
            }
            return new Retorno(null);
           
        }
    }
}
