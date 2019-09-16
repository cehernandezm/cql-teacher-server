using cql_teacher_server.CQL.Arbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes.Try_Catch
{
    public class inThrow : InstruccionCQL
    {
        string tipo { set; get; }

        /*
         * CONSTRUCTOR DE LA CLASE
         * @param {tipo} tipo de Exception a lanzar
         */
        public inThrow(string tipo)
        {
            this.tipo = tipo;
        }

        /*
        * METODO DE LA CLASE PADRE QUE SE IMPLEMENTA
        * @param {ts} tabla de simbolos del padre
        * @param {ambito} ambito de atributos a manejar
        * @param {tsT} tabla de simbolos(CQL) del padre
        */
        public object ejecutar(TablaDeSimbolos ts, Ambito ambito, TablaDeSimbolos tsT)
        {
            ambito.listadoExcepciones.AddLast(new Excepcion(tipo, ""));
            return null;
        }
    }
}
