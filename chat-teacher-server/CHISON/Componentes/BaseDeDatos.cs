using cql_teacher_server.CQL.Componentes.Funcion_Procedure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CHISON.Componentes
{
    public class BaseDeDatos 
    {
        public string nombre { set; get; }
        public Objeto objetos { set; get; }

        public BaseDeDatos(string nombre, Objeto objetos)
        {
            this.nombre = nombre;
            this.objetos = objetos;
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

        public Procedures buscarProcedure(string identificador)
        {
            LinkedList<Procedures> lista = objetos.procedures;
            foreach(Procedures p in lista)
            {
                if (identificador.Equals(p.identificador)) return p;
            }
            return null;
        }

        
    }
}
