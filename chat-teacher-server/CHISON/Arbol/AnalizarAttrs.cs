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
    public class AnalizarAttrs : Importar
    {

        public object analizar(ParseTreeNode raiz, LinkedList<string> mensajes)
        {
            if (raiz != null)
            {
                string etiqueta = raiz.Term.Name.ToLower();
                int l = 0;
                int c = 0;
                switch (etiqueta)
                {


                    case "tipo":
                        if (raiz.ChildNodes.Count() == 2) return new LinkedList<Attrs>();
                        else if (raiz.ChildNodes.Count() == 3) return analizar(raiz.ChildNodes.ElementAt(1), mensajes);
                        mensajes.AddLast("No se reconoce el tipo de objeto para Attrs");
                        break;





                    case "lista":
                        LinkedList<Attrs> lista = new LinkedList<Attrs>();
                        ParseTreeNode hijo;
                        if (raiz.ChildNodes.Count() == 3)
                        {
                            lista = (LinkedList<Attrs>)analizar(raiz.ChildNodes.ElementAt(0), mensajes);

                            hijo = raiz.ChildNodes.ElementAt(2).ChildNodes.ElementAt(1);
                            l = raiz.ChildNodes.ElementAt(1).Token.Location.Line;
                            c = raiz.ChildNodes.ElementAt(1).Token.Location.Column;
                        }
                        else
                        {
                            //-------------------------------- IMPORTAR ------------------------------------------
                            if (raiz.ChildNodes.ElementAt(0).ChildNodes.Count() == 1)
                            {
                                hijo = raiz.ChildNodes.ElementAt(0).ChildNodes.ElementAt(0);
                                string direccion = hijo.Token.Text.TrimStart('$').TrimStart('{').TrimEnd('}').TrimEnd('}').TrimEnd('$').TrimEnd('}');
                                object nuevoNodo = analizarImport(direccion, mensajes);
                                if (nuevoNodo != null) return analizar((ParseTreeNode)nuevoNodo, mensajes);
                                else return null;
                            }
                            l = raiz.ChildNodes.ElementAt(0).ChildNodes.ElementAt(0).Token.Location.Line;
                            c = raiz.ChildNodes.ElementAt(0).ChildNodes.ElementAt(0).Token.Location.Column;
                            hijo = raiz.ChildNodes.ElementAt(0).ChildNodes.ElementAt(1);
                        }
                        LinkedList<Atributo> resultado = (LinkedList<Atributo>)analizar(hijo, mensajes);
                        
                        if (resultado != null)
                        {
                            Atributo temp = buscarAtributo(resultado, "name");
                            if (temp == null)
                            {
                                mensajes.AddLast("Los attr necesitan un atributo name, Linea: " + l + " Columna: " + c);
                                return lista;
                            }
                            string name = temp.valor.ToString().ToLower().TrimEnd().TrimStart();
                            temp = buscarAtributo(resultado, "type");
                            if (temp == null)
                            {
                                mensajes.AddLast("Los attr necesitan un atributo type, Linea: " + l + " Columna: " + c);
                                return lista;
                            }
                            string type = temp.valor.ToString().ToLower().TrimEnd().TrimStart();
                            lista.AddLast(new Attrs(name, type));
                        }
                        return lista;


                    case "inobjetos":
                        LinkedList<Attrs> lista2 = new LinkedList<Attrs>();
                        ParseTreeNode hijo2;
                        if (raiz.ChildNodes.Count() == 5)
                        {
                            lista2 = (LinkedList<Attrs>)analizar(raiz.ChildNodes.ElementAt(0), mensajes);

                            hijo2 = raiz.ChildNodes.ElementAt(3);
                            l = raiz.ChildNodes.ElementAt(1).Token.Location.Line;
                            c = raiz.ChildNodes.ElementAt(1).Token.Location.Column;
                        }
                        else
                        {
                            l = raiz.ChildNodes.ElementAt(0).Token.Location.Line;
                            c = raiz.ChildNodes.ElementAt(0).Token.Location.Column;
                            hijo2 = raiz.ChildNodes.ElementAt(1);
                        }
                        LinkedList<Atributo> resultado2 = (LinkedList<Atributo>)analizar(hijo2, mensajes);

                        if (resultado2 != null)
                        {
                            Atributo temp = buscarAtributo(resultado2, "name");
                            if (temp == null)
                            {
                                mensajes.AddLast("Los attr necesitan un atributo name, Linea: " + l + " Columna: " + c);
                                return lista2;
                            }
                            string name = temp.valor.ToString().ToLower().TrimEnd().TrimStart();
                            temp = buscarAtributo(resultado2, "type");
                            if (temp == null)
                            {
                                mensajes.AddLast("Los attr necesitan un atributo type, Linea: " + l + " Columna: " + c);
                                return lista2;
                            }
                            string type = temp.valor.ToString().ToLower().TrimEnd().TrimStart();
                            lista2.AddLast(new Attrs(name, type));
                        }
                        return lista2;



                    case "objetos":
                        LinkedList<Atributo> atributos = new LinkedList<Atributo>();
                        object resultadoA;
                        if (raiz.ChildNodes.Count() == 3)
                        {
                            atributos = (LinkedList<Atributo>)analizar(raiz.ChildNodes.ElementAt(0), mensajes);
                            resultadoA = analizar(raiz.ChildNodes.ElementAt(2), mensajes);
                        }
                        else resultadoA = analizar(raiz.ChildNodes.ElementAt(0), mensajes);
                        if (resultadoA != null) atributos.AddLast((Atributo)resultadoA);

                        return atributos;



                    case "objeto":
                        string key = raiz.ChildNodes.ElementAt(0).Token.Text.ToLower().TrimStart('\"').TrimEnd('\"').TrimEnd().TrimStart();
                        l = raiz.ChildNodes.ElementAt(0).Token.Location.Line;
                        c = raiz.ChildNodes.ElementAt(0).Token.Location.Column;
                        if (key.Equals("name"))
                        {
                            string valor = raiz.ChildNodes.ElementAt(2).ChildNodes.ElementAt(0).Token.Text;
                            valor = valor.ToLower().TrimStart('\"').TrimEnd('\"').TrimEnd().TrimStart();
                            return new Atributo("name", valor, "");
                        }
                        else if (key.Equals("type"))
                        {
                            string valor = raiz.ChildNodes.ElementAt(2).ChildNodes.ElementAt(0).Token.Text;
                            valor = valor.ToLower().TrimStart('\"').TrimEnd('\"').TrimEnd().TrimStart();
                            return new Atributo("type", valor, "");

                        }
                        mensajes.AddLast("No se reconoce este atributo: " + key + " Linea :" + l + "Columna: " + c);
                        break;
                }
            }
            return null;
        }


        /*----------------------------------------------------------------------------------------------------------------------------------------------------
 * --------------------------------------------------- METODOS VARIOS ---------------------------------------------------------------------------------
 ------------------------------------------------------------------------------------------------------------------------------------------------------*/



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
