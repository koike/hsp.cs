using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace hsp.cs
{
    public class HSPGUI : Form
    {
        public HSPGUI()
        {
            //
        }

        static void Screen(int id)
        {
            if (Program.Window.Count > id)
            {
                //Activate
            }
            else
            {
                Program.Window.Add(new Form());
            }
        }

        public static string Print(string strings)
        {
            strings = Analyzer.StringUnEscape(strings);
            if (strings == string.Empty)
            {
                return "g.DrawString(\"\", font, brush, CurrentPosX, CurrentPosY);\n" +
                       "CurrentPosY += FontSize"; 
            }
            return "g.DrawString(" + strings + ".ToString(), font, brush, CurrentPosX, CurrentPosY);\n" +
                   "CurrentPosY += FontSize";
        }

        public static string Pos(string strings)
        {
            var p = strings.Split(',');

            for (var i = 0; i < p.Count(); i++)
            {
                p[i] = p[i].Trim();
            }

            if (strings.Equals(string.Empty))
            {
               return "CurrentPosX = CurrentPosX" +
                      "CurrentPosY = CurrentPosY";
            }
            else if (p.Count() == 2)
            {
                return "CurrentPosX = " + p[0] + ";\n" +
                       "CurrentPosY = " + p[1];
            }
            else
            {
                return "Console.WriteLine(\"error\")";
            }
        }

        public static string Wait(string strings)
        {
            Program.UsingCheck("using System.Threading");

            return "Thread.Sleep(" + strings + " * 10);\n" +
                   "Application.DoEvents()";
        }


        public static string Screen(string strings)
        {
            var p = strings.Split(',');

            for (var i = 0; i < p.Count(); i++)
            {
                p[i] = p[i].Trim();
            }

            //Program.Window.Add();

            if (!Program.AddFunction[0].Contains("public void screen"))
            {
                Program.AddFunction[0] += "public void screen(Form form, int width, int height)\n{\n" +
                                          "form.ClientSize = new Size(width, height);\n}\n\n";
            }

            return "program.screen(form" + p[0] + ", " + p[1] + ", " + p[2] + ")";
        }

        public static string Title(string strings)
        {
            if (!Program.AddFunction[0].Contains("public void Title"))
            {
                Program.AddFunction[0] += "public void Title(Form form, string strings)\n{\n" +
                                          "form.Text = strings;\n}\n\n";

            }
            
            return "program.Title(CurrentScreenID, " + strings + ")";
        }

        public static string Circle(string strings)
        {
            var p = strings.Split(',');

            for (var i = 0; i < p.Count(); i++)
            {
                p[i] = p[i].Trim();
            }

            /*if (int.Parse(p[0].ToString()) > int.Parse(p[2].ToString()))
            {
                var temp = p[0];
                p[0] = p[2];
                p[2] = temp;
            }
            if (int.Parse(p[1].ToString()) > int.Parse(p[3].ToString()))
            {
                var temp = p[1];
                p[1] = p[3];
                p[3] = temp;
            }*/

            if (p.Count() == 4)
            {
                return "g.FillEllipse(brush, " + p[0] + ", " + p[1] + ", " + 
                       p[2] + " - " + p[0] + ", " + p[3] + " - " + p[1] + ")";
            }
            else if (p.Count() == 5)
            {
                if (p[4].Equals("0"))
                {
                    return "g.DrawEllipse(pen, " + p[0] + ", " + p[1] + ", " + 
                           p[2] + " - " + p[0] + ", " + p[3] + " - " + p[1] + ")";
                }
                else
                {
                    return "g.FillEllipse(brush, " + p[0] + ", " + p[1] + ", " + 
                           p[2] + " - " + p[0] + ", " + p[3] + " - " + p[1] + ")";
                }
            }
            else
            {
                return "Console.WriteLine(\"error\")";
            }
        }

        public static string Boxf(string strings)
        {
            var p = strings.Split(',');

            for (var i = 0; i < p.Count(); i++)
            {
                p[i] = p[i].Trim();
            }

            if (p.Count() == 1)
            {
                return "g.FillRectangle(brush, 0, 0, " +
                       "CurrentScreenID.Width, " + "CurrentScreenID.Height)";
            }
            else if (p.Count() == 2)
            {
                return "g.FillRectangle(brush, " + p[0] + ", " + p[1] + ", " +
                       "CurrentScreenID.Width, " + "CurrentScreenID.Height)";
            }
            else if (p.Count() == 4)
            {
                /*if (int.Parse(p[0].ToString()) > int.Parse(p[2].ToString()))
                {
                    var temp = p[0];
                    p[0] = p[2];
                    p[2] = temp;
                }
                if (int.Parse(p[1].ToString()) > int.Parse(p[3].ToString()))
                {
                    var temp = p[1];
                    p[1] = p[3];
                    p[3] = temp;
                }左上と右下じゃなくても動かすための*/

                if (p[0].Equals(string.Empty))
                {
                    p[0] = "0";
                }
                if (p[1].Equals(string.Empty))
                {
                    p[1] = "0";
                }
                if (p[2].Equals(string.Empty))
                {
                    p[2] = "CurrentScreenID.Width";
                }
                if (p[3].Equals(string.Empty))
                {
                    p[3] = "CurrentScreenID.Height";
                }

                return "g.FillRectangle(brush, " + p[0] + ", " + p[1] + ", " + 
                       p[2] + " - " + p[0] + ", " + p[3] + " - " + p[1] + ")";
            }
            else
            {
                return "Console.WriteLine(\"error\")";
            }
        }

        public static string Line(string strings)
        {
            var p = strings.Split(',');

            for (var i = 0; i < p.Count(); i++)
            {
                p[i] = p[i].Trim();
            }

            if (p.Count() == 2)
            {
                return "g.DrawLine(pen, CurrentPosX, CurrentPosY, " + p[0] + ", " + p[1] + ");\n" +
                       "CurrentPosX = " + p[0] + ";\nCurrentPosY = " + p[1];
            }
            else if (p.Count() == 4)
            {
                return "g.DrawLine(pen, " + p[2] + ", " + p[3] + ", " + p[0] + ", " + p[1] + ");\n" +
                       "CurrentPosX = " + p[0] + ";\nCurrentPosY = " + p[1];
            }
            else
            {
                return "Console.WriteLine(\"error\")";
            }
        }

        public static string Color(string strings)
        {
            var p = strings.Split(',');

            for (var i = 0; i < p.Count(); i++)
            {
                p[i] = p[i].Trim();
            }

            if (p.Count() == 3)
            {
                if (p[0].Equals(string.Empty))
                {
                    p[0] = "0";
                }
                if (p[1].Equals(string.Empty))
                {
                    p[1] = "0";
                }
                if (p[2].Equals(string.Empty))
                {
                    p[2] = "0";
                }

                return "brush = new SolidBrush(Color.FromArgb(" + p[0] + ", " + p[1] + ", " + p[2] + "));\n" +
                       "pen = new Pen(Color.FromArgb(" + p[0] + ", " + p[1] + ", " + p[2] + "))";
            }
            else
            {
                return "Console.WriteLine(\"error\")";
            }
        }

        public static string Getkey(string strings)
        {
            Program.UsingCheck("using System.Runtime.InteropServices");

            if (!Program.ProgramField.Contains("[DllImport(\"user32.dll\")]\n")) {
                Program.ProgramField += "[DllImport(\"user32.dll\")]\n" +
                                        "private static extern ushort GetAsyncKeyState(int vKey);\n";
            }

            var p = strings.Split(',');

            for (var i = 0; i < p.Count(); i++)
            {
                p[i] = p[i].Trim();
            }
            var str = "";
            //変数名として正しいか
            if (Program.VariableNameRule.Contains(p[0][0]))
            {
                //変数名ではない
            }
            else
            {
                //変数リストに含まれていない場合
                if (!Program.VariableList.Contains(p[0]))
                {
                    //変数リストに追加
                    Program.VariableList.Add(p[0]);
                    str = "dynamic ";
                }
            }

            if (p.Count() == 1)
            {
                return str + p[0] + " = GetAsyncKeyState(1) >> 15";
            }
            else if (p.Count() == 2)
            {
                return str + p[0] + " = GetAsyncKeyState(" + p[1] + ") >> 15";
            }
            else
            {
                return "Console.WriteLine(\"error\")";
            }
        }

        public static string Objsize(string strings)
        {
            var p = strings.Split(',');

            for (var i = 0; i < p.Count(); i++)
            {
                p[i] = p[i].Trim();
            }

            Program.ProgramField += "objsizeX = " + p[0] + ";\n" +
                                    "objsizeY = " + p[1] + ";\n" +
                                    "objSpace = " + p[2];
            return "//boxSize(" + p[0] + ", " + p[1] + ")";
        }

        public static string Dialog(string strings)
        {
            strings = Analyzer.StringUnEscape(strings);
            var p = strings.Split(',');

            for (var i = 0; i < p.Count(); i++)
            {
                p[i] = p[i].Trim();
            }
            if (p.Count() == 1)
            {
                return "MessageBox.Show(" + p[0] + ", \"\", " +
                       "MessageBoxButtons.OK, MessageBoxIcon.Information)";
            }
            switch(p[1])
            {
                case "0":
                    return "MessageBox.Show(" + p[0] + ", " + p[2] + ", " +
                           "MessageBoxButtons.OK, MessageBoxIcon.Information)";
                case "1":
                    return "MessageBox.Show(" + p[0] + ", " + p[2] + ", " +
                           "MessageBoxButtons.OK, MessageBoxIcon.Warning);\n";
                case "2":
                    return "MessageBox.Show(" + p[0] + ", " + p[2] + ", " +
                           "MessageBoxButtons.YesNo, MessageBoxIcon.Information)";
                case "3":
                    return "MessageBox.Show(" + p[0] + ", " + p[2] + ", " +
                           "MessageBoxButtons.YesNo, MessageBoxIcon.Warning)";
                case "16":
                    return "";
                case "17":
                    return "";
                case "32":
                    return "";
                case "33":
                    return "";
                default:
                    return "Console.WriteLine(\"error\")";
            }
        }

        public static void Mousex(List<string> sentence, int i)
        {
            sentence[i] = "CurrentScreenID.PointToClient(Cursor.Position).X";
        }

        public static void Mousey(List<string> sentence, int i)
        {
            sentence[i] = "CurrentScreenID.PointToClient(Cursor.Position).Y";
        }

        public static void Ginfo_sizeX(List<string> sentence, int i)
        {
            sentence[i] = "CurrentScreenID.Width";
        }

        public static void Ginfo_sizeY(List<string> sentence, int i)
        {
            sentence[i] = "CurrentScreenID.Height";
        }

        public static void Ginfo_r(List<string> sentence, int i)
        {
            sentence[i] = "pen.Color.R";
        }

        public static void Ginfo_g(List<string> sentence, int i)
        {
            sentence[i] = "pen.Color.G";
        }

        public static void Ginfo_b(List<string> sentence, int i)
        {
            sentence[i] = "pen.Color.B";
        }

        public static void Ginfo_cx(List<string> sentence, int i)
        {
            sentence[i] = "CurrentPosX";
        }

        public static void Ginfo_cy(List<string> sentence, int i)
        {
            sentence[i] = "CurrentPosY";
        }

        public static void Ginfo_dispx(List<string> sentence, int i)
        {
            sentence[i] = "Screen.PrimaryScreen.Bounds.Width";
        }

        public static void Ginfo_dispy(List<string> sentence, int i)
        {
            sentence[i] = "Screen.PrimaryScreen.Bounds.Height";
        }

        public static void Ginfo_wx1(List<string> sentence, int i)
        {
            sentence[i] = "CurrentScreenID.Left";
        }

        public static void Ginfo_wx2(List<string> sentence, int i)
        {
            sentence[i] = "CurrentScreenID.Right";
        }

        public static void Ginfo_wy1(List<string> sentence, int i)
        {
            sentence[i] = "CurrentScreenID.Top";
        }

        public static void Ginfo_wy2(List<string> sentence, int i)
        {
            sentence[i] = "CurrentScreenID.Bottom";
        }

        public static void Ginfo_sel(List<string> sentence, int i)
        {
            sentence[i] = "CurrentScreenID";
        }

        public static void Hwnd(List<string> sentence, int i)
        {
            sentence[i] = "CurrentScreenID.Handle";
        }

        public static void __date__(List<string> sentence, int i)
        {
            sentence[i] = "DateTime.Now.ToString(\"d\")";
        }

        public static void __time__(List<string> sentence, int i)
        {
            sentence[i] = "DateTime.Now.ToString(\"T\")";
        }
    }
}
