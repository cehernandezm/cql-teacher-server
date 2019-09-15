using cql_teacher_server.CHISON;
using cql_teacher_server.CHISON.Componentes;
using cql_teacher_server.CQL.Arbol;
using cql_teacher_server.CQL.Componentes.Cursor;
using cql_teacher_server.Herramientas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes.Ciclos
{
    public class ForEach : InstruccionCQL
    {
        string id { set; get; }
        string identificador { set; get; }
        LinkedList<InstruccionCQL> parametros { set; get; }
        LinkedList<InstruccionCQL> cuerpo { set; get; }
        int l { set; get; }
        int c { set; get; }

        /*
         * CONSTRUCTOR DE LA CLASE
         * @param {id} variable tipo cursor
         * @param {parametros} lista a declarar
         * @param {cuerpo} lista de valores a ejecutar
         * @param {l} linea del id
         * @param {c} columna del id
         */
        public ForEach(string id, LinkedList<InstruccionCQL> parametros, LinkedList<InstruccionCQL> cuerpo, int l, int c)
        {
            this.id = id;
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
        public object ejecutar(TablaDeSimbolos ts, Ambito ambito, TablaDeSimbolos tsT)
        {
            Mensaje ms = new Mensaje();
            object res = ts.getValor(id);
            if (!res.Equals("none"))
            {
                if (res.GetType() == typeof(TypeCursor))
                {
                    TypeCursor tabla = (TypeCursor)res;
                    if(tabla.tabla != null)
                    {
                        generarIdentificador(tabla.tabla);
                        if (tabla.tabla.columnas.Count() == parametros.Count())
                        {
                            string identificadorParametros = generarIdentificadorDeclaracion();
                            if (identificadorParametros.Equals(identificador))
                            {
                                

                                foreach(Data data in tabla.tabla.datos)
                                {
                                    TablaDeSimbolos newAmbito = new TablaDeSimbolos();
                                    foreach (Simbolo s in TablaBaseDeDatos.tablaGeneral)
                                    {
                                        newAmbito.AddLast(s);
                                    }
                                    //-------------------------------------- ASIGNARLE A LOS PARAMETROS LOS VALORES DE LA CONSULTA ----------------------------
                                    for (int i = 0; i < parametros.Count(); i++)
                                    {
                                        Declaracion d = (Declaracion)parametros.ElementAt(i);
                                        d.parametro = true;
                                        object rd = d.ejecutar(newAmbito, ambito, tsT);
                                        if (rd == null) return null;
                                        Atributo atributo = data.valores.ElementAt(i);
                                        newAmbito.setValor(d.id, atributo.valor);
                                    }
                                    //---------------------------------------- EJECUTAR INSTRUCCIONES DENTRO DEL FOREACH -----------------------------------------
                                    foreach(InstruccionCQL i in cuerpo)
                                    {
                                        object resultado = i.ejecutar(newAmbito,ambito, tsT);
                                        if (resultado == null) return null;
                                        else if (resultado.GetType() == typeof(Retorno)) return ((Retorno)resultado);
                                        else if (i.GetType() == typeof(Continue) || resultado.GetType() == typeof(Continue)) break;
                                    }
                                }
                                return "";
                            }
                            else ambito.mensajes.AddLast(ms.error("No coinciden el tipo de parametros con el tipo de columnas",l,c,"Semantico"));
                        }
                        else ambito.mensajes.AddLast(ms.error("No coincide la cantidad de parametros con la cantidad de columnas", l, c, "Semantico"));
                    }
                    else ambito.mensajes.AddLast(ms.error("El cursor: " + id + " no ha sido abierto",l,c,"Semantico"));
                }
                else ambito.mensajes.AddLast(ms.error("La variable tiene que ser de tipo Cursor  no se reconoce: " +res,l,c,"Semantico"));
            }
            else ambito.mensajes.AddLast(ms.error("La variable : " + id + " no existe en este ambito",l,c,"Semantico"));
            return null;
        }

        /*
         * METODOQ QUE GENERA UN IDENTIFICADOR CON LAS COLUMNAS DE LA CONSULTA
         * @param {tabla} tabla que se obtiene del select
         */
        private void generarIdentificador(TablaSelect tabla)
        {
            string identi = "";
            foreach (Columna c in tabla.columnas)
            {
                if (c.tipo.Contains("map")) identi += "_map";
                else if (c.tipo.Contains("list")) identi += "_list";
                else if (c.tipo.Contains("set")) identi += "_set";
                else if (c.tipo.Equals("counter")) identi += "_int";
                else identi += "_" + c.tipo;
            }
            identificador = identi;
        }

        /*
         * METODO QUE GENERA UN IDENTIFICADOR CON LA DECLARACION DE PARAMETROS
         * @return string con identificador
         */

        private string generarIdentificadorDeclaracion()
        {
            string result = "";
            foreach(InstruccionCQL i in parametros)
            {
                Declaracion d = (Declaracion)i;
                result += "_" + d.tipo;
            }
            return result;
        }
    }
}
