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
        public LeerArchivo()
        {
            
               string text = System.IO.File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\DATABASE", "Principal.chison"));

                SintacticoChison sintactico = new SintacticoChison();
                sintactico.analizar(text);
           
            
           
        }

 
    }
}
