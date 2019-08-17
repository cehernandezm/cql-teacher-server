using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CHISON.Componentes
{
    public class Usuario
    {
        public LinkedList<Atributo> atributos { set; get; }

        public Usuario(LinkedList<Atributo> atributos)
        {
            this.atributos = atributos;
        }

        
    }
}
