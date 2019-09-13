using cql_teacher_server.CHISON;
using cql_teacher_server.CHISON.Componentes;
using cql_teacher_server.CQL.Arbol;
using cql_teacher_server.CQL.Componentes.Procedure;
using cql_teacher_server.Herramientas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Componentes.Funcion_Procedure
{
    public class Procedure : InstruccionCQL
    {
        string id { set; get; }
        public string identificador { set; get; }
        LinkedList<listaParametros> parametros { set; get; }
        LinkedList<InstruccionCQL> cuerpo { set; get; }
        int l { set; get; }
        int c { set; get; }
        string codigo { set; get; }

        /*
        * CONSTRUCTOR DE LA CLASE
        * @param {id} nombre del procedimiento
        * @param {parametro} lista de parametros
        * @param {cuerpo} instrucciones a ejecutar
        * @param {l} linea del id
        * @param {c} columna del id
        * @param {codigo} codigo original
        */
        public Procedure(string id, LinkedList<listaParametros> parametros, LinkedList<InstruccionCQL> cuerpo, int l, int c, string codigo)
        {
            this.id = id;
            this.parametros = parametros;
            this.cuerpo = cuerpo;
            this.l = l;
            this.c = c;
            this.codigo = codigo;
            generarID();
        }

        /*
         * Metodo de la implementacion
         * @ts tabla de simbolos global
         * @user usuario que ejecuta la accion
         * @baseD base de datos donde estamos ejecutando todo
         * @mensajes linkedlist con la salida deseada
         */
        public object ejecutar(TablaDeSimbolos ts, string user, ref string baseD, LinkedList<string> mensajes, TablaDeSimbolos tsT)
        {
            Mensaje ms = new Mensaje();
            BaseDeDatos db = TablaBaseDeDatos.getBase(baseD);
            if (db != null)
            {
                if (user.Equals("admin"))
                {

                }
                else
                {
                    Usuario us = TablaBaseDeDatos.getUsuario(user);
                    if (us != null)
                    {
                        if (TablaBaseDeDatos.getPermiso(us, baseD))
                        {
                            if (!TablaBaseDeDatos.getEnUso(baseD, user))
                            {
                                if (db.buscarProcedure(identificador) == null)
                                {
                                    db.objetos.procedures.AddLast(new Procedures(id, codigo, identificador, parametros, cuerpo));
                                    return "";
                                }
                                else mensajes.AddLast(ms.error("El procedure: " + id + " ya existe en esta DB: " + baseD,l,c,"Semantico"));
                            }
                            else mensajes.AddLast(ms.error("La base de datos: " + baseD + " esta siendo utilizada por otro usuario",l,c,"Semantico"));
                        }
                        else mensajes.AddLast(ms.error("El usuario: " + user + " no tiene permisos sobre la DB: " + baseD, l, c, "Semantico"));
                    }
                    else mensajes.AddLast(ms.error("No se encuentra el usuario: " + user, l, c, "Semantico"));
                }
                
            }
            else mensajes.AddLast(ms.error("No se encuentra la base de datos: " + baseD,l,c,"Semantico"));
            return null;
        }

        /*
         * METODO QUE GENERA EL IDENTIFICADOR UNICO PARA CADA PROCEDURE
         */
         private void generarID()
        {
            string iden = id;
            foreach(listaParametros lista in parametros)
            {
                foreach(Declaracion d in lista.lista)
                {
                    iden += "_" + d.tipo;
                }
            }
            identificador = iden;
        }


       
    }
}
