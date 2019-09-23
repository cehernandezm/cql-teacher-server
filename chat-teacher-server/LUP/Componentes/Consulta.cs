using cql_teacher_server.CQL.Gramatica;
using cql_teacher_server.LUP.Arbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.LUP.Componentes
{
    class Consulta : InstruccionLUP
    {
        private string usuario;
        private string codigo;

        public Consulta(string usuario, string codigo)
        {
            this.usuario = usuario;
            this.codigo = codigo;
        }

        public object ejecutar()
        {
            SintacticoCQL sintactio = new SintacticoCQL();

            string salida = sintactio.analizar(codigo, usuario).ToString();
            return salida;
        }
    }
}
