using cql_teacher_server.CHISON.Componentes;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CHISON.Arbol
{
    public class AnalizarPermisos
    {
        int l = 0;
        int c = 0;

        public object inicio(ParseTreeNode raiz, LinkedList<string> mensajes)
        {
            if (raiz.ChildNodes.Count() == 2) return new LinkedList<string>();
            else if (raiz.ChildNodes.Count() == 3)
            {
                object res = (analizar(raiz.ChildNodes.ElementAt(1), mensajes) == null) ? null : (LinkedList<string>)analizar(raiz.ChildNodes.ElementAt(1), mensajes);
                return res;
            }
            l = raiz.ChildNodes.ElementAt(0).Token.Location.Line;
            c = raiz.ChildNodes.ElementAt(0).Token.Location.Column;
            mensajes.AddLast("El valor de Permissions tiene que ser de tipo objeto, Fila: " + l + " Columna: " + c);
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
                        LinkedList<string> lista = new LinkedList<string>();
                        object res;
                        if (raiz.ChildNodes.Count() == 3)
                        {
                            lista = (LinkedList<string>)analizar(raiz.ChildNodes.ElementAt(0), mensajes);
                            l = raiz.ChildNodes.ElementAt(1).Token.Location.Line;
                            c = raiz.ChildNodes.ElementAt(1).Token.Location.Column;
                            res = analizar(raiz.ChildNodes.ElementAt(2).ChildNodes.ElementAt(1), mensajes);
                        }
                        else
                        {
                            l = raiz.ChildNodes.ElementAt(0).ChildNodes.ElementAt(0).Token.Location.Line;
                            c = raiz.ChildNodes.ElementAt(0).ChildNodes.ElementAt(0).Token.Location.Column;
                            res = analizar(raiz.ChildNodes.ElementAt(0).ChildNodes.ElementAt(1), mensajes);
                        }

                        if (res != null)
                        {
                            Atributo atributo = buscarAtributo((LinkedList<Atributo>)res, "name");
                            if(atributo == null)
                            {
                                mensajes.AddLast("Los permisos necesitan un nombre, Linea: " + l + " Columna: " + c);
                                return lista;
                            }
                            lista.AddLast(atributo.valor.ToString());
                        }

                        return lista;
                        



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
                            Atributo a = new Atributo(nombre, valor, "");
                            return a;
                        }
                        
                        return null;
                        break;



                   







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
