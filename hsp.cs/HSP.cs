/*===============================
             hsp.cs
  Created by @kkrnt && @ygcuber
===============================*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace hsp.cs
{
    /// <summary>
    /// HSPの関数とコマンド
    /// </summary>
    public class HSP
    {
        HSP()
        {
            //
        }


        /*========================================
        　　　　　　　　コマンドの定義
        ========================================*/


        /// <summary>
        /// メッセージ表示
        /// </summary>
        /// <param name="strings"></param>
        /// <returns></returns>
        public static string Print(string strings)
        {
            return "Console.WriteLine(" + strings + ");";
        }

        /// <summary>
        /// メッセージ表示
        /// </summary>
        /// <param name="strings"></param>
        /// <returns></returns>
        public static string Mes(string strings)
        {
            return "Console.WriteLine(" + strings + ");";
        }

        /// <summary>
        /// ファイルが存在するか
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string Exist(string filename)
        {
            if (!Program.Using.Contains("using System.IO;"))
            {
                Program.Using += "using System.IO;\n";
            }
            //if (Program.hspStringData.Contains("dynamic strsize = 0;"))
            //{
            // Program.hspStringData += "dynamic strsize = 0;";
            //}

            return "if(File.Exists("
                + filename + "))\n{\nvar fi = new FileInfo("
                + filename + ");\nstrsize = fi.Length;\n}\nelse\n{\nstrsize = -1;\n}";
        }

        /// <summary>
        /// ファイルを削除する
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string Delete(string filename)
        {
            filename = filename.Replace("\"", "");
            if (!Program.Using.Contains("using System.IO;"))
            {
                Program.Using += "using System.IO;\n";
            }

            if (!File.Exists(filename))
            {
                Console.WriteLine("エラー12(「ファイルが見つからないか無効な名前です」)");
            }
            return "File.Delete(\"" + filename + "\");";
        }

        /// <summary>
        /// ディレクトリの作成
        /// </summary>
        /// <param name="dirname"></param>
        /// <returns></returns>
        public static string Mkdir(string dirname)
        {
            if (!Program.Using.Contains("using System.IO;"))
            {
                Program.Using += "using System.IO;\n";
            }
            return "Directory.CreateDirectory(" + dirname + ");";
        }

        public static string Split(string strings)
        {
            var p = strings.Split(',');

            for (var i = 0; i < p.Count(); i++)
            {
                p[i] = p[i].Trim();
            }
            if (p.Count() == 2)
            {
                return "stat = " + p[0] + ".split(\'" + p[1] + "\').Length;";
            }
            else if (p.Count() == 3)
            {
                return "stat = " + p[0] + ".split(\'" + p[1] + "\').Length;\ndynamic " + p[2] + " = "
                     + p[0] + ".split(\'" + p[1] + "\')";
            }
            else
            {
                return "stat = " + p[0] + ".split(\'" + p[1] + "\').Length;\nvar str = " + p[0] + ".split(\'" + p[1] + "\');\n"
                    + "for (int i = 2; i < " + p.Count() + "; i++)\n{\ndynamic p[i] = str[i - 2];\n}";
            }
        }


        /*========================================
        　　　　　　　　　関数の定義
        ========================================*/


        /// <summary>
        /// 整数値に変換
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="j"></param>
        public static void Int(List<string> sentence, int j)
        {
            sentence[j] = "int.Parse";
        }

        /// <summary>
        /// 実数値に変換
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="j"></param>
        public static void Double(List<string> sentence, int j)
        {
            sentence[j] = "double.Parse";
        }

        /// <summary>
        /// 文字列に変換
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        public static void Str(List<string> sentence, int j, int k)
        {
            sentence[j] = "";
            sentence[k] += ".ToString()";
        }

        /// <summary>
        /// 整数の絶対値を返す
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        public static void Abs(List<string> sentence, int j, int k)
        {
            sentence[j] = "Math.Abs";
            sentence[j + 1] += "Math.Truncate(";
            sentence[k - 1] += ")";
        }

        /// <summary>
        /// 実数の絶対値を返す
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        public static void Absf(List<string> sentence, int j, int k)
        {
            sentence[j] = "Math.Abs";
        }

        /// <summary>
        /// 正弦値を返す
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="j"></param>
        public static void Sin(List<string> sentence, int j)
        {
            sentence[j] = "Math.Sin";
        }

        /// <summary>
        /// 余弦値を返す
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="j"></param>
        public static void Cos(List<string> sentence, int j)
        {
            sentence[j] = "Math.Cos";
        }

        /// <summary>
        /// 正接値を返す
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="j"></param>
        public static void Tan(List<string> sentence, int j)
        {
            sentence[j] = "Math.Tan";
        }

        /// <summary>
        /// arctan値を返す
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="j"></param>
        public static void Atan(List<string> sentence, int j)
        {
            sentence[j] = "Math.Atan";
        }


        public static void Expf(List<string> sentence, int j)
        {
            sentence[j] = "Math.Exp";
        }

        public static void Logf(List<string> sentence, int j)
        {
            sentence[j] = "Math.Log";
        }

        public static void Powf(List<string> sentence, int j)
        {
            sentence[j] = "Math.Pow";
        }

        public static void Sqrt(List<string> sentence, int j)
        {
            sentence[j] = "Math.Sqrt";
        }

        public static void Instr(List<string> sentence, int j, int k)
        {
            for (int i = j + 3; i < k; i++)
            {
                sentence[j + 2] += sentence[i];
            }

            var p = sentence[j + 2].Split(',');

            for (var i = 0; i < p.Count(); i++)
            {
                p[i] = p[i].Trim();
            }

            if (!p[1].Contains("-"))
            {
                sentence[j] = p[0] + ".ToString().IndexOf(" + p[2] + ") - " + p[1];
            }
            else
            {
                sentence[j] = "-1";
            }

            for (var i = j + 1; i <= k; i++)
            {
                sentence[i] = string.Empty;
            }
        }

        public static void Strlen(List<string> sentence, int j, int k)
        {
            sentence[j] = sentence[j + 2] + ".Length";

            for (var i = j + 1; i <= k; i++)
            {
                sentence[i] = string.Empty;
            }
        }

        public static void Limit(List<string> sentence, int j, int k)
        {
            for (var i = j + 3; i < k; i++)
            {
                sentence[j + 2] += sentence[i];
            }

            var p = sentence[j + 2].Split(',');

            for (var i = 0; i < p.Count(); i++)
            {
                p[i] = p[i].Trim();
            }
        }


        public static void Limitf(List<string> sentence, int j, int k)
        {
            for (var i = j + 3; i < k; i++)
            {
                sentence[j + 2] += sentence[i];
            }

            var p = sentence[j + 2].Split(',');

            for (var i = 0; i < p.Count(); i++)
            {
                p[i] = p[i].Trim();
            }
        }


        public static void Length(List<string> sentence, int j, int num)
        {
            sentence[j] = sentence[j + 2] + ".GetLength(" + num + " -1 )";
        }


        public static void Gettime(List<string> sentence, int j)
        {
            sentence[j] = "DateTime.Now.ToString";
            switch (sentence[j + 2])
            {
                case "0":
                    sentence[j + 2] = "yyyy";
                    break;
                case "1":
                    sentence[j + 2] = "MM";
                    break;
                case "2":
                    sentence[j + 2] = "dddd";
                    break;
                case "3":
                    sentence[j + 2] = "dd";
                    break;
                case "4":
                    sentence[j + 2] = "hh";
                    break;
                case "5":
                    sentence[j + 2] = "mm";
                    break;
                case "6":
                    sentence[j + 2] = "ss";
                    break;
                case "7":
                    sentence[j + 2] = "fff";
                    break;
            }
        }
    }
}
