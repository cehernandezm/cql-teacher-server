using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Arbol
{
    public class TablaDeSimbolos : LinkedList<Simbolo>
    {
        /*
         * Constructor de la clase. Llama al constructor de la clase Padre (LinkedList)
         */
        public TablaDeSimbolos() : base()
        {
        }


        /*
         * Metodo que busca una variable y devuelve su valor. De no encontrarla devuelve un valor arbitrario
         * @id es el identificador de la variable a buscar
         */

        public object getValor(string id)
        {
            foreach(Simbolo s in this)
            {
                if (s.nombre.Equals(id)) return s.valor;
            }
            return "none";
        }


        /*
         * Metodo que asigna un valor a una variable en especifico. 
         * @id identificador de la variable
         * @valor el nuevo valor de la variable
         * @return True si asigna correctamente False si no la encuentra
         */

        public Boolean setValor(string id, object valor)
        {
            foreach(Simbolo s in this)
            {
                if (s.nombre.Equals(id))
                {
                    s.valor = valor;
                    return true;
                }
            }
            return false;
        }

        /*
         * Metodo que devuelve el tipo de una variable. 
         * @id identificador de la variable
         * @return "none" si no se encuentra
         */

        public string getTipo(string id)
        {
            foreach(Simbolo s in this)
            {
                if (s.nombre.Equals(id)) return s.Tipo;
            }
            return "none";
        }



    }
}
