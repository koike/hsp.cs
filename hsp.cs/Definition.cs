/*===============================
             hsp.cs
  Created by @kkrnt && @ygcuber
===============================*/

using System;
using System.Collections.Generic;
using System.Linq;

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
            "stop"
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
            "M_PI",
            "and",
            "not",
            "or",
            "xor",
            "dir_cur",
            "ginfo_mx",
            "ginfo_my"
        };

        //using
        public static string Using = "using System;\n";
        //class
        private const string Header = "public class Program\n{\n";
        //Main関数以外の関数の定義
        public static string SubFunction = "";
        //Main関数の定義
        private const string MainFunction = "public static void Main()\n{\n";
        //システム変数宣言
        public static string VariableDefinition = "";
        //footer
        private const string Footer = "\n}\n}";

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
        private static List<char> VariableNameRule =
            "0123456789!\"#$%&'()-^\\=~|@[`{;:]+*},./<>?".ToCharArray().ToList();

        private static List<string> ReturnLabelList = new List<string>(); 

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
    }
}
