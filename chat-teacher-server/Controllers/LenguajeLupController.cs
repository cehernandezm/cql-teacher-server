using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            return new string[] { "value1", "value2" };
        }

        // GET: api/LenguajeLup/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/LenguajeLup
        [HttpPost]
        public IActionResult Post(LenguajeLup codigo)
        {
            SintacticoLUP sintactico = new SintacticoLUP();
            string salida = sintactico.analizar(codigo.codigo);
            return Ok(salida);
        }

        // PUT: api/LenguajeLup/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
