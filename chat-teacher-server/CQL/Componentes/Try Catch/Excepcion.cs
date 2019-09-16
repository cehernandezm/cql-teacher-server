using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes.Try_Catch
{
    public class Excepcion
    {
        public string tipo { set; get; }
        public string detalle { set; get; }

        /*
         * Constructor de la clase
         * @param {tipo} tipo de error
         * @param {detalle} detalle del error
         */
        public Excepcion(string tipo, string detalle)
        {
            this.tipo = tipo;
            this.detalle = detalle;
        }
    }
}
