using cql_teacher_server.CHISON;
using cql_teacher_server.CHISON.Componentes;
using cql_teacher_server.CQL.Arbol;
using cql_teacher_server.CQL.Componentes;
using cql_teacher_server.Herramientas;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cql_teacher_server.CQL.Gramatica
{
    public class SintacticoCQL
    {
        bool flagEx = false;
        /*
         * Metodo que analizar la cadena 
         * @cadena es el string a analizar
         * @retonar un set de errores o un set de mensajes
         */

        public void analizar(string cadena, string usuario)
        {
            usuario = usuario.TrimEnd();
            usuario = usuario.TrimStart();
            GramaticaCQL gramatica = new GramaticaCQL();
            LanguageData lenguaje = new LanguageData(gramatica);
            Parser parser = new Parser(gramatica);
            ParseTree arbol = parser.Parse(cadena);
            ParseTreeNode raiz = arbol.Root;

            if (arbol != null)
            {
                for (int i = 0; i < arbol.ParserMessages.Count(); i++)
                {
                    System.Diagnostics.Debug.WriteLine(arbol.ParserMessages.ElementAt(i).Message + " Linea: " + arbol.ParserMessages.ElementAt(i).Location.Line.ToString()
                             + " Columna: " + arbol.ParserMessages.ElementAt(i).Location.Column.ToString() + "\n");
                }

                if (arbol.ParserMessages.Count() < 1)
                {
                    graficar(raiz);

                    LinkedList<InstruccionCQL> listaInstrucciones = instrucciones(raiz.ChildNodes.ElementAt(0));
                    TablaDeSimbolos tablaGlobal = new TablaDeSimbolos();
                    TablaDeSimbolos tablaCQL = new TablaDeSimbolos();
                    LinkedList<string> mensajes = new LinkedList<string>();
                    String baseD = TablaBaseDeDatos.getMine(usuario);
                    foreach (InstruccionCQL ins in listaInstrucciones)
                    {
                        Mensaje mensa = new Mensaje();
                        object res = ins.ejecutar(tablaGlobal, usuario, ref baseD, mensajes,tablaCQL);
                        if (res != null && ins.GetType() == typeof(Expresion)) System.Diagnostics.Debug.WriteLine(mensa.message("El resultado de la operacion es: " + res.ToString()));
                    }

                    foreach (string m in mensajes)
                    {
                        System.Diagnostics.Debug.WriteLine(m);
                    }
                   
                    foreach (Simbolo s in tablaGlobal)
                    {
                        System.Diagnostics.Debug.WriteLine("Tipo: " + s.Tipo + " Nombre: " + s.nombre + " Valor: " + s.valor);
                    }
                    
                }
            }

        }

        /*
         * Metodo que las primeras dos producciones del arbol de irony
         * @raiz es el nodo raiz del arbol a analizar
         * @ retorna una lista de instrucciones
         */

        public LinkedList<InstruccionCQL> instrucciones(ParseTreeNode raiz)
        {
            //------------------ instrucciones instruccion -------------
            if (raiz.ChildNodes.Count == 2)
            {
                LinkedList<InstruccionCQL> lista = instrucciones(raiz.ChildNodes.ElementAt(0));
                object res = instruccion(raiz.ChildNodes.ElementAt(1).ChildNodes.ElementAt(0));
                if (res.GetType() == typeof(InstruccionCQL)) lista.AddLast((InstruccionCQL)res);
                else lista = new LinkedList<InstruccionCQL>(lista.Union((LinkedList<InstruccionCQL>)res));
                return lista;
            }
            //------------------  instruccion -------------
            else
            {
                LinkedList<InstruccionCQL> lista = new LinkedList<InstruccionCQL>();
                object res = instruccion(raiz.ChildNodes.ElementAt(0).ChildNodes.ElementAt(0));
                if (res.GetType() == typeof(InstruccionCQL)) lista.AddLast((InstruccionCQL)res);
                else lista = new LinkedList<InstruccionCQL>(lista.Union((LinkedList<InstruccionCQL>)res));
                return lista;
            }
        }

        /*
         * Metodo que recorre el arbol por completo
         * @raiz es el nodo raiz del arbol a analizar
         * @return una InstruccionCQL o una LinkedList<InstruccionCQL>
         */

        public object instruccion(ParseTreeNode raiz)
        {
            string token = raiz.Term.Name;
            ParseTreeNode hijo = raiz;
            switch (token)
            {
                //-------------------------------- USE DB ----------------------------------------------------------------
                case "use":
                    LinkedList<InstruccionCQL> lu = new LinkedList<InstruccionCQL>();
                    string id = hijo.ChildNodes.ElementAt(1).Token.Text;
                    int linea = hijo.ChildNodes.ElementAt(1).Token.Location.Line;
                    int columna = hijo.ChildNodes.ElementAt(1).Token.Location.Column;
                    lu.AddLast(new Use(id, linea, columna));
                    return lu;

                // ----------------------------------- CREATE DATABASE ------------------------------------------------------
                case "createdatabase":
                    LinkedList<InstruccionCQL> lb = new LinkedList<InstruccionCQL>();
                    string idB = "";
                    int lineaB = 0;
                    int columnaB = 0;
                    bool flag = false;

                    //--------------------------------------- CREATE DATABASE ID ------------------------------------------
                    if (hijo.ChildNodes.Count() == 3)
                    {
                        idB = hijo.ChildNodes.ElementAt(2).ToString().Split(' ')[0];
                        lineaB = hijo.ChildNodes.ElementAt(2).Token.Location.Line;
                        columnaB = hijo.ChildNodes.ElementAt(2).Token.Location.Column;
                        flag = false;
                    }
                    //---------------------------------------- CREATE DATABASE IF NOT EXISTS ID -----------------------------------------
                    else
                    {
                        idB = hijo.ChildNodes.ElementAt(5).ToString().Split(' ')[0];
                        lineaB = hijo.ChildNodes.ElementAt(5).Token.Location.Line;
                        columnaB = hijo.ChildNodes.ElementAt(5).Token.Location.Column;
                        flag = true;
                    }
                    lb.AddLast(new DataBase(idB, lineaB, columnaB, flag));
                    return lb;


                //------------------------------------------- Expresion ---------------------------------------------------------------
                case "expresion":
                    LinkedList<InstruccionCQL> le = new LinkedList<InstruccionCQL>();
                    le.AddLast(resolver_expresion(hijo));
                    return le;

                //----------------------------------------------- DECLARACION DE VARIABLE -----------------------------------------------
                case "declaracion":
                    string tokend = hijo.ChildNodes.ElementAt(0).Term.Name;
                    int l = 0;
                    int c = 0;
                    string t = "";
                    string i = "";
                    LinkedList<InstruccionCQL> lista = new LinkedList<InstruccionCQL>();

                    if (tokend.Equals("declaracion"))
                    {
                        lista = (LinkedList<InstruccionCQL>)instruccion(hijo);
                        t = declaracionTipo(hijo.ChildNodes.ElementAt(0));
                    }
                    else t = hijo.ChildNodes.ElementAt(0).ChildNodes.ElementAt(0).Token.Text;
                    l = hijo.ChildNodes.ElementAt(2).Token.Location.Line;
                    c = hijo.ChildNodes.ElementAt(2).Token.Location.Column;
                    i = hijo.ChildNodes.ElementAt(2).Token.Text;
                    t = t.ToLower().TrimEnd().TrimStart();
                    i = i.ToLower().TrimEnd().TrimStart();
                    Declaracion a = new Declaracion(t, null, i, l, c);
                    lista.AddLast(a);
                    return lista;

                //----------------------------------------------- DECLARACION ASIGNACION -----------------------------------------------------
                case "declaracionA":
                    string tokenA = hijo.ChildNodes.ElementAt(0).Term.Name;
                    LinkedList<InstruccionCQL> liA = new LinkedList<InstruccionCQL>();
                    string tA = "";
                    int lA = 0;
                    int cA = 0;
                    string iA = "";
                    if (tokenA.Equals("declaracion"))
                    {
                        LinkedList<InstruccionCQL> listaTemp = (LinkedList<InstruccionCQL>)instruccion(hijo);
                        liA = new LinkedList<InstruccionCQL>(liA.Union(listaTemp));
                        tA = declaracionTipo(hijo.ChildNodes.ElementAt(0));
                    }
                    else tA = hijo.ChildNodes.ElementAt(0).ChildNodes.ElementAt(0).Token.Text;
                    lA = hijo.ChildNodes.ElementAt(2).Token.Location.Line;
                    cA = hijo.ChildNodes.ElementAt(2).Token.Location.Column;
                    iA = hijo.ChildNodes.ElementAt(2).Token.Text;
                    tA = tA.ToLower().TrimEnd().TrimStart();
                    iA = iA.ToLower().TrimEnd().TrimStart();
                    Declaracion aA = new Declaracion(tA, resolver_expresion(hijo.ChildNodes.ElementAt(4)), iA, lA, cA);
                    liA.AddLast(aA);
                    return liA;

                //--------------------------------------------------- CREACION DE UN USER_TYPE ----------------------------------------------------
                case "usertype":
                    LinkedList<InstruccionCQL> lU = new LinkedList<InstruccionCQL>();
                    int lUT = 0;
                    int cUT = 0;
                    Boolean flagU = false;
                    LinkedList<Attrs> atribuU = new LinkedList<Attrs>();
                    string idU = "";
                    if (hijo.ChildNodes.Count() == 4)
                    {
                        idU = hijo.ChildNodes.ElementAt(2).Token.Text;
                        idU = idU.ToLower().TrimEnd().TrimStart();
                        atribuU = getListaUserType(hijo.ChildNodes.ElementAt(3));
                        lUT = hijo.ChildNodes.ElementAt(2).Token.Location.Line;
                        cUT = hijo.ChildNodes.ElementAt(2).Token.Location.Column;

                    }
                    else
                    {
                        idU = hijo.ChildNodes.ElementAt(5).Token.Text;
                        idU = idU.ToLower().TrimEnd().TrimStart();
                        atribuU = getListaUserType(hijo.ChildNodes.ElementAt(6));
                        lUT = hijo.ChildNodes.ElementAt(5).Token.Location.Line;
                        cUT = hijo.ChildNodes.ElementAt(5).Token.Location.Column;
                        flagU = true;
                    }
                    lU.AddLast(new UserType(idU, atribuU, lUT, cUT, flagU));
                    return lU;


                //------------------------------------------------------ ASIGNACION DE VARIABLES --------------------------------------------------
                case "asignacion":
                    LinkedList<InstruccionCQL> lAs = new LinkedList<InstruccionCQL>();
                    string nameT = hijo.ChildNodes.ElementAt(0).Term.Name;
                    nameT = nameT.ToLower().TrimEnd().TrimStart();
                    string idAs = "";
                    string operaAs = "";
                    int liAs = 0;
                    int coAs = 0;
                    if (nameT.Equals("asignaciona"))
                    {
                        liAs = hijo.ChildNodes.ElementAt(0).ChildNodes.ElementAt(2).Token.Location.Line;
                        coAs = hijo.ChildNodes.ElementAt(0).ChildNodes.ElementAt(2).Token.Location.Column;
                        idAs = hijo.ChildNodes.ElementAt(0).ChildNodes.ElementAt(2).Token.Text;
                        idAs = idAs.ToLower().TrimEnd().TrimStart();
                        operaAs = hijo.ChildNodes.ElementAt(1).Token.Text;
                        if (operaAs.Equals("=")) lAs.AddLast(new Asignacion(idAs, liAs, coAs, resolver_expresion(hijo.ChildNodes.ElementAt(0)), resolver_expresion(hijo.ChildNodes.ElementAt(2)), "ASIGNACIONA"));
                        else if (operaAs.Equals("+="))
                        {
                            flagEx = true;
                            Expresion opa = resolver_expresion(hijo.ChildNodes.ElementAt(0));
                            Expresion ge = new Expresion(opa, resolver_expresion(hijo.ChildNodes.ElementAt(2)), "SUMA", liAs, coAs);
                            flagEx = false;
                            lAs.AddLast(new Asignacion(idAs, liAs, coAs, resolver_expresion(hijo.ChildNodes.ElementAt(0)), ge, "ASIGNACIONA"));
                        }
                        else if (operaAs.Equals("-="))
                        {
                            flagEx = true;
                            Expresion opa = resolver_expresion(hijo.ChildNodes.ElementAt(0));
                            Expresion ge = new Expresion(opa, resolver_expresion(hijo.ChildNodes.ElementAt(2)), "RESTA", liAs, coAs);
                            flagEx = false;
                            lAs.AddLast(new Asignacion(idAs, liAs, coAs, resolver_expresion(hijo.ChildNodes.ElementAt(0)), ge, "ASIGNACIONA"));
                        }
                        else if (operaAs.Equals("*="))
                        {
                            flagEx = true;
                            Expresion opa = resolver_expresion(hijo.ChildNodes.ElementAt(0));
                            Expresion ge = new Expresion(opa, resolver_expresion(hijo.ChildNodes.ElementAt(2)), "MULTIPLICACION", liAs, coAs);
                            flagEx = false;
                            lAs.AddLast(new Asignacion(idAs, liAs, coAs, resolver_expresion(hijo.ChildNodes.ElementAt(0)), ge, "ASIGNACIONA"));
                        }
                        else
                        {
                            flagEx = true;
                            Expresion opa = resolver_expresion(hijo.ChildNodes.ElementAt(0));
                            Expresion ge = new Expresion(opa, resolver_expresion(hijo.ChildNodes.ElementAt(2)), "DIVISION", liAs, coAs);
                            flagEx = false;
                            lAs.AddLast(new Asignacion(idAs, liAs, coAs, resolver_expresion(hijo.ChildNodes.ElementAt(0)), ge, "ASIGNACIONA"));
                        }
                        flagEx = false;

                        return lAs;
                    }


                    idAs = hijo.ChildNodes.ElementAt(1).Token.Text;
                    idAs = idAs.ToLower().TrimEnd().TrimStart();

                    liAs = hijo.ChildNodes.ElementAt(1).Token.Location.Line;
                    coAs = hijo.ChildNodes.ElementAt(1).Token.Location.Column;

                    operaAs = hijo.ChildNodes.ElementAt(2).Token.Text;
                    operaAs = operaAs.ToLower().TrimEnd().TrimStart();

                    if (operaAs.Equals("=")) lAs.AddLast(new Asignacion(idAs, liAs, coAs, resolver_expresion(hijo.ChildNodes.ElementAt(3)), "ASIGNACION"));
                    else if (operaAs.Equals("+="))
                    {
                        Expresion opa = new Expresion(idAs, "ID", liAs, coAs);
                        Expresion ge = new Expresion(opa, resolver_expresion(hijo.ChildNodes.ElementAt(3)), "SUMA", liAs, coAs);
                        lAs.AddLast(new Asignacion(idAs, liAs, coAs, ge, "ASIGNACION"));
                    }
                    else if (operaAs.Equals("-="))
                    {
                        Expresion opa = new Expresion(idAs, "ID", liAs, coAs);
                        Expresion ge = new Expresion(opa, resolver_expresion(hijo.ChildNodes.ElementAt(3)), "RESTA", liAs, coAs);
                        lAs.AddLast(new Asignacion(idAs, liAs, coAs, ge, "ASIGNACION"));
                    }
                    else if (operaAs.Equals("*="))
                    {
                        Expresion opa = new Expresion(idAs, "ID", liAs, coAs);
                        Expresion ge = new Expresion(opa, resolver_expresion(hijo.ChildNodes.ElementAt(3)), "MULTIPLICACION", liAs, coAs);
                        lAs.AddLast(new Asignacion(idAs, liAs, coAs, ge, "ASIGNACION"));
                    }
                    else
                    {
                        Expresion opa = new Expresion(idAs, "ID", liAs, coAs);
                        Expresion ge = new Expresion(opa, resolver_expresion(hijo.ChildNodes.ElementAt(3)), "DIVISION", liAs, coAs);
                        lAs.AddLast(new Asignacion(idAs, liAs, coAs, ge, "ASIGNACION"));
                    }




                    return lAs;
                //------------------------------------------------------------- IF SUPERIOR --------------------------------------------------------------
                case "ifsuperior":
                    //------------------------------------------------------- IF ------------------------------------------------------------------------
                    LinkedList<SubIf> listadoIf = new LinkedList<SubIf>();
                    LinkedList<InstruccionCQL> listaR = new LinkedList<InstruccionCQL>();
                    //------------------------------------------------ IF --------------------------------------------------------------------------------
                    int lif = hijo.ChildNodes.ElementAt(0).ChildNodes.ElementAt(0).Token.Location.Line;
                    int cif = hijo.ChildNodes.ElementAt(0).ChildNodes.ElementAt(0).Token.Location.Column;
                    LinkedList<InstruccionCQL> listadoInS = instrucciones(hijo.ChildNodes.ElementAt(0).ChildNodes.ElementAt(3));

                    listadoIf.AddLast(new SubIf(resolver_expresion(hijo.ChildNodes.ElementAt(0).ChildNodes.ElementAt(1)), listadoInS, lif, cif));

                    //------------------------------------ IF ELSE / IF ELSE IF + ----------------------------------------------------------------------------
                    if (hijo.ChildNodes.Count() == 2)
                    {
                        string idSeparator = hijo.ChildNodes.ElementAt(1).Term.Name;
                        //----------------------------------------------- ELSE -------------------------------------------------------------------------------
                        if (idSeparator.Equals("else"))
                        {

                            lif = hijo.ChildNodes.ElementAt(1).ChildNodes.ElementAt(0).Token.Location.Line;
                            cif = hijo.ChildNodes.ElementAt(1).ChildNodes.ElementAt(0).Token.Location.Column;
                            listadoInS = instrucciones(hijo.ChildNodes.ElementAt(1).ChildNodes.ElementAt(2));
                            listadoIf.AddLast(new SubIf(listadoInS, lif, cif));
                        }
                        else
                        {
                            LinkedList<SubIf> listatemp = resolver_if(hijo.ChildNodes.ElementAt(1));
                            listadoIf = new LinkedList<SubIf>(listadoIf.Union(listatemp));
                        }
                    }
                    //-----------------------------------------IF ELSE IF + ELSE ------------------------------------------------------------------------------
                    else if (hijo.ChildNodes.Count() == 3)
                    {
                        LinkedList<SubIf> listatemp = resolver_if(hijo.ChildNodes.ElementAt(1));
                        listadoIf = new LinkedList<SubIf>(listadoIf.Union(listatemp));

                        lif = hijo.ChildNodes.ElementAt(2).ChildNodes.ElementAt(0).Token.Location.Line;
                        cif = hijo.ChildNodes.ElementAt(2).ChildNodes.ElementAt(0).Token.Location.Column;
                        listadoInS = instrucciones(hijo.ChildNodes.ElementAt(2).ChildNodes.ElementAt(2));
                        listadoIf.AddLast(new SubIf(listadoInS, lif, cif));
                    }
                    listaR.AddLast(new IfSuperior(listadoIf));
                    return listaR;

                //------------------------------------------------------- SWITCH --------------------------------------------------------------------------------
                case "inswitch":
                    Expresion condicionS = resolver_expresion(hijo.ChildNodes.ElementAt(1));
                    LinkedList<Case> listadoCase = resolver_case(hijo.ChildNodes.ElementAt(3),condicionS);
                    LinkedList<InstruccionCQL> listadoR = new LinkedList<InstruccionCQL>();

                    if(hijo.ChildNodes.Count() == 6)
                    {
                        ParseTreeNode nodeDefault = hijo.ChildNodes.ElementAt(4);
                        int lineaD = nodeDefault.ChildNodes.ElementAt(0).Token.Location.Line;
                        int columnaD = nodeDefault.ChildNodes.ElementAt(0).Token.Location.Column;
                        LinkedList<InstruccionCQL> listadoD = instrucciones(nodeDefault.ChildNodes.ElementAt(2));
                        listadoCase.AddLast(new Case(listadoD, lineaD, columnaD));
                    }

                    listadoR.AddLast(new Switch(listadoCase));
                    return listadoR;

                //---------------------------------------------------------- DROP ------------------------------
                case "indrop":
                    int lineaDrop ;
                    int columnaDrop;
                    string idDrop;
                    Boolean flagD;
                    if (raiz.ChildNodes.Count() == 5)
                    {
                        lineaDrop = hijo.ChildNodes.ElementAt(4).Token.Location.Line;
                        columnaDrop = hijo.ChildNodes.ElementAt(4).Token.Location.Column;
                        idDrop = hijo.ChildNodes.ElementAt(4).Token.Text;
                        idDrop = idDrop.ToLower().TrimEnd().TrimStart();
                        flagD = true;
                    }
                    else
                    {
                        lineaDrop = hijo.ChildNodes.ElementAt(2).Token.Location.Line;
                        columnaDrop = hijo.ChildNodes.ElementAt(2).Token.Location.Column;
                        idDrop = hijo.ChildNodes.ElementAt(2).Token.Text;
                        idDrop = idDrop.ToLower().TrimEnd().TrimStart();
                        flagD = false;
                    }

                    LinkedList<InstruccionCQL> listaDR = new LinkedList<InstruccionCQL>();
                    listaDR.AddLast(new Drop(idDrop, lineaDrop, columnaDrop,flagD));
                    return listaDR;



                //--------------------------------------------------------- CREATE TABLE ------------------------------------------------
                case "intable":

                    LinkedList<InstruccionCQL> listaRT = new LinkedList<InstruccionCQL>();
                    string nombreT;
                    int lineaTa;
                    int columnaTa;
                    Boolean flagTa;
                    LinkedList<Columna> listaTa;

                    if (hijo.ChildNodes.Count() == 7)
                    {
                        nombreT = hijo.ChildNodes.ElementAt(5).Token.Text;
                        nombreT = nombreT.ToLower().TrimEnd().TrimStart();

                        listaTa = getListaColumna(hijo.ChildNodes.ElementAt(6));

                        lineaTa = hijo.ChildNodes.ElementAt(5).Token.Location.Line;
                        columnaTa = hijo.ChildNodes.ElementAt(5).Token.Location.Column;
                        flagTa = true;
                    }
                    else if(hijo.ChildNodes.Count() == 8)
                    {
                        nombreT = hijo.ChildNodes.ElementAt(5).Token.Text;
                        nombreT = nombreT.ToLower().TrimEnd().TrimStart();

                        listaTa = getListaColumna(hijo.ChildNodes.ElementAt(6));

                        lineaTa = hijo.ChildNodes.ElementAt(5).Token.Location.Line;
                        columnaTa = hijo.ChildNodes.ElementAt(5).Token.Location.Column;
                        flagTa = true;
                        LinkedList<string> primariasCompuestas = getCompuestas(hijo.ChildNodes.ElementAt(7).ChildNodes.ElementAt(2));
                        listaRT.AddLast(new CreateTable(nombreT,listaTa,primariasCompuestas,lineaTa,columnaTa,flagTa));
                        return listaRT;
                    }
                    else if (hijo.ChildNodes.Count() == 5)
                    {
                        nombreT = hijo.ChildNodes.ElementAt(2).Token.Text;
                        nombreT = nombreT.ToLower().TrimEnd().TrimStart();

                        listaTa = getListaColumna(hijo.ChildNodes.ElementAt(3));

                        lineaTa = hijo.ChildNodes.ElementAt(2).Token.Location.Line;
                        columnaTa = hijo.ChildNodes.ElementAt(2).Token.Location.Column;
                        flagTa = false;

                        LinkedList<string> primariasCompuestas = getCompuestas(hijo.ChildNodes.ElementAt(4).ChildNodes.ElementAt(2));
                        listaRT.AddLast(new CreateTable(nombreT, listaTa, primariasCompuestas, lineaTa, columnaTa, flagTa));
                        return listaRT;

                    }
                    else
                    {
                        nombreT = hijo.ChildNodes.ElementAt(2).Token.Text;
                        nombreT = nombreT.ToLower().TrimEnd().TrimStart();

                        listaTa = getListaColumna(hijo.ChildNodes.ElementAt(3));

                        lineaTa = hijo.ChildNodes.ElementAt(2).Token.Location.Line;
                        columnaTa = hijo.ChildNodes.ElementAt(2).Token.Location.Column;
                        flagTa = false;
                    }

                    listaRT.AddLast(new CreateTable(nombreT, listaTa, lineaTa, columnaTa, flagTa));
                    return listaRT;


                //------------------------------------------------------ ALTER TABLE -----------------------------------------------------
                case "inaltertable":

                    LinkedList<InstruccionCQL> listaAT = new LinkedList<InstruccionCQL>();

                    string idAT = hijo.ChildNodes.ElementAt(2).Token.Text;
                    idAT = idAT.ToLower().TrimEnd().TrimStart();

                    int lAT = hijo.ChildNodes.ElementAt(2).Token.Location.Line;
                    int cAT = hijo.ChildNodes.ElementAt(2).Token.Location.Column;

                    string oAT = hijo.ChildNodes.ElementAt(3).Token.Text;
                    oAT = oAT.ToLower().TrimEnd().TrimStart();

                    if (oAT.Equals("add"))
                    {
                        LinkedList<Columna> listaAddAT = getListaColumnaAdd(hijo.ChildNodes.ElementAt(4));
                        listaAT.AddLast(new AlterTable(idAT, listaAddAT, lAT, cAT, "ADD"));
                    }
                    else
                    {
                        LinkedList<string> listaDropATT = getCompuestas(hijo.ChildNodes.ElementAt(4));
                        listaAT.AddLast(new AlterTable(idAT, listaDropATT, lAT, cAT, "DROP"));
                    }
                    return listaAT;



                //-------------------------------------------------------- DROP TABLE --------------------------------------------------------
                case "indroptable":
                    LinkedList<InstruccionCQL> listaRDT = new LinkedList<InstruccionCQL>();
                    string idDT;
                    int lDT;
                    int cDT;
                    Boolean flagDT;

                    if(hijo.ChildNodes.Count() == 5)
                    {
                        idDT = hijo.ChildNodes.ElementAt(4).Token.Text;
                        idDT = idDT.ToLower().TrimEnd().TrimStart();

                        lDT = hijo.ChildNodes.ElementAt(4).Token.Location.Line;
                        cDT = hijo.ChildNodes.ElementAt(4).Token.Location.Column;

                        flagDT = true;

                    }
                    else
                    {
                        idDT = hijo.ChildNodes.ElementAt(2).Token.Text;
                        idDT = idDT.ToLower().TrimEnd().TrimStart();

                        lDT = hijo.ChildNodes.ElementAt(2).Token.Location.Line;
                        cDT = hijo.ChildNodes.ElementAt(2).Token.Location.Column;

                        flagDT = false;
                    }
                    listaRDT.AddLast(new DropTable(idDT,flagDT, lDT, cDT));
                    return listaRDT;


                //----------------------------------------------------- TRUNCATE TABLE ------------------------------------------------------------
                case "intruncatetable":
                    LinkedList<InstruccionCQL> listaRTT = new LinkedList<InstruccionCQL>();
                    string idTT;
                    int lTT;
                    int cTT;

                    idTT = hijo.ChildNodes.ElementAt(2).Token.Text;
                    idTT = idTT.ToLower().TrimEnd().TrimStart();

                    lTT = hijo.ChildNodes.ElementAt(2).Token.Location.Line;
                    cTT = hijo.ChildNodes.ElementAt(2).Token.Location.Line;

                    listaRTT.AddLast(new TruncateTable(idTT, lTT, cTT));
                    return listaRTT;

                //-------------------------------------------------CREATE USER--------------------------------------------------------------------------
                case "inuser":
                    LinkedList<InstruccionCQL> listaRCU = new LinkedList<InstruccionCQL>();

                    string idCU = hijo.ChildNodes.ElementAt(2).Token.Text;
                    idCU = idCU.ToLower().TrimEnd().TrimStart();

                    string paCU = hijo.ChildNodes.ElementAt(5).Token.Text;
                    paCU = paCU.TrimStart('\"').TrimEnd('\"');

                    int lCU = hijo.ChildNodes.ElementAt(2).Token.Location.Line;
                    int cCU = hijo.ChildNodes.ElementAt(2).Token.Location.Column;

                    listaRCU.AddLast(new CreateUser(idCU, paCU, lCU, cCU));

                    return listaRCU;


                //------------------------------------------------ EDIT USER ----------------------------------------------------------------------------
                case "edituser":
                    LinkedList<InstruccionCQL> listaEU = new LinkedList<InstruccionCQL>();

                    string operacionEU = hijo.ChildNodes.ElementAt(0).Token.Text;
                    operacionEU = operacionEU.ToLower().TrimEnd().TrimStart();

                    string uEU = hijo.ChildNodes.ElementAt(1).Token.Text;
                    uEU = uEU.ToLower().TrimEnd().TrimStart();

                    string dEU = hijo.ChildNodes.ElementAt(3).Token.Text;
                    dEU = dEU.ToLower().TrimEnd().TrimStart();

                    int lEU = hijo.ChildNodes.ElementAt(1).Token.Location.Line;
                    int cEU = hijo.ChildNodes.ElementAt(1).Token.Location.Line;

                    if (operacionEU.Equals("grant")) operacionEU = "GRANT";
                    else operacionEU = "REVOKE";

                    listaEU.AddLast(new EditUser(uEU, dEU, lEU, cEU, operacionEU));

                    return listaEU;




                //-------------------------------------------------- INSERT VALUES --------------------------------------------------------------------------
                case "ininsert":
                    LinkedList<InstruccionCQL> listaII = new LinkedList<InstruccionCQL>();
                    string idII ;
                    int lII ;
                    int cII;
                    idII = hijo.ChildNodes.ElementAt(2).Token.Text;
                    idII = idII.ToLower().TrimEnd().TrimStart();

                    lII = hijo.ChildNodes.ElementAt(2).Token.Location.Line;
                    cII = hijo.ChildNodes.ElementAt(2).Token.Location.Column;
                    if (hijo.ChildNodes.Count() == 5) listaII.AddLast(new Insert(idII, listaExpresiones(hijo.ChildNodes.ElementAt(4)), "NORMAL", lII, cII));
                    else listaII.AddLast(new Insert(idII, listaExpresiones(hijo.ChildNodes.ElementAt(5)), getCompuestas(hijo.ChildNodes.ElementAt(3)),"ESPECIAL", lII, cII));
                   

                    return listaII;


                //-------------------------------------------------- UPDATE --------------------------------------------------------------------------------------
                case "inupdate":
                    LinkedList<InstruccionCQL> listaUP = new LinkedList<InstruccionCQL>();
                    string idUP;
                    int lUP;
                    int cUP;

                    idUP = hijo.ChildNodes.ElementAt(1).Token.Text;
                    idUP = idUP.ToLower().TrimEnd().TrimStart();

                    lUP = hijo.ChildNodes.ElementAt(1).Token.Location.Line;
                    cUP = hijo.ChildNodes.ElementAt(1).Token.Location.Column;

                    LinkedList<SetCQL> listaSet = resolver_setcql(hijo.ChildNodes.ElementAt(3));
                    if(hijo.ChildNodes.Count() == 6) listaUP.AddLast(new Update(idUP, listaSet,resolver_expresion(hijo.ChildNodes.ElementAt(5)), lUP, cUP, "WHERE"));
                    else listaUP.AddLast(new Update(idUP, listaSet, lUP, cUP, "NORMAL"));
                    return listaUP;




                //----------------------------------------------------- DELETE -------------------------------------------------------------------------------------
                case "indelete":
                    LinkedList<InstruccionCQL> listaDE = new LinkedList<InstruccionCQL>();
                    string idDE = hijo.ChildNodes.ElementAt(2).Token.Text;
                    idDE = idDE.ToLower().TrimStart().TrimEnd();
                    int lDE = hijo.ChildNodes.ElementAt(2).Token.Location.Line;
                    int cDE = hijo.ChildNodes.ElementAt(2).Token.Location.Column;

                    if (hijo.ChildNodes.Count() == 5) listaDE.AddLast(new Delete(idDE, lDE, cDE, "WHERE", resolver_expresion(hijo.ChildNodes.ElementAt(4))));
                    else listaDE.AddLast(new Delete(idDE, lDE, cDE, "NORMAL"));

                    return listaDE;







                
                //------------------------------------------------------- SELECT -------------------------------------------------------------------------------------
                case "inselect":
                    LinkedList<InstruccionCQL> listaSE = new LinkedList<InstruccionCQL>();
                    string idSE = hijo.ChildNodes.ElementAt(3).Token.Text;
                    idSE = idSE.ToLower().TrimEnd().TrimStart();

                    int lSE = hijo.ChildNodes.ElementAt(3).Token.Location.Line;
                    int cSE = hijo.ChildNodes.ElementAt(3).Token.Location.Column;

                    string tkSE = hijo.ChildNodes.ElementAt(1).ChildNodes.ElementAt(0).Term.Name;
                    if(hijo.ChildNodes.Count() == 5)
                    {
                        string operaSS = hijo.ChildNodes.ElementAt(4).Term.Name;
                        if (operaSS.Equals("inwhere"))
                        {
                            ParseTreeNode accion = hijo.ChildNodes.ElementAt(4);
                            if (tkSE.Equals("listvalues")) listaSE.AddLast(new Select(idSE, listaExpresiones(hijo.ChildNodes.ElementAt(1).ChildNodes.ElementAt(0)), lSE, cSE, "a",
                                resolver_expresion(accion.ChildNodes.ElementAt(1)),null));
                            else listaSE.AddLast(new Select(idSE, null, lSE, cSE,"a", resolver_expresion(accion.ChildNodes.ElementAt(1)),null));
                        }
                        else if (operaSS.Equals("inlimit"))
                        {
                            ParseTreeNode accion = hijo.ChildNodes.ElementAt(4);
                            if (tkSE.Equals("listvalues")) listaSE.AddLast(new Select(idSE, listaExpresiones(hijo.ChildNodes.ElementAt(1).ChildNodes.ElementAt(0)), lSE, cSE, "c",
                               null, resolver_expresion(accion.ChildNodes.ElementAt(1))));
                            else listaSE.AddLast(new Select(idSE, null, lSE, cSE, "c",null, resolver_expresion(accion.ChildNodes.ElementAt(1))));
                        }
                        else if (operaSS.Equals("inorder"))
                        {
                            ParseTreeNode accion = hijo.ChildNodes.ElementAt(4);
                            if (tkSE.Equals("listvalues")) listaSE.AddLast(new Select(idSE, listaExpresiones(hijo.ChildNodes.ElementAt(1).ChildNodes.ElementAt(0)), lSE, cSE, "b",
                                getOrdenamiento(accion)));
                            else listaSE.AddLast(new Select(idSE, null, lSE, cSE, "b", getOrdenamiento(accion)));
                        }

                    }
                    else if(hijo.ChildNodes.Count() == 6)
                    {
                        string op1 = hijo.ChildNodes.ElementAt(4).Term.Name;
                        string op2 = hijo.ChildNodes.ElementAt(5).Term.Name;
                        if(op1.Equals("inwhere") && op2.Equals("inorder"))
                        {
                            ParseTreeNode accion = hijo.ChildNodes.ElementAt(4);
                            ParseTreeNode accion2 = hijo.ChildNodes.ElementAt(5);
                            if (tkSE.Equals("listvalues")) listaSE.AddLast(new Select(idSE, listaExpresiones(hijo.ChildNodes.ElementAt(1).ChildNodes.ElementAt(0)), lSE, cSE, "ab",
                                resolver_expresion(accion.ChildNodes.ElementAt(1)),null,getOrdenamiento(accion2)));
                            else listaSE.AddLast(new Select(idSE, null, lSE, cSE, "ab", resolver_expresion(accion.ChildNodes.ElementAt(1)),null,getOrdenamiento(accion2)));
                        }
                        else if (op1.Equals("inwhere") && op2.Equals("inlimit"))
                        {
                            ParseTreeNode accion = hijo.ChildNodes.ElementAt(4);
                            ParseTreeNode accion2 = hijo.ChildNodes.ElementAt(5);
                            if (tkSE.Equals("listvalues")) listaSE.AddLast(new Select(idSE, listaExpresiones(hijo.ChildNodes.ElementAt(1).ChildNodes.ElementAt(0)), lSE, cSE, "ac",
                                resolver_expresion(accion.ChildNodes.ElementAt(1)), resolver_expresion(accion2.ChildNodes.ElementAt(1))));
                            else listaSE.AddLast(new Select(idSE, null, lSE, cSE, "ac", resolver_expresion(accion.ChildNodes.ElementAt(1)), resolver_expresion(accion2.ChildNodes.ElementAt(1))));
                        }
                        else
                        {
                            ParseTreeNode accion = hijo.ChildNodes.ElementAt(5);
                            ParseTreeNode accion2 = hijo.ChildNodes.ElementAt(4);
                            if (tkSE.Equals("listvalues")) listaSE.AddLast(new Select(idSE, listaExpresiones(hijo.ChildNodes.ElementAt(1).ChildNodes.ElementAt(0)), lSE, cSE, "bc",
                                null,resolver_expresion(accion.ChildNodes.ElementAt(1)), getOrdenamiento(accion2)));
                            else listaSE.AddLast(new Select(idSE, null, lSE, cSE, "bc", null, resolver_expresion(accion.ChildNodes.ElementAt(1)), getOrdenamiento(accion2)));
                        }
                    }
                    else if (hijo.ChildNodes.Count() == 7)
                    {
                        ParseTreeNode accion = hijo.ChildNodes.ElementAt(4);
                        ParseTreeNode accion2 = hijo.ChildNodes.ElementAt(6);
                        ParseTreeNode accion3 = hijo.ChildNodes.ElementAt(5);
                        if (tkSE.Equals("listvalues")) listaSE.AddLast(new Select(idSE, listaExpresiones(hijo.ChildNodes.ElementAt(1).ChildNodes.ElementAt(0)), lSE, cSE, "abc",
                            resolver_expresion(accion.ChildNodes.ElementAt(1)), resolver_expresion(accion2.ChildNodes.ElementAt(1)),getOrdenamiento(accion3)));
                        else listaSE.AddLast(new Select(idSE, null, lSE, cSE, "abc", resolver_expresion(accion.ChildNodes.ElementAt(1)), resolver_expresion(accion2.ChildNodes.ElementAt(1)), getOrdenamiento(accion3)));
                    }
                    else
                    {
                        if (tkSE.Equals("listvalues")) listaSE.AddLast(new Select(idSE, listaExpresiones(hijo.ChildNodes.ElementAt(1).ChildNodes.ElementAt(0)), lSE, cSE,"none"));
                        else listaSE.AddLast(new Select(idSE, null, lSE, cSE,"none"));
                    }
                   

                    return listaSE;
            }
            return null;


        }

        /*
         * Metodo que obtendra todas las columnas con las cuales se ordenara
         * @param {raiz} es el sub arbol a analizar
         * @return una lista de OrderBy
         */
        private LinkedList<OrderBy> getOrdenamiento(ParseTreeNode raiz)
        {
            LinkedList<OrderBy> lista = new LinkedList<OrderBy>();
            ParseTreeNode hijo;
            if (raiz.ChildNodes.Count() == 2)
            {
                lista = getOrdenamiento(raiz.ChildNodes.ElementAt(0));
                hijo = raiz.ChildNodes.ElementAt(1);
            }
            else hijo = raiz.ChildNodes.ElementAt(2);

            string id = hijo.ChildNodes.ElementAt(0).Token.Text;
            id = id.ToLower().TrimEnd().TrimStart();
            if (hijo.ChildNodes.Count() == 2)
            {
                Boolean asc = false;
                string token = hijo.ChildNodes.ElementAt(1).Token.Text;
                token = token.ToLower().TrimEnd().TrimStart();
                if (token.Equals("asc")) asc = true;
                lista.AddLast(new OrderBy(id,asc));
            }
            else lista.AddLast(new OrderBy(id, false));
            return lista;
        }

        /*
         * Metodo que analiza los set
         * @raiz subarbol a analizar
         * retorna una lista de la clase SETCQL
         */


        private LinkedList<SetCQL> resolver_setcql(ParseTreeNode raiz)
        {
            LinkedList<SetCQL> lista = new LinkedList<SetCQL>();
            string id;
            Expresion expresion;
            int l;
            int c;
            if(raiz.ChildNodes.Count() == 4)
            {
                lista = resolver_setcql(raiz.ChildNodes.ElementAt(0));
                string token = raiz.ChildNodes.ElementAt(1).Term.Name;
                if (token.Equals("usertypecql"))
                {
                    id = raiz.ChildNodes.ElementAt(1).ChildNodes.ElementAt(1).Token.Text;
                    id = id.ToLower().TrimStart().TrimEnd();
                    expresion = resolver_expresion(raiz.ChildNodes.ElementAt(3));
                    l = raiz.ChildNodes.ElementAt(1).ChildNodes.ElementAt(1).Token.Location.Line;
                    c = raiz.ChildNodes.ElementAt(1).ChildNodes.ElementAt(1).Token.Location.Column;
                }
                id = raiz.ChildNodes.ElementAt(1).Token.Text;
                id = id.ToLower().TrimStart().TrimEnd();
                expresion = resolver_expresion(raiz.ChildNodes.ElementAt(3));
                l = raiz.ChildNodes.ElementAt(1).Token.Location.Line;
                c = raiz.ChildNodes.ElementAt(1).Token.Location.Column;
            }
            else
            {
                string token = raiz.ChildNodes.ElementAt(0).Term.Name;
                if (token.Equals("usertypecql"))
                {
                    id = raiz.ChildNodes.ElementAt(0).ChildNodes.ElementAt(2).Token.Text;
                    id = id.ToLower().TrimStart().TrimEnd();
                    expresion = resolver_expresion(raiz.ChildNodes.ElementAt(2));
                    l = raiz.ChildNodes.ElementAt(0).ChildNodes.ElementAt(1).Token.Location.Line;
                    c = raiz.ChildNodes.ElementAt(0).ChildNodes.ElementAt(1).Token.Location.Column;
                    lista.AddLast(new SetCQL(id, expresion, resolver_expresion(raiz.ChildNodes.ElementAt(0).ChildNodes.ElementAt(0)), "USER", l, c));
                    return lista;
                }
                id = raiz.ChildNodes.ElementAt(0).Token.Text;
                id = id.ToLower().TrimStart().TrimEnd();
                expresion = resolver_expresion(raiz.ChildNodes.ElementAt(2));
                l = raiz.ChildNodes.ElementAt(0).Token.Location.Line;
                c = raiz.ChildNodes.ElementAt(0).Token.Location.Column;
            }
            lista.AddLast(new SetCQL(id, expresion,"NORMAL" ,l, c));
            return lista;
        }



        /*
         *Metodo que recorre todos los valores a guardar en un insert
         * @raiz nodo del subarbol a recorrer
         * retonar un lista de expresiones
         */

         private LinkedList<Expresion> listaExpresiones(ParseTreeNode raiz)
        {
            LinkedList<Expresion> lista = new LinkedList<Expresion>();
            if (raiz.ChildNodes.Count() == 2)
            {
                lista = listaExpresiones(raiz.ChildNodes.ElementAt(0));
                lista.AddLast(resolver_expresion(raiz.ChildNodes.ElementAt(1)));
            }
            else lista.AddLast(resolver_expresion(raiz.ChildNodes.ElementAt(0)));
            return lista;

        }



        /*
         * Metodo que obtiene una lista de las primarias compuestas
         * @raiz nodo del sub arbol a analizar
         */


        public LinkedList<string> getCompuestas(ParseTreeNode raiz)
        {
            LinkedList<string> lista = new LinkedList<string>();
            string nombre;
            if (raiz.ChildNodes.Count() == 2)
            {
                lista = getCompuestas(raiz.ChildNodes.ElementAt(0));
                nombre = raiz.ChildNodes.ElementAt(1).Token.Text;
                nombre = nombre.ToLower().TrimEnd().TrimStart();
            }
            else
            {
                nombre = raiz.ChildNodes.ElementAt(0).Token.Text;
                nombre = nombre.ToLower().TrimEnd().TrimStart();
            }
            lista.AddLast(nombre);
            return lista;
        }

        /*
         * Metodo que obtiene una lista de columnas
         * @raiz nodo del sub arbol a analizar
         */

        public LinkedList<Columna> getListaColumna(ParseTreeNode raiz)
        {
            LinkedList<Columna> lista = new LinkedList<Columna>();
            string token = raiz.ChildNodes.ElementAt(0).Term.Name;
            string nombre;
            string tipo;
            Boolean pk = false;
            if (token.Equals("defcolumn"))
            {
                lista = getListaColumna(raiz.ChildNodes.ElementAt(0));
                if (raiz.ChildNodes.Count() == 5) pk = true;

                nombre = raiz.ChildNodes.ElementAt(1).Token.Text;
                nombre = nombre.ToLower().TrimEnd().TrimStart();

                tipo = raiz.ChildNodes.ElementAt(2).ChildNodes.ElementAt(0).Token.Text;
                tipo = tipo.ToLower().TrimEnd().TrimStart();

            }
            else
            {
                if (raiz.ChildNodes.Count() == 4) pk = true;
                nombre = raiz.ChildNodes.ElementAt(0).Token.Text;
                nombre = nombre.ToLower().TrimEnd().TrimStart();

                tipo = raiz.ChildNodes.ElementAt(1).ChildNodes.ElementAt(0).Token.Text;
                tipo = tipo.ToLower().TrimEnd().TrimStart();
            }
            lista.AddLast(new Columna(nombre, tipo, pk));
            return lista;
        }

        /*
         * Metodo que obtiene una lista de columnas
         * @raiz nodo del sub arbol a analizar
         */

        public LinkedList<Columna> getListaColumnaAdd(ParseTreeNode raiz)
        {
            LinkedList<Columna> lista = new LinkedList<Columna>();
            string nombre;
            string tipo;
            Boolean pk = false;
            if (raiz.ChildNodes.Count() == 3)
            {
                lista = getListaColumnaAdd(raiz.ChildNodes.ElementAt(0));

                nombre = raiz.ChildNodes.ElementAt(1).Token.Text;
                nombre = nombre.ToLower().TrimEnd().TrimStart();

                tipo = raiz.ChildNodes.ElementAt(2).ChildNodes.ElementAt(0).Token.Text;
                tipo = tipo.ToLower().TrimEnd().TrimStart();

            }
            else
            {
                nombre = raiz.ChildNodes.ElementAt(0).Token.Text;
                nombre = nombre.ToLower().TrimEnd().TrimStart();

                tipo = raiz.ChildNodes.ElementAt(1).ChildNodes.ElementAt(0).Token.Text;
                tipo = tipo.ToLower().TrimEnd().TrimStart();
            }
            lista.AddLast(new Columna(nombre, tipo, pk));
            return lista;
        }


        /*
         * Metodo que obtiene los atributos de un Usertype
         * @raiz nodo del arbol donde se encuentran los atributos
         */

        public LinkedList<Attrs> getListaUserType(ParseTreeNode raiz)
        {
            LinkedList<Attrs> lista = new LinkedList<Attrs>();
            string id = "";
            string tipo = "";
            if (raiz.ChildNodes.Count() == 3)
            {
                lista = getListaUserType(raiz.ChildNodes.ElementAt(0));

                id = raiz.ChildNodes.ElementAt(1).Token.Text;
                id = id.ToLower().TrimEnd().TrimStart();

                tipo = raiz.ChildNodes.ElementAt(2).ChildNodes.ElementAt(0).Token.Text;
                tipo = tipo.ToLower().TrimEnd().TrimStart();
            }
            else
            {
                id = raiz.ChildNodes.ElementAt(0).Token.Text;
                id = id.ToLower().TrimEnd().TrimStart();

                tipo = raiz.ChildNodes.ElementAt(1).ChildNodes.ElementAt(0).Token.Text;
                tipo = tipo.ToLower().TrimEnd().TrimStart();
            }

            Attrs a = new Attrs(id, tipo);
            lista.AddLast(a);
            return lista;

        }




        /*
         * Metodo que resuelve las expresiones aritmeticas,logicas
         * @raiz nodo principal de la lista de expresiones
         */

        public Expresion resolver_expresion(ParseTreeNode raiz)
        {
            if (raiz.ChildNodes.Count() == 3)
            {
                string toketemp = raiz.ChildNodes.ElementAt(1).Token.Text;
                string iden = raiz.ChildNodes.ElementAt(1).Term.Name;
                if (toketemp.Equals("."))
                {
                    string sepa = raiz.ChildNodes.ElementAt(0).Term.Name;
                    string opee = "";
                    if (sepa.Equals("asignaciona") && !flagEx) opee = "GETATRIBUTO";
                    else opee = "ACCESOUSER";
                    int le = raiz.ChildNodes.ElementAt(2).Token.Location.Line;
                    int ce = raiz.ChildNodes.ElementAt(2).Token.Location.Column;
                    string idA = raiz.ChildNodes.ElementAt(2).Token.Text;
                    idA = idA.ToLower().TrimEnd().TrimStart();
                    return new Expresion(resolver_expresion(raiz.ChildNodes.ElementAt(0)), opee, le, ce, idA);
                }
                else if (iden.Equals("ID"))
                {
                    string idin = raiz.ChildNodes.ElementAt(1).Token.Text;
                    idin = idin.ToLower().TrimEnd().TrimStart();
                    string accio = raiz.ChildNodes.ElementAt(2).Token.Text;
                    int ln = raiz.ChildNodes.ElementAt(1).Token.Location.Line;
                    int cn = raiz.ChildNodes.ElementAt(1).Token.Location.Column;
                    if (accio.Equals("++")) return new Expresion(new Expresion(idin, "ID", ln, cn), "INCREMENTO", ln, cn, idin);
                    else return new Expresion(new Expresion(idin, "ID", ln, cn), "DECREMENTO", ln, cn, idin);
                    
                }
                string toke = raiz.ChildNodes.ElementAt(1).Token.Text;
                int l1 = raiz.ChildNodes.ElementAt(1).Token.Location.Line;
                int c1 = raiz.ChildNodes.ElementAt(1).Token.Location.Column;
                return new Expresion(resolver_expresion(raiz.ChildNodes.ElementAt(0)), resolver_expresion(raiz.ChildNodes.ElementAt(2)), getOperacion(toke), l1, c1);

            }
            else if (raiz.ChildNodes.Count() == 2)
            {
                string iden = raiz.ChildNodes.ElementAt(0).Term.Name;
                if (iden.Equals("tipovariable"))
                {
                    int l1 = raiz.ChildNodes.ElementAt(0).ChildNodes.ElementAt(0).Token.Location.Line;
                    int c1 = raiz.ChildNodes.ElementAt(0).ChildNodes.ElementAt(0).Token.Location.Column;
                    string tipov = raiz.ChildNodes.ElementAt(0).ChildNodes.ElementAt(0).Token.Text.TrimEnd().TrimStart().ToLower();
                    return new Expresion(resolver_expresion(raiz.ChildNodes.ElementAt(1)), "CONVERSION", l1, c1, tipov);
                }
                else
                {
                    string toke = raiz.ChildNodes.ElementAt(0).Token.Text;
                    string idE = raiz.ChildNodes.ElementAt(1).Term.Name;
                    if (toke.Equals("new"))
                    {
                        int ln = raiz.ChildNodes.ElementAt(1).Token.Location.Line;
                        int cn = raiz.ChildNodes.ElementAt(1).Token.Location.Column;
                        string tipoA = raiz.ChildNodes.ElementAt(1).Token.Text;
                        tipoA = tipoA.ToLower().TrimEnd().TrimStart();

                        return new Expresion("INSTANCIA", ln, cn, tipoA);
                    }
                    else if (idE.Equals("ID"))
                    {
                        string valor = raiz.ChildNodes.ElementAt(1).Token.Text;
                        int l11 = raiz.ChildNodes.ElementAt(1).Token.Location.Line;
                        int c11 = raiz.ChildNodes.ElementAt(1).Token.Location.Column;

                        return getValor("id2", valor, l11, c11);
                    }
                    int l1 = raiz.ChildNodes.ElementAt(0).Token.Location.Line;
                    int c1 = raiz.ChildNodes.ElementAt(0).Token.Location.Column;
                    string opera = "";
                    if (toke.Equals("-")) opera = "NEGATIVO";
                    else if (toke.Equals("!")) opera = "NEGACION";
                    return new Expresion(resolver_expresion(raiz.ChildNodes.ElementAt(1)), opera, l1, c1);
                }

            }
            else if (raiz.ChildNodes.Count() == 5)
            {
                string idDiferenciador = raiz.ChildNodes.ElementAt(0).Term.Name;
                //--------------------------------------------- OPERACION TERNARIA ---------------------------------------------------------
                if (idDiferenciador.Equals("expresion"))
                {
                    int lineaT = raiz.ChildNodes.ElementAt(1).Token.Location.Line;
                    int columnaT = raiz.ChildNodes.ElementAt(1).Token.Location.Line;
                    return new Expresion(resolver_expresion(raiz.ChildNodes.ElementAt(2)),
                        resolver_expresion(raiz.ChildNodes.ElementAt(4)),
                        resolver_expresion(raiz.ChildNodes.ElementAt(0)), "TERNARIO", lineaT, columnaT);
                }
                LinkedList<Expresion> lista = resolver_user_type(raiz.ChildNodes.ElementAt(1));
                string idAs = raiz.ChildNodes.ElementAt(4).Token.Text;
                idAs = idAs.ToLower().TrimEnd().TrimStart();
                int lAs = raiz.ChildNodes.ElementAt(4).Token.Location.Line;
                int cAs = raiz.ChildNodes.ElementAt(4).Token.Location.Column;

                return new Expresion("ASIGNACIONUSER", lAs, cAs, lista, idAs);
            }
            else if(raiz.ChildNodes.Count() == 4)
            {
                string token = raiz.ChildNodes.ElementAt(0).Token.Text;
                token = token.ToLower().TrimEnd().TrimStart();
                int lc = raiz.ChildNodes.ElementAt(0).Token.Location.Line;
                int cc = raiz.ChildNodes.ElementAt(0).Token.Location.Column;
                string salida = "";
                if (token.Equals("count")) salida = "COUNT";
                else if (token.Equals("min")) salida = "MIN";
                else if (token.Equals("max")) salida = "MAX";
                else if (token.Equals("sum")) salida = "SUM";
                else salida = "AVG";
                LinkedList<InstruccionCQL> ins = (LinkedList<InstruccionCQL>)instruccion(raiz.ChildNodes.ElementAt(2));
                if (ins.Count() > 0) return new Expresion(ins.ElementAt(0), salida, lc, cc);
                return null;
            }
            else
            {
                string toke = raiz.ChildNodes.ElementAt(0).Term.Name;
                if (toke.Equals("expresion")) return resolver_expresion(raiz.ChildNodes.ElementAt(0));
                string valor = raiz.ChildNodes.ElementAt(0).Token.Text;
                int l1 = raiz.ChildNodes.ElementAt(0).Token.Location.Line;
                int c1 = raiz.ChildNodes.ElementAt(0).Token.Location.Column;

                return getValor(toke, valor, l1, c1);
            }
        }

        /*
         * Metodo que se encarga de regresar un lista de expresiones para asignarsela a un UserType
         * @raiz nodo del arbol para recorrer
         * @return LinkedList<Expresiones> 
         */

        public LinkedList<Expresion> resolver_user_type(ParseTreeNode raiz)
        {
            LinkedList<Expresion> lista = new LinkedList<Expresion>();
            ParseTreeNode hijo;
            if (raiz.ChildNodes.Count() == 2)
            {
                lista = resolver_user_type(raiz.ChildNodes.ElementAt(0));
                hijo = raiz.ChildNodes.ElementAt(1);
            }
            else hijo = raiz.ChildNodes.ElementAt(0);
            lista.AddLast(resolver_expresion(hijo));
            return lista;

        }


        /*
         *  METODO PARA CREAR EL ARCHIVO ASTCQL.TXT
         *  @raiz el la raiz del arbol principal
         */

        public void graficar(ParseTreeNode raiz)
        {
            System.IO.StreamWriter f = new System.IO.StreamWriter("Reportes/ASTCQL.txt");
            f.Write("digraph Arbol{ rankdir=TB; \n node[shape = box, style = filled, color = white];");
            recorrer(raiz, f);
            f.Write("\n}");
            f.Close();
        }


        /*
         *  Metodo que recorre el arbol y escribe en el txt la informacion en Graphviz
         *  @raiz el la raiz del arbol principal
         *  @f es un StreamWriter que nos permitira escribir en el archivo
         */
        public static void recorrer(ParseTreeNode raiz, System.IO.StreamWriter f)
        {
            if (raiz != null)
            {
                f.Write("nodo" + raiz.GetHashCode() + "[label=\"" + raiz.ToString().Replace("\"", "\\\"") + " \", fillcolor=\"LightBlue\", style =\"filled\", shape=\"box\"]; \n");
                if (raiz.ChildNodes.Count > 0)
                {
                    ParseTreeNode[] hijos = raiz.ChildNodes.ToArray();
                    for (int i = 0; i < raiz.ChildNodes.Count; i++)
                    {
                        recorrer(hijos[i], f);
                        f.Write("\"nodo" + raiz.GetHashCode() + "\"-> \"nodo" + hijos[i].GetHashCode() + "\" \n");
                    }
                }
            }

        }


        /*
         * Metodo que devulve que operacion es
         * @raiz es el nodo a buscar su operacion
         */
        public string getOperacion(string token)
        {
            if (token.Equals("+")) return "SUMA";
            else if (token.Equals("-")) return "RESTA";
            else if (token.Equals("*")) return "MULTIPLICACION";
            else if (token.Equals("**")) return "POTENCIA";
            else if (token.Equals("%")) return "MODULO";
            else if (token.Equals("/")) return "DIVISION";
            else if (token.Equals(">")) return "MAYOR";
            else if (token.Equals("<")) return "MENOR";
            else if (token.Equals(">=")) return "MAYORIGUAL";
            else if (token.Equals("<=")) return "MENORIGUAL";
            else if (token.Equals("==")) return "IGUALIGUAL";
            else if (token.Equals("!=")) return "DIFERENTE";
            else if (token.Equals("||")) return "OR";
            else if (token.Equals("&&")) return "AND";
            else if (token.Equals("^")) return "XOR";
            return "none";
        }


        /*
         * Metodo que devuelve que tipo de valor es
         * @raiz es el nodo a buscar su valor
         */
        public Expresion getValor(string token, string valor, int l1, int c1)
        {
            token = token.ToLower().TrimEnd().TrimStart();
            string valorT = valor.ToLower().TrimEnd().TrimStart();
            if (token.Equals("entero")) return new Expresion(valor, "ENTERO", l1, c1);
            else if (token.Equals("decimal")) return new Expresion(valor, "DECIMAL", l1, c1);
            else if (token.Equals("cadena"))
            {
                valor = valor.TrimEnd('"');
                valor = valor.TrimStart('"');
                return new Expresion(valor, "CADENA", l1, c1);
            }
            else if (token.Equals("hora"))
            {
                valor = valor.TrimEnd('\'');
                valor = valor.TrimStart('\'');
                return new Expresion(valor, "HORA", l1, c1);
            }
            else if (token.Equals("fecha"))
            {
                valor = valor.TrimEnd('\'');
                valor = valor.TrimStart('\'');
                return new Expresion(valor, "FECHA", l1, c1);
            }
            else if (valorT.Equals("true") || valorT.Equals("false")) return new Expresion(valor, "BOOLEAN", l1, c1);
            else if (token.Equals("id")) return new Expresion(valor, "IDTABLA", l1, c1);
            else if (token.Equals("id2")) return new Expresion(valor, "ID", l1, c1);
            else return new Expresion(null, "NULL", l1, c1);
        }

        /*
         * Metodo que me devuelve el tipo de declaracion
         * @raiz el es nodo a recorrer del arbol
         */

        public string declaracionTipo(ParseTreeNode raiz)
        {
            string token = raiz.Term.Name;
            if (token.Equals("declaracion")) return declaracionTipo(raiz.ChildNodes.ElementAt(0));

            string t = raiz.ChildNodes.ElementAt(0).Token.Text;
            return t.ToLower().TrimEnd().TrimStart();


        }


        /*
         * Metodo que recorre el arbol y devuelve un LinkedList de IF/ELSEIF/ELSE
         * @raiz sub arbol a analizar
         */
        public LinkedList<SubIf> resolver_if(ParseTreeNode raiz)
        {
            LinkedList<SubIf> lista = new LinkedList<SubIf>();
            string token = raiz.ChildNodes.ElementAt(0).Term.Name;
            LinkedList<InstruccionCQL> cuerpo = null;
            Expresion condicion = null;
            int l = 0;
            int c = 0;
            if (token.Equals("elseif"))
            {
                lista = resolver_if(raiz.ChildNodes.ElementAt(0));
                l = raiz.ChildNodes.ElementAt(2).Token.Location.Line;
                c = raiz.ChildNodes.ElementAt(2).Token.Location.Line;
                condicion = resolver_expresion(raiz.ChildNodes.ElementAt(3));
                cuerpo = instrucciones(raiz.ChildNodes.ElementAt(5));
            }
            else
            {
                l = raiz.ChildNodes.ElementAt(1).Token.Location.Line;
                c = raiz.ChildNodes.ElementAt(1).Token.Location.Line;
                condicion = resolver_expresion(raiz.ChildNodes.ElementAt(2));
                cuerpo = instrucciones(raiz.ChildNodes.ElementAt(4));
            }

            lista.AddLast(new SubIf(condicion, cuerpo, l, c));
            return lista;
        }

        /*
         * Metodo que recorre un sub arbol y devulve un LinkedList de CASE/DEFAULT
         * @raiz subarbol a analizar
         * @condicion condicion del swithc
         */

        public LinkedList<Case> resolver_case(ParseTreeNode raiz,Expresion condicion)
        {
            LinkedList<Case> lista = new LinkedList<Case>();
            LinkedList<InstruccionCQL> cuerpo = new LinkedList<InstruccionCQL>();
            Expresion condicionG;
            int linea = 0;
            int columna = 0;
            ParseTreeNode hijo;
            if(raiz.ChildNodes.Count() == 2)
            {
                lista = resolver_case(raiz.ChildNodes.ElementAt(0),condicion);

                linea = raiz.ChildNodes.ElementAt(1).ChildNodes.ElementAt(0).Token.Location.Line;
                columna = raiz.ChildNodes.ElementAt(1).ChildNodes.ElementAt(0).Token.Location.Line;
                cuerpo = instrucciones(raiz.ChildNodes.ElementAt(1).ChildNodes.ElementAt(3));
                hijo = raiz.ChildNodes.ElementAt(1);
            }
            else
            {
                linea = raiz.ChildNodes.ElementAt(0).ChildNodes.ElementAt(0).Token.Location.Line;
                columna = raiz.ChildNodes.ElementAt(0).ChildNodes.ElementAt(0).Token.Location.Line;
                cuerpo = instrucciones(raiz.ChildNodes.ElementAt(0).ChildNodes.ElementAt(3));
                hijo = raiz.ChildNodes.ElementAt(0);    
            }
            condicionG = new Expresion(condicion, resolver_expresion(hijo.ChildNodes.ElementAt(1)), "IGUALIGUAL", linea, columna);
            if (hijo.ChildNodes.Count() == 5) cuerpo.AddLast(new Break());
            lista.AddLast(new Case(condicionG, cuerpo, linea, columna));
            return lista;
        }
    }
}
