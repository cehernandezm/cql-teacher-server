using cql_teacher_server.CQL.Componentes.Funcion_Procedure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CHISON.Componentes
{
    public class Objeto
    {
       public LinkedList<Tabla> tablas { set; get; }
       public LinkedList<User_Types> user_types { set; get; }
        
        public LinkedList<Procedures> procedures { set; get; }
        public Objeto()
        {
            tablas = new LinkedList<Tabla>();
            user_types = new LinkedList<User_Types>();
            procedures = new LinkedList<Procedures>();
        }
    }
}
