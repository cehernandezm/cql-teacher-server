using cql_teacher_server.CHISON.Componentes;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CHISON.Arbol
{
    public class AnalizarProcedure
    {
        public object analizar(ParseTreeNode raiz)
        {
            if (raiz != null)
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

                            System.Diagnostics.Debug.WriteLine("Error Semantico: No se le puede asignar una lista al atributo: "
                                    + token + ", Linea: " + raiz.ChildNodes.ElementAt(0).Token.Location.Line + " Columna: "
                                    + raiz.ChildNodes.ElementAt(0).Token.Location.Column);
                            return null;

                        }
                        else if (hijoT.ChildNodes.Count() == 3) //---------------------- [ TABLAS ] ------------------------------------------------------------------
                        {
                            System.Diagnostics.Debug.WriteLine("Error Semantico: No se le puede asignar una lista al atributo: "
                                    + token + ", Linea: " + raiz.ChildNodes.ElementAt(0).Token.Location.Line + " Columna: "
                                    + raiz.ChildNodes.ElementAt(0).Token.Location.Column);
                            return null;
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
                                if(valorTemp.Equals("true") || valorTemp.Equals("false")) tipo = "BOOLEAN";
                                else if (valorTemp.Equals("in") || valorTemp.Equals("out")) tipo = "AS";
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
                            else if (token.Equals("AS"))
                            {
                                if (!tipo.Equals("AS"))
                                {
                                    System.Diagnostics.Debug.WriteLine("ERROR AS SOLO ACEPTA UN VALOR IN|OUT NO SE ESPERABA "
                                        + valor + " , Linea : " + hijoT.ChildNodes.ElementAt(0).Token.Location.Line + " Columna: "
                                        + hijoT.ChildNodes.ElementAt(0).Token.Location.Column);
                                    return null;
                                }
                            }
                        }
                        Atributo a = new Atributo(token, valor, tipo);
                        return a;
                        break;



                    //-------------------------------------------------------------- analizar las tablas ---------------------------------------------------------
                    case "listatablas":

                        LinkedList<Columna> listaTablas = new LinkedList<Columna>();
                        LinkedList<Atributo> listaAtri = new LinkedList<Atributo>();
                        ParseTreeNode hijoTa;
                        if (raiz.ChildNodes.Count() == 3)
                        {
                            listaTablas = (LinkedList<Columna>)analizar(raiz.ChildNodes.ElementAt(0));

                            hijoTa = raiz.ChildNodes.ElementAt(2);
                        }
                        else hijoTa = raiz.ChildNodes.ElementAt(0);

                        int linea = hijoTa.ChildNodes.ElementAt(0).Token.Location.Line;
                        int columna = hijoTa.ChildNodes.ElementAt(0).Token.Location.Column;

                        listaAtri = (LinkedList<Atributo>)analizar(hijoTa.ChildNodes.ElementAt(1));


                        if (buscarAtributo(listaAtri, "NAME") && buscarAtributo(listaAtri, "TYPE") && buscarAtributo(listaAtri, "AS"))
                        {
                            Boolean existe = buscarColumna(listaTablas, getNombre(listaAtri));
                            Columna t = new Columna(listaAtri);
                            if (!existe) listaTablas.AddLast(t);
                            else System.Diagnostics.Debug.WriteLine("Error semantico ya existe un Procedure con este nombre: " + getNombre(listaAtri) + ", Linea: "
                                    + linea + " Columna: " + columna);
                        }
                        else System.Diagnostics.Debug.WriteLine("Error semantico los  Procedure tiene que tener NAME , TYPE y AS, Linea: "
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

        public string getNombre(LinkedList<Atributo> lk)
        {
            foreach (Atributo at in lk)
            {
                if (at.nombre.Equals("NAME")) return (String)at.valor;
            }
            return "sinnombre";
        }

        public Boolean buscarAtributo(LinkedList<Atributo> lk, string atributo)
        {
            foreach (Atributo at in lk)
            {
                if (at.nombre.Equals(atributo)) return true;
            }
            return false;
        }

        public Boolean buscarColumna(LinkedList<Columna> lt, string nombre)
        {
            foreach (Columna ta in lt)
            {
                foreach (Atributo at in ta.atributos)
                {
                    if (at.nombre.Equals("NAME") && at.valor.Equals(nombre)) return true;
                }
            }
            return false;
        }

    }
}
