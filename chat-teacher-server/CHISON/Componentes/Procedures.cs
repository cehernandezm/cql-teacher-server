using cql_teacher_server.CQL.Arbol;
using cql_teacher_server.CQL.Componentes.Procedure;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CHISON.Componentes
{
    public class Procedures
    {
        public string nombre { set; get; }
        public LinkedList<Parametros> parametros { set; get; }
        public string instruccion { set; get; }

        public string identificador { set; get; }
        public string identificadorOut { set; get; }
        public LinkedList<listaParametros> parametro { set; get; }
        public LinkedList<listaParametros> retornos { set; get; }
        public LinkedList<InstruccionCQL> cuerpo { set; get; }
       

        public Procedures(string nombre, string instruccion, string identificador, LinkedList<listaParametros> parametro,string identificadorOut, LinkedList<listaParametros> retornos, LinkedList<InstruccionCQL> cuerpo)
        {
            this.nombre = nombre;
            this.instruccion = instruccion;
            this.identificador = identificador;
            this.identificadorOut = identificadorOut;
            this.retornos = retornos;
            this.parametro = parametro;
            this.cuerpo = cuerpo;
        }
    }
}
