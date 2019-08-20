using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Gramatica
{
    public class GramaticaCQL : Grammar 
    {

        public GramaticaCQL() : base(caseSensitive: false)
        {
            #region ER
            StringLiteral CADENA = new StringLiteral("cadena", "\"");
            var FECHA = new RegexBasedTerminal("fecha", "\\'\\d{4}-(((0)[0-9])|((1)[0-2]))-([0-2][0-9]|(3)[0-1])\\'");
            var HORA = new RegexBasedTerminal("hora", "\\'(([0-1][0-9])|2[0-3]):([0-2][0-9]):([0-5][0-9])\\'");
            var ENTERO = new NumberLiteral("entero");
            var DECIMALN = new RegexBasedTerminal("decimal", "[0-9]+'.'[0-9]+");

            IdentifierTerminal ID = new IdentifierTerminal("ID");

            CommentTerminal comentarioLinea = new CommentTerminal("comentarioLinea", "//", "\n", "\r\n");
            CommentTerminal comentarioBloque = new CommentTerminal("comentarioBloque", "/*", "*/");
            #endregion

            #region Terminales
            
            var PTCOMA = ToTerm(";");
            var LLAVEIZQ = ToTerm("[");
            var LLAVEDER = ToTerm("]");

            var NULO = ToTerm("null");
            var INT = ToTerm("int");
            var DECIMAL = ToTerm("double");
            var STRING = ToTerm("string");
            var BOOL = ToTerm("boolean");
            var DATE = ToTerm("date");
            var TIME = ToTerm("time");
            var TRUE = ToTerm("true");
            var FALSE = ToTerm("false");

            var USE = ToTerm("USE");
            var CREATE = ToTerm("CREATE");
            var DATABASE = ToTerm("DATABASE");

            var IF = ToTerm("IF");
            var NOT = ToTerm("NOT");
            var EXISTS = ToTerm("EXISTS");

            #endregion

            #region No Terminales

            NonTerminal inicio = new NonTerminal("inicio");
            NonTerminal instrucciones = new NonTerminal("instrucciones");
            NonTerminal instruccion = new NonTerminal("instruccion");

            NonTerminal use = new NonTerminal("use");

            NonTerminal createDatabase = new NonTerminal("createdatabase");
            #endregion

            #region Gramatica
            inicio.Rule = instrucciones;

            instrucciones.Rule = instrucciones + instruccion
                               | instruccion
                               ;

            instruccion.Rule = use + PTCOMA
                             | createDatabase + PTCOMA
                             ;

            use.Rule = USE + ID;

            createDatabase.Rule = CREATE + DATABASE + ID
                                | CREATE + DATABASE + IF + NOT + EXISTS + ID
                                ;


            #endregion

            #region Preferencias
            this.Root = inicio;
            inicio.ErrorRule = SyntaxError + inicio;
            #endregion
        }
    }
}
