using cql_teacher_server.CHISON;
using cql_teacher_server.CHISON.Componentes;
using cql_teacher_server.CQL.Arbol;
using cql_teacher_server.Herramientas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes
{
    public class Select : InstruccionCQL
    {
        string id { set; get; }
        string all { set; get; }
        LinkedList<Expresion> campos { set; get; }
        int l { set; get; }
        int c { set; get; }

        /*
         * CONSTRUCTOR DE LA CLASE CON UNA LISTA DE CAMPOS
         * @param id: nombre de la tabla
         * @param campos: lista de campos que buscaremos
         * @param l: linea del id
         * @param c: columna del id
         */
        public Select(string id, LinkedList<Expresion> campos, int l, int c)
        {
            this.id = id;
            this.campos = campos;
            this.l = l;
            this.c = c;
        }

        /*
         * CONSTRUCTOR DE LA CLASE CON UNA LISTA DE CAMPOS
         * @param id: nombre de la tabla
         * @param all: todos los campos de la tabla
         * @param l: linea del id
         * @param c: columna del id
         */
        public Select(string id, string all, int l, int c)
        {
            this.id = id;
            this.all = all;
            this.l = l;
            this.c = c;
            this.campos = null;
        }

        /*
             * Constructor de la clase padre
             * @ts tabla de simbolos padre
             * @user usuario que esta ejecutando las acciones
             * @baseD string por referencia de que base de datos estamos trabajando
             * @mensajes el output de la ejecucion
             */
        public object ejecutar(TablaDeSimbolos ts, string user, ref string baseD, LinkedList<string> mensajes, TablaDeSimbolos tsT)
        {
            Mensaje mensa = new Mensaje();
            BaseDeDatos db = TablaBaseDeDatos.getBase(baseD);
            Usuario us = TablaBaseDeDatos.getUsuario(user);
            if (db != null)
            {
                if (user.Equals("admin"))
                {

                }
                else
                {
                    if (us != null)
                    {
                        Boolean permiso = TablaBaseDeDatos.getPermiso(us, baseD);
                        if (permiso)
                        {
                            Boolean enUso = TablaBaseDeDatos.getEnUso(baseD, user);
                            if (!enUso)
                            {
                                Tabla tabla = TablaBaseDeDatos.getTabla(db, id);
                                if (tabla != null)
                                {
                                    LinkedList<Columna> cabecera = new LinkedList<Columna>();
                                    if (campos == null) cabecera = new LinkedList<Columna>(cabecera.Union(tabla.columnas));
                                    else cabecera = getColumnas(tabla, ts, user, ref baseD, mensajes);
                                    if (cabecera != null)
                                    {
                                        LinkedList<Data> datos = new LinkedList<Data>();
                                        if (campos == null) datos = new LinkedList<Data>(datos.Union(tabla.datos));
                                        else datos = getData(tabla,ts,user,ref baseD,mensajes,cabecera);
                                        if (datos != null)
                                        {
                                            TablaSelect  tablaSelect = new TablaSelect(cabecera, datos);
                                            mensajes.AddLast(mensa.consulta(tablaSelect));
                                            return tablaSelect;
                                        }
                                    }
                                }
                                else mensajes.AddLast(mensa.error("La tabla: " + id + " no existe en la DB: " + baseD, l, c, "Semantico"));
                            }
                            else mensajes.AddLast(mensa.error("La DB: " + baseD + " ya esta siendo utilizada por alguien mas", l, c, "Semantico"));
                        }
                        else mensajes.AddLast(mensa.error("El usuario " + user + " no tiene permisos sobre esta base de datos", l, c, "Semantico"));
                    }
                    else mensajes.AddLast(mensa.error("El usuario " + user + " no existe", l, c, "Semantico"));

                }

            }
            else mensajes.AddLast(mensa.error("La base de datos ha eliminar: " + id + " no existe", l, c, "Semantico"));
            return null;
        }


        /*
         * METODO DEVOLVERA  LAS ESPECIFICAS COLUMNAS
         * @param lista: Todas las columnas de la tabla
         * @param ts: lista de simbolos del padre
         * @param user : usuario que ejecuta las acciones
         * @param baseD : nombre de la base de datos se pasa por referencia
         * @param mensajes: output de salida
         */

        private LinkedList<Data> getData(Tabla t, TablaDeSimbolos ts, string user, ref string baseD, LinkedList<string> mensajes, LinkedList<Columna> cabeceras)
        {
            Mensaje mensa = new Mensaje();
            LinkedList<Data> listaR = new LinkedList<Data>();
            

            foreach(Data data in t.datos)
            {
                TablaDeSimbolos tsT = new TablaDeSimbolos();
                guardarTemp(data.valores, tsT);
                LinkedList<Atributo> valores = new LinkedList<Atributo>();
                int i = 0;
                foreach(Expresion e in campos)
                {
                    object res = (e == null) ? null : e.ejecutar(ts, user, ref baseD, mensajes, tsT);
                    if (res != null) valores.AddLast(new Atributo(cabeceras.ElementAt(i).name, res, cabeceras.ElementAt(i).tipo));
                    else return null;
                }
                listaR.AddLast(new Data(valores));
            }
            return listaR;
        }


        /*
        * Metodo que se encargara de guardar los valores como variables temporales
        * @atributos es la data a guardar
        * @tsT es la tabla temporal
        */
        private void guardarTemp(LinkedList<Atributo> atributos, TablaDeSimbolos tsT)
        {
            foreach (Atributo atributo in atributos)
            {
                tsT.AddLast(new Simbolo(atributo.tipo, atributo.nombre));
                tsT.setValor(atributo.nombre, atributo.valor);
            }
        }

        /*
        * Metodo que se encargara de guardar los valores como variables temporales
        * @atributos es la data a guardar
        * @tsT es la tabla temporal
        */
        private void setColumnas(LinkedList<Columna> atributos, TablaDeSimbolos tsT)
        {
            foreach (Columna atributo in atributos)
            {
                tsT.AddLast(new Simbolo(atributo.tipo, atributo.name));
                tsT.setValor(atributo.name, atributo.name);
            }
        }

        /*
         * METODO QUE DEVOLVERA LOS DATOS DE LAS COLUMNAS ESPECIFICAS
         * @param columnas: las columnas ya seleccionadas
         * @param datas: toda la informacion en la base de datos
         */

        private LinkedList<Columna> getColumnas(Tabla t, TablaDeSimbolos ts, string user, ref string baseD, LinkedList<string> mensajes)
        {
            Mensaje mensa = new Mensaje();
            LinkedList<Columna> cabeceras = new LinkedList<Columna>();
            foreach (Expresion e in campos)
            {
                TablaDeSimbolos tsT = new TablaDeSimbolos();
                setColumnas(t.columnas, tsT);
                object res = (e == null) ? null : e.ejecutar(ts, user, ref baseD, mensajes, tsT);
                if (res != null)
                {
                    Columna columna = searchColumna(t.columnas, res.ToString().ToLower().TrimEnd().TrimStart());
                    string tipo = "";

                    if (res.GetType() == typeof(string)) tipo = "string";
                    else if (res.GetType() == typeof(int)) tipo = "int";
                    else if (res.GetType() == typeof(Double)) tipo = "double";
                    else if (res.GetType() == typeof(Boolean)) tipo = "boolean";
                    else if (res.GetType() == typeof(DateTime)) tipo = "date";
                    else if (res.GetType() == typeof(TimeSpan)) tipo = "time";
                    else if (res.GetType() == typeof(InstanciaUserType)) tipo = ((InstanciaUserType)res).tipo;

                    if (columna == null) columna = new Columna("descripcion", tipo, false);
                    cabeceras.AddLast(columna);
                }
                else
                {
                    mensajes.AddLast(mensa.error("No puede haber una columna null", l, c, "Semantico"));
                    return null;
                }
            }
            return cabeceras;
        }

        /*
         * Busca el nombre de la columna en las columnas de la tabla
         * @param {columnas} columnas de la tabla
         * @param {nombre} nombre a buscar
         * @return {Columna | null}
         */
        private Columna searchColumna (LinkedList<Columna> columnas, string nombre)
        {
            foreach(Columna columna in columnas)
            {
                if (nombre.Equals(columna.name)) return columna;
            }
            return null;
        }


    }


}
