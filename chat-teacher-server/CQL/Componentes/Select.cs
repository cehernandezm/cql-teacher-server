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
        LinkedList<Expresion> campos { set; get; }
        int l { set; get; }
        int c { set; get; }
        string operacion { set; get; }
        Expresion condicion { set; get;}
        Expresion condicion2 { set; get; }
        LinkedList<OrderBy> orderBy { set; get; }

       



        /*
         * a = where
         * b = order by
         * c = limit
         * Puede haber cualquier combinacion entre ellas
         */

        /*
         * CONSTRUCTOR DE LA CLASE CON UNA LISTA DE CAMPOS
         * @param id: nombre de la tabla
         * @param campos: lista de campos que buscaremos
         * @param l: linea del id
         * @param c: columna del id
         */
        public Select(string id, LinkedList<Expresion> campos, int l, int c, string operacion)
        {
            this.id = id;
            this.campos = campos;
            this.l = l;
            this.c = c;
            this.operacion = operacion;
        }

        /*
        * CONSTRUCTOR DE LA CLASE CON UNA LISTA DE CAMPOS (con Where o limit)
        * @param id: nombre de la tabla
        * @param campos: lista de campos que buscaremos
        * @param l: linea del id
        * @param c: columna del id
        * @param {condicion} condicion where
        * @param {condicion2} condicion limit
        */
        public Select(string id, LinkedList<Expresion> campos, int l, int c, string operacion, Expresion condicion,Expresion condicion2) : this(id, campos, l, c, operacion)
        {
            this.condicion = condicion;
            this.condicion2 = condicion2;
        }

        /*
        * CONSTRUCTOR DE LA CLASE CON UNA LISTA DE CAMPOS (ORDER BY)
        * @param id: nombre de la tabla
        * @param campos: lista de campos que buscaremos
        * @param l: linea del id
        * @param c: columna del id
        * @param {orderBy} lista con las columnas con las cuales se aplicaran el sort
        */


        public Select(string id, LinkedList<Expresion> campos, int l, int c, string operacion, LinkedList<OrderBy> orderBy) : this(id, campos, l, c, operacion)
        {
            this.orderBy = orderBy;
        }
        /*
        * CONSTRUCTOR DE LA CLASE CON UNA LISTA DE CAMPOS (WHERE ORDER BY | WHERE ORDER BY LIMIT)
        * @param id: nombre de la tabla
        * @param campos: lista de campos que buscaremos
        * @param l: linea del id
        * @param c: columna del id
        * @param {condicion} condicion para el where
        * @param {orderBy} lista con las columnas con las cuales se aplicaran el sort
        */

        public Select(string id, LinkedList<Expresion> campos, int l, int c, string operacion, Expresion condicion, Expresion condicion2, LinkedList<OrderBy> orderBy) : this(id, campos, l, c, operacion, condicion, condicion2)
        {
            this.orderBy = orderBy;
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
                                        if (campos == null) datos = getAllData(tabla,ts,user,ref baseD,mensajes,cabecera);
                                        else datos = getData(tabla,ts,user,ref baseD,mensajes,cabecera);
                                        if (datos != null)
                                        {
                                            TablaSelect  tablaSelect = new TablaSelect(cabecera, datos);
                                            System.Diagnostics.Debug.WriteLine(operacion);
                                            if (operacion.Equals("none")) { }
                                            else
                                            {
                                                if (operacion.Contains("b"))
                                                {
                                                    if (checkOrder(tablaSelect.columnas,mensajes))
                                                    {

                                                        LinkedList<int> pos = posicionesColumnas(tablaSelect.columnas);
                                                        if(pos.Count() > 0) tablaSelect.datos = new LinkedList<Data>(sort(tablaSelect.datos, pos, 0));

                                                    }
                                                    else return null;
                                                }
                                                if (operacion.Contains("c"))
                                                {
                                                    tablaSelect.datos = limitar(tablaSelect.datos, ts,user,ref baseD,mensajes,tsT);
                                                    if(datos == null) return null;
                                                   
                                                }
                                            }
                                            


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
        * METODO que aplica el limit
        * @param lista: Todas las columnas de la tabla
        * @param ts: lista de simbolos del padre
        * @param user : usuario que ejecuta las acciones
        * @param baseD : nombre de la base de datos se pasa por referencia
        * @param mensajes: output de salida
        */
        private LinkedList<Data> limitar(LinkedList<Data> datos, TablaDeSimbolos ts, string user, ref string baseD, LinkedList<string> mensajes, TablaDeSimbolos tsT)
        {
            LinkedList<Data> limiData = new LinkedList<Data>();
            Mensaje mensa = new Mensaje();
            object res = (condicion2 == null) ? null : condicion2.ejecutar(ts, user, ref baseD, mensajes, tsT);
            if(res != null)
            {
                if (res.GetType() == typeof(int))
                {
                    int limit = (int)res;
                    if (limit > 0)
                    {
                        int i = 0;
                        
                        foreach (Data data in datos)
                        {
                            if (i < limit) limiData.AddLast(data);
                            i++;
                        }
                    }
                    else
                    {
                        mensajes.AddLast(mensa.error("Limit tiene que ser entero mayor a cero no se reconoce: " + res, l, c, "Semantico"));
                        return null;
                    }
                }
                else
                {
                    mensajes.AddLast(mensa.error("Limit tiene que ser entero no se reconoce: "  + res, l, c, "Semantico"));
                    return null;
                }
            }
            else
            {
                mensajes.AddLast(mensa.error("No se acepta un limit null", l, c, "Semantico"));
                return null;
            }
            
            
            return limiData;
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
                foreach (Expresion e in campos)
                {
                    object res = (e == null) ? null : e.ejecutar(ts, user, ref baseD, mensajes, tsT);
                    
                    if (operacion.Equals("none"))
                    {
                        if (res != null) valores.AddLast(new Atributo(cabeceras.ElementAt(i).name, res, cabeceras.ElementAt(i).tipo));
                        else
                        {
                            mensajes.AddLast(mensa.error("No se aceptan columnas null", l, c, "Semantico"));
                            return null;
                        }
                    }
                    else
                    {
                        if (operacion.Contains("a"))
                        {
                            object condi = (condicion == null) ? null : condicion.ejecutar(ts, user, ref baseD, mensajes, tsT);
                            if(condi != null)
                            {
                                if (condi != null)
                                {
                                    if (condi.GetType() == typeof(Boolean))
                                    {
                                        if ((Boolean)condi)
                                        {
                                            if (res != null) valores.AddLast(new Atributo(cabeceras.ElementAt(i).name, res, cabeceras.ElementAt(i).tipo));
                                            else
                                            {
                                                mensajes.AddLast(mensa.error("No se aceptan columnas null", l, c, "Semantico"));
                                                return null;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        mensajes.AddLast(mensa.error("La condicion tiene que ser un valor booleano no se reconoce: " + condi, l, c, "Semantico"));
                                        return null;
                                    }
                                    
                                }
                                else
                                {
                                    mensajes.AddLast(mensa.error("No se aceptan columnas null", l, c, "Semantico"));
                                    return null;
                                }
                            }
                            else
                            {
                                mensajes.AddLast(mensa.error("No se aceptan condiciones null", l, c, "Semantico"));
                                return null;
                            }
                        }
                        else
                        {
                            if (res != null) valores.AddLast(new Atributo(cabeceras.ElementAt(i).name, res, cabeceras.ElementAt(i).tipo));
                            else
                            {
                                mensajes.AddLast(mensa.error("No se aceptan columnas null", l, c, "Semantico"));
                                return null;
                            }
                        }
                    }
                    
                }
                if(valores.Count() > 0) listaR.AddLast(new Data(valores));
            }
            return listaR;
        }


        /*
         * METODO DEVOLVERA  Todas las columnas
         * @param lista: Todas las columnas de la tabla
         * @param ts: lista de simbolos del padre
         * @param user : usuario que ejecuta las acciones
         * @param baseD : nombre de la base de datos se pasa por referencia
         * @param mensajes: output de salida
         */

        private LinkedList<Data> getAllData(Tabla t, TablaDeSimbolos ts, string user, ref string baseD, LinkedList<string> mensajes, LinkedList<Columna> cabeceras)
        {
            Mensaje mensa = new Mensaje();
            LinkedList<Data> listaR = new LinkedList<Data>();


            foreach (Data data in t.datos)
            {
                TablaDeSimbolos tsT = new TablaDeSimbolos();
                guardarTemp(data.valores, tsT);
                int i = 0;
                if (operacion.Equals("none")) listaR.AddLast(new Data(data.valores));
                else
                {
                    if (operacion.Contains("a"))
                    {
                        object condi = (condicion == null) ? null : condicion.ejecutar(ts, user, ref baseD, mensajes, tsT);

                        if (condi != null)
                        {
                            if (condi.GetType() == typeof(Boolean))
                            {
                                if ((Boolean)condi) listaR.AddLast(new Data(data.valores));
                            }
                            else
                            {
                                mensajes.AddLast(mensa.error("La condicion tiene que ser un valor booleano no se reconoce: " + condi, l, c, "Semantico"));
                                return null;
                            }

                        }
                        else
                        {
                            mensajes.AddLast(mensa.error("No se aceptan columnas null", l, c, "Semantico"));
                            return null;
                        }

                    }
                    else
                    {
                        listaR.AddLast(new Data(data.valores));
                    }
                }
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

        /*
         * METODO QUE BUSCA LAS COLUMNAS A ORDER 
         * @param {columnas} columnas de la tabla temporal de resultado
         * @param {mensajes} output
         */
        private Boolean checkOrder(LinkedList<Columna> columnas, LinkedList<string> mensajes)
        {
            Mensaje mensa = new Mensaje();
            foreach(OrderBy o in orderBy)
            {
                Columna co = searchColumna(columnas, o.nombre);
                if (co == null)
                {
                    mensajes.AddLast(mensa.error("No se encontro la columna para ordenamiento: " + o.nombre, l, c, "Semantico"));
                    return false;
                }
            }
            return true;
        }

        /*
        * Busca el nombre de la columna en las columnas de la tabla
        * @param {columnas} columnas de la tabla
        * @param {nombre} nombre a buscar
        * @return {int}
        */
        private int posColumna(LinkedList<Columna> columnas, string nombre)
        {
            int i = 0;
            foreach (Columna columna in columnas)
            {
                if (nombre.Equals(columna.name)) return i;
                i++;
            }
            return -1;
        }

        /*
         * Metodo que busca posiciones
         * @param {columnas} columnas de la tabla temporal de resultado
         * @return {Lista de int}
         */
        private LinkedList<int> posicionesColumnas(LinkedList<Columna> columnas)
        {
            LinkedList<int> lis = new LinkedList<int>();
            foreach(OrderBy o in orderBy)
            {
                lis.AddLast(posColumna(columnas, o.nombre));
            }
            return lis;
        }


        /*
         * METODO RECURSIVO QUE SE ENCARGA DE ORDENAR UNA LINKEDLIST DENTRO DE OTRA LINKEDLIST
         * @param{lista} data a ordenar
         * @param{numero} lista de  posiciones de data
         * @params {i} posicion actual
         */
        private IOrderedEnumerable<Data> sort(LinkedList<Data> lista, LinkedList<int> numeros, int i)
        {
            if (i + 1 < numeros.Count())
            {
                if(orderBy.ElementAt(i+1).asc) return (sort(lista, numeros, i + 1)).ThenBy(a => a.valores.ElementAt(numeros.ElementAt(i+1)).valor);
                else return (sort(lista, numeros, i + 1)).ThenByDescending(a => a.valores.ElementAt(numeros.ElementAt(i + 1)).valor);

            }
            if (orderBy.ElementAt(0).asc)  return lista.OrderBy(a => a.valores.ElementAt(numeros.ElementAt(0)).valor);
            else return lista.OrderByDescending(a => a.valores.ElementAt(numeros.ElementAt(0)).valor);
        }
    }


}
