using cql_teacher_server.CQL.Arbol;
using cql_teacher_server.Herramientas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes
{
    public class Funcion : InstruccionCQL
    {
        string tipo { set; get; }
        public string id { set; get; }
        public string identificador { set; get; }
        public LinkedList<InstruccionCQL> parametros { set; get; }
        public LinkedList<Expresion> valores { set; get; }
        LinkedList<InstruccionCQL> cuerpo { set; get; }
        public int l { set; get; }
        public int c { set; get; }

        /*
         * Constructor de la clase
         * @param {id} nombre de la funcion
         * @param {parametros} parametros de la funcion
         * @param {cuerpo} instrucciones a ejecutar
         * @param {l} linea del id
         * @param {c} columnad el id
         */
        public Funcion(string tipo,string id, LinkedList<InstruccionCQL> parametros, LinkedList<InstruccionCQL> cuerpo, int l, int c)
        {
            this.tipo = tipo;
            this.id = id;
            this.parametros = parametros;
            this.cuerpo = cuerpo;
            this.l = l;
            this.c = c;
            generarId();
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
            Mensaje ms = new Mensaje();
            TablaDeSimbolos newAmbito = new TablaDeSimbolos();
            foreach(Simbolo s in ts)
            {
                newAmbito.AddLast(s);
            }

            if (parametros.Count() == valores.Count())
            {
                for( int i = 0; i < parametros.Count(); i++)
                {
                    Declaracion d =(Declaracion)parametros.ElementAt(i);
                    d.parametro = true;
                    d.ejecutar(newAmbito, user, ref baseD, mensajes, tsT);
                    if (d == null) return null;
                    Asignacion a = new Asignacion(d.id, l, c, valores.ElementAt(i), "ASIGNACION");
                    a.tPadre = ts;
                    a.ejecutar(newAmbito, user, ref baseD, mensajes, tsT);
                    if (a == null) return null;
                    
                }

                foreach(InstruccionCQL ins in cuerpo)
                {
                    object r = ins.ejecutar(newAmbito, user, ref baseD, mensajes, tsT);
                    if (r == null) return null;
                    else if (r.GetType() == typeof(Retorno)) return ((Retorno)r).valor;
                }
                mensajes.AddLast(ms.error("La funcion no posee ningun return", l, c, "Semantico"));
                return null;
            }
            else mensajes.AddLast(ms.error("No coinciden el numero de parametros con el numero de valores", l, c, "Semantico"));

            return null;
        }


        /*
       * METODO QUE GENERA UN IDENTIFICADOR UNICO
       */

        public void generarId()
        {
            string identi = id;
            foreach (Declaracion d in parametros)
            {
                identi += "_" + d.tipo;
            }
            identificador = identi;
        }

    }
}
