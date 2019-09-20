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
    public class AnalizarPermisos
    {
        int l = 0;
        int c = 0;

        public object inicio(ParseTreeNode raiz, LinkedList<string> mensajes)
        {
            if (raiz.ChildNodes.Count() == 2) return new LinkedList<string>();
            else if (raiz.ChildNodes.Count() == 3)
            {
                object res = (analizar(raiz.ChildNodes.ElementAt(1), mensajes) == null) ? null : (LinkedList<string>)analizar(raiz.ChildNodes.ElementAt(1), mensajes);
                return res;
            }
            l = raiz.ChildNodes.ElementAt(0).Token.Location.Line;
            c = raiz.ChildNodes.ElementAt(0).Token.Location.Column;
            mensajes.AddLast("El valor de Permissions tiene que ser de tipo objeto, Fila: " + l + " Columna: " + c);
            return null;
        }


        public object analizar(ParseTreeNode raiz, LinkedList<string> mensajes)
        {
            if (raiz != null)
            {
                string etiqueta = raiz.Term.Name.ToLower();
                switch (etiqueta)
                {


                    case "lista":
                        LinkedList<string> lista = new LinkedList<string>();
                        object res;
                        if (raiz.ChildNodes.Count() == 3)
                        {
                            lista = (LinkedList<string>)analizar(raiz.ChildNodes.ElementAt(0), mensajes);
                            l = raiz.ChildNodes.ElementAt(1).Token.Location.Line;
                            c = raiz.ChildNodes.ElementAt(1).Token.Location.Column;
                            res = analizar(raiz.ChildNodes.ElementAt(2).ChildNodes.ElementAt(1), mensajes);
                        }
                        else
                        {
                            //-------------------------------- IMPORTAR ------------------------------------------
                            if (raiz.ChildNodes.ElementAt(0).ChildNodes.Count() == 1)
                            {
                                ParseTreeNode hijo = raiz.ChildNodes.ElementAt(0).ChildNodes.ElementAt(0);
                                string direccion = hijo.Token.Text.TrimStart('$').TrimStart('{').TrimEnd('}').TrimEnd('}').TrimEnd('$').TrimEnd('}');
                                object nuevoNodo = analizarImport(direccion, mensajes);
                                if (nuevoNodo != null) return analizar((ParseTreeNode)nuevoNodo, mensajes);
                                else return null;
                            }
                            l = raiz.ChildNodes.ElementAt(0).ChildNodes.ElementAt(0).Token.Location.Line;
                            c = raiz.ChildNodes.ElementAt(0).ChildNodes.ElementAt(0).Token.Location.Column;
                            res = analizar(raiz.ChildNodes.ElementAt(0).ChildNodes.ElementAt(1), mensajes);
                        }

                        if (res != null)
                        {
                            Atributo atributo = buscarAtributo((LinkedList<Atributo>)res, "name");
                            if(atributo == null)
                            {
                                mensajes.AddLast("Los permisos necesitan un nombre, Linea: " + l + " Columna: " + c);
                                return lista;
                            }
                            lista.AddLast(atributo.valor.ToString());
                        }

                        return lista;




                    case "inobjetos":
                        LinkedList<string> lista2 = new LinkedList<string>();
                        object res2;
                        if (raiz.ChildNodes.Count() == 5)
                        {
                            lista2 = (LinkedList<string>)analizar(raiz.ChildNodes.ElementAt(0), mensajes);
                            l = raiz.ChildNodes.ElementAt(1).Token.Location.Line;
                            c = raiz.ChildNodes.ElementAt(1).Token.Location.Column;
                            res2 = analizar(raiz.ChildNodes.ElementAt(3), mensajes);
                        }
                        else
                        {
                           
                            l = raiz.ChildNodes.ElementAt(0).Token.Location.Line;
                            c = raiz.ChildNodes.ElementAt(0).Token.Location.Column;
                            res2 = analizar(raiz.ChildNodes.ElementAt(1), mensajes);
                        }

                        if (res2 != null)
                        {
                            Atributo atributo = buscarAtributo((LinkedList<Atributo>)res2, "name");
                            if (atributo == null)
                            {
                                mensajes.AddLast("Los permisos necesitan un nombre, Linea: " + l + " Columna: " + c);
                                return lista2;
                            }
                            lista2.AddLast(atributo.valor.ToString());
                        }

                        return lista2;


                    //-------------------------------------- objetos -------------------------------------------------------------------
                    case "objetos":
                        LinkedList<Atributo> listaValores = new LinkedList<Atributo>();
                        object resO;
                        if (raiz.ChildNodes.Count() == 3)
                        {
                            listaValores = (LinkedList<Atributo>)analizar(raiz.ChildNodes.ElementAt(0), mensajes);
                            resO = analizar(raiz.ChildNodes.ElementAt(2), mensajes);
                        }
                        else resO = analizar(raiz.ChildNodes.ElementAt(0), mensajes);

                        if (resO != null) listaValores.AddLast((Atributo)resO);
                        return listaValores;

                    //------------------------------------ OBJETO -----------------------------------------------------------------------
                    case "objeto":

                        string nombre = raiz.ChildNodes.ElementAt(0).Token.Text.ToLower().TrimEnd('\"').TrimStart('\"');

                        if (nombre.Equals("name"))
                        {
                            string valor = raiz.ChildNodes.ElementAt(2).ChildNodes.ElementAt(0).Token.Text.ToLower().TrimEnd('\"').TrimStart('\"');
                            Atributo a = new Atributo(nombre, valor, "");
                            return a;
                        }
                        
                        return null;
                        break;



                   







                }
            }
            return null;
        }


        public Atributo buscarAtributo(LinkedList<Atributo> lk, string atributo)
        {
            foreach (Atributo at in lk)
            {
                if (at.nombre.Equals(atributo)) return at;
            }
            return null;
        }



        public object analizarImport(string direccion, LinkedList<string> mensajes)
        {
            try
            {
                string text = System.IO.File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\DATABASE", direccion));
                GramaticaChison gramatica = new GramaticaChison();
                LanguageData lenguaje = new LanguageData(gramatica);
                Parser parser = new Parser(lenguaje);
                ParseTree arbol = parser.Parse(text);
                ParseTreeNode raiz = arbol.Root;

                if (arbol != null)
                {
                    for (int i = 0; i < arbol.ParserMessages.Count(); i++)
                    {
                        mensajes.AddLast(arbol.ParserMessages.ElementAt(i).Message + " Linea: " + arbol.ParserMessages.ElementAt(i).Location.Line.ToString()
                                  + " Columna: " + arbol.ParserMessages.ElementAt(i).Location.Column.ToString() + ", ARCHIVO: " + direccion);
                    }

                    System.Diagnostics.Debug.WriteLine(raiz.ChildNodes.ElementAt(0).Term.Name);
                    if (arbol.ParserMessages.Count() < 1) return raiz.ChildNodes.ElementAt(0);



                }
                else return null;

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("ERROR CHISON SintacticoChison: " + e.Message);

            }
            return null;
        }


    }
}
