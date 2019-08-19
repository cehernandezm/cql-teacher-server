using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Arbol
{
    public class Simbolo
    {
        public string Tipo { set; get; }
        public string nombre { set; get; }

        public object valor { set; get; }

        /*
         * Constructor de la clase simbolo (Solo se declara)
         * @tipo es el tipo de variable a almacenar
         * @nombre es el identificador para variable
         */
        public Simbolo(string tipo, string nombre)
        {
            Tipo = tipo;
            this.nombre = nombre;
        }

        /*
        * Constructor de la clase simbolo (Se declara y se asigna un valor)
        * @tipo es el tipo de variable a almacenar
        * @nombre es el identificador para variable
        */
        public Simbolo(string tipo, string nombre, object valor) : this(tipo, nombre)
        {
            this.valor = valor;
        }
    }
}
