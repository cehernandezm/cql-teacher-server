using cql_teacher_server.CHISON;
using cql_teacher_server.CHISON.Componentes;
using cql_teacher_server.CQL.Componentes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.Herramientas
{
    public class Mensaje
    {
        public string error(string mensaje, int linea, int columna, string tipo)
        {
            return "[+ERROR]\n[+LINE]\n\t" + linea + "\n[-LINE]\n[+COLUMN]\n\t" + columna + "\n[-COLUMN]" +
                    "\n[+TYPE]\n\t" + tipo + "\n[-TYPE]\n[+DESC]\n\t " + mensaje + "\n[-DESC]\n[-ERROR]";
        }

        public string message(string mensaje)
        {
            return "[+MESSAGE]\n "  + mensaje + "\n[-MESSAGE]";
        }

        public string consulta(TablaSelect tabla)
        {
            string resultado = "";
            foreach(Columna columa in tabla.columnas)
            {
                resultado += columa.name + "|";
            }
            resultado += "\n";
            foreach(Data data in tabla.datos)
            {
                foreach(Atributo atributo in data.valores)
                {
                    resultado += atributo.valor.ToString() + "|";
                }
                resultado += "\n";
            }
            return resultado;
        }
    }
}
