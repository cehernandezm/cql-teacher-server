using cql_teacher_server.CQL.Arbol;
using cql_teacher_server.CQL.Componentes.Try_Catch;
using cql_teacher_server.Herramientas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes
{
    public class Batch : InstruccionCQL
    {
        int l { set; get; }
        int c { set; get; }
        LinkedList<InstruccionCQL> cuerpo { set; get; }

        /*
         * CONTRUCTOR DE LA CLASE
         * @param {l} linea de la ejecucion
         * @param {c} columna de la ejecucion
         * @param {cuerpo} instrucciones a ejecutar
         */
        public Batch(int l, int c, LinkedList<InstruccionCQL> cuerpo)
        {
            this.l = l;
            this.c = c;
            this.cuerpo = cuerpo;
        }
        /*
         * Metodo de la implementacion de la clase InstruccionCQL
         * @ts Tabla de simbolos global
         * @user usuario que ejecuta la accion
         * @baseD base de datos en la que se realizara la accion, es pasada por referencia
         */

        public object ejecutar(TablaDeSimbolos ts, Ambito ambito, TablaDeSimbolos tsT)
        {
            Mensaje ms = new Mensaje();
            GuardarArchivo guardar = new GuardarArchivo();
            guardar.guardarArchivo("Principal2");

            foreach(InstruccionCQL ins in cuerpo)
            {
                object respuesta = ins.ejecutar(ts, ambito, tsT);
                if(respuesta == null)
                {
                    ambito.listadoExcepciones.AddLast(new Excepcion("batchException", "Hubo un error en la ejecucion del batch, Linea: " + l + " Columna: " + c));
                    ambito.mensajes.AddLast(ms.error("Hubo un error en la ejecucion del batch",l,c,"Semantico"));
                    LeerArchivo leer = new LeerArchivo("Principal2.chison");
                    return null;
                    
                }
            }

            return "";

        }
    }
}
