using chat_teacher_server.LUP.Arbol;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace chat_teacher_server.LUP.Gramatica
{
    public class SintacticoLUP
    {

        //------------------------------------------------- Metodo principal que sera llamado para analizar la cadena entrante -------------------------------------
        public string analizar(String cadena)
        {
            GramaticaLUP gramatica = new GramaticaLUP();
            LanguageData lenguaje = new LanguageData(gramatica);
            Parser parser = new Parser(lenguaje);
            ParseTree arbol = parser.Parse(cadena);
            ParseTreeNode raiz = arbol.Root;

            //LinkedList<InstruccionLUP> AST = instrucciones(raiz.ChildNodes.ElementAt(0));
            if (arbol != null)
            {
                GraficarLUP.Construir(raiz);
                string salida = "";
                for (int i = 0; i < arbol.ParserMessages.Count(); i++)
                {

                    salida += arbol.ParserMessages.ElementAt(i).Message + " Linea: " + arbol.ParserMessages.ElementAt(i).Location.Line.ToString()
                              + " Columna: " + arbol.ParserMessages.ElementAt(i).Location.Column.ToString() + "\n";
                }
                return salida;
            }
            return "";

        }


        public LinkedList<InstruccionLUP> instrucciones(ParseTreeNode actual)
        {
            LinkedList<InstruccionLUP> lista = new LinkedList<InstruccionLUP>();
            lista.AddLast(instruccion(actual.ChildNodes.ElementAt(0)));
            return lista;
        }

        public InstruccionLUP instruccion(ParseTreeNode actual)
        {
            string tokenOperacion = actual.ChildNodes.ElementAt(0).ToString().Split(' ')[0].ToLower();
            return null;
        }
       

    }
}
