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

        public Procedures(string nombre, LinkedList<Parametros> parametros, string instruccion)
        {
            this.nombre = nombre;
            this.parametros = parametros;
            this.instruccion = instruccion;
        }
    }
}
