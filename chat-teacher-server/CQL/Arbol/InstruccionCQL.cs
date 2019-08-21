using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Arbol
{
    public interface InstruccionCQL
    {

        /*
         * Metodo que se ejecutara en cada clase que implemente esta clase
         * devuelve un valor segun el objeto a ejecutar
         * @ts Tabla de simbolo en general
         * @user usuario que ejecuta las operaciones
         * @baseD base de datos usando actualmente se pasa por referencia por si en algun lado de la ejecucion cambia
         */

        Object ejecutar(TablaDeSimbolos ts, string user, ref string baseD,LinkedList<string> mensajes);
        


    }
}
