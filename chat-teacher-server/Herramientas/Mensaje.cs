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
            string resultado = "[+DATA]\n";
            resultado += "\n\t<table class=\"table\">\n";
            resultado += "\n\t<thead class=\"thead-dark\">\n";
            resultado += "\t<tr>\n";
            foreach(Columna columa in tabla.columnas)
            {
                resultado += "\t\t<th scope = \"col\">" + columa.name + "</th>\n";
            }
            resultado += "\t</tr>\n";
            resultado += "\t</thead>\n";
            resultado += "\t<tbody>\n";
            foreach (Data data in tabla.datos)
            {
                resultado += "\t<tr>\n";
                foreach (Atributo atributo in data.valores)
                {
                    if(atributo.valor != null)
                    {
                        if (atributo.valor.GetType() == typeof(Map))
                        {
                            string temp = "[";
                            foreach (KeyValue key in ((Map)atributo.valor).datos)
                            {
                                if (key.value != null)
                                {
                                    if (key.value.GetType() == typeof(InstanciaUserType))
                                    {
                                        InstanciaUserType tempInstancia = (InstanciaUserType)key.value;
                                        temp += key.key + ": [" + getStringUser(tempInstancia.lista) + "] ,";
                                    }
                                    else temp += key.key + ":" + key.value + ",";
                                }
                                else temp += key.key + ":" + key.value + ",";
                            }
                            temp = temp.TrimStart(',');
                            temp += "]";
                            resultado += "\t\t<td>" + temp + "</td>\n";
                        }
                        else if (atributo.valor.GetType() == typeof(Set))
                        {
                            string temp = "[";
                            foreach (object p in ((Set)atributo.valor).datos)
                            {
                                if (p != null)
                                {
                                    if (p.GetType() == typeof(InstanciaUserType))
                                    {
                                        InstanciaUserType tempInstancia = (InstanciaUserType)p;
                                        temp +=  " [" + getStringUser(tempInstancia.lista) + "] ,";
                                    }
                                    else temp += p + ",";
                                }
                                else  temp += p + ",";

                            }
                            temp = temp.TrimStart(',');
                            temp += "]";
                            resultado += "\t\t<td>" + temp + "</td>\n";
                        }
                        else if (atributo.valor.GetType() == typeof(List))
                        {
                            string temp = "[";
                            foreach (object p in ((List)atributo.valor).lista)
                            {
                                if (p != null)
                                {
                                    if (p.GetType() == typeof(InstanciaUserType))
                                    {
                                        InstanciaUserType tempInstancia = (InstanciaUserType)p;
                                        temp += " [" + getStringUser(tempInstancia.lista) + "] ,";
                                    }
                                    else temp += p + ",";
                                }
                                else temp += p + ",";
                            }
                            temp = temp.TrimStart(',');
                            temp += "]";
                            resultado += "\t\t<td>" + temp + "</td>\n";
                        }
                        else if (atributo.valor.GetType() == typeof(InstanciaUserType))
                        {
                            string temp = "[";
                            foreach (Atributo a in ((InstanciaUserType)atributo.valor).lista)
                            {
                                temp += a.nombre + ":" + a.valor + ",";
                            }
                            temp = temp.TrimStart(',');
                            temp += "]";
                            resultado += "\t\t<td>" + temp + "</td>\n";
                        }
                        else resultado += "\t\t<td>" + atributo.valor.ToString() + "</td>";
                    }
                    
                }
                resultado += "\t</tr>\n";
            }
            resultado += "\t</tbody>\n";
            resultado += "\t</table>\n";
            resultado += "[-DATA]";
            return resultado;
        }
        private String getStringUser(LinkedList<Atributo> lista)
        {
            String salida = "";
            if (lista != null)
            {
                foreach (Atributo a in lista)
                {
                    salida += a.nombre + ":" + a.valor + ",";
                }
            }

            return salida;
        }
    }


   
}
