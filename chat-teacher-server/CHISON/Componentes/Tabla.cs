using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CHISON.Componentes
{
    public class Tabla
    {
        public string nombre { set; get; }
        public LinkedList<Columna> columnas{set;get;}

        public LinkedList<Atributo> atributos { set; get; }

        public Tabla(LinkedList<Atributo> atributo)
        {
            this.atributos = atributo;
        }

        public Tabla(string nombre, LinkedList<Columna> columnas)
        {
            this.nombre = nombre;
            this.columnas = columnas;
        }

    }
}
