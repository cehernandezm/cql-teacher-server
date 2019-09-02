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



            var INT = ToTerm("int");
            var DECIMAL = ToTerm("double");
            var STRING = ToTerm("string");
            var BOOL = ToTerm("boolean");
            var DATE = ToTerm("date");
            var TIME = ToTerm("time");
            var COUNTER = ToTerm("counter");


            var TRUE = ToTerm("True");
            var FALSE = ToTerm("False");
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
            var DROP = ToTerm("DROP");
            var TABLE = ToTerm("TABLE");
            var PRIMARY = ToTerm("PRIMARY");
            var KEY = ToTerm("KEY");
            var ALTER = ToTerm("ALTER");
            var ADD = ToTerm("ADD");
            var TRUNCATE = ToTerm("TRUNCATE");
            var USER = ToTerm("USER");
            var WITH = ToTerm("WITH");
            var PASSWORD = ToTerm("PASSWORD");
            var GRANT = ToTerm("GRANT");
            var ON = ToTerm("ON");
            var REVOKE = ToTerm("REVOKE");
            var INSERT = ToTerm("INSERT");
            var INTO = ToTerm("INTO");
            var VALUES = ToTerm("VALUES");
            var UPDATE = ToTerm("UPDATE");
            var SET = ToTerm("SET");
            var WHERE = ToTerm("WHERE");
            var DELETE = ToTerm("DELETE");
            var FROM = ToTerm("FROM");
            var SELECT = ToTerm("SELECT");
            var LIMIT = ToTerm("LIMIT");
            var ORDER = ToTerm("ORDER");
            var BY = ToTerm("BY");
            var ASC = ToTerm("ASC");
            var DESC = ToTerm("DESC");
            

            var COUNT = ToTerm("COUNT");
            var MIN = ToTerm("MIN");
            var MAX = ToTerm("MAX");
            var SUM = ToTerm("SUM");
            var AVG = ToTerm("AVG");

            var IF = ToTerm("IF");
            var ELSE = ToTerm("ELSE");
            var NOT = ToTerm("NOT");
            var EXISTS = ToTerm("EXISTS");

            var SWITCH = ToTerm("SWITCH");
            var CASE = ToTerm("CASE");
            var BREAK = ToTerm("Break");
            var DEFAULT = ToTerm("Default");
            









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

            NonTerminal ifsuperior = new NonTerminal("ifsuperior");
            NonTerminal insif = new NonTerminal("if");
            NonTerminal inselseif = new NonTerminal("elseif");
            NonTerminal inselse = new NonTerminal("else");

            NonTerminal inSwitch = new NonTerminal("inswitch");
            NonTerminal lisCase = new NonTerminal("liscase");
            NonTerminal inCase = new NonTerminal("incase");
            NonTerminal inDefault = new NonTerminal("indefault");

            NonTerminal inDrop = new NonTerminal("indrop");

            NonTerminal inTable = new NonTerminal("intable");
            NonTerminal compPrimarias = new NonTerminal("compprimarias");
            NonTerminal defColumn = new NonTerminal("defcolumn");
            NonTerminal listaPrimary = new NonTerminal("listprimary");

            NonTerminal inAlterTable = new NonTerminal("inaltertable");
            NonTerminal colTable = new NonTerminal("coltable");

            NonTerminal inDropTable = new NonTerminal("indroptable");

            NonTerminal inTruncaTable = new NonTerminal("intruncatetable");

            NonTerminal inUser = new NonTerminal("inuser");

            NonTerminal editUser = new NonTerminal("edituser");

            NonTerminal inInsert = new NonTerminal("ininsert");
            NonTerminal listValues = new NonTerminal("listvalues");

            NonTerminal inUpdate = new NonTerminal("inupdate");
            NonTerminal listaSet = new NonTerminal("listaset");

            NonTerminal inDelete = new NonTerminal("indelete");

            NonTerminal tiposCampos = new NonTerminal("tipocampos");
            NonTerminal inSelect = new NonTerminal("inselect");
            NonTerminal inWhere = new NonTerminal("inwhere");
            NonTerminal inLimit = new NonTerminal("inlimit");
            NonTerminal inOrder = new NonTerminal("inorder");
            NonTerminal orderby = new NonTerminal("orderby");

            NonTerminal userTypeCQL = new NonTerminal("usertypecql");

            NonTerminal inBreak = new NonTerminal("inbreak");
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
                             | ifsuperior
                             | inSwitch
                             | inDrop + ";"
                             | inTable + ";"
                             | inAlterTable + ";"
                             | inDropTable + ";"
                             | inTruncaTable + ";"
                             | inUser + ";"
                             | editUser + ";"
                             | inInsert + ";"
                             | inUpdate + ";"
                             | inDelete + ";"
                             | inSelect + ";"
                             | inBreak + ";"
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
                           | expresion + RESTA + expresion
                           | expresion + DIVISION + expresion
                           | expresion + POR + expresion
                           | expresion + MODULO + expresion
                           | expresion + POTENCIA + expresion
                           | RESTA + expresion
                           | COUNT + "(<" + inSelect + ">)"
                           | MIN + "(<" + inSelect + ">)"
                           | MAX + "(<" + inSelect + ">)"
                           | SUM + "(<" + inSelect + ">)"
                           | AVG + "(<" + inSelect + ">)"
                           | ToTerm("(") + expresion + ToTerm(")")
                           | ToTerm("(") + tipoVariable + ToTerm(")") + expresion
                           | NEW + ID
                           | expresion + "." + ID
                           | "{" + asigna_UserType + "}" + AS + ID
                           | ENTERO
                           | ToTerm("@") + ID
                           | ToTerm("@") + ID + INCREMENTO
                           | ToTerm("@") + ID + DECREMENTO
                           | expresion + ToTerm("?") + expresion + ToTerm(":") + expresion
                           | CADENA
                           | FECHA
                           | HORA
                           | TRUE
                           | FALSE
                           | DECIMALN
                           | ID
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
                              | COUNTER
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
                           | CREATE + TYPE + ID + "(" + lista_user_type + ")"
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






            //---------------------------------------------------------- IF SUPERIOR ------------------------------------------------------------------------
            ifsuperior.Rule = insif
                            | insif + inselseif
                            | insif + inselseif + inselse
                            | insif + inselse
                            ;
            //------------------------------------------------------------- IF -----------------------------------------------------------------------------
            insif.Rule = IF + "(" + expresion + ")" + "{" + instrucciones + "}";
            //------------------------------------------------------------- ELSE IF -----------------------------------------------------------------------------
            inselseif.Rule = inselseif + ELSE + IF + "(" + expresion + ")" + "{" + instrucciones + "}"
                            | ELSE + IF + "(" + expresion + ")" + "{" + instrucciones + "}"
                            ;
            //------------------------------------------------------------- IF -----------------------------------------------------------------------------
            inselse.Rule = ELSE + "{" + instrucciones + "}";

            //------------------------------------------------------------ SWITCH -------------------------------------------------------------------------
            inSwitch.Rule = SWITCH + "(" + expresion + ")" + "{" + lisCase  + inDefault + "}";
            //------------------------------------------------------------- LISTADO DE CASES ---------------------------------------------------------------
            lisCase.Rule = lisCase + inCase
                         | inCase
                         ;
            //------------------------------------------------------------- CASE ---------------------------------------------------------------------------
            inCase.Rule = CASE + expresion + ":" + "{" + instrucciones + "}";

            //---------------------------------------------------------------------------------------------------------------------------------------------
            inBreak.Rule = BREAK;
            
            //-------------------------------------------------------------- DEFAULT -----------------------------------------------------------------------
            inDefault.Rule = DEFAULT + ":" + "{" + instrucciones + "}";
            //------------------------------------------------------------- DROP DATABASE ------------------------------------------------------------------
            inDrop.Rule = DROP + DATABASE + ID
                        | DROP + DATABASE + IF + EXISTS + ID;

            //-------------------------------------------------------------- CREATE TABLE ------------------------------------------------------------------
            inTable.Rule = CREATE + TABLE + ID + "(" + defColumn + ")"
                         | CREATE + TABLE + IF + NOT + EXISTS + ID + "(" + defColumn + ")"
                         | CREATE + TABLE + ID + "(" + defColumn + "," + compPrimarias + ")"
                         | CREATE + TABLE + IF + NOT + EXISTS + ID + "(" + defColumn + "," + compPrimarias + ")"
                         ;
            //--------------------------------------------------------------- COLUMNAS ---------------------------------------------------------------------
            defColumn.Rule = defColumn + "," + ID + tipoVariable
                           | defColumn + "," + ID + tipoVariable + PRIMARY + KEY
                           | ID + tipoVariable
                           | ID + tipoVariable + PRIMARY + KEY
                           ;

            //---------------------------------------------------- llaves compuestas ------------------------------------------------------------------------

            compPrimarias.Rule = PRIMARY + KEY + "(" + listaPrimary + ")";

            //---------------------------------------------------------------- LISTA DE COLUMNAS QUE PUEDEN SER PRIMARIAS -----------------------------------
            listaPrimary.Rule = listaPrimary + "," + ID
                              | ID
                              ;

            //----------------------------------------------------------- ALTER TABLE ------------------------------------------------------------------------
            inAlterTable.Rule = ALTER + TABLE + ID + ADD + colTable
                              | ALTER + TABLE + ID + DROP + listaPrimary
                              ;

            //------------------------------------------------------------ LISTA DE COLUMNAS A AGREGAR ------------------------------------------------------
            colTable.Rule = colTable + "," + ID + tipoVariable
                          | ID + tipoVariable
                          ;

            //------------------------------------------------------------ DROP TABLE ----------------------------------------------------------------------
            inDropTable.Rule = DROP + TABLE + ID
                             | DROP + TABLE + IF + EXISTS + ID
                             ;
            //------------------------------------------------------------ TRUNCATE TABLE ------------------------------------------------------------------
            inTruncaTable.Rule = TRUNCATE + TABLE + ID;

            //------------------------------------------------------------- CREATE USER ---------------------------------------------------------------------
            inUser.Rule = CREATE + USER + ID + WITH + PASSWORD + CADENA;


            //------------------------------------------------------------ GRANT/REVOKE USER----------------------------------------------------------------
            editUser.Rule = GRANT + ID + ON + ID
                          | REVOKE + ID + ON + ID
                          ;

            //------------------------------------------------------------ INSERT INTO --------------------------------------------------------------------
            inInsert.Rule = INSERT + INTO + ID + VALUES + "(" + listValues + ")"
                          | INSERT + INTO + ID + "(" + listaPrimary + ")" + VALUES + "(" + listValues + ")";
            ;

            listValues.Rule = listValues + "," + expresion
                            | expresion
                            ;



            //---------------------------------------------------------------UPDATE--------------------------------------------------------------------------
            inUpdate.Rule = UPDATE + ID + SET + listaSet
                          | UPDATE + ID + SET + listaSet + WHERE + expresion
                          ;

            listaSet.Rule = listaSet + "," + ID + "=" + expresion
                          | listaSet + "," + userTypeCQL + "=" + expresion
                          | ID + "=" + expresion
                          | userTypeCQL + "=" + expresion
                          ;

            userTypeCQL.Rule = userTypeCQL + "." + ID
                             | ID
                             ;

            //--------------------------------------------------------------- DELETE -------------------------------------------------------------------------
            inDelete.Rule = DELETE + FROM + ID
                          | DELETE + FROM + ID + WHERE + expresion
                          ;


            //---------------------------------------------------------------- SELECT -------------------------------------------------------------------------
            tiposCampos.Rule = POR
                             | listValues
                             ;

            inSelect.Rule = SELECT + tiposCampos + FROM + ID
                          | SELECT + tiposCampos + FROM + ID + inWhere
                          | SELECT + tiposCampos + FROM + ID + inLimit
                          | SELECT + tiposCampos + FROM + ID + inOrder
                          | SELECT + tiposCampos + FROM + ID + inWhere + inOrder
                          | SELECT + tiposCampos + FROM + ID + inWhere + inLimit
                          | SELECT + tiposCampos + FROM + ID + inOrder + inLimit
                          | SELECT + tiposCampos + FROM + ID + inWhere + inOrder + inLimit
                          ;


            inLimit.Rule = LIMIT + expresion;

            inWhere.Rule = WHERE + expresion;

            inOrder.Rule = inOrder + "," + orderby
                         | ORDER + BY + orderby
                         ;

            orderby.Rule = ID
                         | ID + ASC
                         | ID + DESC
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

            this.MarkPunctuation("(", ")",";",",");



            this.Root = inicio;
            inicio.ErrorRule = SyntaxError + inicio;
            #endregion
        }
    }
}
