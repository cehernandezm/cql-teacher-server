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
    public class AnalizarUsuario
    {

        public object analizar(ParseTreeNode raiz)
        {
            if (raiz != null)
            {
                string etiqueta = raiz.ToString().Split(' ')[0].ToLower();
                switch (etiqueta)
                {

                    //-------------------------------------- objetos -------------------------------------------------------------------
                    case "objetos":
                        //-------------------------- objetos , objeto -----------------------------------------------------------------
                        if (raiz.ChildNodes.Count() == 3)
                        {
                            LinkedList<Atributo> listaA = (LinkedList<Atributo>)analizar(raiz.ChildNodes.ElementAt(0));
                            Atributo aa = (Atributo)analizar(raiz.ChildNodes.ElementAt(2));
                            if (aa != null) listaA.AddLast(aa);
                            return listaA;
                        }
                        else if (raiz.ChildNodes.Count() == 1)
                        {
                            //---------------------------- objeto -----------------------------------------------------------------------
                            LinkedList<Atributo> listaA = new LinkedList<Atributo>();
                            Atributo aa = (Atributo)analizar(raiz.ChildNodes.ElementAt(0));
                            if (aa != null) listaA.AddLast(aa);
                            return listaA;
                        }
                        break;

                    //------------------------------------ OBJETO -----------------------------------------------------------------------
                    case "objeto":


                        string token = raiz.ChildNodes.ElementAt(0).ToString().Split("(")[0];
                        token = token.TrimEnd();

                        Object valor = null;
                        string tipo = "";
                        ParseTreeNode hijoT = raiz.ChildNodes.ElementAt(2);
                        if (hijoT.ChildNodes.Count() == 2) // -------------------------------------------- [ ] -------------------------------------------------------
                        {

                            if (token.Equals("PERMISSIONS"))
                            {
                                tipo = "PERMISSIONS";
                                valor = new LinkedList<string>();
                            }
                            else valor = new LinkedList<string>();
                            

                        }
                        else if (hijoT.ChildNodes.Count() == 3) //---------------------- [ TABLAS ] ------------------------------------------------------------------
                        {
                            string token1 = hijoT.ChildNodes.ElementAt(1).ToString().Split(' ')[0].ToLower();
                            AnalizarPermisos analisis = new AnalizarPermisos();
                            tipo = "PERMISSIONS";
                            if (token.Equals("PERMISSIONS"))
                            {
                                if (token1.Equals("importar"))
                                {
                                    string direccion = hijoT.ChildNodes.ElementAt(1).ChildNodes.ElementAt(2).ToString().Split('(')[0];
                                    direccion = direccion.TrimEnd();
                                    direccion += ".chison";
                                    object res = analizarImport(direccion);
                                    valor = (LinkedList<string>)analisis.analizar((ParseTreeNode)res);

                                }
                                else valor = (LinkedList<string>)analisis.analizar(hijoT.ChildNodes.ElementAt(1));
                            }
                            else valor = "";
                            
                        }
                        else
                        {

                            tipo = hijoT.ChildNodes.ElementAt(0).ToString().Split("(")[1];

                            if (tipo.Equals("hora)"))
                            {
                                tipo = "HORA";
                                string valorTemp = hijoT.ChildNodes.ElementAt(0).ToString().Split("(")[0];
                                valorTemp = valorTemp.Replace("\'", string.Empty);
                                valorTemp = valorTemp.TrimEnd();
                                valorTemp = valorTemp.TrimStart();
                                valor = (string)valorTemp;
                            }
                            else if (tipo.Equals("fecha)"))
                            {
                                tipo = "FECHA";
                                string valorTemp = hijoT.ChildNodes.ElementAt(0).ToString().Split("(")[0];
                                valorTemp = valorTemp.Replace("\'", string.Empty);
                                valorTemp = valorTemp.TrimEnd();
                                valorTemp = valorTemp.TrimStart();
                                valor = (string)valorTemp;
                            }
                            else if (tipo.Equals("cadena)"))
                            {
                                tipo = "CADENA";
                                string valorTemp = hijoT.ChildNodes.ElementAt(0).ToString().Split("(")[0];
                                valorTemp = valorTemp.TrimEnd();
                                valor = (string)valorTemp;
                            }
                            else if (tipo.Equals("entero)"))
                            {
                                tipo = "ENTERO";
                                string valorTemp = hijoT.ChildNodes.ElementAt(0).ToString().Split("(")[0];
                                valorTemp = valorTemp.TrimEnd();
                                valor = (string)valorTemp;
                            }
                            else if (tipo.Equals("decimal)"))
                            {
                                tipo = "DECIMAL";
                                string valorTemp = hijoT.ChildNodes.ElementAt(0).ToString().Split("(")[0];
                                valorTemp = valorTemp.TrimEnd();
                                valor = (string)valorTemp;
                            }
                            else if (tipo.Equals("Keyword)"))
                            {
                                string valorTemp = hijoT.ChildNodes.ElementAt(0).ToString().Split("(")[0];
                                valorTemp = valorTemp.Replace("\"", string.Empty);
                                valorTemp = valorTemp.TrimEnd();
                                valorTemp = valorTemp.TrimStart();
                                if (valorTemp.Equals("true") || valorTemp.Equals("false")) tipo = "BOOLEAN";
                                valor = (string)valorTemp;
                            }
                            if (token.Equals("NAME"))
                            {
                                if (!tipo.Equals("CADENA"))
                                {
                                    System.Diagnostics.Debug.WriteLine("ERROR NAME SOLO ACEPTA UN VALOR CADENA NO SE ESPERABA "
                                        + valor + " , Linea : " + hijoT.ChildNodes.ElementAt(0).Token.Location.Line + " Columna: "
                                        + hijoT.ChildNodes.ElementAt(0).Token.Location.Column);
                                    return null;
                                }

                            }
                            
                        }
                        Atributo a = new Atributo(token, valor, tipo);
                        return a;
                        break;



                    //-------------------------------------------------------------- analizar las tablas ---------------------------------------------------------
                    case "listatablas":

                        LinkedList<Atributo> listaAtri = new LinkedList<Atributo>();
                        ParseTreeNode hijoTa;
                        if (raiz.ChildNodes.Count() == 3)
                        {
                            analizar(raiz.ChildNodes.ElementAt(0));

                            hijoTa = raiz.ChildNodes.ElementAt(2);
                        }
                        else hijoTa = raiz.ChildNodes.ElementAt(0);

                        int linea = hijoTa.ChildNodes.ElementAt(0).Token.Location.Line;
                        int columna = hijoTa.ChildNodes.ElementAt(0).Token.Location.Column;

                        listaAtri = (LinkedList<Atributo>)analizar(hijoTa.ChildNodes.ElementAt(1));


                        if (buscarAtributo(listaAtri, "NAME") != null && buscarAtributo(listaAtri, "PASSWORD") != null)
                        {
                            string nombre = (string)buscarAtributo(listaAtri, "NAME");
                            nombre = nombre.ToLower().TrimEnd().TrimStart();
                            string password = (string)buscarAtributo(listaAtri, "PASSWORD");

                            object res = buscarAtributo(listaAtri, "PERMISSIONS");

                            LinkedList<string> listaBases = new LinkedList<string>();
                            if (res != null) listaBases = (LinkedList<String>)res;

                            Usuario existe = TablaBaseDeDatos.getUsuario(nombre);
                            Usuario us = new Usuario(nombre,password,listaBases);
                            if (existe == null) TablaBaseDeDatos.listaUsuario.AddLast(us);
                            else System.Diagnostics.Debug.WriteLine("Error semantico ya existe un usuario con este nombre: " + nombre + ", Linea: " +
                                    + linea + " Columna: " + columna);
                        }
                        else System.Diagnostics.Debug.WriteLine("Error semantico un Usuario tiene que tener NAME y PASSWORD"
                                    + linea + " Columna: " + columna);

                        break;
                }
            }
            return null;
        }



        public object buscarAtributo(LinkedList<Atributo> lk, string atributo)
        {
            foreach (Atributo at in lk)
            {
                if (at.nombre.Equals(atributo)) return at.valor;
            }
            return null;
        }






        public object analizarImport(string direccion)
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
                        System.Diagnostics.Debug.WriteLine(arbol.ParserMessages.ElementAt(i).Message + " Linea: " + arbol.ParserMessages.ElementAt(i).Location.Line.ToString()
                                  + " Columna: " + arbol.ParserMessages.ElementAt(i).Location.Column.ToString() + " Archivo : " + direccion +"\n");
                    }

                    if (arbol.ParserMessages.Count() < 1)
                    {

                        return raiz.ChildNodes.ElementAt(0);

                    }

                }
                else return null;

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("ERROR CHISON AnalizarTablas: " + e.Message);

            }
            return null;
        }


    }
}
