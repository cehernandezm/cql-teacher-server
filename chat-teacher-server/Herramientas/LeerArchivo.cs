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
            try
            {
                using(StreamReader sr = new StreamReader("DATABASE/Principal.chison"))
                {
                    string text = sr.ReadToEnd();
                    System.Diagnostics.Debug.WriteLine("TEXTO: " + text);

                }


            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error al leer el archivo: " + e.Message);
            }
        }
    }
}
