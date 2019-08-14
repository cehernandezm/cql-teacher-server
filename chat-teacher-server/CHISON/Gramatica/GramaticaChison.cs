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
            var ENTERO = new NumberLiteral("entero");
            var DECIMAL = new RegexBasedTerminal("decimal", "[0-9]+'.'[0-9]+");
            var FECHA = new RegexBasedTerminal("fecha","((19|20)\\d{2})'-'((0|1)\\d{1})'-'((0|1|2)\\d{1})");
            var HORA = new RegexBasedTerminal("hora", "(([0-1]\\d{1})|(2[0-3]))':'([0-5]\\d{1})':'([0-5]\\d{1})");
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
            var NAME = ToTerm("\"NAME\"");
            var DATA = ToTerm("\"DATA\"");
            var PASSWORD = ToTerm("\"PASSWORD\"");
            var PERMISSIONS = ToTerm("\"PERMISSIONS\"");
            var CQL_TYPE = ToTerm("\"CQL-TYPE\"");
            var COLUMNS = ToTerm("\"COLUMNS\"");
            var TYPE = ToTerm("\"TYPE\"");
            var PK = ToTerm("\"PK\"");
            var ATTRS = ToTerm("\"ATTRS\"");
            var INSTR = ToTerm("\"INSTR\"");
            var AS = ToTerm("\"AS\"");
            #endregion


            #region No Terminales
            NonTerminal inicio = new NonTerminal("inicio");
            NonTerminal instruccion_superior = new NonTerminal("instruccion_superior");
            NonTerminal database = new NonTerminal("database");
            #endregion

            #region Gramatica
            inicio.Rule = instruccion_superior;

            instruccion_superior.Rule = DOLAR + MENOR + database + MAYOR + DOLAR;

            database.Rule = DATABASES + IGUAL + LLAVEIZQ + LLAVEDER;

            #endregion

            #region Preferencias
            this.Root = inicio;
            instruccion_superior.ErrorRule = SyntaxError + instruccion_superior;
            #endregion
            
        }
    }
}
