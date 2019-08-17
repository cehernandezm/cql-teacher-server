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
    public class AnalizarTablas
    {
        string cql_type = "none";

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
                            if (token.Equals("DATA"))
                            {
                                valor = new LinkedList<Data>();
                            }
                            else if (token.Equals("COLUMNS") && cql_type.Equals("TABLE"))
                            {
                                tipo = "COLUMNS";
                                valor = new LinkedList<Columna>();
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("Error Semantico: No se le puede asignar una lista al atributo: "
                                    + token + ", Linea: " + raiz.ChildNodes.ElementAt(0).Token.Location.Line + " Columna: "
                                    + raiz.ChildNodes.ElementAt(0).Token.Location.Column);
                                return null;
                            }
                        }
                        else if (hijoT.ChildNodes.Count() == 3) //---------------------- [ TABLAS ] ------------------------------------------------------------------
                        {
                            string token1 = hijoT.ChildNodes.ElementAt(1).ToString().Split(' ')[0].ToLower();
                            if (token.Equals("DATA"))
                            {
                                System.Diagnostics.Debug.WriteLine("Entro aqui");
                                AnalizarData analisis = new AnalizarData();
                                tipo = "DATA";
                                if (token1.Equals("importar"))
                                {
                                    string direccion = hijoT.ChildNodes.ElementAt(1).ChildNodes.ElementAt(2).ToString().Split('(')[0];
                                    direccion = direccion.TrimEnd();
                                    direccion += ".chison";
                                    object res = analizarImport(direccion);
                                    valor = (LinkedList<Data>)analisis.analizar((ParseTreeNode)res);

                                }
                                else valor = (LinkedList<Data>)analisis.analizar(hijoT.ChildNodes.ElementAt(1));

                            }
                            else if (token.Equals("COLUMNS") && cql_type.Equals("TABLE"))
                            {
                                tipo = "COLUMNS";
                                AnalizarColumna analisis = new AnalizarColumna();
                                if (token1.Equals("importar"))
                                {
                                    string direccion = hijoT.ChildNodes.ElementAt(1).ChildNodes.ElementAt(2).ToString().Split('(')[0];
                                    direccion = direccion.TrimEnd();
                                    direccion += ".chison";
                                    object res = analizarImport(direccion);
                                    valor = (LinkedList<Columna>)analisis.analizar((ParseTreeNode)res);

                                }
                                else valor = (LinkedList<Columna>)analisis.analizar(hijoT.ChildNodes.ElementAt(1));
                            }
                            else if(token.Equals("ATTRS") && cql_type.Equals("OBJECT"))
                            {
                                tipo = "ATTRS";
                                AnalizarObject analisis = new AnalizarObject();
                                if (token1.Equals("importar"))
                                {
                                    string direccion = hijoT.ChildNodes.ElementAt(1).ChildNodes.ElementAt(2).ToString().Split('(')[0];
                                    direccion = direccion.TrimEnd();
                                    direccion += ".chison";
                                    object res = analizarImport(direccion);
                                    valor = (LinkedList<Columna>)analisis.analizar((ParseTreeNode)res);

                                }
                                else valor = (LinkedList<Columna>)analisis.analizar(hijoT.ChildNodes.ElementAt(1));
                            }
                            else if(token.Equals("PARAMETERS") && cql_type.Equals("PROCEDURE"))
                            {
                                tipo = "PARAMETERS";
                                AnalizarProcedure analisis = new AnalizarProcedure();
                                if (token1.Equals("importar"))
                                {
                                    string direccion = hijoT.ChildNodes.ElementAt(1).ChildNodes.ElementAt(2).ToString().Split('(')[0];
                                    direccion = direccion.TrimEnd();
                                    direccion += ".chison";
                                    object res = analizarImport(direccion);
                                    valor = (LinkedList<Columna>)analisis.analizar((ParseTreeNode)res);

                                }
                                else valor = (LinkedList<Columna>)analisis.analizar(hijoT.ChildNodes.ElementAt(1));
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("Error Semantico: No se le puede asignar una lista al atributo: "
                                    + token + ", Linea: " + raiz.ChildNodes.ElementAt(0).Token.Location.Line + " Columna: "
                                    + raiz.ChildNodes.ElementAt(0).Token.Location.Column);
                                return null;
                            }
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
                            else if (tipo.Equals("Keyword)"))
                            {
                                string valorTemp = hijoT.ChildNodes.ElementAt(0).ToString().Split("(")[0];
                                valorTemp = valorTemp.Replace("\"", string.Empty);
                                valorTemp = valorTemp.TrimEnd();
                                valorTemp = valorTemp.TrimStart();
                                if (valorTemp.Equals("true") || valorTemp.Equals("false")) tipo = "BOOLEAN";
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
                            else if (token.Equals("CQL-TYPE")) cql_type = (String)valor;
                        }
                        Atributo a = new Atributo(token, valor, tipo);
                        return a;
                        break;



                    //-------------------------------------------------------------- analizar las tablas ---------------------------------------------------------
                    case "listatablas":

                        LinkedList<Tabla> listaTablas = new LinkedList<Tabla>();
                        LinkedList<Atributo> listaAtri = new LinkedList<Atributo>();
                        ParseTreeNode hijoTa;
                        if (raiz.ChildNodes.Count() == 3)
                        {
                            listaTablas = (LinkedList<Tabla>)analizar(raiz.ChildNodes.ElementAt(0));

                            hijoTa = raiz.ChildNodes.ElementAt(2);
                        }
                        else hijoTa = raiz.ChildNodes.ElementAt(0);

                        int linea = hijoTa.ChildNodes.ElementAt(0).Token.Location.Line;
                        int columna = hijoTa.ChildNodes.ElementAt(0).Token.Location.Column;

                        listaAtri = (LinkedList<Atributo>)analizar(hijoTa.ChildNodes.ElementAt(1));


                        if (buscarAtributo(listaAtri, "NAME") && buscarAtributo(listaAtri, "CQL-TYPE"))
                        {
                            string nombre = (String)valorAtributo(listaAtri, "NAME");

                            Boolean existe = buscarTabla(listaTablas, nombre);

                            object cols = valorAtributo(listaAtri, "COLUMNS");
                            LinkedList<Columna> listaCol = new LinkedList<Columna>();
                            if (cols != null) listaCol = (LinkedList<Columna>)cols;

                            object inf = valorAtributo(listaAtri, "DATA");
                            LinkedList<Data> listainfo = new LinkedList<Data>();
                            if (inf != null) listainfo = (LinkedList<Data>)inf;


                            Tabla t = new Tabla(nombre,listaCol,listainfo);


                            if (!existe) listaTablas.AddLast(t);
                            else System.Diagnostics.Debug.WriteLine("Error semantico ya existe una tabla con este nombre: " + nombre + ", Linea: "
                                    + linea + " Columna: " + columna);
                        }
                        else System.Diagnostics.Debug.WriteLine("Error semantico las tablas tiene que tener NAME Y CQL-TYPE, Linea: "
                                    + linea + " Columna: " + columna); 
                        return listaTablas;

                        break;
                }
            }
            return null;
        }



    /*----------------------------------------------------------------------------------------------------------------------------------------------------
     * --------------------------------------------------- METODOS VARIOS ---------------------------------------------------------------------------------
     ------------------------------------------------------------------------------------------------------------------------------------------------------*/


    //------------------------------------------------ Devuelve el nombre del objeto a buscar ----------------------------------------------------------------



    public Boolean buscarAtributo(LinkedList<Atributo> lk, string atributo)
    {
        foreach (Atributo at in lk)
        {
            if (at.nombre.Equals(atributo)) return true;
        }
        return false;
    }

    public Boolean buscarTabla(LinkedList<Tabla> lt, string nombre)
    {
        foreach (Tabla ta in lt)
        {
                if (ta.nombre.Equals(nombre)) return true;
        }
        return false;
    }


    public object valorAtributo(LinkedList<Atributo> lk, string atributo)
    {
        foreach (Atributo at in lk)
        {
           if (at.nombre.Equals(atributo)) return at.valor;
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
                System.Diagnostics.Debug.WriteLine("ERROR CHISON AnalizarTablas: " + e.Message);

            }
            return null;
        }




    }
}
