using cql_teacher_server.CHISON.Componentes;
using cql_teacher_server.CHISON.Gramatica;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CHISON.Arbol
{
    public class AnalizarColumna : Importar
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



                    case "lista":
                        LinkedList<Columna> lista = new LinkedList<Columna>();
                        ParseTreeNode hijoI;
                        if (raiz.ChildNodes.Count() == 3)
                        {
                            lista = (LinkedList<Columna>)analizar(raiz.ChildNodes.ElementAt(0), mensajes);
                            hijoI = raiz.ChildNodes.ElementAt(2).ChildNodes.ElementAt(1);
                            l = raiz.ChildNodes.ElementAt(1).Token.Location.Line;
                            c = raiz.ChildNodes.ElementAt(1).Token.Location.Column;
                        }
                        else
                        {
                            //----------------------------------------------- import ------------------------------------------------------
                            if (raiz.ChildNodes.ElementAt(0).ChildNodes.Count() == 1)
                            {
                                hijoI = raiz.ChildNodes.ElementAt(0).ChildNodes.ElementAt(0);
                                string direccion = hijoI.Token.Text.TrimStart('$').TrimStart('{').TrimEnd('}').TrimEnd('}').TrimEnd('$').TrimEnd('}');
                                object nuevoNodo = analizarImport(direccion, mensajes);
                                if (nuevoNodo != null) return analizar((ParseTreeNode)nuevoNodo, mensajes);
                                else return null;
                            }
                            l = raiz.ChildNodes.ElementAt(0).ChildNodes.ElementAt(0).Token.Location.Line;
                            c = raiz.ChildNodes.ElementAt(0).ChildNodes.ElementAt(0).Token.Location.Column;
                            hijoI = raiz.ChildNodes.ElementAt(0).ChildNodes.ElementAt(1);
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


                    case "inobjetos":
                        LinkedList<Columna> lista2 = new LinkedList<Columna>();
                        ParseTreeNode hijoI2;
                        if (raiz.ChildNodes.Count() == 5)
                        {
                            lista2 = (LinkedList<Columna>)analizar(raiz.ChildNodes.ElementAt(0), mensajes);
                            hijoI2 = raiz.ChildNodes.ElementAt(3);
                            l = raiz.ChildNodes.ElementAt(1).Token.Location.Line;
                            c = raiz.ChildNodes.ElementAt(1).Token.Location.Column;
                        }
                        else
                        {
                            l = raiz.ChildNodes.ElementAt(0).Token.Location.Line;
                            c = raiz.ChildNodes.ElementAt(0).Token.Location.Column;
                            hijoI2 = raiz.ChildNodes.ElementAt(1);
                        }
                        object resI2 = analizar(hijoI2, mensajes);
                        if (resI2 != null)
                        {
                            LinkedList<Atributo> temp = (LinkedList<Atributo>)resI2;
                            Atributo a = valorAtributo(temp, "name");
                            if (a == null)
                            {
                                mensajes.AddLast("Una columna necesita el atributo: name Linea:" + l + " Columna: " + c);
                                return lista2;
                            }
                            string nombre = a.valor.ToString();

                            a = valorAtributo(temp, "type");
                            if (a == null)
                            {
                                mensajes.AddLast("Una columna necesita el atributo: Type Linea:" + l + " Columna: " + c);
                                return lista2;
                            }
                            string type = a.valor.ToString();

                            a = valorAtributo(temp, "pk");
                            if (a == null)
                            {
                                mensajes.AddLast("Una columna necesita el atributo: PK Linea:" + l + " Columna: " + c);
                                return lista2;
                            }
                            Boolean pk = (Boolean)a.valor;

                            lista2.AddLast(new Columna(nombre, type, pk));
                        }
                        return lista2;



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
