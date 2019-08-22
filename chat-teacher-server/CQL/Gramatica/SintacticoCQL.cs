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

                    foreach(Simbolo s in tablaGlobal)
                    {
                        System.Diagnostics.Debug.WriteLine("Tipo: " + s.Tipo + " Nombre: " + s.nombre + " Valor: " + s.valor);
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
                object res = instruccion(raiz.ChildNodes.ElementAt(1));
                if (res.GetType() == typeof(InstruccionCQL)) lista.AddLast((InstruccionCQL)res);
                else lista = new LinkedList<InstruccionCQL>(lista.Union((LinkedList<InstruccionCQL>)res));          
                return lista;
            }
            //------------------  instruccion -------------
            else
            {
                LinkedList<InstruccionCQL> lista = new LinkedList<InstruccionCQL>();
                object res = instruccion(raiz.ChildNodes.ElementAt(0));
                if (res.GetType() == typeof(InstruccionCQL)) lista.AddLast((InstruccionCQL)res);
                else lista = new LinkedList<InstruccionCQL>(lista.Union((LinkedList<InstruccionCQL>)res));
                return lista;
            }
        }

        /*
         * Metodo que recorre el arbol por completo
         * @raiz es el nodo raiz del arbol a analizar
         * @return una InstruccionCQL o una LinkedList<InstruccionCQL>
         */

        public object instruccion(ParseTreeNode raiz)
        {
            string token = raiz.ChildNodes.ElementAt(0).Term.Name;
            ParseTreeNode hijo = raiz.ChildNodes.ElementAt(0);
            switch (token)
            {
                //-------------------------------- USE DB ----------------------------------------------------------------
                case "use":
                    LinkedList<InstruccionCQL> lu = new LinkedList<InstruccionCQL>();
                    string id = hijo.ChildNodes.ElementAt(1).Token.Text;
                    int linea = hijo.ChildNodes.ElementAt(1).Token.Location.Line;
                    int columna = hijo.ChildNodes.ElementAt(1).Token.Location.Column;
                    lu.AddLast(new Use(id, linea, columna));
                    return lu;

                // ----------------------------------- CREATE DATABASE ------------------------------------------------------
                case "createdatabase":
                    LinkedList<InstruccionCQL> lb = new LinkedList<InstruccionCQL>();
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
                    lb.AddLast(new DataBase(idB, lineaB, columnaB, flag));
                    return lb;


                //------------------------------------------- Expresion ---------------------------------------------------------------
                case "expresion":
                    LinkedList<InstruccionCQL> le = new LinkedList<InstruccionCQL>();
                    le.AddLast(resolver_expresion(hijo));
                    return le;

                //----------------------------------------------- DECLARACION DE VARIABLE -----------------------------------------------
                case "declaracion":
                    string tokend = hijo.ChildNodes.ElementAt(0).Term.Name;
                    int l = 0;
                    int c = 0;
                    string t = "";
                    string i = "";
                    LinkedList<InstruccionCQL> lista = new LinkedList<InstruccionCQL>();

                    if (tokend.Equals("declaracion"))
                    {
                        lista = (LinkedList<InstruccionCQL>)instruccion(hijo);
                        t = declaracionTipo(hijo.ChildNodes.ElementAt(0)); 
                    }
                    else  t = hijo.ChildNodes.ElementAt(0).ChildNodes.ElementAt(0).Token.Text;
                    l = hijo.ChildNodes.ElementAt(1).Token.Location.Line;
                    c = hijo.ChildNodes.ElementAt(1).Token.Location.Column;
                    i = hijo.ChildNodes.ElementAt(1).Token.Text;
                    t = t.ToLower().TrimEnd().TrimStart();
                    i = i.ToLower().TrimEnd().TrimStart();
                    Declaracion a = new Declaracion(t, null, i, l, c);
                    lista.AddLast(a);
                    return lista;

                //----------------------------------------------- DECLARACION ASIGNACION -----------------------------------------------------
                case "declaracionA":
                    string tokenA = hijo.ChildNodes.ElementAt(0).Term.Name;
                    LinkedList<InstruccionCQL> liA = new LinkedList<InstruccionCQL>();
                    string tA = "";
                    int lA = 0;
                    int cA = 0;
                    string iA = "";
                    if (tokenA.Equals("declaracion")) {
                         LinkedList<InstruccionCQL> listaTemp = (LinkedList<InstruccionCQL>)instruccion(hijo);
                         liA = new LinkedList<InstruccionCQL>(liA.Union(listaTemp));
                         tA = declaracionTipo(hijo.ChildNodes.ElementAt(0));
                    }else tA = hijo.ChildNodes.ElementAt(0).ChildNodes.ElementAt(0).Token.Text;
                    lA = hijo.ChildNodes.ElementAt(1).Token.Location.Line;
                    cA = hijo.ChildNodes.ElementAt(1).Token.Location.Column;
                    iA = hijo.ChildNodes.ElementAt(1).Token.Text;
                    tA = tA.ToLower().TrimEnd().TrimStart();
                    iA = iA.ToLower().TrimEnd().TrimStart();
                    Declaracion aA = new Declaracion(tA, resolver_expresion(hijo.ChildNodes.ElementAt(3)), iA, lA, cA);
                    liA.AddLast(aA);
                    return liA;
            }
            return null;
        }




        /*
         * Metodo que resuelve las expresiones aritmeticas,logicas
         * @raiz nodo principal de la lista de expresiones
         */

         public Expresion resolver_expresion(ParseTreeNode raiz)
        {
            if (raiz.ChildNodes.Count() == 3)
            {
                string toke = raiz.ChildNodes.ElementAt(1).Token.Text;
                int l1 = raiz.ChildNodes.ElementAt(1).Token.Location.Line;
                int c1 = raiz.ChildNodes.ElementAt(1).Token.Location.Column;
                return new Expresion(resolver_expresion(raiz.ChildNodes.ElementAt(0)), resolver_expresion(raiz.ChildNodes.ElementAt(2)), getOperacion(toke), l1, c1);

            }
            else if (raiz.ChildNodes.Count() == 2)
            {
                string iden = raiz.ChildNodes.ElementAt(0).Term.Name;
                if (iden.Equals("tipovariable"))
                {
                    int l1 = raiz.ChildNodes.ElementAt(0).ChildNodes.ElementAt(0).Token.Location.Line;
                    int c1 = raiz.ChildNodes.ElementAt(0).ChildNodes.ElementAt(0).Token.Location.Column;
                    string tipov = raiz.ChildNodes.ElementAt(0).ChildNodes.ElementAt(0).Token.Text.TrimEnd().TrimStart().ToLower();
                    return new Expresion(resolver_expresion(raiz.ChildNodes.ElementAt(1)), "CONVERSION", l1, c1, tipov);
                }
                else
                {
                    string toke = raiz.ChildNodes.ElementAt(0).Token.Text;
                    int l1 = raiz.ChildNodes.ElementAt(0).Token.Location.Line;
                    int c1 = raiz.ChildNodes.ElementAt(0).Token.Location.Column;
                    string opera = "";
                    if (toke.Equals("-")) opera = "NEGATIVO";
                    else if (toke.Equals("!")) opera = "NEGACION";
                    return new Expresion(resolver_expresion(raiz.ChildNodes.ElementAt(1)), opera, l1, c1);
                }
                
            }
            else
            {
                string toke = raiz.ChildNodes.ElementAt(0).Term.Name;
                if (toke.Equals("expresion")) return resolver_expresion(raiz.ChildNodes.ElementAt(0));
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
            else if (token.Equals(">")) return "MAYOR";
            else if (token.Equals("<")) return "MENOR";
            else if (token.Equals(">=")) return "MAYORIGUAL";
            else if (token.Equals("<=")) return "MENORIGUAL";
            else if (token.Equals("==")) return "IGUALIGUAL";
            else if (token.Equals("!=")) return "DIFERENTE";
            else if (token.Equals("||")) return "OR";
            else if (token.Equals("&&")) return "AND";
            else if (token.Equals("^")) return "XOR";
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
            else if (token.Equals("hora"))
            {
                valor = valor.TrimEnd('\'');
                valor = valor.TrimStart('\'');
                return new Expresion(valor, "HORA", l1, c1);
            }
            else if (token.Equals("fecha"))
            {
                valor = valor.TrimEnd('\'');
                valor = valor.TrimStart('\'');
                return new Expresion(valor, "FECHA", l1, c1);
            }
            else if (valor.Equals("True") || valor.Equals("False")) return new Expresion(valor, "BOOLEAN", l1, c1);
            else if (token.Equals("ID"))
            {
                System.Diagnostics.Debug.WriteLine("ID");
                return new Expresion(valor, "ID", l1, c1);
            }
            else return new Expresion(null, "NULL", l1, c1);
        }

        /*
         * Metodo que me devuelve el tipo de declaracion
         * @raiz el es nodo a recorrer del arbol
         */
        
        public string declaracionTipo(ParseTreeNode raiz)
        {
            string token = raiz.Term.Name;
            if (token.Equals("declaracion")) return declaracionTipo(raiz.ChildNodes.ElementAt(0));

            string t = raiz.ChildNodes.ElementAt(0).Token.Text;
            return t.ToLower().TrimEnd().TrimStart();


        }
    }
}
