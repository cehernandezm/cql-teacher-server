using cql_teacher_server.CHISON;
using cql_teacher_server.CQL.Arbol;
using cql_teacher_server.CQL.Componentes;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Gramatica
{
    public class SintacticoCQL
    {
        /*
         * Metodo que analizar la cadena 
         * @cadena es el string a analizar
         * @retonar un set de errores o un set de mensajes
         */

        public void analizar(string cadena, string usuario)
        {
            GramaticaCQL gramatica = new GramaticaCQL();
            LanguageData lenguaje = new LanguageData(gramatica);
            Parser parser = new Parser(gramatica);
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
                    LinkedList<InstruccionCQL> listaInstrucciones = instrucciones(raiz.ChildNodes.ElementAt(0));
                    TablaDeSimbolos tablaGlobal = new TablaDeSimbolos();
                    String baseD = TablaBaseDeDatos.getMine(usuario);
                    foreach(InstruccionCQL ins in listaInstrucciones)
                    {
                        object res = ins.ejecutar(tablaGlobal,usuario, ref baseD);
                        System.Diagnostics.Debug.WriteLine(res.ToString());
                    }

                }
            }

        }

        /*
         * Metodo que las primeras dos producciones del arbol de irony
         * @raiz es el nodo raiz del arbol a analizar
         * @ retorna una lista de instrucciones
         */

        public LinkedList<InstruccionCQL> instrucciones(ParseTreeNode raiz)
        {
            //------------------ instrucciones instruccion -------------
            if (raiz.ChildNodes.Count == 2)
            {
                LinkedList<InstruccionCQL> lista = instrucciones(raiz.ChildNodes.ElementAt(0));
                lista.AddLast(instruccion(raiz.ChildNodes.ElementAt(1)));
                return lista;
            }
            //------------------  instruccion -------------
            else
            {
                LinkedList<InstruccionCQL> lista = new LinkedList<InstruccionCQL>();
                lista.AddLast(instruccion(raiz.ChildNodes.ElementAt(0)));
                return lista;
            }
        }

        /*
         * Metodo que recorre el arbol por completo
         * @raiz es el nodo raiz del arbol a analizar
         * @return una Instruccion
         */

        public InstruccionCQL instruccion(ParseTreeNode raiz)
        {
            string token = raiz.ChildNodes.ElementAt(0).ToString().Split(' ')[0].ToLower();
            ParseTreeNode hijo = raiz.ChildNodes.ElementAt(0);
            switch (token)
            {
                //-------------------------------- USE DB ----------------------------------------------------------------
                case "use":
                    string id = hijo.ChildNodes.ElementAt(1).ToString().Split(' ')[0];
                    int linea = hijo.ChildNodes.ElementAt(1).Token.Location.Line;
                    int columna = hijo.ChildNodes.ElementAt(1).Token.Location.Line;
                    return new Use(id,linea,columna);

            }
            return null;
        }







        /*
         *  METODO PARA CREAR EL ARCHIVO ASTCQL.TXT
         *  @raiz el la raiz del arbol principal
         */

        public void graficar(ParseTreeNode raiz)
        {
            System.IO.StreamWriter f = new System.IO.StreamWriter("Reportes/ASTCQL.txt");
            f.Write("digraph Arbol{ rankdir=TB; \n node[shape = box, style = filled, color = white];");
            recorrer(raiz, f);
            f.Write("\n}");
            f.Close();
        }

        /*
         *  Metodo que recorre el arbol y escribe en el txt la informacion en Graphviz
         *  @raiz el la raiz del arbol principal
         *  @f es un StreamWriter que nos permitira escribir en el archivo
         */
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
