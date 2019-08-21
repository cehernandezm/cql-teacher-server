using cql_teacher_server.CHISON;
using cql_teacher_server.CQL.Arbol;
using cql_teacher_server.CQL.Componentes;
using cql_teacher_server.Herramientas;
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
            usuario = usuario.TrimEnd();
            usuario = usuario.TrimStart();
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
                    LinkedList<string> mensajes = new LinkedList<string>();
                    String baseD = TablaBaseDeDatos.getMine(usuario);
                    foreach(InstruccionCQL ins in listaInstrucciones)
                    {
                        Mensaje mensa = new Mensaje();
                        object res = ins.ejecutar(tablaGlobal,usuario, ref baseD,mensajes);
                        if(res != null && ins.GetType() == typeof(Expresion) ) System.Diagnostics.Debug.WriteLine(mensa.message("El resultado de la operacion es: " + res.ToString()));
                    }

                    foreach(string m in mensajes)
                    {
                        System.Diagnostics.Debug.WriteLine(m);
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
            string token = raiz.ChildNodes.ElementAt(0).Term.Name;
            ParseTreeNode hijo = raiz.ChildNodes.ElementAt(0);
            switch (token)
            {
                //-------------------------------- USE DB ----------------------------------------------------------------
                case "use":
                    string id = hijo.ChildNodes.ElementAt(1).Token.Text;
                    int linea = hijo.ChildNodes.ElementAt(1).Token.Location.Line;
                    int columna = hijo.ChildNodes.ElementAt(1).Token.Location.Column;
                    return new Use(id, linea, columna);

                // ----------------------------------- CREATE DATABASE ------------------------------------------------------
                case "createdatabase":
                    string idB = "";
                    int lineaB = 0;
                    int columnaB = 0;
                    bool flag = false;

                    //--------------------------------------- CREATE DATABASE ID ------------------------------------------
                    if (hijo.ChildNodes.Count() == 3)
                    {
                        idB = hijo.ChildNodes.ElementAt(2).ToString().Split(' ')[0];
                        lineaB = hijo.ChildNodes.ElementAt(2).Token.Location.Line;
                        columnaB = hijo.ChildNodes.ElementAt(2).Token.Location.Column;
                        flag = false;
                    }
                    //---------------------------------------- CREATE DATABASE IF NOT EXISTS ID -----------------------------------------
                    else
                    {
                        idB = hijo.ChildNodes.ElementAt(5).ToString().Split(' ')[0];
                        lineaB = hijo.ChildNodes.ElementAt(5).Token.Location.Line;
                        columnaB = hijo.ChildNodes.ElementAt(5).Token.Location.Column;
                        flag = true;
                    }
                    return new DataBase(idB,lineaB,columnaB,flag);


                //------------------------------------------- Expresion ---------------------------------------------------------------
                case "expresion":
                    //--------------------------------- expresion operador expresion ---------------------------------------------------
                    if (hijo.ChildNodes.Count() == 3)
                    {
                        string toke = hijo.ChildNodes.ElementAt(1).Token.Text;
                        int l1 = hijo.ChildNodes.ElementAt(1).Token.Location.Line;
                        int c1 = hijo.ChildNodes.ElementAt(1).Token.Location.Column;
                        return new Expresion(resolver_expresion(hijo.ChildNodes.ElementAt(0)), resolver_expresion(hijo.ChildNodes.ElementAt(2)), getOperacion(toke), l1, c1);
                    }
                    //--------------------------------- operador expresion -----------------------------------------------------------
                    else if (hijo.ChildNodes.Count() == 2)
                    {

                    }
                    //--------------------------------------- valor ------------------------------------------------------------------
                    else
                    {
                        string toke = hijo.ChildNodes.ElementAt(0).Term.Name;
                        string valor = hijo.ChildNodes.ElementAt(0).Token.Text;
                        int l1 = hijo.ChildNodes.ElementAt(0).Token.Location.Line;
                        int c1 = hijo.ChildNodes.ElementAt(0).Token.Location.Column;
                        valor = valor.TrimEnd();
                        valor = valor.TrimStart();
                        return getValor(toke, valor, l1, c1);
                    }
                    return null;
            }
            return null;
        }




        /*
         * Metodo que resuelve las expresiones aritmeticas,logicas
         * 
         */


         public Expresion resolver_expresion(ParseTreeNode raiz)
        {
            if(raiz.ChildNodes.Count() == 3)
            {
                string toke = raiz.ChildNodes.ElementAt(1).Token.Text;
                int l1 = raiz.ChildNodes.ElementAt(1).Token.Location.Line;
                int c1 = raiz.ChildNodes.ElementAt(1).Token.Location.Column;
                return new Expresion(resolver_expresion(raiz.ChildNodes.ElementAt(0)), resolver_expresion(raiz.ChildNodes.ElementAt(2)), getOperacion(toke), l1, c1);

            }
            else if(raiz.ChildNodes.Count() == 2)
            {
                return null;
            }
            else
            {
                string toke = raiz.ChildNodes.ElementAt(0).Term.Name;
                string valor = raiz.ChildNodes.ElementAt(0).Token.Text;
                int l1 = raiz.ChildNodes.ElementAt(0).Token.Location.Line;
                int c1 = raiz.ChildNodes.ElementAt(0).Token.Location.Column;

                return getValor(toke, valor, l1, c1);
            }
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


        /*
         * Metodo que devulve que operacion es
         * @raiz es el nodo a buscar su operacion
         */

        public string getOperacion(string token)
        {
            if (token.Equals("+")) return "SUMA";
            else if (token.Equals("-")) return "RESTA";
            else if (token.Equals("*")) return "MULTIPLICACION";
            else if (token.Equals("**")) return "POTENCIA";
            else if (token.Equals("%")) return "MODULO";
            else if (token.Equals("/")) return "DIVISION";
            return "none";
        }


        /*
         * Metodo que devuelve que tipo de valor es
         * @raiz es el nodo a buscar su valor
         */

        public Expresion getValor(string token, string valor, int l1, int c1)
        {
            System.Diagnostics.Debug.WriteLine(valor);
            if (token.Equals("entero")) return new Expresion(valor, "ENTERO", l1, c1);
            else if (token.Equals("decimal")) return new Expresion(valor, "DECIMAL", l1, c1);
            else if (token.Equals("cadena"))
            {
                valor = valor.TrimEnd('"');
                valor = valor.TrimStart('"');
                return new Expresion(valor, "CADENA", l1, c1);
            }
            else if (valor.Equals("True") || valor.Equals("False")) return new Expresion(valor, "BOOLEAN", l1, c1);
            return null;
        }
    }
}
