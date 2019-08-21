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
            var DECIMALN = new RegexBasedTerminal("decimal", "[0-9]+.[0-9]+");
            var ENTERO = new NumberLiteral("entero");
            

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

            var MAYOR = ToTerm(">");
            var MENOR = ToTerm("<");
            var MAYORIG = ToTerm(">=");
            var MENORIG = ToTerm("<=");
            var IGUAIG = ToTerm("==");
            var DIFERENTE = ToTerm("!=");
            var NEGACION = ToTerm("!");
            var OR = ToTerm("||");
            var AND = ToTerm("&&");
            var XOR = ToTerm("^");
            var TERNARIO = ToTerm("?");

            var DOSPTS = ToTerm(":");
            var PARIZQ = ToTerm("(");
            var PARDER = ToTerm("(");

            var DIVISION = ToTerm("/");
            var MODULO = ToTerm("%");
            var POTENCIA = ToTerm("**");
            var POR = ToTerm("*");
            var RESTA = ToTerm("-");
            var SUMA = ToTerm("+");


            var USE = ToTerm("USE");
            var CREATE = ToTerm("CREATE");
            var DATABASE = ToTerm("DATABASE");

            var IF = ToTerm("IF");
            var NOT = ToTerm("NOT");
            var EXISTS = ToTerm("EXISTS");






            RegisterOperators(1, Associativity.Left, OR);
            RegisterOperators(2, Associativity.Left, AND,XOR);
            RegisterOperators(3, Associativity.Right, NEGACION);
            RegisterOperators(4, Associativity.Left, IGUAIG,MAYOR,MAYORIG,MENOR,MENORIG);
            RegisterOperators(5, Associativity.Left, SUMA,RESTA);
            RegisterOperators(6, Associativity.Left, POR,DIVISION,MODULO);
            RegisterOperators(7, Associativity.Left, POTENCIA);



            NonGrammarTerminals.Add(comentarioLinea);
            NonGrammarTerminals.Add(comentarioBloque);
            #endregion

            #region No Terminales

            NonTerminal inicio = new NonTerminal("inicio");
            NonTerminal instrucciones = new NonTerminal("instrucciones");
            NonTerminal instruccion = new NonTerminal("instruccion");

            NonTerminal use = new NonTerminal("use");

            NonTerminal createDatabase = new NonTerminal("createdatabase");

            NonTerminal expresion = new NonTerminal("expresion");
            #endregion

            #region Gramatica
            inicio.Rule = instrucciones;

            instrucciones.Rule = instrucciones + instruccion
                               | instruccion
                               ;

            instruccion.Rule = use + PTCOMA
                             | createDatabase + PTCOMA
                             | expresion
                             ;

            use.Rule = USE + ID;

            createDatabase.Rule = CREATE + DATABASE + ID
                                | CREATE + DATABASE + IF + NOT + EXISTS + ID
                                ;



            expresion.Rule = expresion + OR + expresion
                           | expresion + AND + expresion
                           | expresion + XOR + expresion
                           | NEGACION + expresion
                           | expresion + MAYOR + expresion 
                           | expresion + MENOR + expresion
                           | expresion + MAYORIG + expresion
                           | expresion + MENORIG + expresion
                           | expresion + IGUAIG + expresion 
                           | expresion + DIFERENTE + expresion
                           | expresion + SUMA + expresion 
                           | expresion +  RESTA + expresion
                           | expresion + DIVISION + expresion 
                           | expresion + POR + expresion 
                           | expresion + MODULO + expresion 
                           | expresion + POTENCIA + expresion 
                           | PARIZQ + expresion + PARDER
                           | RESTA + expresion
                           | ENTERO
                           | ID
                           | CADENA
                           | FECHA
                           | HORA
                           | TRUE
                           | FALSE
                           | DECIMALN
                           ;


            #endregion

            #region Preferencias
            this.Root = inicio;
            inicio.ErrorRule = SyntaxError + inicio;
            #endregion
        }
    }
}
