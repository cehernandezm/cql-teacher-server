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
    public class AnalizarUsuario
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

                    case "inobjetos":
                        ParseTreeNode hijo;
                        if (raiz.ChildNodes.Count() == 5)
                        {
                            analizar(raiz.ChildNodes.ElementAt(0), mensajes);
                            hijo = raiz.ChildNodes.ElementAt(3);
                        }
                        else hijo = raiz.ChildNodes.ElementAt(1);
                        object res = analizar(hijo, mensajes);
                        if (res != null)
                        {
                            l = raiz.ChildNodes.ElementAt(2).Token.Location.Line;
                            c = raiz.ChildNodes.ElementAt(2).Token.Location.Column;
                            if (res.GetType() == typeof(LinkedList<Atributo>))
                            {
                                LinkedList<Atributo> temp = (LinkedList<Atributo>)res;
                                Atributo atri = buscarAtributo(temp,"name");
                                string nombreDB = "";
                                if (atri == null)
                                {
                                    mensajes.AddLast("Se necesita un nombre para el usuario Linea: " + l + " Columna: " + c);
                                    return null;
                                }
                                nombreDB = atri.valor.ToString();

                                atri = buscarAtributo(temp, "password");
                                string password = "";
                                if (atri == null)
                                {
                                    mensajes.AddLast("Se necesita un password para el usuario Linea: " + l + " Columna: " + c);
                                    return null;
                                }
                                password = atri.valor.ToString();

                                atri = buscarAtributo(temp, "permiso");
                                if(atri == null)
                                {
                                    mensajes.AddLast("Se necesitan permisos para los usuarios Linea: " + l + " Columna: " + c);
                                    return null;
                                }
                                LinkedList<string> lista = (LinkedList<string>)atri.valor;

                                TablaBaseDeDatos.listaUsuario.AddLast(new Usuario(nombreDB, password, lista));
                                
                                return "";
                            }
                        }
                        break;



                    case "objetos":
                        LinkedList<Atributo> listaAtributos = new LinkedList<Atributo>();
                        object resultado;
                        if (raiz.ChildNodes.Count() == 3)
                        {
                            listaAtributos = (LinkedList<Atributo>)analizar(raiz.ChildNodes.ElementAt(0), mensajes);
                            resultado = analizar(raiz.ChildNodes.ElementAt(2), mensajes);
                        }
                        else resultado = analizar(raiz.ChildNodes.ElementAt(0), mensajes);
                        if (resultado != null) listaAtributos.AddLast((Atributo)resultado);

                        return listaAtributos;



                    case "objeto":
                        string key = raiz.ChildNodes.ElementAt(0).Token.Text.ToLower().TrimEnd('\"').TrimStart('\"').TrimEnd().TrimStart();
                        l = raiz.ChildNodes.ElementAt(0).Token.Location.Line;
                        c = raiz.ChildNodes.ElementAt(0).Token.Location.Column;
                        if (key.Equals("name"))
                        {
                            string name = raiz.ChildNodes.ElementAt(2).ChildNodes.ElementAt(0).Token.Text.ToLower();
                            name = name.TrimStart('\"').TrimEnd('\"').TrimEnd().TrimStart();
                            return new Atributo("name", name, "");
                        }
                        else if (key.Equals("password"))
                        {
                            string name = raiz.ChildNodes.ElementAt(2).ChildNodes.ElementAt(0).Token.Text.ToLower();
                            name = name.TrimStart('\"').TrimEnd('\"').TrimEnd().TrimStart();
                            return new Atributo("password", name, "");
                        }
                        else if (key.Equals("permissions"))
                        {
                            AnalizarPermisos analisis = new AnalizarPermisos();
                            object resData = analisis.inicio(raiz.ChildNodes.ElementAt(2), mensajes);
                            if (resData != null) return new Atributo("permiso", resData, "");
                            mensajes.AddLast("Permissions de un usuario necesita una lista de bases de datos Linea : " + l + " Columna:  " + c);
                        }
                        else mensajes.AddLast("No se reconoce el atributo para un usuario: " + key + " Linea: " + l + " Columna: " + c);
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
                                  + " Columna: " + arbol.ParserMessages.ElementAt(i).Location.Column.ToString() + " Archivo : " + direccion +"\n");
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
