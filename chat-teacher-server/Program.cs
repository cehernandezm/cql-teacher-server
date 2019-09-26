using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using cql_teacher_server.CHISON;
using cql_teacher_server.CHISON.Componentes;
using cql_teacher_server.CQL.Componentes;
using cql_teacher_server.Herramientas;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace chat_teacher_server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(ProcessExit);
            TablaBaseDeDatos.global = new LinkedList<BaseDeDatos>();
            TablaBaseDeDatos.listaUsuario = new LinkedList<Usuario>();
            TablaBaseDeDatos.listaEnUso = new LinkedList<USO>();
            TablaBaseDeDatos.listaFunciones = new LinkedList<Funcion>();

            LeerArchivo leer = new LeerArchivo("Principal.chison");
            CreateWebHostBuilder(args).Build().Run();
        }

        static void  ProcessExit(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("SE HIZO ROLLBACK");
            GuardarArchivo archivo = new GuardarArchivo();
            archivo.guardarArchivo("Principal");
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
