using cql_teacher_server.CQL.Arbol;
using cql_teacher_server.Herramientas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes.Cursor
{
    public class Cursor : InstruccionCQL
    {
        string id { set; get; }
        InstruccionCQL select { set; get; }
        int l { set; get; }
        int c { set; get; }

        /*
         * CONSTRUCTOR DE LA CLASE
         * @param {id} nombre de la variable que almacenara la consulta
         * @param {select} es la consulta que se ejecutara cada vez que se haga un OPEN O CLOSE
         * @param {l} linea del id
         * @param {c} columna del id
         */
        public Cursor(string id, InstruccionCQL select, int l, int c)
        {
            this.id = id;
            this.select = select;
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
            Mensaje ms = new Mensaje();
            object existeVariable = ts.getValor(id);
            if (existeVariable.Equals("none"))
            {
                TypeCursor typeCursor;
                if(select.GetType() == typeof(Expresion))
                {
                    object respuesta = (select == null) ? null : select.ejecutar(ts, ambito, tsT);
                    if (respuesta == null) return null;
                    if (respuesta.GetType() == typeof(TypeCursor)) typeCursor = (TypeCursor)respuesta;
                    else
                    {
                        ambito.mensajes.AddLast(ms.error("A un cursor solo se le puede asignar un tipo Select no se reconoce: " + respuesta,l,c,"Semantico"));
                        return null;
                    }
                }else typeCursor = new TypeCursor(null, (Select)select);
                ts.AddLast(new Simbolo("cursor", id));
                ts.setValor(id, typeCursor);
                return "";
            }
            else ambito.mensajes.AddLast(ms.error("Ya existe la variable: " + id + " en este ambito",l,c,"Semantico" ));
            return null;
        }
    }
}
