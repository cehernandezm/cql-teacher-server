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

        public static LinkedList<Usuario> listaUsuario = new LinkedList<Usuario>();

        public static LinkedList<USO> listaEnUso = new LinkedList<USO>();

        

        public static BaseDeDatos getBase(string nombre)
        {
            foreach(BaseDeDatos db in global)
            {
                if (db.nombre.Equals(nombre)) return db;
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

        public static Usuario getUsuario( string nombre)
        {
            foreach(Usuario us in listaUsuario)
            {
                string user = us.nombre.ToString();
                user = user.TrimEnd();
                if (user.Equals(nombre)) return us;
            }
            return null;
        }

        public static Boolean getPermiso(Usuario user, string bdC)
        {
            foreach(string bd in user.bases)
            {
                if (bd.Equals(bdC)) return true;
            }
            return false;
        }

        public static Boolean  getEnUso (string nombre , string usuario)
        {
            foreach(USO o in listaEnUso)
            {
                System.Diagnostics.Debug.WriteLine("Usuario 1" + o.nombre + " Usuario2" + usuario);
                if (o.bd.Equals(nombre) && !(o.nombre.Equals(usuario))) return true;
            }
            return false;
        }

        public static string getMine(string usuario)
        {
            foreach(USO o in listaEnUso)
            {
                if (o.nombre.Equals(usuario)) return o.bd;
            }
            return "none";
        }

        public static void deleteMine(string usuario)
        {
            var node = listaEnUso.First;
            while(node != null)
            {
                var nextNode = node.Next;
                if (node.Value.nombre.Equals(usuario)) listaEnUso.Remove(node);
                node = nextNode;
            }
        }

        public static Boolean getUserType(string nombre, BaseDeDatos db)
        {
            Objeto o = db.objetos;
            foreach(User_Types a in o.user_types)
            {
                if (a.name.Equals(nombre)) return true;
            }
            return false;
        }


        public static Tabla getTabla(BaseDeDatos db, string nombre)
        {
            foreach(Tabla t in db.objetos.tablas)
            {
                if (t.nombre.Equals(nombre)) return t;
            }
            return null;
        }

        public static User_Types getUserTypeV(string nombre, BaseDeDatos db)
        {
            Objeto o = db.objetos;
            foreach (User_Types a in o.user_types)
            {
                if (a.name.Equals(nombre)) return a;
            }
            return null;
        }
    }
}
