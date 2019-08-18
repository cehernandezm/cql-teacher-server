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
                Objeto objeto = bd.objetos;
                salida += "\n\t{";
                salida += "\n\t\t \"NAME\" : \"" + bd.nombre + "\",";

                salida += "\n\t\t \"TABLAS\" : [";
                foreach (Tabla tb in objeto.tablas)
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
                        salida += "\n\t\t\t\t\t{";
                        foreach (Atributo a in da.valores)
                        {
                            salida += "\n\t\t\t\t\t\t \"COLUMNA\" : \"" + a.nombre + "\",";
                            
                            if (a.valor.GetType() == typeof(LinkedList<object>))
                            {
                                salida += "\n\t\t\t\t\t\t \"VALOR\" : [ " + getElementos((LinkedList<object>)a.valor) + "],";
                            }
                            else salida += "\n\t\t\t\t\t\t \"VALOR\" : \"" + a.valor + "\",";



                        }
                        salida += "\n\t\t\t\t\t},";

                    }
                    salida += "\n\t\t\t\t]";

                    salida += "\n\t\t\t},";
                }
                salida += "\n\t},";

                salida += "\n\t\t \"USER_TYPES\" : [";
                foreach (User_Types tb in objeto.user_types)
                {
                    salida += "\n\t\t\t{";

                    salida += "\n\t\t\t\t\"NAME\": \"" + tb.name + "\",";

                    salida += "\n\t\t\t\t\"ATTRS\": [";
                    foreach (Attrs da in tb.type)
                    {

                        salida += "\n\t\t\t\t\t{";
                        salida += "\n\t\t\t\t\t\t \"NAME\" : \"" + da.name + "\",";
                        salida += "\n\t\t\t\t\t\t \"TYPE\" : \"" + da.type + "\",";
                        salida += "\n\t\t\t\t\t},";

                    }
                    salida += "\n\t\t\t\t],";

                    salida += "\n\t\t\t},";
                }
                salida += "\n\t},";

                salida += "\n\t\t \"PROCEDURES\" : [";
                foreach (Procedures tb in objeto.procedures)
                {
                    salida += "\n\t\t\t{";

                    salida += "\n\t\t\t\t\"NAME\": \"" + tb.nombre + "\",";

                    salida += "\n\t\t\t\t\"PARAMETERS\": [";
                    foreach (Parametros da in tb.parametros)
                    {

                        salida += "\n\t\t\t\t\t{";
                        salida += "\n\t\t\t\t\t\t \"NAME\" : \"" + da.nombre + "\",";
                        salida += "\n\t\t\t\t\t\t \"TYPE\" : \"" + da.tipo + "\",";
                        salida += "\n\t\t\t\t\t\t \"AS\" : \"" + da.ass + "\"";
                        salida += "\n\t\t\t\t\t},";

                    }
                    salida += "\n\t\t\t\t],";
                    salida += "\n\t\t\t\t\"INSTRUCCIONES\": \"" + tb.instruccion + "\",";
                    salida += "\n\t\t\t},";
                }
                salida += "\n\t},";
            }

            salida += "\n],";

            salida += "\n\"USUARIOS\" : [";
            foreach(Usuario us in TablaBaseDeDatos.listaUsuario)
            {
                salida += "\n\t{";
                salida += "\n\t\t \"NAME\" : \"" + us.nombre + "\",";
                salida += "\n\t\t \"PASSWORD\" : \"" + us.password + "\",";
                salida += "\n\t\t \"PERMISSIONS\" : [";
                foreach(string dbname in us.bases)
                {
                    salida += "\n\t\t\t{";
                    salida += "\n\t\t\t\t\"NAME\" : \"" + dbname + "\",";
                    salida += "\n\t\t\t},";
                }
                salida += "\n\t\t ]";
            }
            salida += "\n],";

            return salida;
        }

        // POST: api/LenguajeLup
        [HttpPost]
        public IEnumerable<string> Post(LenguajeLup codigo)
        {
            SintacticoLUP sintactico = new SintacticoLUP();
            object res = sintactico.analizar(codigo.codigo);
            if (res != null) return new string[] { res.ToString() };
            else return new string[] { "Hubo un error" };
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


        public string getElementos(LinkedList<object> lista)
        {
            string cadena = "";
                foreach(object o in lista)
            {
                if (o.GetType() == typeof(LinkedList<object>)) cadena += "[ " + getElementos((LinkedList<object>)o) + "],";
                else cadena += o.ToString() + ",";
            }
            return cadena;
        }
    }
}
