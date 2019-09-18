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





                    case "lista":
                        Objeto objeto = new Objeto();
                        ParseTreeNode hijo;
                        if (raiz.ChildNodes.Count() == 3)
                        {
                            objeto = (Objeto)analizar(raiz.ChildNodes.ElementAt(0), mensajes);
                            hijo = raiz.ChildNodes.ElementAt(2).ChildNodes.ElementAt(1);
                        }
                        else hijo = raiz.ChildNodes.ElementAt(0).ChildNodes.ElementAt(1);
                        LinkedList<Atributo> resultado = (LinkedList<Atributo>) analizar(hijo, mensajes);
                        object tipo = tipoCQLTYPE(resultado, mensajes);
                        if(tipo != null)
                        {
                            if (tipo.GetType() == typeof(Tabla)) objeto.tablas.AddLast((Tabla)tipo);
                            else if (tipo.GetType() == typeof(User_Types)) objeto.user_types.AddLast((User_Types)tipo);
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
                        else if (key.Equals("attrs"))
                        {
                            AnalizarAttrs analizar = new AnalizarAttrs();
                            object resT = analizar.analizar(raiz.ChildNodes.ElementAt(2), mensajes);
                            if (resT != null) return new Atributo("attrs", (LinkedList<Attrs>)resT, "");
                            return new Atributo("attrs", new LinkedList<Attrs>(), "");
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

            // ----------------------------------------------- USER TYPES -----------------------------
            else if (cql_type.Equals("object"))
            {
                resA = buscarAtributo(atributos, "name");
                if (resA == null)
                {
                    mensajes.AddLast("Se necesita un Nombre para el User type Linea: " + l + " Columna: " + c);
                    return null;
                }
                string name = resA.valor.ToString();

                resA = buscarAtributo(atributos, "attrs");
                if (resA == null)
                {
                    mensajes.AddLast("Se necesita un ATTRS para el User type Linea: " + l + " Columna: " + c);
                    return null;
                }
                LinkedList<Attrs> attrs = (LinkedList<Attrs>)resA.valor;

                return new User_Types(name, attrs);
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

       


    }
}
