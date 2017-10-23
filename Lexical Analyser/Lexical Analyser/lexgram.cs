using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Lexical_Analyser
{
    public class lexgram
    {
        public string kod ="";
        public string[] bosluk;
        public bool acceptStatement = true;
        public string output = "<input>";
        public int b = 0;

        public void change()
        {
            for (int i = 0; i<bosluk.Length ;i++ )
            {
                bosluk[i] = decide(bosluk[i]);
            }
        }

        public void trimmer()
        {
            kod = kod.Trim();
            kod = kod.Replace("(", " ");
            kod = kod.Replace(")", " ");
            kod = kod.Replace(":="," := ");
            kod = kod.Replace("=", "= ");
            kod = kod.Replace(">", " > ");
            kod = kod.Replace("<", " < ");
            kod = kod.Replace("> =", " >= ");
            kod = kod.Replace("< =", " <= ");
            kod = kod.Replace("< >","<>");
            kod = kod.Replace("+"," + ");
            kod = kod.Replace("-", " - ");
            kod = kod.Replace("*", " * ");
            kod = kod.Replace("/", " / ");
            while(kod.Contains("  "))kod = kod.Replace("  "," ");
            while(kod.Contains("\t"))kod = kod.Replace("\t", "");
            while(kod.Contains("\n "))kod = kod.Replace("\n ","\n");
            while (kod.Contains(" \n")) kod = kod.Replace(" \n", "\n");
            while (kod.Contains("\n\n")) kod = kod.Replace("\n\n", "\n");
            kod = kod.Replace("\n"," ");
            kod = kod.ToLower();
            kod = kod.Trim();
            bosluk = kod.Split(' ');
            change();
        }

        public string decide(string abbas) //decide if string is identifier or value with the help of regex
        {
            Match int_value = Regex.Match(abbas, @"^(\d)(\d*)$");
            Match identifier = Regex.Match(abbas, @"^(\D+)(\d*)(\w*)$");
            //Match işlem = Regex.Match(abbas, @"+*-*/*");
            //Match karşılaştır = Regex.Match(abbas, @"^(>*)|(<*)|(>=*)|(<=*)|(<>*)$");
            switch(abbas)
            {
                case "program":
                case "begin":
                case "end":
                case "then":
                case "if":
                case "while":
                case "else":
                case ":=":
                case "+":
                case "-":
                case "*":
                case "/":
                case ">":
                case "<":
                case ">=":
                case "<=":
                case "<>":
                    return abbas;
                default:
                    break;

            }
            
            if (identifier.Success)
            {
                return "<identifier>";
            }
            else if (int_value.Success)
            {
                return "<int_value>";
            }
            else 
            {
                acceptStatement = false;
                return "olmadı";
            }
        }
        
        public void check()
            {
            
                    if (kod.Contains("program"))
                    {
                        output += "(<main>";
                        s_main();
                        switcher();
                        output += ")end";
                    }
                    else
                    {
                        output += "(<stmt>(";
                        s_stmt();
                    
                        switcher();
                        output += ")";
                    }
            
            }
        
        void switcher()
        {
            if (b < bosluk.Length)
            {
                switch (bosluk[b])
                {
                    case "<identifier>":
                        output += "<stmt>(";
                        s_stmt();
                        output += ")";
                        break;
                    case "while":
                        output += "<stmt>(";
                        s_stmt();
                        output += ")";
                        break;
                    case "if":
                        output += "<s_stmt>(";
                        s_stmt();
                        output += ")";
                        break;
                    default:
                        b++;
                        break;
                }
                if (acceptStatement == true && b<bosluk.Length)
                    switcher();
            }
        }

        public void BeginWhileEndKontrol()
        {
            if (kod.Contains("program"))
            {
                if (kod.Contains("while"))
                {
                    if (kod.Contains("begin"))
                    {
                        if (TextTool.CountStringOccurrences(kod, "end") == 2)
                            acceptStatement = true;
                        else
                            acceptStatement = false;
                    }
                    else
                        acceptStatement = false;
                }
                else
                {
                    if (TextTool.CountStringOccurrences(kod, "end") == 1)
                        acceptStatement = true;
                    else
                        acceptStatement = false;
                }


            }
            else
            {
                if (kod.Contains("while"))
                {
                    if (kod.Contains("begin"))
                    {
                        if (TextTool.CountStringOccurrences(kod, "end") == 1)
                            acceptStatement = true;
                        else
                            acceptStatement = false;
                    }
                    else
                        acceptStatement = false;
                }
                else
                {
                    if (TextTool.CountStringOccurrences(kod, "end") == 0)
                        acceptStatement = true;
                    else
                        acceptStatement = false;
                }
            }
        }

        void s_main()
        {
            output += "(program)";
            ++b;
            if (bosluk[b] == "<identifier>")
            {
                s_identifier();
            }
            else
            {
                acceptStatement = false;
                return;
            }
        }
        void s_stmt()
        {
            if (bosluk[b] == "if")
            {
                output += "<if_stmt>(";
                s_if_stmt();
                output += ")";
            }
            else if (bosluk[b] == "while")
            {
                output += "<while_stmt>(";
                s_while_stmt();
                output += ")";
            }
            else if (bosluk[b + 1] == ":=") //assignment
            {
                output += "<assign>(";
                s_assign();
                output += ")";

            }
            else
            {
                acceptStatement = false;
                return;
            }
        }
        void s_assign()
        {
            if (bosluk[b] == "<identifier>")
            {
                s_identifier();
                output += bosluk[b++];
                output += "<expr>(";
                s_expr();
                output += ")";
            }
            else
            {
                acceptStatement = false;
                return;
            }
            
        }
        void s_if_stmt()
        {
            ++b;
            output += "if(<cond>(";
            s_cond();
            output += "))";
            if (b + 1 < bosluk.Length)
            {
                output += "then<stmt>";
                ++b;
                s_stmt();
            }
            else
            {
                acceptStatement = false;
                return;
            }

        }
        void s_while_stmt()
        {
            ++b;
            output += "while(<cond>(";
            s_cond();
            output += ")<block>(";
            s_block();
            output += ")";
        }
        void s_cond()
        {
            s_identifier();
            output += bosluk[b++];
            s_identifier();
        }
        void s_block()
        {
            if (bosluk[b] == "begin")
            {
                output += "begin<stmt>(";
                b++;
                s_stmt();
            }
            else
            {
                acceptStatement = false;
                return;
            }

            if (bosluk[b] == "end")
            {
                output += ")end";
            }
            else
            {
                switcher();
                output += ")end";
            }
        }
        void s_expr()
        {
            if (bosluk[b] == "<identifier>")
            {
                if (b + 1 < bosluk.Length)
                {
                    if (bosluk[b + 1] != "+" && bosluk[b + 1] != "-" && bosluk[b + 1] != "*" && bosluk[b + 1] != "/")
                    {
                        s_identifier();
                    }
                    else
                    {
                        b++;
                        output += "<expr>(";
                        s_expr();
                        //output += ")";
                    }
                }
                else
                {
                    s_identifier();
                }

            }
            else if (bosluk[b] == "<int_value>")
            {
                if (b + 1 < bosluk.Length)
                {
                    if (bosluk[b + 1] != "+" && bosluk[b + 1] != "-" && bosluk[b + 1] != "*" && bosluk[b + 1] != "/")
                    {
                        s_int_value();
                    }
                    else
                    {
                        b++;
                        output += "<expr>(";
                        s_expr();
                        output += ")";
                    }
                }
                else
                {
                    s_int_value();
                }

            }
            else if (bosluk[b] == "+" || bosluk[b] == "-" || bosluk[b] == "*" || bosluk[b] == "/")
            {
                --b;
                s_identifier();
                output += ")";
                output += bosluk[b++];
                output += "<expr>(";
                s_expr();
                output += ")";
            }
            else
            {
                acceptStatement = false;
                return;
            }

        }
        void s_identifier()
        {
            output += bosluk[b++];
        }
        void s_int_value()
        {
            output += bosluk[b++];
        }

    }
}