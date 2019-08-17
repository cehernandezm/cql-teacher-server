using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CHISON.Componentes
{
    public class Parametros
    {
        public string nombre { set; get; }
        public string tipo { set; get; }
        public string ass { set; get; }

        public Parametros(string nombre, string tipo, string ass)
        {
            this.nombre = nombre;
            this.tipo = tipo;
            this.ass = ass;
        }
    }
}
