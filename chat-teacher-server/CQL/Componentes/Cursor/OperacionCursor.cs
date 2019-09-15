using cql_teacher_server.CQL.Arbol;
using cql_teacher_server.Herramientas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes.Cursor
{
    public class OperacionCursor : InstruccionCQL
    {
        string operacion { set; get; }
        string id { set; get; }
        int l { set; get; }
        int c { set; get; }

        /*
         * CONSTRUCTOR DE LA CLASE
         * @param {operacion} OPEN | CLOSE
         * @param {id} variable tipo cursor
         * @param {l} linea del id
         * @param {c} columna del id
         */
        public OperacionCursor(string operacion, string id, int l, int c)
        {
            this.operacion = operacion;
            this.id = id;
            this.l = l;
            this.c = c;
        }


        /*
         * Constructor de la clase padre
         * @ts tabla de simbolos padre
         * @user usuario que esta ejecutando las acciones
         * @baseD string por referencia de que base de datos estamos trabajando
         * @mensajes el output de la ejecucion
       */
        public object ejecutar(TablaDeSimbolos ts, Ambito ambito, TablaDeSimbolos tsT)
        {
            object res = ts.getValor(id);
            Mensaje ms = new Mensaje();
            if (!res.Equals("none"))
            {
                if (res.GetType() == typeof(TypeCursor))
                {
                    if (operacion.Equals("open"))
                    {
                        object tabla = ((TypeCursor)res).consulta.ejecutar(ts, ambito, tsT);
                        if(tabla != null)
                        {
                            ambito.mensajes.RemoveLast();
                            if (tabla.GetType() == typeof(TablaSelect)) ((TypeCursor)res).tabla = (TablaSelect)tabla;
                        }
                    }
                    else((TypeCursor)res).tabla = null;
                    
                    return "";
                }
                else ambito.mensajes.AddLast(ms.error("La variable tiene que ser de tipo cursor, no se reconoce: " + res,l,c,"Semantico"));
            }
            else ambito.mensajes.AddLast(ms.error("No se encuentra la variable: " + id + " en este ambito",l,c,"Semantico"));
            return null;
        }
    }
}
