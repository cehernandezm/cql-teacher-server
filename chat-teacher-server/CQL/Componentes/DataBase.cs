using cql_teacher_server.CQL.Arbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes
{
    public class DataBase : InstruccionCQL
    {
        string id { set; get; }
        int linea { set; get; }
        int columna { set; get; }
        Boolean ifnot { set; get; }

        /*
         * Constructor de la clase 
         * @id nombre de la base de datos a crear
         * @linea linea  del id
         * @columna columna del id
         * @ifnot si viene o no un if not exists
         */
        public DataBase(string id, int linea, int columna, bool ifnot)
        {
            this.id = id;
            this.linea = linea;
            this.columna = columna;
            this.ifnot = ifnot;
        }

        /*
         * Metodo de la implementacion de InstruccionCQL
         * @ts tabla de simbolos global
         * @user usuario que esta ejecutando la accion
         * @baseD es la base actual en este caso sera none
         */
        public object ejecutar(TablaDeSimbolos ts, string user, ref string baseD)
        {
            throw new NotImplementedException();
        }
    }
}
