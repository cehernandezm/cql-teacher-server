using cql_teacher_server.CHISON.Componentes;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CHISON.Arbol
{
    public class AnalizarData
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

                        string nombre = raiz.ChildNodes.ElementAt(0).Token.Text.ToLower().TrimEnd('\"').TrimStart('\"');

                        object valor = analizar(raiz.ChildNodes.ElementAt(2));

                        Atributo a = new Atributo(nombre, valor, "");
                        return a;
                        break;



                    //-------------------------------------------------------------- analizar las tablas ---------------------------------------------------------
                    case "tipo":

                        if (raiz.ChildNodes.Count() == 1)
                        {
                            string term = raiz.ChildNodes.ElementAt(0).Term.Name.ToLower().TrimStart().TrimEnd();
                            string valorRetornar = raiz.ChildNodes.ElementAt(0).Token.Text.TrimEnd('\"').TrimStart('\"');
                            valorRetornar = valorRetornar.TrimStart('\'').TrimEnd('\'');
                            valorRetornar = valorRetornar.TrimEnd().TrimStart();
                            if (term.Equals("cadena")) return valorRetornar;
                            else if (term.Equals("true")) return true;
                            else if (term.Equals("false")) return false;
                            else if (term.Equals("entero")) return Int32.Parse(valorRetornar);
                            else if (term.Equals("decimal")) return Double.Parse(valorRetornar);
                            else if (term.Equals("fecha")) return DateTime.Parse(valorRetornar);
                            else if (term.Equals("hora")) return TimeSpan.Parse(valorRetornar);
                        }

                        break;
                }
            }
            return null;
        }








    }
}
