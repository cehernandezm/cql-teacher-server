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
    public class AlterTable : InstruccionCQL
    {
        string id { set; get; }
        LinkedList<Columna> listaAdd { set; get; }
        LinkedList<string> listaDrop { set; get; }

        int l { set; get; }

        int c { set; get; }

        string operacion { set; get; }


        /*
         * Constructor para agregar Columnas
         * @id tabla donde se realizara todo
         * @listaAdd lista de columnas a Agregar
         * @l linea del id
         * @c columna del id
         * @operacion tipo de operacion(ADD)
         */
        public AlterTable(string id, LinkedList<Columna> listaAdd, int l, int c, string operacion)
        {
            this.listaAdd = listaAdd;
            this.l = l;
            this.c = c;
            this.operacion = operacion;
            this.id = id;
        }
        /*
         * Constructor para Eliminar Columnas
         * @listaDrop lista de columnas a elininar
         * @l linea del id
         * @c columna del id
         * @operacion tipo de operacion(DROP)
         */
        public AlterTable(string id, LinkedList<string> listaDrop, int l, int c, string operacion)
        {
            this.id = id;
            this.listaDrop = listaDrop;
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

        public object ejecutar(TablaDeSimbolos ts, Ambito ambito, TablaDeSimbolos tsT)
        {
            Mensaje mensa = new Mensaje();
            BaseDeDatos db = TablaBaseDeDatos.getBase(ambito.baseD);

            if (db != null)
            {
                Tabla tabla = TablaBaseDeDatos.getTabla(db, id);
                if(tabla != null)
                {
                    if (ambito.usuario.Equals("admin"))
                    {
                      
                    }
                    else
                    {
                        Usuario usuario = TablaBaseDeDatos.getUsuario(ambito.usuario);
                        if (usuario != null)
                        {
                            Boolean permiso = TablaBaseDeDatos.getPermiso(usuario, ambito.baseD);
                            if (permiso)
                            {
                                Boolean enUso = TablaBaseDeDatos.getEnUso(ambito.baseD, ambito.usuario);
                                if (!enUso)
                                {
                                    if (operacion.Equals("ADD"))
                                    {
                                        if (!searchColumns(listaAdd, tabla.columnas, ambito.mensajes))
                                        {
                                            if (searchTipo(listaAdd, ambito.mensajes, db))
                                            {
                                                foreach (Columna cc in listaAdd)
                                                {
                                                    tabla.columnas.AddLast(cc);
                                                    agregarColumna(tabla.datos, cc);
                                                    ambito.mensajes.AddLast(mensa.message("Se agrego exitosamente la columna: " + cc.name));
                                                }
                                                return "";
                                            }

                                        }
                                    }
                                    else
                                    {
                                        if (existeColumna(listaDrop, tabla.columnas,ambito.mensajes))
                                        {
                                            if (!isPK(listaDrop, tabla.columnas, ambito.mensajes))
                                            {
                                                foreach (string s in listaDrop)
                                                {
                                                    Columna columna = getColumna(s, tabla.columnas);
                                                    int index = indexColumna(s, tabla.columnas);
                                                    deleteData(index, tabla.datos);
                                                    tabla.columnas.Remove(columna);
                                                    ambito.mensajes.AddLast(mensa.message("Columna: " + s + " ha sido eliminada con existo"));
                                                }
                                                return "";
                                            }

                                        }
                                    }
                                }
                                else ambito.mensajes.AddLast(mensa.error("Otro usuario esta usando la DB:  " + ambito.baseD, l, c, "Semantico"));

                            }
                            else ambito.mensajes.AddLast(mensa.error("El usuario: " + ambito.usuario + " no tiene permiso en la DB: " + ambito.baseD, l, c, "Semantico"));
                        }
                        else ambito.mensajes.AddLast(mensa.error("No existe el usuario: " + ambito.usuario, l, c, "Semantico"));
                    }
                    
                }
                else ambito.mensajes.AddLast(mensa.error("La tabla: " + id + " no existe en la DB: " + ambito.baseD, l, c, "Semantico"));
                
            }
            else ambito.mensajes.AddLast(mensa.error("No existe la base de datos: " + ambito.baseD + " o no se ha usado el comando use", l, c, "Semantico"));

            
            return null;
        }



        /*
         * Metodo que se encargara de ver si no hay columnas repetidas
         * @lista1 nuevas columnas
         * @lista2 columnas ya existentes
         * @mensajes output de mensajes
         */
        public Boolean searchColumns(LinkedList<Columna> lista1,LinkedList<Columna> lista2, LinkedList<string> mensajes)
        {
            foreach(Columna cc in lista1)
            {
                foreach(Columna ccc in lista2)
                {
                    if (cc.name.Equals(ccc.name))
                    {
                        Mensaje mensa = new Mensaje();
                        mensajes.AddLast(mensa.error("La columna " + cc.name + " ya se encuentra en esta Tabla: " + id, l, c, "Semantico"));
                        return true;
                    }
                }
            }
            return false;
        }

        /*
         * Metodo que se encargara agregar la data a la lista de data
         * @lista1 data de informacion
         * @cc inforamcion de la columna
         */
        public void agregarColumna(LinkedList<Data> lista1, Columna cc)
        {
            foreach(Data d in lista1)
            {
                if (cc.tipo.Equals("int") || cc.tipo.Equals("double")) d.valores.AddLast(new Atributo(cc.name, 0, cc.tipo));
                else if (cc.tipo.Contains("map")) d.valores.AddLast(new Atributo(cc.name, new Map(cc.tipo, new LinkedList<KeyValue>()), "map"));
                else if (cc.tipo.Contains("set")) d.valores.AddLast(new Atributo(cc.name, new Set(cc.tipo, new LinkedList<object>()), "set"));
                else if (cc.tipo.Contains("list")) d.valores.AddLast(new Atributo(cc.name, new List(cc.tipo, new LinkedList<object>()), "list"));
                else if (cc.tipo.Equals("string") || cc.tipo.Equals("date") || cc.tipo.Equals("time")) d.valores.AddLast(new Atributo(cc.name, null, cc.tipo));
                else d.valores.AddLast(new Atributo(cc.name, new InstanciaUserType(cc.tipo, null), cc.tipo));
            }
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
            foreach(Columna cc in lista)
            {
                if(cc.tipo.Equals("string") || cc.tipo.Equals("double") || cc.tipo.Equals("int") || cc.tipo.Equals("boolean") || cc.tipo.Equals("date") || cc.tipo.Equals("time")) { }
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

        /*
         * Metodo que se encargara de ver si existen las columnas
         * @lista1 strings de lista a eliminar
         * @lista2 columnas ya existentes
         * @mensajes output de mensajes
         */
        public Boolean existeColumna(LinkedList<string> lista, LinkedList<Columna> lista2, LinkedList<string> mensajes)
        {
            Mensaje mensa = new Mensaje();
            Boolean flag;

            foreach (string s in lista)
            {
                flag = false;
                foreach (Columna cc in lista2)
                {
                    if (cc.name.Equals(s)) flag = true;
                }
                if (!flag)
                {
                    mensajes.AddLast(mensa.error("No existe la columna: " + s + " en la tabla: " + id, l, c, "Semantico"));
                    return false;
                }
            }

            return true;
        }

        /*
        * Metodo que se encargara de devolver el index de una columna
        * @nombre columna a buscar
        * @lista columnas ya existentes
        */
        public int indexColumna(string nombre, LinkedList<Columna>lista)
        {
            int index = 0;
            foreach(Columna cc in lista)
            {
                if (cc.name.Equals(nombre)) return index;
                index++;
            }
            return -1;
        }

        /*
        * Metodo que se encargara de devolver el index de una columna
        * @nombre columna a buscar
        * @lista columnas ya existentes
        */
        public Columna getColumna(string nombre, LinkedList<Columna> lista)
        {
            foreach (Columna cc in lista)
            {
                if (cc.name.Equals(nombre)) return cc;
            }
            return null;
        }

        /*
         * Metodo que eliminara la data de la informacion
         * @index que posicion ocupa la columna
         * @lista lista de informacion
         */
        public void deleteData(int index, LinkedList<Data> lista)
        {
            foreach (Data d in lista)
            {
                int i = 0;
                var node = d.valores.First;
                while(node != null)
                {
                    var nextNode = node.Next;
                    if (i == index) d.valores.Remove(node);
                    i++;
                    node = nextNode;
                }
            }
        }

        /*
         * Metodo que vera si es primari key  
         * @nombre nombre de la columna
         * @lista lista de columnas ya existentes
         */
         public Boolean isPK(LinkedList<string> lista, LinkedList<Columna> lista2, LinkedList<string> mensajes)
        {
            Mensaje mensa = new Mensaje();
            foreach(string s in lista)
            {
               foreach(Columna cc in lista2)
                {
                    if (cc.name.Equals(s))
                    {
                        if (cc.pk)
                        {
                            mensajes.AddLast(mensa.error("La columna: " + s + " es una primary key, no se puede eliminar", l, c, "Semantico"));
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
