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
    public class Update : InstruccionCQL
    {
        string id { set; get; }
        LinkedList<SetCQL> asignacion { set; get; }

        Expresion condicion { set; get; }

        int l { set; get; }
        int c {set;get;}

        string operacion { set; get; }


        /*
         * UPDATE QUE actualiza todos los datos (SIN WHERE)
         * @id nombre de la tabla
         * @asignacion campos a cambiar su valor
         * @l linea del id
         * @c columna del id
         * @operacion si viene o no WHERE
         */
        public Update(string id, LinkedList<SetCQL> asignacion, int l, int c, string operacion)
        {
            this.id = id;
            this.asignacion = asignacion;
            this.l = l;
            this.c = c;
            this.operacion = operacion;
        }
        /*
         * UPDATE QUE actualiza todos los datos con WHERE
         * @id nombre de la tabla
         * @asignacion campos a cambiar su valor
         * @condicion es la condicion a cumplirse
         * @l linea del id
         * @c columna del id
         * @operacion si viene o no WHERE
         */
        public Update(string id, LinkedList<SetCQL> asignacion, Expresion condicion, int l, int c, string operacion)
        {
            this.id = id;
            this.asignacion = asignacion;
            this.condicion = condicion;
            this.l = l;
            this.c = c;
            this.operacion = operacion;
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
                    Tabla tabla = TablaBaseDeDatos.getTabla(db, id);
                    if (tabla != null)
                    {
                        object res;
                        if (operacion.Equals("NORMAL")) res = changeAll(ts, user, ref baseD, mensajes, tabla);
                        else res = changeSpecific(ts, user, ref baseD, mensajes, tabla);
                        if (res != null) return "";
                    }
                    else mensajes.AddLast(mensa.error("La tabla: " + id + " no existe en la DB: " + baseD, l, c, "Semantico"));
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
                                    object res;
                                    if(operacion.Equals("NORMAL"))  res = changeAll(ts, user, ref baseD, mensajes, tabla);
                                    else res = changeSpecific(ts, user, ref baseD, mensajes, tabla);
                                    if (res != null) return "";
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
       * Metodo que se encargara de editar todos los campos especificos
       * @ts tabla de simbolos padre
       * @user usuario que esta ejecutando las acciones
       * @baseD string por referencia de que base de datos estamos trabajando
       * @mensajes el output de la ejecucion
       * @tsT se encargara de guardar todos los datos en una tabla temporal para la tabla
       * @t tabla actual
     */
        private object changeSpecific(TablaDeSimbolos ts, string user, ref string baseD, LinkedList<string> mensajes, Tabla t)
        {
            Mensaje mensa = new Mensaje();
            BaseDeDatos db = TablaBaseDeDatos.getBase(baseD);
            foreach (Data data in t.datos)
            {
                TablaDeSimbolos tsT = new TablaDeSimbolos();
                guardarTemp(data.valores, tsT);
                if (checkColumns(t.columnas, mensajes))
                {
                    foreach (SetCQL set in asignacion)
                    {
                        foreach (Atributo atributo in data.valores)
                        {
                            if (set.campo.Equals(atributo.nombre))
                            {
                                object res = (condicion == null) ? null : condicion.ejecutar(ts, user, ref baseD, mensajes, tsT);

                                if (condicion != null)
                                {
                                    if (res != null)
                                    {
                                        if (res.GetType() == typeof(Boolean))
                                        {
                                            if ((Boolean)res)
                                            {
                                                object op1 = (set.valor == null) ? null : set.valor.ejecutar(ts, user, ref baseD, mensajes, tsT);
                                                Atributo temp = checkinfo(getColumna(t.columnas, set.campo), op1, set.valor, mensajes, db);
                                                if (temp != null) atributo.valor = temp.valor;
                                                else return null;
                                            }
                                        }
                                        else
                                        {
                                            mensajes.AddLast(mensa.error("La condicion tiene que ser de tipo bool no ser reconoce: " + res, l, c, "Semantico"));
                                            return null;
                                        }
                                    }
                                    else return null;
                                }
                                else
                                {
                                    mensajes.AddLast(mensa.error("La condicion en un where no puede ser null", l, c, "Semantico"));
                                    return null;
                                }

                            }
                        }
                    }

                }
                else return null;
            }
            mensajes.AddLast(mensa.message("Datos actualizados con exito"));
            return "";
        }

        /*
        * Metodo que se encargara de editar todos los campos
        * @ts tabla de simbolos padre
        * @user usuario que esta ejecutando las acciones
        * @baseD string por referencia de que base de datos estamos trabajando
        * @mensajes el output de la ejecucion
        * @tsT se encargara de guardar todos los datos en una tabla temporal para la tabla
        * @t tabla actual
      */

        private object changeAll(TablaDeSimbolos ts, string user, ref string baseD, LinkedList<string> mensajes,Tabla t)
        {
            Mensaje mensa = new Mensaje();
            BaseDeDatos db = TablaBaseDeDatos.getBase(baseD);
            foreach(Data data in t.datos)
            {
                TablaDeSimbolos tsT = new TablaDeSimbolos();
                guardarTemp(data.valores, tsT);
                if (checkColumns(t.columnas, mensajes))
                {
                    foreach (SetCQL set in asignacion)
                    {
                        foreach (Atributo atributo in data.valores)
                        {
                            if (set.campo.Equals(atributo.nombre))
                            {

                                object op1 = (set.valor == null) ? null : set.valor.ejecutar(ts, user, ref baseD, mensajes, tsT);
                                Atributo temp = checkinfo(getColumna(t.columnas, set.campo), op1, set.valor, mensajes, db);
                                if (temp != null) atributo.valor = temp.valor;
                                else return null;
                            }
                        }
                    }

                }
                else return null;
            }
            mensajes.AddLast(mensa.message("Datos actualizados con exito"));
            return "";
        }

        /*
         * Metodo que se encargara de guardar los valores como variables temporales
         * @atributos es la data a guardar
         * @tsT es la tabla temporal
         */
        private void guardarTemp(LinkedList<Atributo> atributos, TablaDeSimbolos tsT)
        {
            foreach(Atributo atributo in atributos)
            {
                tsT.AddLast(new Simbolo(atributo.tipo, atributo.nombre));
                tsT.setValor(atributo.nombre, atributo.valor);
            }
        }

        /*
         * Verifica que existan todas las columnas a setear
         * @columnas son las columnas de la tabla
         * @mensajes output
         */
        private Boolean checkColumns(LinkedList<Columna> columnas,LinkedList<string> mensajes)
        {
            Mensaje mensa = new Mensaje();
            foreach(SetCQL setcql in asignacion)
            {
                Boolean flag = false;
                foreach(Columna columna in columnas)
                {
                    if (columna.name.Equals(setcql.campo)) flag = true;
                }
                if (!flag)
                {
                    mensajes.AddLast(mensa.error("No se encontro la columna: " + setcql.campo, setcql.l, setcql.c, "Semantico"));
                    return false;
                }
            }
            return true;
        }

        /*
          * Metodo que verificara el tipo a guardar
          * @columna columan actual
          * @valor es el valor  a guardar
          * @original es la expresion sin ejecutar
          * @mensajes output de salida
          * @db BaseDeDatos actual
          */

        private Atributo checkinfo(Columna columna, object valor, object original, LinkedList<string> mensajes, BaseDeDatos db)
        {
            Mensaje mensa = new Mensaje();
            if (original == null)
            {
                if (!columna.pk)
                {
                    if (columna.tipo.Equals("string") || columna.tipo.Equals("date") || columna.tipo.Equals("time")) return new Atributo(columna.name, null, columna.tipo);
                    else if (columna.tipo.Equals("int") || columna.tipo.Equals("double") || columna.tipo.Equals("boolean"))
                        mensajes.AddLast(mensa.error("No se le puede asignar a un tipo: " + columna.tipo + " un valor null", l, c, "Semantico"));
                    else
                    {
                        Boolean temp = TablaBaseDeDatos.getUserType(columna.tipo, db);
                        if (temp) return new Atributo(columna.name, new InstanciaUserType(columna.tipo, null), columna.tipo);
                        else mensajes.AddLast(mensa.error("No existe el USER TYPE: " + columna.tipo + " en la DB: " + db.nombre, l, c, "Semantico"));
                    }
                }
                else mensajes.AddLast(mensa.error("La columna: " + columna.name + " es clave primaria no puede asignarsele un valor null", l, c, "Semantico"));

            }
            else
            {
                if (valor != null)
                {
                    if (columna.tipo.Equals("string") && valor.GetType() == typeof(string)) return new Atributo(columna.name, (string)valor, "string");
                    else if (columna.tipo.Equals("int") && valor.GetType() == typeof(int)) return new Atributo(columna.name, (int)valor, "int");
                    else if (columna.tipo.Equals("double") && valor.GetType() == typeof(Double)) return new Atributo(columna.name, (Double)valor, "double");
                    else if (columna.tipo.Equals("boolean") && valor.GetType() == typeof(Boolean)) return new Atributo(columna.name, (Boolean)valor, "boolean");
                    else if (columna.tipo.Equals("date") && valor.GetType() == typeof(DateTime)) return new Atributo(columna.name, (DateTime)valor, "date");
                    else if (columna.tipo.Equals("time") && valor.GetType() == typeof(TimeSpan)) return new Atributo(columna.name, (TimeSpan)valor, "time");
                    else if (valor.GetType() == typeof(InstanciaUserType))
                    {
                        InstanciaUserType temp = (InstanciaUserType)valor;
                        if (columna.tipo.Equals(temp.tipo)) return new Atributo(columna.name, (InstanciaUserType)valor, columna.tipo);
                        else mensajes.AddLast(mensa.error("No se le puede asignar a la columna: " + columna.name + " un tipo: " + temp.tipo, l, c, "Semantico"));
                    }
                    else mensajes.AddLast(mensa.error("No se puede asignar a la columna: " + columna.name + " el valor: " + valor, l, c, "Semantico"));
                }
                else
                {
                    if (columna.tipo.Equals("string") || columna.tipo.Equals("date") || columna.tipo.Equals("time")) return new Atributo(columna.name, null, columna.tipo);
                    else if (columna.tipo.Equals("boolean") || columna.tipo.Equals("int") || columna.tipo.Equals("double")) mensajes.AddLast(mensa.error("No se puede asignar a la columna: " + columna.name + " el valor: " + valor, l, c, "Semantico"));
                    else return new Atributo(columna.name, new InstanciaUserType(columna.tipo, null), columna.tipo);
                }
            }
            return null;
        }

        /*
         * Metodo que retorna una columna
         */

        private Columna getColumna(LinkedList<Columna> columnas, string nombre)
        {
            foreach(Columna columna in columnas)
            {
                if (nombre.Equals(columna.name)) return columna;
            }
            return null;
        }
    }
}
