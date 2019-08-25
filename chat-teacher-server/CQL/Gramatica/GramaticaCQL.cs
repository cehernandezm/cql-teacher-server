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
            var DECIMALN = new RegexBasedTerminal("decimal", "[0-9]+\\.[0-9]+");
            var ENTERO = new NumberLiteral("entero");
            

            IdentifierTerminal ID = new IdentifierTerminal("ID");

            CommentTerminal comentarioLinea = new CommentTerminal("comentarioLinea", "//", "\n", "\r\n");
            CommentTerminal comentarioBloque = new CommentTerminal("comentarioBloque", "/*", "*/");
            #endregion

            #region Terminales
            


            var NULO = ToTerm("null");
            var INT = ToTerm("int");
            var DECIMAL = ToTerm("double");
            var STRING = ToTerm("string");
            var BOOL = ToTerm("boolean");
            var DATE = ToTerm("date");
            var TIME = ToTerm("time");


            var TRUE = ToTerm("true");
            var FALSE = ToTerm("false");
            var NULL = ToTerm("null");

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

            var DIVISION = ToTerm("/");
            var MODULO = ToTerm("%");
            var POTENCIA = ToTerm("**");
            var POR = ToTerm("*");
            var RESTA = ToTerm("-");
            var SUMA = ToTerm("+");

            var INCREMENTO = ToTerm("++");
            var DECREMENTO = ToTerm("--");

            


            var USE = ToTerm("USE");
            var CREATE = ToTerm("CREATE");
            var TYPE = ToTerm("TYPE");
            var DATABASE = ToTerm("DATABASE");
            var NEW = ToTerm("new");
            var AS = ToTerm("AS");

            var IF = ToTerm("IF");
            var NOT = ToTerm("NOT");
            var EXISTS = ToTerm("EXISTS");






            



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

            NonTerminal tipoVariable = new NonTerminal("tipovariable");

            NonTerminal declaracion = new NonTerminal("declaracion");

            NonTerminal declaracionA = new NonTerminal("declaracionA");

            NonTerminal user_type = new NonTerminal("usertype");

            NonTerminal lista_user_type = new NonTerminal("listausertype");

            NonTerminal asigna_UserType = new NonTerminal("asignausertype");

            NonTerminal asignacion = new NonTerminal("asignacion");

            NonTerminal asignacionA = new NonTerminal("asignaciona");

            #endregion

            #region Gramatica
            inicio.Rule = instrucciones;

            instrucciones.Rule = instrucciones + instruccion
                               | instruccion
                               ;

            instruccion.Rule = use + ";"
                             | createDatabase + ";"
                             | declaracion + ";"
                             | declaracionA + ";"
                             | user_type + ";"
                             | asignacion + ";"
                             ;

            //--------------------------------------------------- USE ---------------------------------------------------------------------------------------

            use.Rule = USE + ID;

            //--------------------------------------------------- BASES DE DATOS-----------------------------------------------------------------------------
            createDatabase.Rule = CREATE + DATABASE + ID
                                | CREATE + DATABASE + IF + NOT + EXISTS + ID
                                ;


            //---------------------------------------------------------- EXPRESIONES ------------------------------------------------------------------------

            expresion.Rule = expresion + OR + expresion
                           | expresion + AND + expresion
                           | expresion + XOR + expresion
                           | NEGACION + expresion
                           | expresion + IGUAIG + expresion
                           | expresion + DIFERENTE + expresion
                           | expresion + MAYOR + expresion 
                           | expresion + MENOR + expresion
                           | expresion + MAYORIG + expresion
                           | expresion + MENORIG + expresion
                           | expresion + SUMA + expresion 
                           | expresion +  RESTA + expresion
                           | expresion + DIVISION + expresion 
                           | expresion + POR + expresion 
                           | expresion + MODULO + expresion 
                           | expresion + POTENCIA + expresion
                           | RESTA + expresion
                           | ToTerm("(") + expresion + ToTerm(")")
                           | ToTerm("(") + tipoVariable + ToTerm(")") + expresion
                           | NEW + ID 
                           | expresion + "." + ID
                           | "{" + asigna_UserType + "}"  + AS + ID
                           | ENTERO
                           | ToTerm("@") + ID
                           | ToTerm("@") + ID + INCREMENTO
                           | ToTerm("@") + ID + DECREMENTO
                           | CADENA
                           | FECHA
                           | HORA
                           | TRUE
                           | FALSE
                           | DECIMALN
                           | NULL
                           ;


            //-------------------------------------------------------- asignacion de valores de un usertype ----------------------------------------------------
            asigna_UserType.Rule = asigna_UserType + "," + expresion
                                 | expresion
                                 ;

            //------------------------------------------------------ TIPO DE VARIABLES --------------------------------------------------------------------------

            tipoVariable.Rule = STRING
                              | INT
                              | BOOL
                              | DECIMAL
                              | DATE
                              | TIME
                              | ID
                              ;

            //------------------------------------------------------- DECLARACION DE VARIABLE --------------------------------------------------------------------

            declaracion.Rule = declaracion + "," + "@" + ID
                             | tipoVariable + "@" + ID
                             ;
            //--------------------------------------------------------- DECLARAR Y ASIGNAR UNA VARIABLE ------------------------------------------------------------

            declaracionA.Rule = tipoVariable + "@" + ID + "=" + expresion
                              | declaracion + "," + "@" + ID + "=" + expresion
                              ;

            //---------------------------------------------------------- CREACION DE USER TYPES ---------------------------------------------------------------------
            user_type.Rule = CREATE + TYPE + IF + NOT + EXISTS + ID + "(" + lista_user_type + ")"
                           | CREATE + TYPE + ID + "(" + lista_user_type  +")"
                           ;

            //---------------------------------------------------------- Lista de atributos de un user_type ----------------------------------------------------------
            lista_user_type.Rule = lista_user_type + "," + ID + tipoVariable 
                                 | ID + tipoVariable 
                                 ;


            //--------------------------------------------------------- ASIGNACION DE VARIABLES ----------------------------------------------------------------------
            asignacion.Rule = "@" + ID + ToTerm("=") + expresion
                            | "@" + ID + ToTerm("+=") + expresion
                            | "@" + ID + ToTerm("*=") + expresion
                            | "@" + ID + ToTerm("-=") + expresion
                            | "@" + ID + ToTerm("/=") + expresion
                            | asignacionA + ToTerm("=") + expresion
                            | asignacionA + ToTerm("+=") + expresion
                            | asignacionA + ToTerm("*=") + expresion
                            | asignacionA + ToTerm("-=") + expresion
                            | asignacionA + ToTerm("/=") + expresion
                            ;

            //-------------------------------------------------------- asignacion de un atributo ----------------------------------------------------------------------
            asignacionA.Rule = asignacionA + "." + ID
                             | ToTerm("@") + ID
                             ;

            #endregion

            #region Preferencias

            RegisterOperators(1, Associativity.Left, OR);
            RegisterOperators(2, Associativity.Left, AND, XOR);
            RegisterOperators(3, Associativity.Left, IGUAIG, DIFERENTE);
            RegisterOperators(4, Associativity.Left, MAYOR, MAYORIG, MENOR, MENORIG);
            RegisterOperators(5, Associativity.Left, SUMA, RESTA);
            RegisterOperators(6, Associativity.Left, POR, DIVISION, MODULO);
            RegisterOperators(7, Associativity.Right, POTENCIA);
            RegisterOperators(8, Associativity.Right, NEGACION);
            RegisterOperators(9, Associativity.Neutral, ToTerm("("), ToTerm(")"));

            this.MarkPunctuation("(", ")",";", "@",",");



            this.Root = inicio;
            inicio.ErrorRule = SyntaxError + inicio;
            #endregion
        }
    }
}
