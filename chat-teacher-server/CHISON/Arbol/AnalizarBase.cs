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
    public class AnalizarBase
    {
        

        public object analizar(ParseTreeNode raiz)
        {
            if(raiz != null)
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
                            if (token.Equals("Data"))
                            {
                                tipo = "OBJETOS";
                                valor = new LinkedList<Tabla>();
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("Error Semantico: No se le puede asignar una lista al atributo: "
                                    + token + ", Linea: " + raiz.ChildNodes.ElementAt(0).Token.Location.Line + " Columna: "
                                    + raiz.ChildNodes.ElementAt(0).Token.Location.Column);
                                return null;
                            }
                        }
                        else if (hijoT.ChildNodes.Count() == 3) //---------------------- [ TABLAS/importar] ------------------------------------------------------------------
                        {
                            if (token.Equals("Data"))
                            {
                                string token1 = hijoT.ChildNodes.ElementAt(1).ToString().Split(' ')[0].ToLower();
                                tipo = "OBJETOS";
                                AnalizarTablas analisis = new AnalizarTablas();
                                if (token1.Equals("importar"))
                                {
                                    string direccion = hijoT.ChildNodes.ElementAt(1).ChildNodes.ElementAt(2).ToString().Split('(')[0];
                                    direccion = direccion.TrimEnd();
                                    direccion += ".chison";
                                    object res = analizarImport(direccion);
                                    if (res != null) valor = (LinkedList<Tabla>)analisis.analizar((ParseTreeNode)res);
                                    else valor = new LinkedList<Tabla>();
                                }
                                else valor = (LinkedList<Tabla>)analisis.analizar(hijoT.ChildNodes.ElementAt(1));

                                
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("Error Semantico: No se le puede asignar una lista al atributo: "
                                    + token + ", Linea: " + raiz.ChildNodes.ElementAt(0).Token.Location.Line + " Columna: "
                                    + raiz.ChildNodes.ElementAt(0).Token.Location.Column);
                                return null;
                            }
                        }else if(hijoT.ChildNodes.Count() == 8)
                        {

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
                            else if (tipo.Equals("Key symbol)"))
                            {
                                tipo = "BOOLEAN";
                                string valorTemp = hijoT.ChildNodes.ElementAt(0).ToString().Split("(")[0];
                                valorTemp = valorTemp.Replace("\"", string.Empty);
                                valorTemp = valorTemp.TrimEnd();
                                valorTemp = valorTemp.TrimStart();
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
                }
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
                                  + " Columna: " + arbol.ParserMessages.ElementAt(i).Location.Column.ToString() + "\n");
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
                System.Diagnostics.Debug.WriteLine("ERROR CHISON AnanalizarBase: " + e.Message);

            }
            return null;
        }




    }
}
