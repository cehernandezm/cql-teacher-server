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
            BaseDeDatos db = TablaBaseDeDatos.getBase(id);
            //--------------------------- si existe una base de datos pero  no tiene un if not exist --------------------------------------------
            if (db != null && !ifnot) return "[+ERROR]\n[+LINE]\n\t" + linea + "\n[-LINE]\n[+COLUMN]\n\t" + columna + "\n[-COLUMN]" +
                    "\n[+TYPE]\n\tSemantico\n[-TYPE]\n[+DESC]\n\t Ya existe una base de datos llamada " + id + "\n[-DESC]\n[-ERROR]";

            if(db == null)
            {
                Objeto ls = new Objeto();
                BaseDeDatos newDb = new BaseDeDatos(id,ls);

                
                Usuario us = TablaBaseDeDatos.getUsuario(user);
                if(us == null ) System.Diagnostics.Debug.WriteLine("Usuario : " + user);
                if (us != null)
                {
                    us.bases.AddLast(id);
                    TablaBaseDeDatos.global.AddLast(newDb);
                    GuardarArchivo backup= new GuardarArchivo();
                    backup.guardarArchivo();
                    return "[+MESSAGE]\n La base de datos " + id + "ha sido creada exitosamente\n[-MESSAGE]";
                }
                else return "none";


            }
            return "none";
        }
    }
}
