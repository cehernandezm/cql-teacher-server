using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CHISON.Componentes
{
    public class BaseDeDatos 
    {
        public string nombre { set; get; }
        public LinkedList<Tabla> listaTablas { set; get; }

        public BaseDeDatos(string nombre, LinkedList<Tabla> listaTablas)
        {
            this.nombre = nombre;
            this.listaTablas = listaTablas;
        }

        public LinkedList<Atributo> atributos { get; set; }
        public string usuarioActual { get; set; }

        public BaseDeDatos(LinkedList<Atributo> atributos, string usuarioActual)
        {
            this.atributos = atributos;
            this.usuarioActual = usuarioActual;
        }

        public Atributo getAtributo(string name)
        {
            foreach(Atributo s in atributos)
            {
                if (s.nombre.Equals(name)) return s;
            }
            return null;
        }

        
    }
}
