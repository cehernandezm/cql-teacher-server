using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CHISON
{
    public class Atributo
    {
        public string nombre { get; set; }
        public Object valor { get; set; }
        public string tipo { get; set; }

        public Atributo(string nombre, object valor, string tipo)
        {
            this.nombre = nombre;
            this.valor = valor;
            this.tipo = tipo;
        }
    }
}
