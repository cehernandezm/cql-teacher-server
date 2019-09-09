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
                    if(atributo.valor.GetType() == typeof(Map))
                    {
                        string temp = "[";
                        foreach(KeyValue key in ((Map)atributo.valor).datos)
                        {
                            temp += key.key + ":" + key.value + ",";
                        }
                        temp += "]"; 
                        resultado +=temp + "|";
                    }
                    else if(atributo.valor.GetType() == typeof(InstanciaUserType))
                    {
                        string temp = "[";
                        foreach(Atributo a in ((InstanciaUserType)atributo.valor).lista)
                        {
                            temp += a.nombre + ":" + a.valor + ",";
                        }
                        temp += "]";
                        resultado += temp + "|";
                    }
                    else resultado += atributo.valor.ToString() + "|";
                }
                resultado += "\n";
            }
            return resultado;
        }
    }
}
