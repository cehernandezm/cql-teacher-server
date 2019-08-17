using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cql_teacher_server.CHISON;
using cql_teacher_server.CHISON.Componentes;
using cql_teacher_server.CHISON.Gramatica;
using cql_teacher_server.Herramientas;
using cql_teacher_server.LUP.Gramatica;
using cql_teacher_server.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace cql_teacher_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LenguajeLupController : ControllerBase
    {
        // GET: api/LenguajeLup
        [HttpGet]
        public IEnumerable<string> Get()
        {
            TablaBaseDeDatos.global = new LinkedList<BaseDeDatos>();
            TablaBaseDeDatos.listaUsuario = new LinkedList<Usuario>();

            LeerArchivo leer = new LeerArchivo();
            
            return new string[] { "value1", "value2" };
        }

        // GET: api/LenguajeLup/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            string salida = "";
            LinkedList <BaseDeDatos> global = TablaBaseDeDatos.global;
            salida += "\"BASES\" : [";
            foreach (BaseDeDatos bd in global)
            {

                salida += "\n\t{";
                salida += "\n\t\t \"NAME\" : \"" + bd.nombre + "\",";
                salida += "\n\t\t \"TABLAS\" : [";
                foreach (Tabla tb in bd.listaTablas)
                {
                    salida += "\n\t\t\t{";

                    salida += "\n\t\t\t\t\"NAME\": \"" + tb.nombre + "\",";

                    salida += "\n\t\t\t\t\"COLUMNAS\": [";
                    foreach (Columna co in tb.columnas)
                    {
                        salida += "\n\t\t\t\t\t{";
                        salida += "\n\t\t\t\t\t\t \"NAME\" : \"" + co.name + "\",";
                        salida += "\n\t\t\t\t\t\t \"TIPO\" : \"" + co.tipo + "\",";
                        salida += "\n\t\t\t\t\t\t \"FK\" : \"" + co.pk + "\",";
                        salida += "\n\t\t\t\t\t},";
                    }
                    salida += "\n\t\t\t\t],";

                    salida += "\n\t\t\t\t\"INFO\" : [";
                    foreach (Data da in tb.datos)
                    {
                        foreach(Atributo a in da.valores)
                        {
                            salida += "\n\t\t\t\t\t{";
                            salida += "\n\t\t\t\t\t\t \"COLUMNA\" : \"" + a.nombre + "\",";
                            salida += "\n\t\t\t\t\t\t \"VALOR\" : \"" + a.valor + "\",";
                            salida += "\n\t\t\t\t\t},";
                        }
                        
                    }
                    salida += "\n\t\t\t\t]";

                    salida += "\n\t\t\t},";
                }
                salida += "\n\t},";
            }
            salida += "\n],";


            return salida;
        }

        // POST: api/LenguajeLup
        [HttpPost]
        public void Post(LenguajeLup codigo)
        {
            
        }

        // PUT: api/LenguajeLup/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] string value)
        {
            return Ok("HOLA");
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
