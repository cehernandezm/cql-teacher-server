using cql_teacher_server.CHISON.Componentes;
using cql_teacher_server.CHISON.Gramatica;
using cql_teacher_server.CQL.Componentes;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CHISON.Arbol
{
    public class AnalizarBase : Importar
    {
        

        public object analizar(ParseTreeNode raiz, LinkedList<string> mensajes)
        {
            if(raiz != null)
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
                            analizar(raiz.ChildNodes.ElementAt(0),mensajes);
                            hijo = raiz.ChildNodes.ElementAt(3);
                        }
                        else hijo = raiz.ChildNodes.ElementAt(1);
                        object res = analizar(hijo,mensajes);
                        if(res != null)
                        {
                            l = raiz.ChildNodes.ElementAt(2).Token.Location.Line;
                            c = raiz.ChildNodes.ElementAt(2).Token.Location.Column;
                            if (res.GetType() == typeof(LinkedList<Atributo>))
                            {
                                LinkedList<Atributo> temp = (LinkedList<Atributo>)res;
                                Atributo atri = getValor("name", temp);
                                string nombreDB = "";
                                Objeto info;
                                if(atri == null)
                                {
                                    mensajes.AddLast("Se necesita un nombre para la base de datos Linea: " + l + " Columna: " + c);
                                    return null;
                                }
                                nombreDB = atri.valor.ToString();
                                atri = getValor("data", temp);
                                if(atri == null)
                                {
                                    mensajes.AddLast("Se necesita data para la base de datos Linea: " + l + " Columna: " + c);
                                    return null;
                                }
                                info = (Objeto)atri.valor;
                                info = fixData(info);
                                TablaBaseDeDatos.global.AddLast(new BaseDeDatos(nombreDB, info));
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
                            resultado = analizar(raiz.ChildNodes.ElementAt(2),mensajes);
                        }
                        else resultado = analizar(raiz.ChildNodes.ElementAt(0),mensajes);
                        if(resultado != null) listaAtributos.AddLast((Atributo)resultado);
                        
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
                        else if (key.Equals("data"))
                        {
                            AnalizarObject analisis = new AnalizarObject();
                            object resData = analisis.analizar(raiz.ChildNodes.ElementAt(2),mensajes);
                            if (resData != null) return new Atributo("data", resData, "");
                            mensajes.AddLast("LA DATA DE BASES DE DATOS necesita tablas,procedures y USER TYPES Linea : " + l + " Columna:  " + c);
                        }
                        else mensajes.AddLast("No se reconoce el atributo para una base de datos: " + key + " Linea: " + l + " Columna: " + c);
                        break;


                }
            }
            return null;
        }




      
        public Atributo getValor(string nombre,LinkedList<Atributo> lista)
        {
            foreach(Atributo a in lista)
            {
                if (a.nombre.Equals(nombre)) return a;
            }
            return null;
        }


        public Objeto fixData(Objeto datos)
        {
            Objeto nuevo = datos;
            foreach(Tabla tabla in nuevo.tablas)
            {
                LinkedList<Data> temp = formatearData(tabla.datos, tabla.columnas, nuevo.user_types);
                tabla.datos = temp;
            }
            return datos;
        }


        public Columna buscarColumna(LinkedList<Columna> lt, string nombre)
        {
            foreach (Columna ta in lt)
            {
                if (ta.name.Equals(nombre)) return ta;
            }
            return null;
        }

        private LinkedList<Data> formatearData(LinkedList<Data> lista, LinkedList<Columna> columnas, LinkedList<User_Types> user_Types)
        {
            LinkedList<Data> nueva = new LinkedList<Data>();

            foreach (Data data in lista)
            {
                Data d = new Data(new LinkedList<Atributo>());
                foreach (Atributo a in data.valores)
                {
                    Columna columna = buscarColumna(columnas, a.nombre);
                    if (columna != null)
                    {
                        string tipo = columna.tipo.ToLower();
                        if (a.valor != null)
                        {
                            if (tipo.Equals("string") && a.valor.GetType() == typeof(string)) d.valores.AddLast(new Atributo(columna.name, (string)a.valor, tipo));
                            else if (tipo.Equals("int") && a.valor.GetType() == typeof(int)) d.valores.AddLast(new Atributo(columna.name, (int)a.valor, tipo));
                            else if (tipo.Equals("double") && a.valor.GetType() == typeof(Double)) d.valores.AddLast(new Atributo(columna.name, (Double)a.valor, tipo));
                            else if (tipo.Equals("date") && a.valor.GetType() == typeof(DateTime)) d.valores.AddLast(new Atributo(columna.name, (DateTime)a.valor, tipo));
                            else if (tipo.Equals("time") && a.valor.GetType() == typeof(TimeSpan)) d.valores.AddLast(new Atributo(columna.name, (TimeSpan)a.valor, tipo));
                            else if (tipo.Equals("counter") && a.valor.GetType() == typeof(int)) d.valores.AddLast(new Atributo(columna.name, (int)a.valor, tipo));
                            else if (tipo.Contains("list") && a.valor.GetType() == typeof(Set))
                            {
                                string id = tipo.TrimStart('l').TrimStart('i').TrimStart('s').TrimStart('t').TrimStart('>').TrimEnd('>');

                                List temp = new List(id, ((Set)a.valor).datos);
                                d.valores.AddLast(new Atributo(columna.name, temp, columna.tipo));
                            }
                            else if (tipo.Contains("set") && a.valor.GetType() == typeof(Set))
                            {
                                string id = tipo.TrimStart('s').TrimStart('e').TrimStart('t').TrimStart('>').TrimEnd('>');

                                Set temp = (Set)a.valor;
                                temp.order();
                                d.valores.AddLast(new Atributo(columna.name, temp, columna.tipo));
                            }
                            else if (tipo.Contains("map") && a.valor.GetType() == typeof(LinkedList<Atributo>))
                            {
                                string id = tipo.TrimStart('m').TrimStart('a').TrimStart('p').TrimStart('>').TrimEnd('>');
                                string tipoKey = id.Split(',')[0];
                                LinkedList<Atributo> listatemp =(LinkedList<Atributo>)a.valor;
                                if (listatemp.Count() > 0)
                                {
                                    LinkedList<KeyValue> listaRetorno = new LinkedList<KeyValue>();

                                    foreach (Atributo at in listatemp)
                                    {
                                        KeyValue keyValue = new KeyValue(keyMap(tipoKey,at.nombre), at.valor);
                                        listaRetorno.AddLast(keyValue);
                                    }

                                    d.valores.AddLast(new Atributo(columna.name, new Map(id, listaRetorno), tipo));
                                }
                                else d.valores.AddLast(new Atributo(columna.name, new Map(id, new LinkedList<KeyValue>()), tipo));
                            
                            }
                            else if (a.valor.GetType() == typeof(LinkedList<Atributo>) && tipo.Equals(a.nombre))
                            {
                                LinkedList<Atributo> at = (LinkedList<Atributo>)a.valor; ;
                                if (at.Count() > 0)
                                {
                                    User_Types user = buscarUser(a.nombre, user_Types);
                                    if (user != null) d.valores.AddLast(new Atributo(columna.name, setearUserType(user.type, at, tipo, user_Types), tipo));
                                    else
                                    {
                                        System.Diagnostics.Debug.WriteLine("No existe usertype: " + tipo);
                                        d.valores.AddLast(new Atributo(columna.name, new InstanciaUserType(tipo, null), tipo));
                                    }
                                }

                            }
                        }
                        else
                        {
                            if (tipo.Equals("string") || tipo.Equals("date") || tipo.Equals("time")) d.valores.AddLast(new Atributo(columna.name, null, tipo));
                        }
                    }
                }
                if (d.valores.Count() > 0) nueva.AddLast(d);
            }


            return nueva;
        }


        private InstanciaUserType setearUserType(LinkedList<Attrs> lista, LinkedList<Atributo> valores, string tipoV, LinkedList<User_Types> user_Types)
        {
            InstanciaUserType instancia = new InstanciaUserType(tipoV, new LinkedList<Atributo>());
            foreach(Atributo a in valores)
            {
                Attrs attrs = buscarAttr(lista, a.nombre);
                if(attrs != null)
                {
                    string tipo = attrs.type;
                    if (a.valor != null)
                    {
                        if (tipo.Equals("string") && a.valor.GetType() == typeof(string)) instancia.lista.AddLast(new Atributo(attrs.name, (string)a.valor, tipo));
                        else if (tipo.Equals("int") && a.valor.GetType() == typeof(int)) instancia.lista.AddLast(new Atributo(attrs.name, (int)a.valor, tipo));
                        else if (tipo.Equals("double") && a.valor.GetType() == typeof(Double)) instancia.lista.AddLast(new Atributo(attrs.name, (Double)a.valor, tipo));
                        else if (tipo.Equals("date") && a.valor.GetType() == typeof(DateTime)) instancia.lista.AddLast(new Atributo(attrs.name, (DateTime)a.valor, tipo));
                        else if (tipo.Equals("time") && a.valor.GetType() == typeof(TimeSpan)) instancia.lista.AddLast(new Atributo(attrs.name, (TimeSpan)a.valor, tipo));
                        else if (tipo.Equals("counter") && a.valor.GetType() == typeof(int)) instancia.lista.AddLast(new Atributo(attrs.name, (int)a.valor, tipo));
                        else if (tipo.Contains("list") && a.valor.GetType() == typeof(Set))
                        {
                            string id = tipo.TrimStart('l').TrimStart('i').TrimStart('s').TrimStart('t').TrimStart('>').TrimEnd('>');

                            List temp = new List(id, ((Set)a.valor).datos);
                            instancia.lista.AddLast(new Atributo(attrs.name, temp, tipo));
                        }
                        else if (tipo.Contains("set") && a.valor.GetType() == typeof(Set))
                        {
                            string id = tipo.TrimStart('s').TrimStart('e').TrimStart('t').TrimStart('>').TrimEnd('>');

                            Set temp = (Set)a.valor;
                            temp.order();
                            instancia.lista.AddLast(new Atributo(attrs.name, temp,tipo));
                        }
                        else if (tipo.Contains("map") && a.valor.GetType() == typeof(LinkedList<Atributo>))
                        {
                            string id = tipo.TrimStart('m').TrimStart('a').TrimStart('p').TrimStart('>').TrimEnd('>');
                            string tipoKey = id.Split(',')[0];
                            LinkedList<Atributo> listatemp = (LinkedList<Atributo>)a.valor ;
                            if (listatemp.Count() > 0)
                            {
                                LinkedList<KeyValue> listaRetorno = new LinkedList<KeyValue>();

                                foreach (Atributo at in listatemp)
                                {
                                    KeyValue keyValue = new KeyValue(keyMap(tipoKey,at.nombre), at.valor);
                                    listaRetorno.AddLast(keyValue);
                                }

                                instancia.lista.AddLast(new Atributo(attrs.name, new Map(id, listaRetorno), tipo));
                            }
                            else instancia.lista.AddLast(new Atributo(attrs.name, new Map(id, new LinkedList<KeyValue>()), tipo));

                        }
                        else if (a.valor.GetType() == typeof(LinkedList<Atributo>) && tipo.Equals(a.nombre))
                        {

                            LinkedList<Atributo> at = (LinkedList<Atributo>)a.valor;
                            if (at.Count() > 0)
                            {
                                User_Types user = buscarUser(a.nombre, user_Types);
                                if (user != null) instancia.lista.AddLast(new Atributo(attrs.name, new InstanciaUserType(tipo, null), tipo));
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine("No existe usertype: " + tipo);
                                    instancia.lista.AddLast(new Atributo(attrs.name, new InstanciaUserType(tipo, null), tipo));
                                }
                            }

                        }
                    }
                    else
                    {
                        if (tipo.Equals("string") || tipo.Equals("date") || tipo.Equals("time")) instancia.lista.AddLast(new Atributo(attrs.name, null, tipo));
                    }
                }
            }
            return instancia;
        }

        private Attrs buscarAttr(LinkedList<Attrs> lista, string nombre)
        {
            foreach(Attrs a in lista)
            {
                if (a.name.Equals(nombre)) return a;
            }
            return null;
        }

        private object keyMap(string tipo, string valor)
        {
            tipo = tipo.ToLower();
            if (tipo.Equals("int"))
            {
                int salida = 0;
                Int32.TryParse(valor, out salida);
                return salida;
            }
            else if (tipo.Equals("string")) return valor;
            else if (tipo.Equals("double"))
            {
                Double salida = 0;
                Double.TryParse(valor, out salida);
                return salida;
            }
            else if (tipo.Equals("boolean"))
            {
                Boolean salida = false;
                Boolean.TryParse(valor, out salida);
                return salida;
            }
            else if (tipo.Equals("date"))
            {
                DateTime salida = DateTime.Today;
                DateTime.TryParse(valor, out salida);
                return salida;
            }
            else if (tipo.Equals("time"))
            {
                TimeSpan salida = DateTime.Now.TimeOfDay;
                TimeSpan.TryParse(valor, out salida);
                return salida;
            }
            return valor;
        }

        private User_Types buscarUser(string nombre, LinkedList<User_Types> lista)
        {
            foreach(User_Types u in lista)
            {
                if (u.name.Equals(nombre)) return u;
            }
            return null;
        }

    }
}
