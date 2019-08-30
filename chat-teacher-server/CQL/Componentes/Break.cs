using cql_teacher_server.CQL.Arbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes
{
    public class Break : InstruccionCQL
    {


        /*
        * Constructor de la clase padre
        * @ts tabla de simbolos padre
        * @user usuario que esta ejecutando las acciones
        * @baseD string por referencia de que base de datos estamos trabajando
        * @mensajes el output de la ejecucion
        */
        public object ejecutar(TablaDeSimbolos ts, string user, ref string baseD, LinkedList<string> mensajes,TablaDeSimbolos tsT)
        {
            return null;
        }
    }
}
