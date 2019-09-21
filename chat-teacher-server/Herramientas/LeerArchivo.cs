using cql_teacher_server.CHISON;
using cql_teacher_server.CHISON.Componentes;
using cql_teacher_server.CHISON.Gramatica;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.Herramientas
{
    public class LeerArchivo
    {
        string direccion { set; get; }
        public LeerArchivo(string direccion)
        {
            this.direccion = direccion;
            TablaBaseDeDatos.global = new LinkedList<BaseDeDatos>();
            TablaBaseDeDatos.listaUsuario = new LinkedList<Usuario>();
            string text = System.IO.File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\DATABASE", direccion));

            SintacticoChison sintactico = new SintacticoChison();
            sintactico.analizar(text);



        }


    }
}
