using cql_teacher_server.CQL.Arbol;
using cql_teacher_server.CQL.Componentes.Try_Catch;
using cql_teacher_server.Herramientas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes
{
    public class RemoveMap : InstruccionCQL
    {
        Expresion id { set; get; }
        Expresion key { set; get; }
        int l { set; get; }
        int c { set; get; }

        /*
         * CONSTRUCTOR DE LA CLASE
         * @param {id} Expresion de tipo MAP
         * @param {key} valor a eliminar
         * @param {l} linea del id
         * @param {c} columna del id
         */
        public RemoveMap(Expresion id, Expresion key, int l, int c)
        {
            this.id = id;
            this.key = key;
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
        public object ejecutar(TablaDeSimbolos ts,Ambito ambito, TablaDeSimbolos tsT)
        {
            Mensaje ms = new Mensaje();
            object mp = (id == null) ? null : id.ejecutar(ts, ambito, tsT);
            object ky = (key == null) ? null : key.ejecutar(ts, ambito, tsT);
            if (mp != null)
            {
                if (ky != null)
                {
                    if (mp.GetType() == typeof(Map))
                    {
                        Map temp = (Map)mp;
                        if(temp.datos.Count() > 0)
                        {
                            var node = temp.datos.First;
                            while(node != null)
                            {
                                var next = node.Next;
                                if (((KeyValue)node.Value).key.Equals(ky))
                                {
                                    temp.datos.Remove(node);
                                    return "";
                                }
                                node = next;
                            }
                        }
                        else
                        {
                            ambito.mensajes.AddLast(ms.error("El map esta vacio, no se puede aplicar REMOVE" , l, c, "Semantico"));
                            return null;
                        }
                        ambito.listadoExcepciones.AddLast(new Excepcion("indexoutexception", "No se encontro la key: " + ky));
                        ambito.mensajes.AddLast(ms.error("No se encontro la key: " + ky, l, c, "Semantico"));
                    }
                    else if(mp.GetType() == typeof(List))
                    {
                        if (ky.GetType() == typeof(int))
                        {
                            List temp = (List)mp;
                            int index = (int)ky;
                            if (index > -1)
                            {
                                if (index < temp.lista.Count())
                                {
                                    int i = 0;
                                    if (temp.lista.Count() > 0)
                                    {
                                        var node = temp.lista.First;
                                        while (node != null)
                                        {
                                            var nodeNext = node.Next;
                                            if (i == index)
                                            {
                                                temp.lista.Remove(node);
                                                return "";
                                            }
                                            i++;
                                            node = nodeNext;
                                        }
                                    }
                                }
                                else
                                {
                                    ambito.listadoExcepciones.AddLast(new Excepcion("indexoutexception", "El index es mayor al tamaño de la lista"));
                                    ambito.mensajes.AddLast(ms.error("El index supera el tamaño de la lista", l, c, "Semantico"));
                                }
                            }
                            else
                            {
                                ambito.listadoExcepciones.AddLast(new Excepcion("indexoutexception", "Index tiene que ser mayor a 0 : " + index));
                                ambito.mensajes.AddLast(ms.error("El index debe ser mayor a -1", l, c, "Semantico"));
                            }
                        }
                        else ambito.mensajes.AddLast(ms.error("El index debe de ser de tipo int: " + ky,l,c,"Semantico"));
                    }
                    else if (mp.GetType() == typeof(Set))
                    {
                        if (ky.GetType() == typeof(int))
                        {
                            Set temp = (Set)mp;
                            int index = (int)ky;
                            if (index > -1)
                            {
                                if (index < temp.datos.Count())
                                {
                                    int i = 0;
                                    if (temp.datos.Count() > 0)
                                    {
                                        var node = temp.datos.First;
                                        while (node != null)
                                        {
                                            var nodeNext = node.Next;
                                            if (i == index)
                                            {
                                                temp.datos.Remove(node);
                                                return "";
                                            }
                                            i++;
                                            node = nodeNext;
                                        }
                                    }
                                }
                                else
                                {
                                    ambito.listadoExcepciones.AddLast(new Excepcion("indexoutexception", "El index es mayor al tamaño de la lista"));
                                    ambito.mensajes.AddLast(ms.error("El index supera el tamaño de la lista", l, c, "Semantico"));
                                }
                            }
                            else
                            {
                                ambito.listadoExcepciones.AddLast(new Excepcion("indexoutexception", "Index tiene que ser mayor a 0 : " + index));
                                ambito.mensajes.AddLast(ms.error("El index debe ser mayor a -1", l, c, "Semantico"));
                            }
                        }
                        else ambito.mensajes.AddLast(ms.error("El index debe de ser de tipo int: " + ky, l, c, "Semantico"));
                    }
                    else ambito.mensajes.AddLast(ms.error("No se puede aplicar un REMOVE en un tipo no Collection, no se reconoce: " + mp, l, c, "Semantico"));
                }
                else ambito.mensajes.AddLast(ms.error("La key no puede ser null", l, c, "Semantico"));
            }
            else ambito.mensajes.AddLast(ms.error("No se puede REMOVER en un null", l, c, "Semantico"));

            return null;
        }
    }
}
