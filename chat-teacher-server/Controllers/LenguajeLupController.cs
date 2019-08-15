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
            LeerArchivo leer = new LeerArchivo();
            
            return new string[] { "value1", "value2" };
        }

        // GET: api/LenguajeLup/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            string salida = "";
            LinkedList <BaseDeDatos> global = TablaBaseDeDatos.global;
            foreach(BaseDeDatos bd in global)
            {
                salida += "{\n\t";
                foreach(Atributo a in bd.atributos)
                {
                    salida += "\"" + a.nombre + "\": \"" + a.valor + "\",\n\t";
                }
                salida += "\n}";
            }
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
