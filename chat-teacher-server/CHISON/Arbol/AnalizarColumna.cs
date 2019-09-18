using cql_teacher_server.CHISON.Componentes;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CHISON.Arbol
{
    public class AnalizarColumna
    {
        int l = 0;
        int c = 0;
        public object analizar(ParseTreeNode raiz, LinkedList<string> mensajes)
        {
            if (raiz != null)
            {
                string etiqueta = raiz.Term.Name.ToLower();
               
                switch (etiqueta)
                {

                    case "tipo":
                        l = raiz.ChildNodes.ElementAt(0).Token.Location.Line;
                        c = raiz.ChildNodes.ElementAt(0).Token.Location.Column;
                        if (raiz.ChildNodes.Count == 2) return new LinkedList<Columna>();
                        else if(raiz.ChildNodes.Count() == 3)
                        {
                            object res = analizar(raiz.ChildNodes.ElementAt(1),mensajes);
                            if (res == null) return null;
                            return (LinkedList<Columna>)res;
                        }
                        mensajes.AddLast("La informacion para Columns tiene que ser de tipo objeto Linea: " + l + " Columna: " + c);
                        break;



                    case "inobjetos":
                        LinkedList<Columna> lista = new LinkedList<Columna>();
                        ParseTreeNode hijoI;
                        if (raiz.ChildNodes.Count() == 5)
                        {
                            lista = (LinkedList<Columna>)analizar(raiz.ChildNodes.ElementAt(0), mensajes);
                            hijoI = raiz.ChildNodes.ElementAt(3);
                            l = raiz.ChildNodes.ElementAt(2).Token.Location.Line;
                            c = raiz.ChildNodes.ElementAt(2).Token.Location.Column;
                        }
                        else
                        {
                            l = raiz.ChildNodes.ElementAt(0).Token.Location.Line;
                            c = raiz.ChildNodes.ElementAt(0).Token.Location.Column;
                            hijoI = raiz.ChildNodes.ElementAt(1);
                        }
                        object resI = analizar(hijoI, mensajes);
                        if (resI != null)
                        {
                            LinkedList<Atributo> temp = (LinkedList<Atributo>)resI;
                            Atributo a = valorAtributo(temp, "name");
                            if (a == null)
                            {
                                mensajes.AddLast("Una columna necesita el atributo: name Linea:" + l + " Columna: " + c);
                                return lista;
                            }
                            string nombre = a.valor.ToString();

                            a = valorAtributo(temp, "type");
                            if (a == null)
                            {
                                mensajes.AddLast("Una columna necesita el atributo: Type Linea:" + l + " Columna: " + c);
                                return lista;
                            }
                            string type = a.valor.ToString();

                            a = valorAtributo(temp, "pk");
                            if (a == null)
                            {
                                mensajes.AddLast("Una columna necesita el atributo: PK Linea:" + l + " Columna: " + c);
                                return lista;
                            }
                            Boolean pk = (Boolean)a.valor;

                            lista.AddLast(new Columna(nombre, type, pk));
                        }
                        return lista;
                        

                    case "objetos":
                        LinkedList<Atributo> listaValores = new LinkedList<Atributo>();
                        object resO;
                       
                        if (raiz.ChildNodes.Count() == 3)
                        {
                            listaValores = (LinkedList<Atributo>)analizar(raiz.ChildNodes.ElementAt(0), mensajes);
                            resO = analizar(raiz.ChildNodes.ElementAt(2), mensajes);

                        }
                        else resO = analizar(raiz.ChildNodes.ElementAt(0), mensajes);

                        if(resO != null) listaValores.AddLast((Atributo)resO);

                        return listaValores;
                        

                        break;


                    case "objeto":
                        string key = raiz.ChildNodes.ElementAt(0).Token.Text.ToLower().TrimEnd('\"').TrimStart('\"').TrimStart().TrimEnd();
                        l = raiz.ChildNodes.ElementAt(0).Token.Location.Line;
                        c = raiz.ChildNodes.ElementAt(0).Token.Location.Column;
                        string valor = raiz.ChildNodes.ElementAt(2).ChildNodes.ElementAt(0).Token.Text.ToLower().TrimEnd('\"').TrimStart('\"').TrimStart().TrimEnd();
                        if (key.Equals("name")) return new Atributo("name", valor , "");
                        else if (key.Equals("type")) return new Atributo("type", valor, "");
                        else if (key.Equals("pk")) return new Atributo("pk", Boolean.Parse(valor), "");
                        else
                        {
                            mensajes.AddLast("No se reconoce el atributo: " + key + " para una columna");
                            return null;
                        }

                        break;
                }
            }
            return null;
        }


        /*----------------------------------------------------------------------------------------------------------------------------------------------------
 * --------------------------------------------------- METODOS VARIOS ---------------------------------------------------------------------------------
 ------------------------------------------------------------------------------------------------------------------------------------------------------*/



        public Atributo valorAtributo(LinkedList<Atributo> lk, string atributo)
        {
            foreach (Atributo at in lk)
            {
                if (at.nombre.Equals(atributo)) return at;
            }
            return null;
        }


    }

}
