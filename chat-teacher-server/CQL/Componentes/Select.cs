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
                                    LinkedList<Columna> camposR;
                                    if (campos == null) camposR = tabla.columnas;
                                    else camposR = getSpecificCampos(tabla.columnas, ts, user, ref baseD, mensajes);
                                    if(camposR != null)
                                    {
                                        LinkedList<Data> newData = getData(camposR, tabla.datos);
                                        TablaSelect tablaSelect = new TablaSelect(camposR, newData);
                                        mensajes.AddLast(mensa.consulta(tablaSelect));
                                        return tablaSelect;
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

        private LinkedList<Columna> getSpecificCampos(LinkedList<Columna> lista, TablaDeSimbolos ts, string user, ref string baseD, LinkedList<string> mensajes)
        {
            Mensaje mensa = new Mensaje();
            LinkedList<Columna> listaR = new LinkedList<Columna>();
            TablaDeSimbolos tsT = new TablaDeSimbolos();
            guardarTemp(lista, tsT);

            foreach(Expresion e in campos)
            {
                object res = (e == null) ? null : e.ejecutar(ts, user, ref baseD, mensajes, tsT);
                if (res != null)
                {
                    if(res.GetType() == typeof(string))
                    {
                        Boolean flag = false;
                        string nombre = ((string)res).ToLower().TrimEnd().TrimStart();
                        foreach(Columna columna in lista)
                        {
                            if (nombre.Equals(columna.name))
                            {
                                listaR.AddLast(columna);
                                flag = true;
                            }
                        }

                        if (!flag)
                        {
                            mensajes.AddLast(mensa.error("No se encontro la  columnas: " + res, l, c, "Semantico"));
                            return null;
                        }
                    }
                    else
                    {
                        mensajes.AddLast(mensa.error("No se puede buscar una columna: " + res, l, c, "Semantico"));
                        return null;
                    }
                }
                else
                {
                    mensajes.AddLast(mensa.error("No se puede usar una columna null", l, c, "Semantico"));
                    return null;
                }
            }
            return listaR;
        }


        /*
        * Metodo que se encargara de guardar los valores como variables temporales
        * @atributos es la data a guardar
        * @tsT es la tabla temporal
        */
        private void guardarTemp(LinkedList<Columna> atributos, TablaDeSimbolos tsT)
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

        private LinkedList<Data> getData(LinkedList<Columna> columnas, LinkedList<Data> datas)
        {
            LinkedList<Data> newData = new LinkedList<Data>();
            foreach(Data data in datas)
            {
                LinkedList<Atributo> nuevosDatos = new LinkedList<Atributo>();
                foreach(Atributo atributo in data.valores)
                {
                    foreach(Columna columna in columnas)
                    {
                        if (columna.name.Equals(atributo.nombre)) nuevosDatos.AddLast(atributo);
                    }
                }
                newData.AddLast(new Data(nuevosDatos));
            }
            return newData;
        }


    }


}
