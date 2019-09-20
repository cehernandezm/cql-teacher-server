using cql_teacher_server.CHISON.Gramatica;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CHISON.Arbol
{
    public class AnalizarParameters : Importar
    {
        int l = 0;
        int c = 0;

        public object inicio(ParseTreeNode raiz, LinkedList<string> mensajes)
        {
            if (raiz.ChildNodes.Count() == 2) return new LinkedList<Atributo>();
            else if (raiz.ChildNodes.Count() == 3)
            {
                object res = (analizar(raiz.ChildNodes.ElementAt(1), mensajes) == null) ? null : (LinkedList<Atributo>)analizar(raiz.ChildNodes.ElementAt(1), mensajes);
                return res;
            }
            l = raiz.ChildNodes.ElementAt(0).Token.Location.Line;
            c = raiz.ChildNodes.ElementAt(0).Token.Location.Column;
            mensajes.AddLast("El valor de Paremerts tiene que ser de tipo objeto, Fila: " + l + " Columna: " + c);
            return null;
        }


        public object analizar(ParseTreeNode raiz, LinkedList<string> mensajes)
        {
            if (raiz != null)
            {
                string etiqueta = raiz.Term.Name.ToLower();
                switch (etiqueta)
                {


                    case "lista":
                        LinkedList<Atributo> lista = new LinkedList<Atributo>();
                        object res;
                        if (raiz.ChildNodes.Count() == 3)
                        {
                            lista = (LinkedList<Atributo>)analizar(raiz.ChildNodes.ElementAt(0), mensajes);
                            l = raiz.ChildNodes.ElementAt(1).Token.Location.Line;
                            c = raiz.ChildNodes.ElementAt(1).Token.Location.Column;
                            res = analizar(raiz.ChildNodes.ElementAt(2).ChildNodes.ElementAt(1), mensajes);
                        }
                        else
                        {
                            //-------------------------------- IMPORTAR ------------------------------------------
                            if (raiz.ChildNodes.ElementAt(0).ChildNodes.Count() == 1)
                            {
                                ParseTreeNode hijo = raiz.ChildNodes.ElementAt(0).ChildNodes.ElementAt(0);
                                string direccion = hijo.Token.Text.TrimStart('$').TrimStart('{').TrimEnd('}').TrimEnd('}').TrimEnd('$').TrimEnd('}');
                                object nuevoNodo = analizarImport(direccion, mensajes);
                                if (nuevoNodo != null) return analizar((ParseTreeNode)nuevoNodo, mensajes);
                                else return null;
                            }
                            l = raiz.ChildNodes.ElementAt(0).ChildNodes.ElementAt(0).Token.Location.Line;
                            c = raiz.ChildNodes.ElementAt(0).ChildNodes.ElementAt(0).Token.Location.Column;
                            res = analizar(raiz.ChildNodes.ElementAt(0).ChildNodes.ElementAt(1), mensajes);
                        }

                        if (res != null)
                        {
                            Atributo atributo = buscarAtributo((LinkedList<Atributo>)res, "name");
                            if (atributo == null)
                            {
                                mensajes.AddLast("Los Parametros necesitan un nombre, Linea: " + l + " Columna: " + c);
                                return lista;
                            }
                            string nombreP = atributo.valor.ToString();

                            atributo = buscarAtributo((LinkedList<Atributo>)res, "type");
                            if (atributo == null)
                            {
                                mensajes.AddLast("Los Parametros necesitan un typr, Linea: " + l + " Columna: " + c);
                                return lista;
                            }
                            string tipoP = atributo.valor.ToString();

                            atributo = buscarAtributo((LinkedList<Atributo>)res, "as");
                            if (atributo == null)
                            {
                                mensajes.AddLast("Los Parametros necesitan un AS, Linea: " + l + " Columna: " + c);
                                return lista;
                            }

                            string asP = atributo.valor.ToString();



                            lista.AddLast(new Atributo(nombreP, asP, tipoP));
                        }

                        return lista;

                    case "inobjetos":
                        LinkedList<Atributo> lista2 = new LinkedList<Atributo>();
                        object res2;
                        if (raiz.ChildNodes.Count() == 5)
                        {
                            lista2 = (LinkedList<Atributo>)analizar(raiz.ChildNodes.ElementAt(0), mensajes);
                            l = raiz.ChildNodes.ElementAt(1).Token.Location.Line;
                            c = raiz.ChildNodes.ElementAt(1).Token.Location.Column;
                            res2 = analizar(raiz.ChildNodes.ElementAt(3), mensajes);
                        }
                        else
                        {
                            l = raiz.ChildNodes.ElementAt(0).Token.Location.Line;
                            c = raiz.ChildNodes.ElementAt(0).Token.Location.Column;
                            res2 = analizar(raiz.ChildNodes.ElementAt(1), mensajes);
                        }

                        if (res2 != null)
                        {
                            Atributo atributo = buscarAtributo((LinkedList<Atributo>)res2, "name");
                            if (atributo == null)
                            {
                                mensajes.AddLast("Los Parametros necesitan un nombre, Linea: " + l + " Columna: " + c);
                                return lista2;
                            }
                            string nombreP = atributo.valor.ToString();

                            atributo = buscarAtributo((LinkedList<Atributo>)res2, "type");
                            if (atributo == null)
                            {
                                mensajes.AddLast("Los Parametros necesitan un typr, Linea: " + l + " Columna: " + c);
                                return lista2;
                            }
                            string tipoP = atributo.valor.ToString();

                            atributo = buscarAtributo((LinkedList<Atributo>)res2, "as");
                            if (atributo == null)
                            {
                                mensajes.AddLast("Los Parametros necesitan un AS, Linea: " + l + " Columna: " + c);
                                return lista2;
                            }

                            string asP = atributo.valor.ToString();



                            lista2.AddLast(new Atributo(nombreP, asP, tipoP));
                        }

                        return lista2;



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

                        if (nombre.Equals("name"))
                        {
                            string valor = raiz.ChildNodes.ElementAt(2).ChildNodes.ElementAt(0).Token.Text.ToLower().TrimEnd('\"').TrimStart('\"');
                            Atributo a = new Atributo(nombre, valor, "name");
                            return a;
                        }
                        else if (nombre.Equals("type"))
                        {
                            string valor = raiz.ChildNodes.ElementAt(2).ChildNodes.ElementAt(0).Token.Text.ToLower().TrimEnd('\"').TrimStart('\"');
                            Atributo a = new Atributo(nombre, valor, "type");
                            return a;
                        }
                        else if (nombre.Equals("as"))
                        {
                            string valor = raiz.ChildNodes.ElementAt(2).ChildNodes.ElementAt(0).Token.Text.ToLower().TrimEnd('\"').TrimStart('\"');
                            Atributo a = new Atributo(nombre, valor, "as");
                            return a;
                        }
                        else mensajes.AddLast("No re reconoce el atributo para un parametro: " + nombre + " Linea: " + l + " Columna: " + c);

                        return null;


                }
            }
            return null;
        }


        public Atributo buscarAtributo(LinkedList<Atributo> lk, string atributo)
        {
            foreach (Atributo at in lk)
            {
                if (at.nombre.Equals(atributo)) return at;
            }
            return null;
        }




    }
}
