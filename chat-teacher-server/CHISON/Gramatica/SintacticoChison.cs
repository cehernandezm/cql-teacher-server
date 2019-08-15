using cql_teacher_server.CHISON.Componentes;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cql_teacher_server.CHISON.Gramatica
{
    class SintacticoChison
    {
        string usuarioActual = "none";
        string baseActual = "none";
        
        LinkedList<BaseDeDatos> global = TablaBaseDeDatos.global;
        public void analizar(string cadena)
        {
            GramaticaChison gramatica = new GramaticaChison();
            LanguageData lenguaje = new LanguageData(gramatica);
            Parser parser = new Parser(lenguaje);
            ParseTree arbol = parser.Parse(cadena);
            ParseTreeNode raiz = arbol.Root;

            if(arbol != null)
            {
                for(int i = 0; i < arbol.ParserMessages.Count(); i++)
                {
                    System.Diagnostics.Debug.WriteLine(arbol.ParserMessages.ElementAt(i).Message + " Linea: " + arbol.ParserMessages.ElementAt(i).Location.Line.ToString()
                              + " Columna: " + arbol.ParserMessages.ElementAt(i).Location.Column.ToString() + "\n");
                }

                if(arbol.ParserMessages.Count() < 1)
                {
                    graficar(raiz);

                    ejecutar(raiz.ChildNodes.ElementAt(2));
                    
                }
                    
            }else System.Diagnostics.Debug.WriteLine("ERROR CHISON VACIO");



        }


        /*-------------------------------------------------------------------------------------------------------------------------------------------------
         ---------------------------------------------------------- METODOS PARA ANALIZAR EL ARBOL --------------------------------------------------------
         -------------------------------------------------------------------------------------------------------------------------------------------------*/

        public Object ejecutar(ParseTreeNode raiz)
        {
            if (raiz != null)
            {
                string etiqueta = raiz.ToString().Split(' ')[0].ToLower();
                switch (etiqueta)
                {
                    //-------------------------------------- Instruccion Superior ------------------------------
                    case "intruccion_superior":
                        ejecutar(raiz.ChildNodes.ElementAt(0));
                         //ejecutar(raiz.ChildNodes.ElementAt(2));   
                        break;

                    //--------------------------------- database ----------------------------------------------------------------
                    case "database":
                        if (raiz.ChildNodes.Count() == 4) { }
                        else ejecutar(raiz.ChildNodes.ElementAt(3));
                        break;


                    //------------------------------------- bases ----------------------------------------------------------------------
                    case "bases":
                        if (raiz.ChildNodes.Count() == 3)
                        {
                            //-------------------------------- bases coma baseU ------------------------------------------------------

                            ejecutar(raiz.ChildNodes.ElementAt(0));
                            ParseTreeNode hijo = raiz.ChildNodes.ElementAt(2);
                            LinkedList<Atributo> lista = (LinkedList<Atributo>)ejecutar(hijo.ChildNodes.ElementAt(1));
                            BaseDeDatos newBase = new BaseDeDatos(lista, "sin usar");
                            
                            global.AddLast(newBase);
                        }
                        else
                        {   
                            //-------------------------------- baseU ------------------------------------------------------

                            ParseTreeNode hijo = raiz.ChildNodes.ElementAt(0);
                            LinkedList<Atributo> lista = (LinkedList<Atributo>)ejecutar(hijo.ChildNodes.ElementAt(1));
                            BaseDeDatos newBase = new BaseDeDatos(lista, "sin usar");
                            global.AddLast(newBase);
                        }
                        break;

                    //-------------------------------------- objetos -------------------------------------------------------------------
                    case "objetos":
                        //-------------------------- objetos , objeto -----------------------------------------------------------------
                        if(raiz.ChildNodes.Count() == 3)
                        {
                            LinkedList<Atributo> listaA = (LinkedList<Atributo>)ejecutar(raiz.ChildNodes.ElementAt(0));
                            Atributo aa = (Atributo)ejecutar(raiz.ChildNodes.ElementAt(2));
                            if( aa != null) listaA.AddLast(aa);
                            return listaA;
                        }
                        else if(raiz.ChildNodes.Count() == 1)
                        {
                            //---------------------------- objeto -----------------------------------------------------------------------
                            LinkedList<Atributo> listaA = new LinkedList<Atributo>();
                            Atributo aa = (Atributo)ejecutar(raiz.ChildNodes.ElementAt(0));
                            if (aa != null) listaA.AddLast(aa);
                            return listaA;
                        }
                        break;

                    //------------------------------------ CADENA -----------------------------------------------------------------------
                    case "objeto":
                        string token = raiz.ChildNodes.ElementAt(0).ToString().Split("(")[0];
                        token = token.TrimEnd();

                        Object valor = null;

                        ParseTreeNode hijoT = raiz.ChildNodes.ElementAt(2);
                        string tipo = hijoT.ChildNodes.ElementAt(0).ToString().Split("(")[1];

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
                        else if(tipo.Equals("Key symbol)"))
                        {
                            tipo = "BOOLEAN";
                            string valorTemp = hijoT.ChildNodes.ElementAt(0).ToString().Split("(")[0];
                            valorTemp = valorTemp.Replace("\"", string.Empty);
                            valorTemp = valorTemp.TrimEnd();
                            valorTemp = valorTemp.TrimStart();
                            valor = (string)valorTemp;
                        }

                        Atributo a = new Atributo(token, valor, tipo);
                        if(token.Equals("NAME"))
                        {
                            if (tipo.Equals("CADENA"))
                            {
                                usuarioActual = (String)valor;
                                return a;
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("ERROR NAME SOLO ACEPTA UN VALOR CADENA NO SE ESPERABA "  
                                    + valor + " , Linea : " + hijoT.ChildNodes.ElementAt(0).Token.Location.Line + " Columna: " 
                                    + hijoT.ChildNodes.ElementAt(0).Token.Location.Column);
                                return null;
                            }

                        }else return a;

                        break;
                }
            }
            return null;
        }
















        /*-------------------------------------------------------------------------------------------------------------------------------------------------------
         * ---------------------------------------------------- METODOS PARA GRAFICAR ---------------------------------------------------------------------------
         -------------------------------------------------------------------------------------------------------------------------------------------------------*/

        public void graficar(ParseTreeNode raiz)
        {
            System.IO.StreamWriter f = new System.IO.StreamWriter("Reportes/ASTChison.txt");
            f.Write("digraph Arbol{ rankdir=TB; \n node[shape = box, style = filled, color = white];");
            recorrer(raiz, f);
            f.Write("\n}");
            f.Close();
        }


        //-------------------------------------------------- RECORRE TODO EL ARBOL GUARDANDOLO EN UN ARCHIVO--------------------------------------------------------
        public static void recorrer(ParseTreeNode raiz, System.IO.StreamWriter f)
        {
            if (raiz != null)
            {
                f.Write("nodo" + raiz.GetHashCode() + "[label=\"" + raiz.ToString().Replace("\"", "\\\"") + " \", fillcolor=\"LightBlue\", style =\"filled\", shape=\"box\"]; \n");
                if (raiz.ChildNodes.Count > 0)
                {
                    ParseTreeNode[] hijos = raiz.ChildNodes.ToArray();
                    for (int i = 0; i < raiz.ChildNodes.Count; i++)
                    {
                        recorrer(hijos[i], f);
                        f.Write("\"nodo" + raiz.GetHashCode() + "\"-> \"nodo" + hijos[i].GetHashCode() + "\" \n");
                    }
                }
            }

        }

    }
}
