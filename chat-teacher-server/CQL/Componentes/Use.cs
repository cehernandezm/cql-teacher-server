using cql_teacher_server.CHISON;
using cql_teacher_server.CHISON.Componentes;
using cql_teacher_server.CQL.Arbol;
using cql_teacher_server.Herramientas;
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

        public object ejecutar(TablaDeSimbolos ts, string user, ref string baseD, LinkedList<string>mensajes, TablaDeSimbolos tsT)
        {
            Mensaje mensa = new Mensaje();
            BaseDeDatos db = TablaBaseDeDatos.getBase(bd);
            if (db == null)
            {
                mensajes.AddLast(mensa.error("La base de datos: " + bd + " no existe ", linea, columna, "Semantico"));
                return "";
            }
            if (TablaBaseDeDatos.getEnUso(bd, user))
            {
                mensajes.AddLast(mensa.error("La base de datos: " + bd + " esta siendo utilizada por otro usuario ", linea, columna, "Semantico"));
                return "";
            }
            Usuario usu = TablaBaseDeDatos.getUsuario(user);
            if( usu == null)
            {
                mensajes.AddLast(mensa.error("El usuario: " + user + " no existe ", linea, columna, "Semantico"));
                return "";
            }
            baseD = bd;
            USO newU = new USO(baseD, user);
            TablaBaseDeDatos.deleteMine(user);
            TablaBaseDeDatos.listaEnUso.AddLast(newU);
            mensajes.AddLast("[+MESSAGE]\n\t Se esta utilizando la base " + baseD + " exitosamente \n[-MESSAGE]");
            return "";
        }
    }
}
