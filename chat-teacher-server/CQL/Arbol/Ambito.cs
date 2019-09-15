using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Arbol
{
    public class Ambito
    {
        public TablaDeSimbolos tablaPadre { set; get; }
        public LinkedList<string> mensajes { set; get; }
        public string usuario { set; get; }
        public string baseD { set; get; }

        /*
         * CONSTRUCTOR DE LA CLASE
         * @param {tablaPadre} tabla de valores GLOBAL
         * @param {mensajes} output
         * @param {usuario} usuario que ejecuta todos las acciones
         * @param {baseD} bases de datos actual
         */
        public Ambito(TablaDeSimbolos tablaPadre, LinkedList<string> mensajes, string usuario, string baseD)
        {
            this.tablaPadre = tablaPadre;
            this.mensajes = mensajes;
            this.usuario = usuario;
            this.baseD = baseD;
        }
    }
}
