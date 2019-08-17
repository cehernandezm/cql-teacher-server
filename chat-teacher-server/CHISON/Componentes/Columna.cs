using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CHISON.Componentes
{
    public class Columna
    {

        public string name { set; get; }
        public string tipo { set; get; }
        public Boolean pk { set; get; }

        public LinkedList<Atributo> atributos { set; get; }

        public Columna(LinkedList<Atributo> atributos)
        {
            this.atributos = atributos;
        }

        public Columna(string name, string tipo, bool pk)
        {
            this.name = name;
            this.tipo = tipo;
            this.pk = pk;
        }


    }
}
