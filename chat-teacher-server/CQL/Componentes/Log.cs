using cql_teacher_server.CQL.Arbol;
using cql_teacher_server.Herramientas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes
{
    public class Log : InstruccionCQL
    {
        Expresion expresion { set; get; }

        /*
         * CONSTRUCTOR DE LA CLASE
         * param {expresion} expresion a mostrar
         */
        public Log(Expresion expresion)
        {
            this.expresion = expresion;
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
            Mensaje ms = new Mensaje();
            object r = expresion.ejecutar(ts, user,ref baseD, mensajes, tsT);
            if(r != null) mensajes.AddLast(ms.message(r.ToString()));
            return "";
        }
    }
}
