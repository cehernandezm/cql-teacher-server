using cql_teacher_server.CHISON.Componentes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CHISON
{
   
    class TablaBaseDeDatos
    {
        public static LinkedList<BaseDeDatos> global = new LinkedList<BaseDeDatos>();

        public static LinkedList<BaseDeDatos> getTabla()
        {
            return global;
        }

        public static BaseDeDatos getBase(string nombre)
        {
            foreach(BaseDeDatos db in global)
            {
                foreach(Atributo a in db.atributos)
                {
                    if (a.nombre.Equals("NAME") && a.valor.Equals(nombre)) return db; 
                }
            }
            return null;
        }

        public static Tabla getTabla(string nombre, BaseDeDatos db)
        {
            foreach(Atributo a in db.atributos)
            {
                if (a.nombre.Equals("DATA"))
                {
                    foreach(Tabla tb in (LinkedList<Tabla>)a.valor)
                    {
                       foreach(Atributo at in tb.atributos)
                       {
                            if (at.nombre.Equals("NAME") && at.valor.Equals(nombre)) return tb;
                       }
                    }
                }
             }
            
            return null;
        }
        
        
    }
}
