using cql_teacher_server.CHISON;
using cql_teacher_server.CHISON.Componentes;
using cql_teacher_server.CQL.Arbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes
{
    public class Use : InstruccionCQL
    {
        string bd {set;get;}
        int linea { set; get; }
        int columna { set; get; }






        /*
         * Constructor de la clase use
         * @bd nombre de la base de datos a buscar
         * @linea linea donde se encuentra el nombre de la base de datos
         * @columna columna donde se encuentra el nombre de la base de datos
         */
        public Use(string bd, int linea, int columna)
        {
            this.bd = bd;
            this.linea = linea;
            this.columna = columna;
        }

        /*
         * Metodo que ejecutara las acciones
         * @ts tabla de simbolos
         * @user usuario que ejecuta la accion
         * return Mensaje LUP de Correcta accion o Incorrecta
         */

        public object ejecutar(TablaDeSimbolos ts, string user, ref string baseD)
        {
            BaseDeDatos db = TablaBaseDeDatos.getBase(bd);
            if (db == null) return "[+ERROR]\n[+LINE]\n\t" + linea + "\n[-LINE]\n[+COLUMN]\n\t" + columna + "\n[-COLUMN]\n" +
                    "[+TYPE]\n\tSemantico\n[-TYPE]\n[+DESC]\n\t No existe la base de datos a utilizar \n[-DESC]\n[-ERROR]";
            if (TablaBaseDeDatos.getEnUso(bd,user)) return "[+ERROR]\n[+LINE]\n\t" + linea + "\n[-LINE]\n[+COLUMN]\n\t" + columna + "\n[-COLUMN]\n" +
                    "[+TYPE]\n\tSemantico\n[-TYPE]\n[+DESC]\n\t La base ya esta siendo utilizada por otro usuario \n[-DESC]\n[-ERROR]";
            baseD = bd;
            USO newU = new USO(baseD, user);
            TablaBaseDeDatos.deleteMine(user);
            TablaBaseDeDatos.listaEnUso.AddLast(newU);
            return "[+MESSAGE]\n\t Se esta utilizando la base " + baseD + " exitosamente \n[-MESSAGE]";
        }
    }
}
