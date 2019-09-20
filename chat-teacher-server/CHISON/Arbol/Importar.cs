using cql_teacher_server.CHISON.Gramatica;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CHISON.Arbol
{
    public class Importar
    {

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
