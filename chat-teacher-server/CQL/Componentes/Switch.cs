using cql_teacher_server.CQL.Arbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes
{

    public class Switch : InstruccionCQL
    {
        LinkedList<Case> listado { set; get; }

        /*
         * Constructor de la clase que recibe almenos un case
         * @listado listade de case y/o default
         */

        public Switch(LinkedList<Case> listado)
        {
            this.listado = listado;
        }





        /*
        * Constructor de la clase padre
        * @ts tabla de simbolos padre
        * @user usuario que esta ejecutando las acciones
        * @baseD string por referencia de que base de datos estamos trabajando
        * @mensajes el output de la ejecucion
        */
        public object ejecutar(TablaDeSimbolos ts, string user, ref string baseD, LinkedList<string> mensajes)
        {
            Boolean ejecutar = false ;
            foreach(Case c in listado)
            {
                if((c.isDefault && !ejecutar) || (!c.isDefault))
                {
                    object res = c.ejecutar(ts, user, ref baseD, mensajes);
                    if (res == null) return null;
                }

                if (c.flag && !ejecutar) ejecutar = c.flag;
                
            }
            return "";
        }
    }
}
