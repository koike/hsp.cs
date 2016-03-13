/*===============================
             hsp.cs
  Created by @kkrnt && @ygcuber
===============================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace hsp.cs
{
    public partial class Program
    {
        //基本文法
        public static List<string> BasicList = new List<string>()
        {
            "if",
            "else",
            "while",
            "wend",
            "for",
            "next",
            "_break",
            "_continue",
            "repeat",
            "loop",
            "switch",
            "swend",
            "swbreak",
            "case",
            "default",
            "goto",
            "gosub",
            "return"
        };

        //プリプロセッサ
        public static List<string> PreprocessorList = new List<string>()
        {
            "#uselib"
        };

        //文字列を格納するリスト
        public static List<string> StringList = new List<string>();

        //関数リスト
        public static readonly List<string> FunctionList = new List<string>()
        {
            "int",
            "double",
            "str",
            "abs",
            "absf",
            "sin",
            "cos",
            "tan",
            "atan",
            "deg2rad",
            "rad2deg",
            "expf",
            "logf",
            "powf",
            "sqrt",
            "instr",
            "strlen",
            "strmid",
            "strtrim",
            "limit",
            "limitf",
            "length",
            "length2",
            "length3",
            "length4",
            "gettime",
            "rnd"
        };

        //コマンドリスト
        public static readonly List<string> CommandList = new List<string>()
        {
            "print",
            "mes",
            "exist",
            "delete",
            "bcopy",
            "mkdir",
            "chdir",
            "split",
            "strrep",
            "dim",
            "ddim",
            "end",
            "stop",
            "wait",
            "pos",
            "screen",
            "bgscr",
            "title",
            "redraw",
            "mouse",
            "font",
            "circle",
            "boxf",
            "line",
            "color",
            "picload",
            "getkey",
            "objsize",
            "dialog"
        };

        //変数リスト
        public static List<string> VariableList = new List<string>()
        {
            "strsize",
            "stat",
            "cnt"
        };

        //配列変数リスト
        public static List<string> ArrayVariableList = new List<string>();

        //マクロリスト
        public static List<string> MacroList = new List<string>()
        {
            "m_pi",
            "and",
            "not",
            "or",
            "xor",
            "mousex",
            "mousey",
            "dir_cmdline",
            "dir_cur",
            "dir_desktop",
            "dir_exe",
            "dir_mydoc",
            "dir_sys",
            "dir_win",
            "ginfo_mx",
            "ginfo_my",
            "ginfo_sizex",
            "ginfo_sizey",
            "ginfo_r",
            "ginfo_g",
            "ginfo_b",
            "ginfo_cx",
            "ginfo_cy",
            "ginfo_dispx",
            "ginfo_dispy",
            "ginfo_wx1",
            "ginfo_wx2",
            "ginfo_wy1",
            "ginfo_wy2",
            "ginfo_sel",
            "hwnd",
            "__date__",
            "__time__",
            "msgothic",
            "msmincho",
            "font_normal",
            "font_bold",
            "font_italic",
            "font_underline",
            "font_strikeout",
            "screen_normal",
            "screen_hide",
            "screen_fixedsize",
            "screen_tool",
            "screen_frame"
        };

        //using
        public static string Using = "using System;\nusing System.Drawing;\nusing System.Windows.Forms;\n";
        //header
        private const string ProgramHeader = "public class Program\n{\n";
        //field
        public static string ProgramField = "public static Form form0 = new Form();\n" +
                                            "public static Form CurrentScreenID = form0;\n" +
                                            "public static Program program = new Program();\n";
        //Main関数以外の関数の定義
        public static string SubFunction = "";
        //Main関数の定義
        private const string MainFunction = "public static void Main()\n{\n" +
                                            "program.initScreen(form0);\n";
        //システム変数宣言
        public static string VariableDefinition = "";
        //ウィンドウを動かすためのコードの追加
        private const string AddMainFunction = "Application.EnableVisualStyles();\n" +
                                               "//Application.SetCompatibleTextRenderingDefault(false);\n" +
                                               "Application.Run(form0);\n";
        //Main関数とSub関数以外で必要な関数
        public static List<string> AddFunction = new List<string>()
        {
            "public void initScreen(Form form)\n{\n" +
            "form.ClientSize = new Size(640, 480);\n" +
            "form.Text = \"hsp.cs\";\n" +
            "form.BackColor = Color.FromArgb(255, 255, 255);\n" +
            "form.MaximizeBox = false;\n" +
            "form.FormBorderStyle = FormBorderStyle.FixedSingle;\n" +
            "form.Paint += paint;\n}\n\n",

            "public void paint(object sender, PaintEventArgs e)\n{\n" +
            "var FontSize = 14;\n"+
            "var CurrentPosX = 0;\n" +
            "var CurrentPosY = 0;\n" +
            "Graphics g = e.Graphics;\n" +
            "Brush brush = new SolidBrush(Color.FromArgb(0, 0, 0));\n" +
            "Pen pen = new Pen(Color.FromArgb(0, 0, 0));\n" +
            "Font font = new Font(\"FixedSys\", FontSize);\n" +
            "try\n{\n"
        };
        //footer
        public const string ProgramFooter = "\n}\n";

        //if文の末尾に"}"を付けるためのフラグ
        private static List<int> ifFlag = new List<int>();

        //コメントをエスケープするためのフラグ
        public static bool commentFlag = false;

        //switch文の中にいるかどうか
        private static bool switchFlag = false;
        //switch文の行数を入れるためのリスト
        private static List<int> switchList = new List<int>();
        //1つ目のcase文
        private static bool firstCase = true;

        //変数名の先頭として存在してはいけない文字
        public static List<char> VariableNameRule =
            "0123456789!\"#$%&'()-^\\=~|@[`{;:]+*},./<>?".ToCharArray().ToList();

        private static List<string> ReturnLabelList = new List<string>(); 

        public static List<Form> Window = new List<Form>();

        public static List<int[]> errorLine = new List<int[]>();

        /// <summary>
        /// ローカル変数名を作成する関数
        /// GUIDを生成し, 変数名の末尾に追加する
        /// </summary>
        /// <param name="variableName"></param>
        /// <returns></returns>
        public static string __LocalName(string variableName)
        {
            return variableName + "_" + Guid.NewGuid().ToString("N");
        }

        public static void UsingCheck(string usingName)
        {
            if (!Program.Using.Contains(usingName))
            {
                Program.Using += usingName + ";\n";
            }
        }
    }
}
