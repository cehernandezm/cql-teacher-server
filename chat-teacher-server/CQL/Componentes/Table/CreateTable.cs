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
    public class CreateTable : InstruccionCQL
    {
        string nombre { set; get; }
        LinkedList<Columna> lista { set; get; }

        LinkedList<string> primarias { set; get; }
        int l { set; get; }
        int c { set; get; }

        Boolean flag { set; get; }

        /*
         * Constructor de la clase cuando solo viene una primary key
         * @nombre nombre de la tabla
         * @lista es todas las columnas que almacenaran la tabla
         * @l linea del id
         * @c columna del id
         */
        public CreateTable(string nombre, LinkedList<Columna> lista, int l, int c,Boolean flag)
        {
            this.nombre = nombre;
            this.lista = lista;
            this.l = l;
            this.c = c;
            this.primarias = null;
            this.flag = flag;
        }
        /*
         * Constructor de la clase cuando vienen varias primary key;
         * @nombre nombre de la tabla
         * @lista es todas las columnas que almacenaran la tabla
         * @l linea del id
         * @c columna del id
         * @primarias es el listado de primarias
         */
        public CreateTable(string nombre, LinkedList<Columna> lista, LinkedList<string> primarias, int l, int c,Boolean flag)
        {
            this.nombre = nombre;
            this.lista = lista;
            this.primarias = primarias;
            this.l = l;
            this.c = c;
            this.flag = flag;
        }

        /*
       * Metodo de la implementacion
       * @ts tabla de simbolos global
       * @user usuario que ejecuta la accion
       * @baseD base de datos donde estamos ejecutando todo
       * @mensajes linkedlist con la salida deseada
       */
        public object ejecutar(TablaDeSimbolos ts, string user, ref string baseD, LinkedList<string> mensajes, TablaDeSimbolos tsT)
        {
            Mensaje mensa = new Mensaje();
            BaseDeDatos db = TablaBaseDeDatos.getBase(baseD);
            Usuario usuario = TablaBaseDeDatos.getUsuario(user);
            if (db != null)
            {
                if (user.Equals("admin"))
                {
                    
                }
                else
                {
                    if (usuario != null)
                    {
                        Boolean permiso = TablaBaseDeDatos.getPermiso(usuario, baseD);
                        if (permiso)
                        {
                            Boolean enUso = TablaBaseDeDatos.getEnUso(baseD, user);
                            if (!enUso)
                            {
                                //------------------------------------  SOLO HAY UNA PRIMARIA ------------------------------------------------------------
                                if (primarias == null)
                                {
                                    if (!(cantidadDePrimarias(lista) > 1))
                                    {
                                        if (!columnasRepetidas(lista, lista, mensajes))
                                        {
                                            Tabla old = TablaBaseDeDatos.getTabla(db, nombre);
                                            if (old == null)
                                            {
                                                if (searchTipo(lista, mensajes, db))
                                                {
                                                    Tabla nueva = new Tabla(nombre, lista, new LinkedList<Data>());
                                                    db.objetos.tablas.AddLast(nueva);
                                                    mensajes.AddLast(mensa.message("Se creo exitosamente la tabla: " + nombre));
                                                    return "";
                                                }
                                                
                                            }
                                            else
                                            {
                                                if (flag) return "";
                                                mensajes.AddLast(mensa.error("La tabla: " + nombre + " ya existe en la DB: " + baseD, l, c, "Semantico"));
                                            }
                                        }
                                    }
                                    else mensajes.AddLast(mensa.error("La tabla: " + nombre + " solo puede tener una clave primaria o use llaves compuestas", l, c, "Semantico"));
                                }
                                //-------------------------------------- LLAVE PRIMARIA COMPUESTA -------------------------------------------------------
                                else
                                {
                                    if (!(cantidadDePrimarias(lista) > 0))
                                    {
                                        if (!(columnasRepetidas(lista, lista, mensajes)))
                                        {
                                            if (!hayCounter(primarias, lista, mensajes))
                                            {
                                                if (existenColumnas(primarias, lista, mensajes))
                                                {
                                                    Tabla temp = TablaBaseDeDatos.getTabla(db, nombre);
                                                    if (temp == null)
                                                    {
                                                        if (searchTipo(lista, mensajes, db))
                                                        {
                                                            temp = new Tabla(nombre, lista, new LinkedList<Data>());
                                                            db.objetos.tablas.AddLast(temp);
                                                            mensajes.AddLast(mensa.message("Se creo exitosamente la tabla: " + nombre));
                                                            return "";
                                                        }
                                                        
                                                    }
                                                    else
                                                    {
                                                        if (flag) return "";
                                                        mensajes.AddLast(mensa.error("La tabla: " + nombre + " ya existe en la DB: " + baseD, l, c, "Semantico"));
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else mensajes.AddLast(mensa.error("La tabla: " + nombre + " solo puede tener primarias compuestas", l, c, "Semantico"));
                                }
                            }
                            else mensajes.AddLast(mensa.error("La base de datos: " + baseD + "esta siendo usada por otro usuario", l, c, "Semantico"));
                        }
                        else mensajes.AddLast(mensa.error("El usuario:  " + user + " no tiene permisos en la base de datos: " + baseD, l, c, "Semantico"));
                    }
                    else mensajes.AddLast(mensa.error("No existe el usuario: " + user, l, c, "Semantico"));
                }

                
            }
            else mensajes.AddLast(mensa.error("No se encuentra la base de datos: " + baseD + " o no se ha usado el comando USE", l, c, "Semantico"));
            return null;
        }



        /*
         * Verifica que si hay un tipo counter todas tiene que ser counter
         */

        public Boolean hayCounter(LinkedList<string> lista,LinkedList<Columna> lista2,LinkedList<string> mensajes)
        {
            Mensaje mensa = new Mensaje();
            Boolean flag = false;
            foreach(string s in lista)
            {
                Columna c = searchColumn(s, lista2);
                if (c != null) if (c.tipo.Equals("counter")) flag = true;
                
            }

            if (flag)
            {
                foreach (string s in lista)
                {
                    Columna cc = searchColumn(s, lista2);
                    if (cc != null)
                    {
                        if (!cc.tipo.Equals("counter"))
                        {
                            mensajes.AddLast(mensa.error("La columna: " + s + " no es de tipo counter, si hay un counter todas las primarias tienen que ser counter", l, c, "Semantico"));
                            return true;
                        }
                    } 

                }
            }

            return false;
        }

        public Boolean existenColumnas(LinkedList<string> lista, LinkedList<Columna> lista2, LinkedList<string> mensajes)
        {
            Mensaje mensa = new Mensaje();
            foreach (string s in lista)
            {
                Columna cc = searchColumn(s, lista2);
                if (cc == null)
                {
                    mensajes.AddLast(mensa.error("La columna: " + s + " no existe", l, c, "Semantico"));
                    return false;
                }
                else
                {
                    cc.pk = true;
                }

            }
            return true;
        }

        public Columna searchColumn(string nombre, LinkedList<Columna> lista)
        {
            foreach(Columna c in lista)
            {
                if (c.name.Equals(nombre)) return c;
            }
            return null;
        }

        /*
         * Cuenta cuantas claves primarias encuentra en la tabla
         * @lista es la lista de columnas
         */
        public int cantidadDePrimarias(LinkedList<Columna> lista)
        {
            int i = 0;
            foreach(Columna c in lista)
            {
                if (c.pk) i++;
            }
            return i;
        }
        
        /*
         * Busca columnas repetidas
         * @lista1 lista de columnas
         * @lista2 lista de columnas
         */

        public Boolean columnasRepetidas(LinkedList<Columna> lista1, LinkedList<Columna> lista2 , LinkedList<string> mensajes)
        {
            Mensaje mensa = new Mensaje();
            foreach(Columna cc in lista1)
            {
                int repitencia = 0;
                foreach(Columna ccc in lista2)
                {
                    if (cc.name.Equals(ccc.name)) repitencia++;
                    if (repitencia > 1)
                    {
                        mensajes.AddLast(mensa.error("La columna: " + cc.name + " se encuentra repetida",l,c,"Semantico"));
                        return true;
                    }
                }
            }
            return false;
        }

        /*
         * Metodo que se encargara de ver si existen los tipos de columnas
         * @lista1 nuevas columnas
         * @lista2 columnas ya existentes
         * @mensajes output de mensajes
         */
        public Boolean searchTipo(LinkedList<Columna> lista, LinkedList<string> mensajes, BaseDeDatos db)
        {
            Mensaje mensa = new Mensaje();
            foreach (Columna cc in lista)
            {
                if (cc.tipo.Equals("string") || cc.tipo.Equals("counter")  || cc.tipo.Equals("double") || cc.tipo.Equals("int") || cc.tipo.Equals("boolean") || cc.tipo.Equals("date") || cc.tipo.Equals("time")) { }
                else if (cc.tipo.Contains("map") || cc.tipo.Contains("list") || cc.tipo.Contains("set")) { }
                else
                {
                    Boolean existe = TablaBaseDeDatos.getUserType(cc.tipo, db);
                    if (!existe)
                    {
                        mensajes.AddLast(mensa.error("No existe este USER TYPE: " + cc.tipo, l, c, "Sematico"));
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
