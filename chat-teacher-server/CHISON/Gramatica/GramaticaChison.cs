using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CHISON.Gramatica
{
    class GramaticaChison : Grammar
    {
        public GramaticaChison() : base(caseSensitive: false)
        {
            #region ER
            StringLiteral CADENA = new StringLiteral("cadena", "\"");
            var FECHA = new RegexBasedTerminal("fecha", "\\'\\d{4}-(((0)[0-9])|((1)[0-2]))-([0-2][0-9]|(3)[0-1])\\'");
            var HORA = new RegexBasedTerminal("hora", "\\'(([0-1][0-9])|2[0-3]):([0-2][0-9]):([0-5][0-9])\\'");
            var ENTERO = new NumberLiteral("entero");
            var DECIMAL = new RegexBasedTerminal("decimal", "[0-9]+'.'[0-9]+");
            IdentifierTerminal ID = new IdentifierTerminal("ID");

            #endregion

            #region Terminales
            var TRUE = ToTerm("TRUE");
            var FALSE = ToTerm("FALSE");
            var IN = ToTerm("IN");
            var OUT = ToTerm("OUT");
            var MENOR = ToTerm("<");
            var MAYOR = ToTerm(">");
            var IGUAL = ToTerm("=");
            var CHISON = ToTerm(".chison");
            var LLAVEIZQ = ToTerm("[");
            var CORIZQ = ToTerm("{");
            var CORDER = ToTerm("}");
            var LLAVEDER = ToTerm("]");
            var COMA = ToTerm(",");
            var DOLAR = ToTerm("$");
            var DATABASES = ToTerm("\"DATABASES\"");
            var USERS = ToTerm("\"USERS\"");
            #endregion


            #region No Terminales
            NonTerminal inicio = new NonTerminal("inicio");
            NonTerminal instruccion_superior = new NonTerminal("intruccion_superior");

            NonTerminal database = new NonTerminal("database");
            NonTerminal bases = new NonTerminal("bases");
            NonTerminal baseU = new NonTerminal("base");

            NonTerminal user = new NonTerminal("user");

            NonTerminal objetos = new NonTerminal("objetos");
            NonTerminal objeto = new NonTerminal("objeto");
            NonTerminal tipo = new NonTerminal("tipo");

            NonTerminal listaTablas = new NonTerminal("listaTablas");
            NonTerminal tabla = new NonTerminal("tabla");

            NonTerminal importar = new NonTerminal("importar");

            #endregion

            #region Gramatica
            inicio.Rule = DOLAR + MENOR + instruccion_superior + MAYOR + DOLAR
                        | listaTablas;


            instruccion_superior.Rule = database + COMA + user;

            database.Rule = DATABASES + IGUAL + LLAVEIZQ + LLAVEDER
                          | DATABASES + IGUAL + LLAVEIZQ + bases + LLAVEDER;

            bases.Rule = bases + COMA + baseU
                       | baseU;

            user.Rule = USERS + IGUAL + LLAVEIZQ + LLAVEDER;

            baseU.Rule = MENOR + objetos + MAYOR;

            objetos.Rule = objetos + COMA + objeto
                         | objeto;

            objeto.Rule = CADENA + IGUAL + tipo;

            tipo.Rule = CADENA
                      | TRUE
                      | FALSE
                      | ENTERO
                      | DECIMAL
                      | FECHA
                      | HORA
                      | IN
                      | OUT
                      | LLAVEIZQ + listaTablas + LLAVEDER
                      | LLAVEIZQ + LLAVEDER
                      | LLAVEIZQ + importar + LLAVEDER
                      ;

            listaTablas.Rule = listaTablas + COMA + tabla
                             | tabla
                             ;

            tabla.Rule = MENOR + objetos + MAYOR;

            importar.Rule = DOLAR + CORIZQ + ID + CHISON + CORDER + DOLAR;

            #endregion

            #region Preferencias
            this.Root = inicio;
            instruccion_superior.ErrorRule = SyntaxError + instruccion_superior;
            #endregion
            
        }
    }
}
