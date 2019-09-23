using cql_teacher_server.CHISON;
using cql_teacher_server.CHISON.Componentes;
using cql_teacher_server.LUP.Arbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.LUP.Componentes
{
    public class InStruct : InstruccionLUP
    {
        string usuario { set; get; }

        /*
         * CONSTRUCTOR DE LA CLASE
         * @param {usuario} usuario que se buscara su informacion
         */
        public InStruct(string usuario)
        {
            this.usuario = usuario;
        }

        /*
         * CONSTRUCTOR DE LA CLASE PADRE
         * @return string con la informacion
         */
        public object ejecutar()
        {
            string salida = "[+DATABASES]\n";
            Usuario user = TablaBaseDeDatos.getUsuario(usuario);
            if(user != null)
            {
                LinkedList<string> lista = user.bases;
                foreach(string s in lista)
                {
                    BaseDeDatos db = TablaBaseDeDatos.getBase(s);
                    if(db != null)
                    {
                        salida += "[+DATABASE]\n";
                        salida += "[+NAME]\n" + db.nombre + "\n[-NAME]\n";
                        salida += "[+TABLES]\n";
                        //---------------------------------------- TABLAS -------------------------------------------------------
                        foreach(Tabla t in db.objetos.tablas)
                        {
                            salida += "[+TABLE]\n";
                            salida += "[+NAME]\n" + t.nombre + "\n[-NAME]\n";
                            salida += "[+COLUMNS]\n";

                            foreach(Columna c in t.columnas)
                            {
                                salida += "[+COLUMN]\n" + c.name + "\n[-COLUMN]\n";
                            }

                            salida += "[-COLUMNS]\n";
                            salida += "[-TABLE]\n";
                        }
                        salida += "[-TABLES]\n";
                        //-------------------------------------------- USER TYPE ----------------------------------------------------
                        salida += "[+TYPES]\n";
                        foreach(User_Types u in db.objetos.user_types)
                        {
                            salida += "[+TYPE]\n";
                            salida += "[+NAME]\n" + u.name + "\n[-NAME]\n";
                            salida += "[+ATTRIBUTES]\n";

                            foreach(Attrs a in u.type)
                            {
                                salida += "[+ATTRIBUTE]\n";
                                salida += a.name + "\n";
                                salida += "[-ATTRIBUTE]\n";
                            }

                            salida += "[-ATTRIBUTES]\n";
                            salida += "[-TYPE]\n";
                        }
                        salida += "[-TYPES]\n";

                        //----------------------------------------------- PROCEDURES --------------------------------------------------

                        salida += "[+PROCEDURES]\n";
                        foreach(Procedures p  in db.objetos.procedures)
                        {
                            salida += "[+PROCEDURE]\n";
                            salida += p.nombre + "\n";
                            salida += "[-PROCEDURE]\n";
                        }
                        salida += "[-PROCEDURES]\n";
                        salida += "[-DATABASE]\n";
                    }
                }
            }
            salida += "[-DATABASES]";
            return salida;
        }
    }
}
