using chat_teacher_server.LUP.Arbol;
using chat_teacher_server.LUP.Componentes;
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

            
            if (arbol != null)
            {
                GraficarLUP.Construir(raiz);
                string salida = "";

                for (int i = 0; i < arbol.ParserMessages.Count(); i++)
                {

                    salida += arbol.ParserMessages.ElementAt(i).Message + " Linea: " + arbol.ParserMessages.ElementAt(i).Location.Line.ToString()
                              + " Columna: " + arbol.ParserMessages.ElementAt(i).Location.Column.ToString() + "\n";
                }

                if (arbol.ParserMessages.Count() < 1)
                {
                    InstruccionLUP AST = instrucciones(raiz.ChildNodes.ElementAt(0));
                    if (AST != null)
                    {
                        Object res = AST.ejecutar();
                        if (AST.GetType() == typeof(Usuario))
                        {
                            Boolean flag = (Boolean) res;
                            salida = (flag) ? "[+LOGIN]\n\t[SUCCESS]\n[-LOGIN]": "[+LOGIN]\n\t[FAIL]\n[-LOGIN]";
                            
                        }
                    } 
                }

                return salida;
            }
            return "";

        }


        public InstruccionLUP instrucciones(ParseTreeNode actual)
        {
            return instruccion(actual.ChildNodes.ElementAt(0));
        }

        public InstruccionLUP instruccion(ParseTreeNode actual)
        {
            string token = actual.ToString().Split(' ')[0].ToLower();
            switch (token)
            {
                //------------------------------------------- SI EL PAQUETE ES DE LOGUEO --------------------------------------------------------------------------------
                case "login":

                    string usuario = actual.ChildNodes.ElementAt(8).ToString().Split(' ')[0];
                    string password = actual.ChildNodes.ElementAt(17).ToString().Split(' ')[0];
                    return new Usuario(usuario, password);

                    break;
            }
            
            return null;
        }
       

    }
}
