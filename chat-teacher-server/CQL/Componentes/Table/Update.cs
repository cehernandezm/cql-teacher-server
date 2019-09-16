using cql_teacher_server.CHISON;
using cql_teacher_server.CHISON.Componentes;
using cql_teacher_server.CQL.Arbol;
using cql_teacher_server.CQL.Componentes.Try_Catch;
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

        Expresion key { set; get; }

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
        public object ejecutar(TablaDeSimbolos ts,Ambito ambito, TablaDeSimbolos tsT)
        {
            Mensaje mensa = new Mensaje();
            string user = ambito.usuario;
            string baseD = ambito.baseD;
            LinkedList<string> mensajes = ambito.mensajes;
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
                                    object res;
                                    if(operacion.Equals("NORMAL"))  res = changeAll(ts,ambito, tabla);
                                    else res = changeSpecific(ts, ambito, tabla);
                                    if (res != null) return "";
                                }
                                else
                                {
                                    ambito.listadoExcepciones.AddLast(new Excepcion("tabledontexists", "La tabla: " + id + " no existe en la DB: " + ambito.baseD));
                                    ambito.mensajes.AddLast(mensa.error("La tabla: " + id + " no existe en la DB: " + ambito.baseD, l, c, "Semantico"));
                                }
                            }
                            else mensajes.AddLast(mensa.error("La DB: " + baseD + " ya esta siendo utilizada por alguien mas", l, c, "Semantico"));
                        }
                        else mensajes.AddLast(mensa.error("El usuario " + user + " no tiene permisos sobre esta base de datos", l, c, "Semantico"));
                    }
                    else mensajes.AddLast(mensa.error("El usuario " + user + " no existe", l, c, "Semantico"));

                }

            }
            else
            {
                ambito.listadoExcepciones.AddLast(new Excepcion("usedbexception", "No existe la base de datos: " + ambito.baseD + " o no se ha usado el comando use"));

                ambito.mensajes.AddLast(mensa.error("La base de datos ha usar: " + ambito.baseD + " no existe", l, c, "Semantico"));
            }
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
        private object changeSpecific(TablaDeSimbolos ts, Ambito ambito, Tabla t)
        {
            Mensaje mensa = new Mensaje();
            string baseD = ambito.baseD;
            LinkedList<string> mensajes = ambito.mensajes;
            BaseDeDatos db = TablaBaseDeDatos.getBase(baseD);
            foreach (Data data in t.datos)
            {
                TablaDeSimbolos tsT = new TablaDeSimbolos();
                guardarTemp(data.valores, tsT);
                if (checkColumns(t.columnas, mensajes,ambito))
                {
                    foreach (SetCQL set in asignacion)
                    {
                        foreach (Atributo atributo in data.valores)
                        {

                            object res = (condicion == null) ? null : condicion.ejecutar(ts,ambito, tsT);

                            if (condicion != null)
                            {
                                if (res != null)
                                {
                                    if (res.GetType() == typeof(Boolean))
                                    {
                                        if ((Boolean)res)
                                        {
                                            object op1 = (set.valor == null) ? null : set.valor.ejecutar(ts,ambito, tsT);
                                            if (set.operacion.Equals("NORMAL"))
                                            {

                                                if (set.campo.Equals(atributo.nombre))
                                                {
                                                    Atributo temp = checkinfo(getColumna(t.columnas, set.campo), op1, set.valor, mensajes, db,ambito);
                                                    if (temp != null) atributo.valor = temp.valor;
                                                    else return null;
                                                }

                                            }
                                            else
                                            {
                                                object op2 = (set.accesoUS == null) ? null : set.accesoUS.ejecutar(ts,ambito, tsT);

                                                if (op2 != null)
                                                {
                                                    if (op2.GetType() == typeof(InstanciaUserType))
                                                    {

                                                        InstanciaUserType temp = (InstanciaUserType)op2;
                                                        foreach (Atributo a in temp.lista)
                                                        {
                                                            
                                                            if (a.nombre.Equals(set.campo))
                                                            {

                                                                Columna co = new Columna(a.nombre, a.tipo, false);
                                                                Atributo temp2 = checkinfo(co, op1, set.valor, mensajes, db,ambito);
                                                                if (temp2 != null) a.valor = temp2.valor;
                                                                else return null;
                                                            }
                                                        }
                                                    }
                                                    else if (op2.GetType() == typeof(Map))
                                                    {
                                                        object campo = (set.key == null) ? null : set.key.ejecutar(ts,ambito, tsT);
                                                        Map temp = (Map)op2;
                                                        string tipo = temp.id.Split(new[] { ',' }, 2)[1];
                                                        foreach (KeyValue ky in temp.datos)
                                                        {
                                                            if (ky.key.ToString().Equals(campo))
                                                            {
                                                                Columna co = new Columna(ky.key.ToString(), tipo, false);
                                                                Atributo temp2 = checkinfo(co, op1, set.valor, mensajes, db,ambito);
                                                                if (temp2 != null) ky.value = temp2.valor;
                                                                else return null;
                                                            }
                                                        }
                                                    }
                                                    else if(op2.GetType() == typeof(List))
                                                    {
                                                        List temp = (List)op2;
                                                        object campo = (set.key == null) ? null : set.key.ejecutar(ts, ambito, tsT);
                                                        if(campo != null)
                                                        {
                                                            if(campo.GetType() == typeof(int))
                                                            {
                                                                if((int)campo > -1)
                                                                {
                                                                    if((int)campo < temp.lista.Count())
                                                                    {
                                                                        if (temp.lista.Count() > 0)
                                                                        {
                                                                            var node = temp.lista.First;
                                                                            int index = 0;
                                                                            while (node != null)
                                                                            {
                                                                                var nodeNext = node.Next;
                                                                                if(index == (int)campo)
                                                                                {
                                                                                    Columna co = new Columna("", temp.id, false);
                                                                                    Atributo temp2 = checkinfo(co, op1, set.valor, mensajes, db,ambito);
                                                                                    if (temp2 != null) node.Value = temp2.valor;
                                                                                    else return null;
                                                                                    break;
                                                                                }
                                                                                node = nodeNext;
                                                                                index++;
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        ambito.listadoExcepciones.AddLast(new Excepcion("indexoutexception", "El index es mayor al tamaño de la lista"));
                                                                        mensajes.AddLast(mensa.error("El index supera el tamanio de la lista", l, c, "Semantico"));
                                                                        return null;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    ambito.listadoExcepciones.AddLast(new Excepcion("indexoutexception", "Index tiene que ser mayor a 0 "));
                                                                    mensajes.AddLast(mensa.error("El index debe de ser positivo: " + campo, l, c, "Semantico"));
                                                                    return null;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                mensajes.AddLast(mensa.error("El index debe de ser de tipo numerico: " + campo,l,c,"Semantico"));
                                                                return null;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            mensajes.AddLast(mensa.error("El index no puede ser null", l, c, "Semantico"));
                                                            return null;
                                                        }
                                                    }
                                                }
                                            }
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

        private object changeAll(TablaDeSimbolos ts, Ambito ambito,Tabla t)
        {
            Mensaje mensa = new Mensaje();
            string baseD = ambito.baseD;
            LinkedList<string> mensajes = ambito.mensajes;
            BaseDeDatos db = TablaBaseDeDatos.getBase(baseD);
            foreach(Data data in t.datos)
            {
                TablaDeSimbolos tsT = new TablaDeSimbolos();
                guardarTemp(data.valores, tsT);
                if (checkColumns(t.columnas, mensajes,ambito))
                {
                    foreach (SetCQL set in asignacion)
                    {
                        foreach (Atributo atributo in data.valores)
                        {
                            object op1 = (set.valor == null) ? null : set.valor.ejecutar(ts, ambito, tsT);
                            if (set.operacion.Equals("NORMAL"))
                            {

                                if (set.campo.Equals(atributo.nombre))
                                {
                                    Atributo temp = checkinfo(getColumna(t.columnas, set.campo), op1, set.valor, mensajes, db,ambito);
                                    if (temp != null) atributo.valor = temp.valor;
                                    else return null;
                                }

                            }
                            else
                            {
                                object op2 = (set.accesoUS == null) ? null : set.accesoUS.ejecutar(ts,ambito, tsT);

                                if (op2 != null)
                                {
                                    if (op2.GetType() == typeof(InstanciaUserType))
                                    {

                                        InstanciaUserType temp = (InstanciaUserType)op2;
                                        foreach (Atributo a in temp.lista)
                                        {

                                            if (a.nombre.Equals(set.campo))
                                            {

                                                Columna co = new Columna(a.nombre, a.tipo, false);
                                                Atributo temp2 = checkinfo(co, op1, set.valor, mensajes, db,ambito);
                                                if (temp2 != null) a.valor = temp2.valor;
                                                else return null;
                                            }
                                        }
                                    }
                                    else if (op2.GetType() == typeof(Map))
                                    {
                                        object campo = (set.key == null) ? null : set.key.ejecutar(ts, ambito, tsT);
                                        Map temp = (Map)op2;
                                        string tipo = temp.id.Split(new[] { ',' }, 2)[1];
                                        foreach (KeyValue ky in temp.datos)
                                        {
                                            if (ky.key.ToString().Equals(campo))
                                            {
                                                Columna co = new Columna(ky.key.ToString(), tipo, false);
                                                Atributo temp2 = checkinfo(co, op1, set.valor, mensajes, db,ambito);
                                                if (temp2 != null) ky.value = temp2.valor;
                                                else return null;
                                            }
                                        }
                                        ambito.listadoExcepciones.AddLast(new Excepcion("indexoutexception", "No se encontro la key" ));
                                    }
                                    else if (op2.GetType() == typeof(List))
                                    {
                                        List temp = (List)op2;
                                        object campo = (set.key == null) ? null : set.key.ejecutar(ts, ambito, tsT);
                                        if (campo != null)
                                        {
                                            if (campo.GetType() == typeof(int))
                                            {
                                                if ((int)campo > -1)
                                                {
                                                    if ((int)campo < temp.lista.Count())
                                                    {
                                                        if (temp.lista.Count() > 0)
                                                        {
                                                            var node = temp.lista.First;
                                                            int index = 0;
                                                            while (node != null)
                                                            {
                                                                var nodeNext = node.Next;
                                                                if (index == (int)campo)
                                                                {
                                                                    Columna co = new Columna("", temp.id, false);
                                                                    Atributo temp2 = checkinfo(co, op1, set.valor, mensajes, db,ambito);
                                                                    if (temp2 != null) node.Value = temp2.valor;
                                                                    else return null;
                                                                    break;
                                                                }
                                                                node = nodeNext;
                                                                index++;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ambito.listadoExcepciones.AddLast(new Excepcion("indexoutexception", "El index es mayor al tamaño de la lista"));
                                                        mensajes.AddLast(mensa.error("El index supera el tamanio de la lista", l, c, "Semantico"));
                                                        return null;
                                                    }
                                                }
                                                else
                                                {
                                                    ambito.mensajes.AddLast(mensa.error("Index tiene que ser mayor a 0  ", l, c, "Semantico"));
                                                    mensajes.AddLast(mensa.error("El index debe de ser positivo: " + campo, l, c, "Semantico"));
                                                    return null;
                                                }
                                            }
                                            else
                                            {
                                                mensajes.AddLast(mensa.error("El index debe de ser de tipo numerico: " + campo, l, c, "Semantico"));
                                                return null;
                                            }
                                        }
                                        else
                                        {
                                            mensajes.AddLast(mensa.error("El index no puede ser null", l, c, "Semantico"));
                                            return null;
                                        }
                                    }
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
        private Boolean checkColumns(LinkedList<Columna> columnas,LinkedList<string> mensajes, Ambito ambito)
        {
            Mensaje mensa = new Mensaje();
            foreach(SetCQL setcql in asignacion)
            {
                Boolean flag = false;
                foreach(Columna columna in columnas)
                {
                    if (columna.name.Equals(setcql.campo)) flag = true;
                }
                if (!flag && setcql.accesoUS == null)
                {
                    ambito.listadoExcepciones.AddLast(new Excepcion("columnexception", "No se encontro la columna: " + setcql.campo));
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

        private Atributo checkinfo(Columna columna, object valor, object original, LinkedList<string> mensajes, BaseDeDatos db, Ambito ambito)
        {
            Mensaje mensa = new Mensaje();
            if (original == null)
            {
                if (!columna.pk)
                {
                    if (columna.tipo.Equals("string") || columna.tipo.Equals("date") || columna.tipo.Equals("time")) return new Atributo(columna.name, null, columna.tipo);
                    else if (columna.tipo.Equals("int") || columna.tipo.Equals("double") || columna.tipo.Equals("boolean") || columna.tipo.Contains("list") || columna.tipo.Contains("map") || columna.tipo.Contains("set"))
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
                    else if (columna.tipo.Contains("map") && valor.GetType() == typeof(Map))
                    {
                        string tipo = columna.tipo.TrimStart('m').TrimStart('a').TrimStart('p').TrimStart('<');
                        if (tipo.EndsWith('>')) tipo = tipo.Substring(0, tipo.Length - 1);
                        Map temp = (Map)valor;
                        if (tipo.Equals(temp.id)) return new Atributo(columna.name, temp, tipo);
                        else mensajes.AddLast(mensa.error("No coinciden los tipos en el MAP: " + tipo + " con: " + temp.id, l, c, "Semantico"));
                    }
                    else if (columna.tipo.Contains("set") && valor.GetType() == typeof(Set))
                    {
                        string tipo = columna.tipo.TrimStart('s').TrimStart('e').TrimStart('t').TrimStart('<');
                        if (tipo.EndsWith('>')) tipo = tipo.Substring(0, tipo.Length - 1);
                        Set temp = (Set)valor;
                        if (tipo.Equals(temp.id)) return new Atributo(columna.name, temp, tipo);
                        else mensajes.AddLast(mensa.error("No coinciden los tipos en el SET: " + tipo + " con: " + temp.id, l, c, "Semantico"));
                    }
                    else if (columna.tipo.Contains("list") && valor.GetType() == typeof(List))
                    {
                        string tipo = columna.tipo.TrimStart('l').TrimStart('i').TrimStart('s').TrimStart('t').TrimStart('<');
                        if (tipo.EndsWith('>')) tipo = tipo.Substring(0, tipo.Length - 1);
                        List temp = (List)valor;
                        if (tipo.Equals(temp.id)) return new Atributo(columna.name, temp, tipo);
                        else mensajes.AddLast(mensa.error("No coinciden los tipos en el LIST: " + tipo + " con: " + temp.id, l, c, "Semantico"));
                    }
                    else if (valor.GetType() == typeof(InstanciaUserType))
                    {
                        InstanciaUserType temp = (InstanciaUserType)valor;
                        if (columna.tipo.Equals(temp.tipo)) return new Atributo(columna.name, (InstanciaUserType)valor, columna.tipo);
                        else mensajes.AddLast(mensa.error("No se le puede asignar a la columna: " + columna.name + " un tipo: " + temp.tipo, l, c, "Semantico"));
                    }
                    else if (columna.tipo.Equals("counter"))
                    {
                        ambito.listadoExcepciones.AddLast(new Excepcion("countertypeexception", "No se puede actualizar un valor counter"));
                        mensajes.AddLast(mensa.error("No se puede actualizar un valor counter",l,c,"Semantico"));
                        return null;
                    }
                    else mensajes.AddLast(mensa.error("No se puede asignar a la columna: " + columna.name + " el valor: " + valor, l, c, "Semantico"));
                }
                else
                {
                    if (columna.tipo.Equals("string") || columna.tipo.Equals("date") || columna.tipo.Equals("time")) return new Atributo(columna.name, null, columna.tipo);
                    else if (columna.tipo.Equals("boolean") || columna.tipo.Equals("int") || columna.tipo.Equals("double") || columna.tipo.Contains("map") || columna.tipo.Contains("list") || columna.tipo.Contains("set")) mensajes.AddLast(mensa.error("No se puede asignar a la columna: " + columna.name + " el valor: " + valor, l, c, "Semantico"));
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
