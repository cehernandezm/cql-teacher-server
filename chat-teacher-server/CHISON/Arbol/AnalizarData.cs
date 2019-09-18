using cql_teacher_server.CHISON.Componentes;
using cql_teacher_server.CQL.Componentes;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CHISON.Arbol
{
    public class AnalizarData
    {
        int l = 0;
        int c = 0;

        

        public object inicio(ParseTreeNode raiz, LinkedList<string> mensajes)
        {
            if (raiz.ChildNodes.Count() == 2) return new LinkedList<Data>();
            else if (raiz.ChildNodes.Count() == 3)
            {
                object res = (analizar(raiz.ChildNodes.ElementAt(1), mensajes) == null) ? null : (LinkedList<Data>)analizar(raiz.ChildNodes.ElementAt(1), mensajes);
                return res;
            }
            l = raiz.ChildNodes.ElementAt(0).Token.Location.Line;
            c = raiz.ChildNodes.ElementAt(0).Token.Location.Column;
            mensajes.AddLast("El valor de DATA tiene que ser de tipo objeto, Fila: " + l + " Columna: " + c);
            return null;
        }


        public object analizar(ParseTreeNode raiz, LinkedList<string> mensajes)
        {
            if (raiz != null)
            {
                string etiqueta = raiz.Term.Name.ToLower() ;
                switch (etiqueta)
                {


                    case "inobjetos":
                        LinkedList<Data> lista = new LinkedList<Data>();
                        object res;
                        if(raiz.ChildNodes.Count() == 5)
                        {
                            lista = (LinkedList<Data>)analizar(raiz.ChildNodes.ElementAt(0), mensajes);
                            l = raiz.ChildNodes.ElementAt(2).Token.Location.Line;
                            c = raiz.ChildNodes.ElementAt(2).Token.Location.Column;
                            res = analizar(raiz.ChildNodes.ElementAt(3),mensajes);
                        }
                        else
                        {
                            l = raiz.ChildNodes.ElementAt(0).Token.Location.Line;
                            c = raiz.ChildNodes.ElementAt(0).Token.Location.Column;
                            res = analizar(raiz.ChildNodes.ElementAt(1),mensajes);
                        }

                        if (res != null) lista.AddLast(new Data((LinkedList<Atributo>)res));

                        return lista;
                        break;



                    //-------------------------------------- objetos -------------------------------------------------------------------
                    case "objetos":
                        LinkedList<Atributo> listaValores = new LinkedList<Atributo>();
                        object resO;
                        if (raiz.ChildNodes.Count() == 3)
                        {
                            listaValores = (LinkedList<Atributo>)analizar(raiz.ChildNodes.ElementAt(0), mensajes);
                            resO = analizar(raiz.ChildNodes.ElementAt(2), mensajes);
                        }
                        else resO = analizar(raiz.ChildNodes.ElementAt(0), mensajes);

                        if (resO != null) listaValores.AddLast((Atributo)resO);
                        return listaValores;

                    //------------------------------------ OBJETO -----------------------------------------------------------------------
                    case "objeto":

                        string nombre = raiz.ChildNodes.ElementAt(0).Token.Text.ToLower().TrimEnd('\"').TrimStart('\"');

                        object valor = analizar(raiz.ChildNodes.ElementAt(2),mensajes);

                        Atributo a = new Atributo(nombre, valor, "");
                        return a;
                        break;



                    //-------------------------------------------------------------- analizar las tablas ---------------------------------------------------------
                    case "tipo":

                        if (raiz.ChildNodes.Count() == 1)
                        {
                            string term = raiz.ChildNodes.ElementAt(0).Term.Name.ToLower().TrimStart().TrimEnd();
                            if (term.Equals("inobjetos")) return (LinkedList<Data>)analizar(raiz.ChildNodes.ElementAt(0), mensajes);
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
                        else if (raiz.ChildNodes.Count() == 2) return new Set("", new LinkedList<object>());
                        else if(raiz.ChildNodes.Count() == 3)
                        {
                            string token = raiz.ChildNodes.ElementAt(1).Term.Name.ToLower();
                            if (token.Equals("lista")) return (Set)analizar(raiz.ChildNodes.ElementAt(1), mensajes);
                           

                        }

                        break;




                    case "lista":
                        Set set = new Set("",new LinkedList<object>());
                        if (raiz.ChildNodes.Count() == 3)
                        {
                            set = (Set)analizar(raiz.ChildNodes.ElementAt(0), mensajes);
                            set.datos.AddLast(analizar(raiz.ChildNodes.ElementAt(2), mensajes));
                        }
                        else set.datos.AddLast(analizar(raiz.ChildNodes.ElementAt(0), mensajes));
                        return set;
                        break;
                }
            }
            return null;
        }








    }
}
