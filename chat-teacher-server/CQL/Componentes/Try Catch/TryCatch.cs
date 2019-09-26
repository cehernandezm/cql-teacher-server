using cql_teacher_server.CQL.Arbol;
using cql_teacher_server.Herramientas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes.Try_Catch
{
    public class TryCatch : InstruccionCQL
    {
        LinkedList<InstruccionCQL> cuerpoTry { set; get; }
        LinkedList<InstruccionCQL> cuerpoCatch { set; get; }
        string id { set; get; }
        string tipo { set; get; }
        int l { set; get; }
        int c { set; get; }

        /*
         * CONSTRUCTOR DE LA CLASE
         * @param {cuerpoTry} operaciones que se ejecutaran en el try
         * @param {cuerpoCatch} operaciones que se ejecutaran en el catch
         * @param {id} variable del catch
         * @param {tipo} tipo de error a manejar
         * @param {l} linea del id
         * @param {c} columna del id
         */
        public TryCatch(LinkedList<InstruccionCQL> cuerpoTry, LinkedList<InstruccionCQL> cuerpoCatch, string id, string tipo, int l, int c)
        {
            this.cuerpoTry = cuerpoTry;
            this.cuerpoCatch = cuerpoCatch;
            this.id = id;
            this.tipo = tipo;
            this.l = l;
            this.c = c;
        }

        /*
         * METODO DE LA CLASE PADRE QUE SE IMPLEMENTA
         * @param {ts} tabla de simbolos del padre
         * @param {ambito} ambito de atributos a manejar
         * @param {tsT} tabla de simbolos(CQL) del padre
         */
        public object ejecutar(TablaDeSimbolos ts, Ambito ambito, TablaDeSimbolos tsT)
        {
            Mensaje ms = new Mensaje();
            TablaDeSimbolos newAmbito = new TablaDeSimbolos();
            Boolean flagCatch = false;
            foreach(Simbolo s in ts)
            {
                newAmbito.AddLast(s);
            }

            foreach(InstruccionCQL ins in cuerpoTry)
            {
                object res = ins.ejecutar(newAmbito, ambito, tsT);
                if (res == null)
                {
                    System.Diagnostics.Debug.WriteLine("TIPO: " + ins.GetType());
                    flagCatch = true;
                    break;
                }
                else if (ins.GetType() == typeof(Return)) return (Retorno)res;

            }

            if (flagCatch)
            {
                if(ambito.listadoExcepciones.Count() > 0)
                {
                    Excepcion e = (Excepcion)ambito.listadoExcepciones.Last.Value;
                    if (e.tipo.Equals(tipo) || e.tipo.Equals("exception"))
                    {
                        ambito.listadoExcepciones.RemoveLast();
                        ambito.mensajes.RemoveLast();                       
                        newAmbito = new TablaDeSimbolos();
                        foreach (Simbolo s in ts)
                        {
                            newAmbito.AddLast(s);
                        }
                        newAmbito.AddLast(new Simbolo("exception", id));
                        newAmbito.setValor(id, e);
                        foreach (InstruccionCQL ins in cuerpoCatch)
                        {
                            object res = ins.ejecutar(newAmbito, ambito, tsT);
                            if (res == null)
                            {
                                System.Diagnostics.Debug.WriteLine("TIPO2: " + ins.GetType());
                                return null;
                            }
                            else if (ins.GetType() == typeof(Return)) return (Retorno)res;
                        }
                        return "";
                    }
                    else ambito.mensajes.AddLast(ms.error("No coinciden el tipo de Excepcion en el catch con la Excepcion lanzada: " + tipo + " con: " +e.tipo,l,c,"Semantico"));

                }
            }
            else return "";

            return null;
        }
    }
}
