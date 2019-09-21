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
    public class Insert : InstruccionCQL
    {
        string id { set; get; }
        LinkedList<Expresion> values { set; get; }
        LinkedList<string> campos { set; get; }
        string operacion { set; get; }

        int l { set; get; }
        int c { set; get; }




        /*
        * Constructor de la clase
        * @id nombre de la tabla
        * @values lista de valores a guardar
        * @l linea del id
        * @c columna del id
        */

        public Insert(string id, LinkedList<Expresion> values, string operacion, int l, int c)
        {
            this.id = id;
            this.values = values;
            this.operacion = operacion;
            this.l = l;
            this.c = c;
        }


        /*
        * Constructor de la clase
        * @id nombre de la tabla
        * @values lista de valores a guardar
        * @campos lista de campos donde se asignara el valor
        * @l linea del id
        * @c columna del id
        */

        public Insert(string id, LinkedList<Expresion> values, LinkedList<string> campos, string operacion, int l, int c)
        {
            this.id = id;
            this.values = values;
            this.campos = campos;
            this.operacion = operacion;
            this.l = l;
            this.c = c;
        }





        /*
          * Constructor de la clase padre
          * @ts tabla de simbolos padre
          * @user usuario que esta ejecutando las acciones
          * @baseD string por referencia de que base de datos estamos trabajando
          * @mensajes el output de la ejecucion
        */
        public object ejecutar(TablaDeSimbolos ts, Ambito ambito, TablaDeSimbolos tsT)
        {
            Mensaje mensa = new Mensaje();
            string baseD = ambito.baseD;
            string user = ambito.usuario;
            LinkedList<string> mensajes = ambito.mensajes;
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
                        if (operacion.Equals("NORMAL")) res = guardarNormal(tabla, ts, db, ambito, tsT);
                        else res = guardadoEspecial(tabla, ts, db, ambito, tsT);
                        if (res != null) return res;
                    }
                    else
                    {
                        ambito.listadoExcepciones.AddLast(new Excepcion("tabledontexists", "La tabla: " + id + " no existe en la DB: " + ambito.baseD));
                        ambito.mensajes.AddLast(mensa.error("La tabla: " + id + " no existe en la DB: " + ambito.baseD, l, c, "Semantico"));
                    }
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
                                    if (operacion.Equals("NORMAL")) res = guardarNormal(tabla, ts, db, ambito, tsT);
                                    else res = guardadoEspecial(tabla, ts, db, ambito, tsT);
                                    if (res != null) return res;
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
         * Metodo que inserta Normalmente en la tabla
         * @t Tabla donde guardaremos los datos
         * @mensajes output
         * @TablaDeSimbolos Tabla de simoblos padre
         * @user usuario que esta ejecutando las acciones
         * @db base de datos actual
         * @baseD nombre de la base de datos por referencia
         */
        private object guardarNormal(Tabla t,TablaDeSimbolos ts,BaseDeDatos db, Ambito ambito , TablaDeSimbolos tsT)
        {
            Mensaje mensa = new Mensaje();
            string user = ambito.usuario;
            string baseD = ambito.baseD;
            LinkedList<string> mensajes = ambito.mensajes;
            int i = 0;
            int cantidad = cantidadCounters(t);
            if((values.Count() == t.columnas.Count()) && cantidad  > 0)
            {
                ambito.listadoExcepciones.AddLast(new Excepcion("countertypeexception", "No se le puede insertar un valor a un tipo Counter"));
                mensajes.AddLast(mensa.error("No se le puede insertar un valor a un tipo Counter", l, c, "Semantico"));
                return null;
            }
            if ((cantidad + values.Count()) == t.columnas.Count())
            {
                LinkedList<Atributo> insercion = new LinkedList<Atributo>();
                LinkedList<int> posiciones = new LinkedList<int>();
                foreach (Columna co in t.columnas)
                {
                    if (co.tipo.Equals("counter"))
                    {
                        int counter = getLastCounter(i, t.datos);
                        Atributo atributo = new Atributo(co.name, counter + 1, "counter");
                        insercion.AddLast(atributo);
                    }
                    else
                    {
                        object op1 = (values.ElementAt(i) == null) ? null : values.ElementAt(i).ejecutar(ts, ambito, tsT);
                        Atributo atributo = checkinfo(co, op1, values.ElementAt(i), mensajes, db,ambito);
                        if (atributo == null) return null;
                        else insercion.AddLast(atributo);
                    }
                    if (co.pk) posiciones.AddLast(i);
                    if (co.tipo.Equals("counter")) i--;
                    i++;
                }
                if (!checkPrimaryKey(posiciones, insercion, mensajes, t.datos, 0))
                {
                    t.datos.AddLast(new Data(insercion));
                    mensajes.AddLast(mensa.message("Informacion insertada con exito"));
                    return "";
                }
                else mensajes.AddLast(mensa.error("Ya hay un dato que posee esa clave primaria", l, c, "Semantico"));

            }
            else
            {
                ambito.listadoExcepciones.AddLast(new Excepcion("valuesexception", "La cantidad de elementos en Values no coincide con la cantidad de Columnas de la tabla: " + t.nombre));
                mensajes.AddLast(mensa.error("La cantidad de elementos en Values no coincide con la cantidad de Columnas de la tabla: " + t.nombre, l, c, "Semantico"));
            }

            return null;
        }




        /*
        * Metodo que inserta Especialmente en la tabla
        * @t Tabla donde guardaremos los datos
        * @mensajes output
        * @TablaDeSimbolos Tabla de simoblos padre
        * @user usuario que esta ejecutando las acciones
        * @db base de datos actual
        * @baseD nombre de la base de datos por referencia
        */
        private object guardadoEspecial(Tabla t,TablaDeSimbolos ts, BaseDeDatos db, Ambito ambito, TablaDeSimbolos tsT)
        {
            Mensaje mensa = new Mensaje();
            string user = ambito.usuario;
            string baseD = ambito.baseD;
            LinkedList<string> mensajes = ambito.mensajes;
            if (values.Count() == campos.Count())
            {
                if (existColumn(t.columnas, campos, mensajes,ambito))
                {
                    LinkedList<Atributo> info = new LinkedList<Atributo>();
                    LinkedList<int> pos = new LinkedList<int>();
                    int posicion = 0;
                    foreach (Columna columna in t.columnas)
                    {
                        Boolean flag = false;
                        int i = 0;

                        foreach (string campo in campos)
                        {
                            if (campo.Equals(columna.name))
                            {
                                if (columna.tipo.Equals("counter"))
                                {
                                    ambito.listadoExcepciones.AddLast(new Excepcion("countertypeexception", "No se le puede insertar un valor a un tipo Counter"));
                                    mensajes.AddLast(mensa.error("No se le puede insertar un valor a un tipo Counter", l, c, "Semantico"));
                                    return null;
                                }
                                else
                                {
                                    flag = true;
                                    object op1 = (values.ElementAt(i) == null) ? null : values.ElementAt(i).ejecutar(ts, ambito, tsT);
                                    Atributo a = checkinfo(columna, op1, values.ElementAt(i), mensajes, db,ambito);
                                    if (a != null) info.AddLast(a);
                                    else return null;
                                }

                            }
                            i++;
                        }
                        if (!flag)
                        {
                            if (columna.pk)
                            {
                                if (columna.tipo.Equals("counter"))
                                {
                                    int index = getLastCounter(posicion, t.datos);
                                    info.AddLast(new Atributo(columna.name, index + 1, "counter"));
                                }
                                else
                                {
                                    Atributo a = checkinfo(columna, null, null, mensajes, db,ambito);
                                    if (a != null) info.AddLast(a);
                                    else return null;
                                    pos.AddLast(posicion);
                                }

                            }
                            else
                            {
                                object val;
                                if (columna.tipo.Equals("string") || columna.tipo.Equals("date") || columna.tipo.Equals("time")) val = null;
                                else if (columna.tipo.Equals("double") || columna.tipo.Equals("int")) val = 0;
                                else if (columna.tipo.Equals("boolean")) val = false;
                                else if (columna.tipo.Equals("counter")) val = getLastCounter(posicion, t.datos) + 1;
                                else if (columna.tipo.Contains("map") || columna.tipo.Contains("list") || columna.tipo.Contains("set"))
                                {
                                    mensajes.AddLast(mensa.error("El tipo: " + columna.tipo + " no puede ser null", l, c, "Semantico"));
                                    return null;
                                }
                                else val = new InstanciaUserType(columna.tipo, null);

                                Atributo a = checkinfo(columna, val, 0, mensajes, db,ambito);
                                if (a != null) info.AddLast(a);
                                else return null;
                            }

                        }
                        else if (columna.pk) pos.AddLast(posicion);
                        posicion++;

                    }

                    if (!checkPrimaryKey(pos, info, mensajes, t.datos, 0))
                    {
                        t.datos.AddLast(new Data(info));
                        mensajes.AddLast(mensa.message("Se inserto exitosamente la informacion"));
                        return "";
                    }
                    else mensajes.AddLast(mensa.error("Ya hay un dato que posee esa clave primaria", l, c, "Semantico"));
                }

            }
            else
            {
                ambito.listadoExcepciones.AddLast(new Excepcion("valuesexception", "No coinciden la cantidad de campos con la cantidad de valores"));
                mensajes.AddLast(mensa.error("No coinciden la cantidad de campos con la cantidad de valores", l, c, "Semantico"));
            }


            return null;
        }


        /*
        * Metodo que contara cuantos atributos tipo counter hay
        * @t tabla donde se encuentran las columnas
        */
        private int cantidadCounters(Tabla t)
        {
            int contador = 0;
            foreach (Columna co in t.columnas)
            {
                if (co.tipo.Equals("counter")) contador++;
            }
            return contador;
        }


        /*
         * Metodo que verificara el tipo a guardar
         * @columna columan actual
         * @valor es el valor  a guardar
         * @original es la expresion sin ejecutar
         * @mensajes output de salida
         * @db BaseDeDatos actual
         */

        private Atributo checkinfo(Columna columna, object valor, object original, LinkedList<string> mensajes, BaseDeDatos db,Ambito ambito)
        {
            Mensaje mensa = new Mensaje();
            if (original == null)
            {
                if (!columna.pk)
                {
                    if (columna.tipo.Equals("string") || columna.tipo.Equals("date") || columna.tipo.Equals("time")) return new Atributo(columna.name, null, columna.tipo);
                    else if (columna.tipo.Equals("int") || columna.tipo.Equals("double") || columna.tipo.Equals("boolean") || columna.tipo.Contains("map") || columna.tipo.Contains("list") || columna.tipo.Contains("set"))
                    {
                        ambito.listadoExcepciones.AddLast(new Excepcion("valuesexception", "No se le puede asignar a un tipo: " + columna.tipo + " un valor null"));
                        mensajes.AddLast(mensa.error("No se le puede asignar a un tipo: " + columna.tipo + " un valor null", l, c, "Semantico"));
                    }
                    else
                    {
                        Boolean temp = TablaBaseDeDatos.getUserType(columna.tipo, db);
                        if (temp) return new Atributo(columna.name, new InstanciaUserType(columna.tipo, null), columna.tipo);
                        else mensajes.AddLast(mensa.error("No existe el USER TYPE: " + columna.tipo + " en la DB: " + db.nombre, l, c, "Semantico"));
                    }
                }
                else
                {
                    ambito.listadoExcepciones.AddLast(new Excepcion("valuesexception", "La columna: " + columna.name + " es clave primaria no puede asignarsele un valor null"));

                    mensajes.AddLast(mensa.error("La columna: " + columna.name + " es clave primaria no puede asignarsele un valor null", l, c, "Semantico"));
                }
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
                    else if (columna.tipo.Equals("counter") && valor.GetType() == typeof(int)) return new Atributo(columna.name, (int)valor, "int");
                    else if (columna.tipo.Contains("map") && valor.GetType() == typeof(Map))
                    {
                        Map temp = (Map)valor;
                        string tipo = columna.tipo.TrimStart('m').TrimStart('a').TrimStart('p').TrimStart('<');
                        if (tipo.EndsWith('>')) tipo = tipo.Substring(0, tipo.Length - 1);
                        if (temp.id.Equals(tipo)) return new Atributo(columna.name, temp, tipo);
                        mensajes.AddLast(mensa.error("No coinciden los tipos de map: " + tipo + " con: " + temp.id, l, c, "Semantico"));
                    }
                    else if (columna.tipo.Contains("set") && valor.GetType() == typeof(Set))
                    {
                        Set temp = (Set)valor;

                        string tipo = columna.tipo.TrimStart('s').TrimStart('e').TrimStart('t').TrimStart('<');
                        if (tipo.EndsWith('>')) tipo = tipo.Substring(0, tipo.Length - 1);
                        if (temp.id.Equals(tipo)) return new Atributo(columna.name, temp, tipo);
                        mensajes.AddLast(mensa.error("No coinciden los tipos de Set: " + tipo + " con: " + temp.id, l, c, "Semantico"));
                    }
                    else if (columna.tipo.Contains("list") && valor.GetType() == typeof(List))
                    {
                        List temp = (List)valor;

                        string tipo = columna.tipo.TrimStart('l').TrimStart('i').TrimStart('s').TrimStart('t').TrimStart('<');
                        if (tipo.EndsWith('>')) tipo = tipo.Substring(0, tipo.Length - 1);
                        if (temp.id.Equals(tipo)) return new Atributo(columna.name, temp, tipo);
                        mensajes.AddLast(mensa.error("No coinciden los tipos de List: " + tipo + " con: " + temp.id, l, c, "Semantico"));
                    }
                    else if (valor.GetType() == typeof(InstanciaUserType))
                    {
                        InstanciaUserType temp = (InstanciaUserType)valor;
                        if (columna.tipo.Equals(temp.tipo)) return new Atributo(columna.name, (InstanciaUserType)valor, columna.tipo);
                        else mensajes.AddLast(mensa.error("No se le puede asignar a la columna: " + columna.name + " un tipo: " + temp.tipo, l, c, "Semantico"));
                    }
                    else
                    {
                        ambito.listadoExcepciones.AddLast(new Excepcion("valuesexception", "No se puede asignar a la columna: " + columna.name + " el valor: " + valor));
                        mensajes.AddLast(mensa.error("No se puede asignar a la columna: " + columna.name + " el valor: " + valor, l, c, "Semantico"));
                    }
                }
                else
                {
                    if (columna.tipo.Equals("string") || columna.tipo.Equals("date") || columna.tipo.Equals("time")) return new Atributo(columna.name, null, columna.tipo);
                    else if (columna.tipo.Equals("boolean") || columna.tipo.Equals("int") || columna.tipo.Equals("double") || columna.tipo.Contains("map") || columna.tipo.Contains("list") || columna.tipo.Contains("set"))
                    {
                        ambito.listadoExcepciones.AddLast(new Excepcion("valuesexception", "No se puede asignar a la columna: " + columna.name + " el valor: " + valor + valor));

                        mensajes.AddLast(mensa.error("No se puede asignar a la columna: " + columna.name + " el valor: " + valor, l, c, "Semantico"));
                    }
                    else return new Atributo(columna.name, new InstanciaUserType(columna.tipo, null), columna.tipo);
                }
            }
            return null;
        }

        /*
         * Metodo que devuelve el ultimo counter guardado
         * @index es la posicion de la columna
         * @datas es toda la informacion de la tabla
         */
        private int getLastCounter(int index, LinkedList<Data> datas)
        {
            if (datas.Count() == 0) return 0;
            var node = datas.Last;
            LinkedList<Atributo> atributos = ((Data)node.Value).valores;
            Atributo atributo = atributos.ElementAt(index);
            return (int)atributo.valor;
        }
        
        /*
         * Metodo que busca en toda la informacion guardada que no haya otras claves primarias iguales
         * @posiciones es una lista de posiciones donde se encuentran las pk
         * @atributos es una lista con la informacion a guardar
         * @mensajes output
         * @datas es toda la informacion de la base de datos
         * @pos es la posicion de la lista de posiciones
         */
        private Boolean checkPrimaryKey(LinkedList<int> posiciones, LinkedList<Atributo> atributos, LinkedList<string> mensajes, LinkedList<Data> datas, int pos)
        {
            if (posiciones.Count() > 0)
            {
                //----------------------------------------Esto significa que estamos en la ultima posicion a verificar
                if (pos + 1 == posiciones.Count())
                {
                    foreach(Data data in datas)
                    {
                        Atributo at = data.valores.ElementAt(posiciones.ElementAt(pos));
                        Atributo at2 = atributos.ElementAt(posiciones.ElementAt(pos));
                        if (at.valor.Equals(at2.valor)) return true;
                    }
                }
                else
                {
                    foreach (Data data in datas)
                    {
                        Atributo at = data.valores.ElementAt(posiciones.ElementAt(pos));
                        Atributo at2 = atributos.ElementAt(posiciones.ElementAt(pos));
                        if (at.valor.Equals(at2.valor)) return true && checkPrimaryKey(posiciones,atributos,mensajes,datas,pos + 1);
                    }
                }
            }
            return false;
        }

        /*
         * Metodo que busca todas los campos en la tabla para verificar que si existan
         * @columnas es la lista de columnas de la tabla
         * @campos es el nombre de los campos que ingreso el usuario
         * @mensajes output
         */
        private Boolean existColumn(LinkedList<Columna> columnas,LinkedList<string> campos,LinkedList<string> mensajes,Ambito ambito)
        {
            Mensaje mensa = new Mensaje();
            foreach(string s in campos)
            {
                Boolean flag = false;
                foreach(Columna columna in columnas)
                {
                    if (columna.name.Equals(s)) flag = true;
                }
                if (!flag)
                {
                    ambito.listadoExcepciones.AddLast(new Excepcion("columnexception", "La columna: " + s + " no existe en esta tabla " + id));
                    mensajes.AddLast(mensa.error("La columna: " + s + " no existe en esta tabla " + id, l, c, "Semantico"));
                    return false;
                }
            }
            return true;
        }
    }



   
}
