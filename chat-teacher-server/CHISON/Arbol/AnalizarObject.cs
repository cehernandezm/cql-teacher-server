using cql_teacher_server.CHISON.Componentes;
using cql_teacher_server.CQL.Componentes;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CHISON.Arbol
{
    public class AnalizarObject
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
                        if (raiz.ChildNodes.Count() == 2) return new Objeto();
                        else if(raiz.ChildNodes.Count() == 3) return analizar(raiz.ChildNodes.ElementAt(1),mensajes);
                        mensajes.AddLast("No se reconoce el tipo de objeto para data");
                        break;





                    case "inobjetos":
                        Objeto objeto = new Objeto();
                        ParseTreeNode hijo;
                        if (raiz.ChildNodes.Count() == 5)
                        {
                            objeto = (Objeto)analizar(raiz.ChildNodes.ElementAt(0), mensajes);
                            hijo = raiz.ChildNodes.ElementAt(3);
                        }
                        else hijo = raiz.ChildNodes.ElementAt(1);
                        LinkedList<Atributo> resultado = (LinkedList<Atributo>) analizar(hijo, mensajes);
                        object tipo = tipoCQLTYPE(resultado, mensajes);
                        if(tipo != null)
                        {
                            if (tipo.GetType() == typeof(Tabla)) objeto.tablas.AddLast((Tabla)tipo);
                        }
                        return objeto;




                    case "objetos":
                        LinkedList<Atributo> atributos = new LinkedList<Atributo>();
                        object resultadoA;
                        if (raiz.ChildNodes.Count() == 3)
                        {
                            atributos = (LinkedList<Atributo>)analizar(raiz.ChildNodes.ElementAt(0), mensajes);
                            resultadoA = analizar(raiz.ChildNodes.ElementAt(2), mensajes);
                        }
                        else resultadoA = analizar(raiz.ChildNodes.ElementAt(0), mensajes);
                        if(resultadoA != null) atributos.AddLast((Atributo)resultadoA);

                        return atributos;



                    case "objeto":
                        string key = raiz.ChildNodes.ElementAt(0).Token.Text.ToLower().TrimStart('\"').TrimEnd('\"').TrimEnd().TrimStart();
                        l = raiz.ChildNodes.ElementAt(0).Token.Location.Line;
                        c = raiz.ChildNodes.ElementAt(0).Token.Location.Column;
                        if (key.Equals("cql-type"))
                        {
                            string valor = raiz.ChildNodes.ElementAt(2).ChildNodes.ElementAt(0).Token.Text;
                            valor = valor.ToLower().TrimStart('\"').TrimEnd('\"').TrimEnd().TrimStart();
                            return new Atributo("cql-type", valor, "");
                        }
                        else if (key.Equals("name"))
                        {
                            string valor = raiz.ChildNodes.ElementAt(2).ChildNodes.ElementAt(0).Token.Text;
                            valor = valor.ToLower().TrimStart('\"').TrimEnd('\"').TrimEnd().TrimStart();
                            return new Atributo("name", valor, "");
                        }
                        else if (key.Equals("columns"))
                        {
                            AnalizarColumna analizar = new AnalizarColumna();
                            object resT = analizar.analizar(raiz.ChildNodes.ElementAt(2), mensajes);
                            if(resT != null) return new Atributo("columns", (LinkedList<Columna>)resT, "");
                            return new Atributo("columns", new LinkedList<Columna>(), "");

                        }
                        else if (key.Equals("data"))
                        {
                            AnalizarData analizar = new AnalizarData();
                            object resT = analizar.inicio(raiz.ChildNodes.ElementAt(2), mensajes);
                            if (resT != null) return new Atributo("data", (LinkedList<Data>)resT, "");
                            return new Atributo("data",new LinkedList<Data>(), "");

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



        private object tipoCQLTYPE(LinkedList<Atributo> atributos, LinkedList<string> mensajes)
        {
            int l = 0;
            int c = 0;
            Atributo resA = buscarAtributo(atributos, "cql-type");
            if (resA == null)
            {
                mensajes.AddLast("Se necesita un CQL-TYPE Linea: " + l + " Columna: " + c);
                return null;
            }
            string cql_type = resA.valor.ToString();
            //--------------------------------------------- TABLAS ----------------------------------------------------------
            if (cql_type.Equals("table"))
            {
                resA = buscarAtributo(atributos, "name");
                if (resA == null)
                {
                    mensajes.AddLast("Se necesita un Nombre para la tabla Linea: " + l + " Columna: " + c);
                    return null;
                }
                string name = resA.valor.ToString();

                resA = buscarAtributo(atributos, "columns");
                if (resA == null)
                {
                    mensajes.AddLast("Se necesitan Columnas para la tabla Linea: " + l + " Columna: " + c);
                    return null;
                }
                LinkedList<Columna> columnas = (LinkedList<Columna>)resA.valor;

                resA = buscarAtributo(atributos, "data");
                if (resA == null)
                {
                    mensajes.AddLast("Se necesitan Data para la tabla Linea: " + l + " Columna: " + c);
                    return null;
                }
                LinkedList<Data> data = (LinkedList<Data>)resA.valor;
                return new Tabla(name, columnas, data);
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

        public Columna buscarColumna(LinkedList<Columna> lt, string nombre)
        {
            foreach (Columna ta in lt)
            {
                if (ta.name.Equals(nombre)) return ta;
            }
            return null;
        }

        private LinkedList<Data> formatearData(LinkedList<Data> lista, LinkedList<Columna> columnas)
        {
            LinkedList<Data> nueva = new LinkedList<Data>();

            foreach(Data data in lista)
            {
                Data d = new Data(new LinkedList<Atributo>());
                foreach(Atributo a in data.valores)
                {
                    Columna columna = buscarColumna(columnas, a.nombre);
                    if(columna != null)
                    {
                        string tipo = columna.tipo.ToLower();
                        if(a.valor != null)
                        {
                            if(tipo.Equals("string") && a.valor.GetType() == typeof(string)) d.valores.AddLast(new Atributo(columna.name, (string)a.valor, tipo));
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
                            else if(tipo.Contains("map") && a.valor.GetType() == typeof(LinkedList<Data>))
                            {
                                string id = tipo.TrimStart('m').TrimStart('a').TrimStart('p').TrimStart('>').TrimEnd('>');
                                LinkedList<Data> temp = (LinkedList<Data>)a.valor;
                                if (temp.Count() > 0)
                                {
                                    LinkedList<Atributo> listatemp = temp.ElementAt(0).valores;
                                    if(listatemp.Count() > 0)
                                    {
                                        LinkedList<KeyValue> listaRetorno = new LinkedList<KeyValue>();

                                        foreach(Atributo at in listatemp)
                                        {
                                            KeyValue keyValue = new KeyValue(at.nombre, at.valor);
                                            listaRetorno.AddLast(keyValue);
                                        }

                                        d.valores.AddLast(new Atributo(columna.name, new Map(id, listaRetorno), tipo));
                                    }
                                    else d.valores.AddLast(new Atributo(columna.name, new Map(id, new LinkedList<KeyValue>()), tipo));
                                }
                                else d.valores.AddLast(new Atributo(columna.name, new Map(id, new LinkedList<KeyValue>()),tipo));
                            }
                            else if (a.valor.GetType() == typeof(LinkedList<Data>)){
                                LinkedList<Data> temp = (LinkedList<Data>)a.valor;
                                if(temp.Count() > 0)
                                {
                                    LinkedList<Atributo> at = temp.ElementAt(0).valores;
                                    if(at.Count() > 0)
                                    {
                                        
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


    }
}
