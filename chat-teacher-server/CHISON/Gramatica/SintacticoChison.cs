using cql_teacher_server.LUP.Gramatica;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CHISON.Gramatica
{
    class SintacticoChison
    {

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
                    graficar(raiz);
            }else System.Diagnostics.Debug.WriteLine("ERROR CHISON VACIO");



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
