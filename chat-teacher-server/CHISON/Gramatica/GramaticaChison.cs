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
            var TRUE = ToTerm("True");
            var FALSE = ToTerm("False");
            var MENOR = ToTerm("<");
            var MAYOR = ToTerm(">");
            var IGUAL = ToTerm("=");
            var LLAVEIZQ = ToTerm("[");
            var LLAVEDER = ToTerm("]");
            var COMA = ToTerm(",");
            var DOLAR = ToTerm("$");
            var DATABASES = ToTerm("\"DATABASES\"");
            var USERS = ToTerm("\"USERS\"");
            #endregion


            #region No Terminales
            NonTerminal inicio = new NonTerminal("inicio");
            NonTerminal instrucciones_superior = new NonTerminal("instrucciones_superior");
            NonTerminal instruccion_superior = new NonTerminal("intruccion_superior");

            NonTerminal database = new NonTerminal("database");
            NonTerminal bases = new NonTerminal("bases");
            NonTerminal baseU = new NonTerminal("base");

            NonTerminal objetos = new NonTerminal("objetos");
            NonTerminal objeto = new NonTerminal("objeto");
            NonTerminal tipo = new NonTerminal("tipo");
            
            #endregion

            #region Gramatica
            inicio.Rule = DOLAR + MENOR + instrucciones_superior + MAYOR + DOLAR;

            instrucciones_superior.Rule = instrucciones_superior + COMA + instruccion_superior 
                                        | instruccion_superior;

            instruccion_superior.Rule = database;

            database.Rule = DATABASES + IGUAL + LLAVEIZQ + LLAVEDER
                          | DATABASES + IGUAL + LLAVEIZQ + bases + LLAVEDER ;

            bases.Rule = bases + COMA + baseU
                       | baseU;

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
                      ;
            #endregion

            #region Preferencias
            this.Root = inicio;
            instrucciones_superior.ErrorRule = SyntaxError + instrucciones_superior;
            #endregion
            
        }
    }
}
