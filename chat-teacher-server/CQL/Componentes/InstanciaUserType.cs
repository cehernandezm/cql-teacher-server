using cql_teacher_server.CHISON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes
{
    public class InstanciaUserType
    {
        public string tipo { set; get; }
        public LinkedList<Atributo> lista { set; get; }

        /*
         * Constructo de la clase que creara la instancia
         * @tipo es el tipo de userType a instanciar
         * @lista es la lista de atributos a guardar
         */
        public InstanciaUserType(string tipo, LinkedList<Atributo> lista)
        {
            this.tipo = tipo;
            this.lista = lista;
        }
    }
}
