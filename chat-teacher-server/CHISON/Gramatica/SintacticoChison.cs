using cql_teacher_server.CHISON.Arbol;
using cql_teacher_server.CHISON.Componentes;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace cql_teacher_server.CHISON.Gramatica
{
    class SintacticoChison
    {
        
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
                        ParseTreeNode hijo = null;
                        BaseDeDatos newBase;
                        string baseActual;
                        BaseDeDatos oldBase;
                        LinkedList<Atributo> lista;
                        if (raiz.ChildNodes.Count() == 3)
                        {
                            //-------------------------------- bases coma baseU ------------------------------------------------------
                            ejecutar(raiz.ChildNodes.ElementAt(0));
                            hijo = raiz.ChildNodes.ElementAt(2);  
                        }
                        else hijo = raiz.ChildNodes.ElementAt(0);
                        //---------------------------------------------------------- Almacenar la base de datos --------------------------------------------------
                        if( hijo != null)
                        {

                            AnalizarBase analisis = new AnalizarBase();
                            lista = (LinkedList<Atributo>)analisis.analizar(hijo.ChildNodes.ElementAt(1));
                            newBase = new BaseDeDatos(lista, "sin usar");
                            baseActual = getNombre(lista);
                            if (!baseActual.Equals("sinnombre"))
                            {
                                oldBase = TablaBaseDeDatos.getBase(baseActual);

                                if (oldBase == null) global.AddLast(newBase);
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine("Error Semantico: Ya existe una base de datos con este nombre: " + baseActual + ", Linea: "
                                     + hijo.ChildNodes.ElementAt(0).Token.Location.Line + " Columna : " + hijo.ChildNodes.ElementAt(0).Token.Location.Column);
                                }
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("Error Semantico: La base de datos necesita un nombre, Linea: "
                                 + hijo.ChildNodes.ElementAt(0).Token.Location.Line + " Columna : " + hijo.ChildNodes.ElementAt(0).Token.Location.Column);
                            }
                        }
                        
                        break;
                    
                }
            }
            return null;
        }
















        /*-------------------------------------------------------------------------------------------------------------------------------------------------------
         * ---------------------------------------------------- METODOS PARA GRAFICAR ---------------------------------------------------------------------------
         -------------------------------------------------------------------------------------------------------------------------------------------------------*/

            //----------------------------------------------------------------- Crea el archivo --------------------------------------------------------------------
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





        /*----------------------------------------------------------------------------------------------------------------------------------------------------
         * --------------------------------------------------- METODOS VARIOS ---------------------------------------------------------------------------------
         ------------------------------------------------------------------------------------------------------------------------------------------------------*/


        //------------------------------------------------ Devuelve el nombre del objeto a buscar ----------------------------------------------------------------

        public string getNombre(LinkedList<Atributo> lk)
        {
            foreach(Atributo at in lk)
            {
                if (at.nombre.Equals("NAME")) return (String)at.valor;
            }
            return "sinnombre";
        }

        public Boolean buscarAtributo(LinkedList<Atributo> lk, string atributo)
        {
            foreach(Atributo at in lk)
            {
                if (at.nombre.Equals(atributo)) return true;
            }
            return false;
        }

        public Boolean buscarTabla(LinkedList<Tabla> lt , string nombre)
        {
            foreach(Tabla ta in lt)
            {
                foreach(Atributo at in ta.atributos)
                {
                    if (at.nombre.Equals("NAME") && at.valor.Equals(nombre)) return true;
                }
            }
            return false;
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

                        return raiz;

                    }

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
