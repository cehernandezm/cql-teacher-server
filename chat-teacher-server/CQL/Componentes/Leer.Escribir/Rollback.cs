using cql_teacher_server.CQL.Arbol;
using cql_teacher_server.Herramientas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes.Leer.Escribir
{
    public class Rollback : InstruccionCQL
    {
       

        public Rollback()
        {
            
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
            LeerArchivo leer = new LeerArchivo("Principal.chison");
            return "";
        }
    }
}
