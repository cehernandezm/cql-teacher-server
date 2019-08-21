﻿using cql_teacher_server.CHISON;
using cql_teacher_server.CHISON.Componentes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.Herramientas
{
    public class GuardarArchivo
    {
        /*
         * Metodo se encarga de recorrer todo nuestra base de datos no relacional y guardarla en forma fisica
         */
        public void guardarArchivo()
        {
            using (FileStream fileStream = File.Open("DATABASE/Principal2.chison", FileMode.OpenOrCreate))
            {
                using(StreamWriter f = new StreamWriter(fileStream))
                {
                    BaseDeDatos last = TablaBaseDeDatos.global.Count() > 0 ? TablaBaseDeDatos.global.Last() : null;
                    f.WriteLine("$<");
                    f.WriteLine("\t\"DATABASES\" = [");
                    //-------------------------------------------------------------BASES DE DATOS ---------------------------------------------------------------------
                    foreach (BaseDeDatos bd in TablaBaseDeDatos.global)
                    {
                        f.WriteLine("\t\t<");
                        f.WriteLine("\t\t\t\"NAME\" = \"" + bd.nombre + "\",");
                        f.WriteLine("\t\t\t\"Data\" = [");

                        Objeto o = bd.objetos;
                        Tabla lastT = o.tablas.Count > 0 ? o.tablas.Last() : null;
                        User_Types lastU = o.user_types.Count() > 0 ? o.user_types.Last() : null;
                        Procedures lastP = o.procedures.Count() > 0 ? o.procedures.Last() : null;

                        //---------------------------------------------- TABLAS --------------------------------------------------------------------------------------
                        foreach (Tabla t in o.tablas)
                        {
                            f.WriteLine("\t\t\t\t<");
                            f.WriteLine("\t\t\t\t\t\"CQL-TYPE\" = \"TABLE\",");
                            f.WriteLine("\t\t\t\t\t\"NAME\" = \"" + t.nombre + "\",");
                            f.WriteLine("\t\t\t\t\t\"COLUMNS\" = [");

                            //--------------------------------------------COLUMNAS ---------------------------------------------------------------------------------
                            Columna lastC = t.columnas.Count() > 0 ? t.columnas.Last() : null;
                            foreach (Columna c in t.columnas)
                            {
                                f.WriteLine("\t\t\t\t\t\t<");
                                f.WriteLine("\t\t\t\t\t\t\t\"NAME\" = \"" + c.name + "\",");
                                f.WriteLine("\t\t\t\t\t\t\t\"TYPE\" = \"" + c.tipo + "\",");
                                f.WriteLine("\t\t\t\t\t\t\t\"PK\" = " + c.pk);

                                if (c.Equals(lastC)) f.WriteLine("\t\t\t\t\t\t>");
                                else f.WriteLine("\t\t\t\t\t\t>,");

                            }
                            f.WriteLine("\t\t\t\t\t],");

                            //-------------------------------------------- DATA ------------------------------------------------------------------------------------
                            f.WriteLine("\t\t\t\t\t\"DATA\" = [");
                            Data lastD = t.datos.Count() > 0 ? t.datos.Last() : null;
                            foreach (Data d in t.datos)
                            {
                                f.WriteLine("\t\t\t\t\t<");

                                //------------------------------------------------------ lista de Atributos --------------------------------------------------------
                                Atributo lastA = d.valores.Count() > 0 ? d.valores.Last() : null;
                                foreach (Atributo a in d.valores)
                                {
                                    if (a.valor.GetType() == typeof(LinkedList<object>))
                                    {

                                        if (a.Equals(lastA)) f.WriteLine("\t\t\t\t\t\t\"" + a.nombre + "\" = [" + getElementos((LinkedList<object>)a.valor) + "]");
                                        else f.WriteLine("\t\t\t\t\t\t\"" + a.nombre + "\" = [" + getElementos((LinkedList<object>)a.valor) + "],");
                                      
                                    }
                                    else
                                    {
                                        if (a.Equals(lastA))
                                        {
                                            if (a.tipo.Equals("HORA") || a.tipo.Equals("FECHA")) f.WriteLine("\t\t\t\t\t\t\"" + a.nombre + "\" = \'" + a.valor + "\'");
                                            else if (a.valor.GetType() == typeof(string)) f.WriteLine("\t\t\t\t\t\t\"" + a.nombre + "\" = \"" + a.valor + "\"");
                                            else f.WriteLine("\t\t\t\t\t\t\"" + a.nombre + "\" = " + a.valor);
                                        }
                                        else
                                        {
                                            if (a.tipo.Equals("HORA") || a.tipo.Equals("FECHA")) f.WriteLine("\t\t\t\t\t\t\"" + a.nombre + "\" = \'" + a.valor + "\',");
                                            else if (a.valor.GetType() == typeof(string)) f.WriteLine("\t\t\t\t\t\t\"" + a.nombre + "\" = \"" + a.valor + "\",");
                                            else f.WriteLine("\t\t\t\t\t\t\"" + a.nombre + "\" = " + a.valor + ",");
                                        }
                                    }

                                }

                                if (d.Equals(lastD)) f.WriteLine("\t\t\t\t\t>");
                                else f.WriteLine("\t\t\t\t\t>,");
                            }
                            f.WriteLine("\t\t\t\t\t]");

                            if (t.Equals(lastT))
                            {
                                if (lastU != null || lastP != null) f.WriteLine("\t\t\t\t>,");
                                else f.WriteLine("\t\t\t\t>");
                            }
                            else f.WriteLine("\t\t\t\t>,");
                        }

                        //--------------------------------------------- OBJECT ---------------------------------------------------------------------------------------

                        foreach (User_Types us in o.user_types)
                        {
                            f.WriteLine("\t\t\t\t<");
                            f.WriteLine("\t\t\t\t\t\"CQL-TYPE\" = \"OBJECT\",");
                            f.WriteLine("\t\t\t\t\t\"NAME\" = \"" + us.name + "\",");
                            f.WriteLine("\t\t\t\t\t\"ATTRS\" = [");

                            //------------------------------------------------------ ATTRS ---------------------------------------------------------------------------
                            Attrs lastA = us.type.Count() > 0 ? us.type.Last() : null;
                            foreach (Attrs a in us.type)
                            {
                                f.WriteLine("\t\t\t\t\t\t<");
                                f.WriteLine("\t\t\t\t\t\t\t\"NAME\" = \"" + a.name + "\",");
                                f.WriteLine("\t\t\t\t\t\t\t\"TYPE\" = \"" + a.type + "\",");

                                if (a.Equals(lastA)) f.WriteLine("\t\t\t\t\t\t>");
                                else f.WriteLine("\t\t\t\t\t\t>,");
                            }

                            f.WriteLine("\t\t\t\t\t]");
                            if (us.Equals(lastU))
                            {
                                if (lastP != null) f.WriteLine("\t\t\t\t>,");
                                else f.WriteLine("\t\t\t\t>");
                            }
                            else f.WriteLine("\t\t\t\t>,");
                        }

                        //------------------------------------------------ PROCEDURES -------------------------------------------------------------------------------
                        foreach (Procedures p in o.procedures)
                        {
                            f.WriteLine("\t\t\t\t<");
                            f.WriteLine("\t\t\t\t\t\"CQL-TYPE\" = \"PROCEDURE\",");
                            f.WriteLine("\t\t\t\t\t\"NAME\" = \"" + p.nombre + "\",");
                            f.WriteLine("\t\t\t\t\t\"PARAMETERS\" = [");

                            //------------------------------------------------------ PARAMETROS ---------------------------------------------------------------------
                            Parametros lastPa = p.parametros.Count() > 0 ? p.parametros.Last() : null;
                            foreach (Parametros pa in p.parametros)
                            {
                                f.WriteLine("\t\t\t\t\t\t<");
                                f.WriteLine("\t\t\t\t\t\t\t\"NAME\" = \"" + pa.nombre + "\",");
                                f.WriteLine("\t\t\t\t\t\t\t\"TYPE\" = \"" + pa.tipo + "\",");
                                f.WriteLine("\t\t\t\t\t\t\t\"AS\" = " + pa.ass);

                                if (pa.Equals(lastPa)) f.WriteLine("\t\t\t\t\t\t>");
                                else f.WriteLine("\t\t\t\t\t\t>,");
                            }

                            f.WriteLine("\t\t\t\t\t],");
                            f.WriteLine("\t\t\t\t\t\"INSTR\" = \"" + p.instruccion + "\"");
                        }

                        f.WriteLine("\t\t\t]");
                        if (bd.Equals(last)) f.WriteLine("\t\t>");
                        else f.WriteLine("\t\t>,");
                    }

                    f.WriteLine("\t],");
                    f.WriteLine("\t\"USERS\" = [");

                    //---------------------------------------------------- USUARIOS -------------------------------------------------------------------------------
                    Usuario lastUser = TablaBaseDeDatos.listaUsuario.Count() > 0 ? TablaBaseDeDatos.listaUsuario.Last() : null;
                    foreach (Usuario us in TablaBaseDeDatos.listaUsuario)
                    {
                        f.WriteLine("\t\t<");
                        f.WriteLine("\t\t\t\"NAME\" = \"" + us.nombre + "\",");
                        f.WriteLine("\t\t\t\"PASSWORD\" = \"" + us.password + "\",");
                        f.WriteLine("\t\t\t\"PERMISSIONS\" = [");

                        string lasPer = us.bases.Count() > 0 ? us.bases.Last() : null;
                        foreach (string p in us.bases)
                        {
                            f.WriteLine("\t\t\t<");
                            f.WriteLine("\t\t\t\"NAME\" = \"" + p + "\"");
                            if (p.Equals(lasPer)) f.WriteLine("\t\t\t>");
                            else f.WriteLine("\t\t\t>,");
                        }

                        f.WriteLine("\t\t\t]");
                        if (us.Equals(lastUser)) f.WriteLine("\t\t>");
                        else f.WriteLine("\t\t>,");
                    }


                    f.WriteLine("\t]");

                    f.WriteLine(">$");
                }
            }

            
        }


        /*
         * Este metodo devuelve en un string una lista de elementos
         * @lista lista de objetos
         * @return retonar un string u otra lista dependiendo del objeto en su interior
         */

        public string getElementos(LinkedList<object> lista)
        {
            string cadena = "";
            object lastL = lista.Count() > 0 ? lista.Last() : null;
            foreach (object o in lista)
            {
                if (o.Equals(lastL))
                {
                    if (o.GetType() == typeof(LinkedList<object>)) cadena += "[ " + getElementos((LinkedList<object>)o) + "]";
                    else
                    {
                        if (o.GetType() == typeof(string)) cadena += "\"" + o.ToString() + "\"";
                        else cadena += o.ToString();

                    }
                }
                else
                {
                    if (o.GetType() == typeof(LinkedList<object>)) cadena += "[ " + getElementos((LinkedList<object>)o) + "],";
                    else
                    {
                        if (o.GetType() == typeof(string)) cadena += "\"" + o.ToString() + "\",";
                        else cadena += o.ToString() +",";

                    }
                }
                
            }
            return cadena;
        }



    }
}