using cql_teacher_server.CHISON;
using cql_teacher_server.CQL.Arbol;
using cql_teacher_server.CQL.Componentes.Procedure;
using cql_teacher_server.Herramientas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes.Funcion_Procedure
{
    public class CallProcedure : InstruccionCQL
    {
        LinkedList<listaParametros> parametros { set; get; }
        LinkedList<InstruccionCQL> cuerpo { set; get; }
        public LinkedList<Expresion> valores { set; get; }
        int l { set; get; }
        int c { set; get; }

        /*
        * CONSTRUCTOR DE LA CLASE
        * @param {id} nombre del procedimiento
        * @param {parametro} lista de parametros
        * @param {cuerpo} instrucciones a ejecutar
        * @param {l} linea del id
        * @param {c} columna del id
        * @param {codigo} codigo original
        */
        public CallProcedure(LinkedList<listaParametros> parametros, LinkedList<InstruccionCQL> cuerpo, int l, int c)
        {
            this.parametros = parametros;
            this.cuerpo = cuerpo;
            this.l = l;
            this.c = c;
        }
        /*
        * Metodo de la implementacion
        * @ts tabla de simbolos global
        * @user usuario que ejecuta la accion
        * @baseD base de datos donde estamos ejecutando todo
        * @mensajes linkedlist con la salida deseada
        */
        public object ejecutar(TablaDeSimbolos ts,Ambito ambito, TablaDeSimbolos tsT)
        {
            Mensaje ms = new Mensaje();
            TablaDeSimbolos newAmbito = new TablaDeSimbolos();
            foreach (Simbolo s in ambito.tablaPadre)
            {
                newAmbito.AddLast(s);
            }

            if (tamanioTotalParametros() == valores.Count())
            {
                int index = 0;
                //-------------------------------------------- CREACION Y ASIGNACION DE PARAMETROS --------------------------------------------------------------
                for (int i = 0; i < parametros.Count(); i++)
                {
                    LinkedList<InstruccionCQL> parametro =parametros.ElementAt(i).lista;

                    for (int j = 0; j < parametro.Count(); j++)
                    {
                        Declaracion d = (Declaracion)parametro.ElementAt(j);
                        d.parametro = true;
                        object rd = d.ejecutar(newAmbito,ambito, tsT);
                        if (rd == null) return null;
                        Asignacion a = new Asignacion(d.id, l, c, valores.ElementAt(index), "ASIGNACION");
                        a.tPadre = ts;
                        object ra = a.ejecutar(newAmbito, ambito, tsT);
                        if (ra == null) return null;
                        index++;
                    }

                }
                //--------------------------------------------------- INSTRUCCIONES DEL PROCEDURE ----------------------------------------------
                foreach (InstruccionCQL ins in cuerpo)
                {
                    object r = ins.ejecutar(newAmbito, ambito, tsT);
                    if (r == null) return null;
                    else if (r.GetType() == typeof(Retorno))
                    {
                        object re = ((Retorno)r).valor;
                        if (re != null)
                        {
                            if (re.GetType() == typeof(LinkedList<object>))
                            {
                                LinkedList<object> temp = (LinkedList<object>)re;
                                if (temp.Count() == parametros.Count()) return temp;
                                else
                                {
                                    ambito.mensajes.AddLast(ms.error("La cantidad de valores retornados no concuerda con el valor de parametros", l, c, "Semantico"));
                                    return null;
                                }
                            }
                            else
                            {
                                LinkedList<object> temp = new LinkedList<object>();
                                temp.AddLast(re);
                                if (parametros.Count() == 1) return temp;
                                else
                                {
                                    ambito.mensajes.AddLast(ms.error("La cantidad de valores retornados no concuerda con el valor de parametros", l, c, "Semantico"));
                                    return null;
                                }
                            }
                        }
                        return null;
                    }
                }
            }
            else ambito.mensajes.AddLast(ms.error("La cantidad de valores no concuerda con la cantidad de parametros", l, c, "Semantico"));

            return null;
        }

        int tamanioTotalParametros()
        {
            int index = 0;
            foreach(listaParametros lista in parametros)
            {
                index += lista.lista.Count();
            }
            return index;
        }
    }
}
