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
        /// ファイルのコピー
        /// </summary>
        /// <param name="strings"></param>
        /// <returns></returns>
        public static string Bcopy(string strings)
        {
            var p = strings.Split(',');

            for (var i = 0; i < p.Count(); i++)
            {
                p[i] = p[i].Trim();
            }

            if (!Program.Using.Contains("using System.IO;"))
            {
                Program.Using += "using System.IO;\n";
            }

            return "File.Copy(" + p[0] + "," + p[1] + ");";
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


        public static string Chdir(string dirname)
        {
            return "Environment.CurrentDirectory = " + dirname + ";";
        }

        /// <summary>
        /// 文字列から分割された要素を代入
        /// </summary>
        /// <param name="strings"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 文字列の置換をする
        /// </summary>
        /// <param name="strings"></param>
        /// <returns></returns>
        public static string Strrep(string strings)
        {
            if (!Program.Using.Contains("using System.Text.RegularExpressions;"))
            {
                Program.Using += "using System.Text.RegularExpressions;\n";
            }

            var p = strings.Split(',');

            for (var i = 0; i < p.Count(); i++)
            {
                p[i] = p[i].Trim();
            }

            return "stac = 0;\nvar re = new Regex(" + p[1] + ");\nwhile(" + p[0] + ".indexof(" + p[1] + "))\n"
                + "{\nre.Replace(" + p[0] + "," + p[2] + ",1);\nstac++\n}";

        }

        /// <summary>
        /// 配列変数の作成
        /// </summary>
        /// <param name="strings"></param>
        /// <returns></returns>
        public static string Dim(string strings)
        {
            var p = strings.Split(',');

            for (var i = 0; i < p.Count(); i++)
            {
                p[i] = p[i].Trim();
            }

            var str = "";
            //変数リストに含まれていない場合
            if (!Program.VariableList.Contains(p[0]))
            {
                //変数リストに追加
                Program.VariableList.Add(p[0]);
                str = "dynamic ";
            }

            switch (p.Count())
            {
                case 1:
                    return str + p[0] + " = new dynamic [1];";
                case 2:
                    return str + p[0] + " = new dynamic [" + p[1] + "];";
                case 3:
                    return str + p[0] + " = new dynamic [" + p[1] + "," + p[2] + "];";
                case 4:
                    return str + p[0] + " = new dynamic [" + p[1] + "," + p[2] + "," + p[2] + "]; ";
                case 5:
                    return str + p[0] + " = new dynamic [" + p[1] + "," + p[2] + "," + p[3] + "," + p[4] + "]; ";
                default:
                    return "Console.WriteLine(\"エラー16(「パラメータの数が多すぎます」)\")";
            }
        }

        /// <summary>
        ///  実数型配列変数の作成
        /// </summary>
        /// <param name="strings"></param>
        /// <returns></returns>
        public static string Ddim(string strings)
        {
            var p = strings.Split(',');

            for (var i = 0; i < p.Count(); i++)
            {
                p[i] = p[i].Trim();
            }

            var str = "";
            //変数リストに含まれていない場合
            if (!Program.VariableList.Contains(p[0]))
            {
                //変数リストに追加
                Program.VariableList.Add(p[0]);
                str = "dynamic ";
            }

            switch (p.Count())
            {
                case 1:
                    return str + p[0] + " = new double [1];";
                case 2:
                    return str + p[0] + " = new double [" + p[1] + "];";
                case 3:
                    return str + p[0] + " = new double [" + p[1] + "," + p[2] + "];";
                case 4:
                    return str + p[0] + " = new double [" + p[1] + "," + p[2] + "," + p[2] + "]; ";
                case 5:
                    return str + p[0] + " = new double [" + p[1] + "," + p[2] + "," + p[3] + "," + p[4] + "]; ";
                default:
                    return "Console.WriteLine(\"エラー16(「パラメータの数が多すぎます」)\")";
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

        /// <summary>
        /// 度をラジアンに変換
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="j"></param>
        public static void Deg2rad(List<string> sentence, int j)
        {
            sentence[j] = "(Math.PI / 180) *";
        }

        /// <summary>
        /// ラジアンを度に変換
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="j"></param>
        public static void Rad2deg(List<string> sentence, int j)
        {
            sentence[j] = "(180 / Math.PI) *";
        }

        /// <summary>
        /// 指数を返す
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="j"></param>
        public static void Expf(List<string> sentence, int j)
        {
            sentence[j] = "Math.Exp";
        }

        /// <summary>
        /// 対数を返す
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="j"></param>
        public static void Logf(List<string> sentence, int j)
        {
            sentence[j] = "Math.Log";
        }

        /// <summary>
        /// 累乗（べき乗）を求める
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="j"></param>
        public static void Powf(List<string> sentence, int j)
        {
            sentence[j] = "Math.Pow";
        }

        /// <summary>
        /// ルート値を返す
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="j"></param>
        public static void Sqrt(List<string> sentence, int j)
        {
            sentence[j] = "Math.Sqrt";
        }

        /// <summary>
        /// 文字列の検索をする
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
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
                sentence[j] = p[0] + ".Substring(" + p[1] + ").IndexOf(" + p[2] + ")";
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

        /// <summary>
        /// 文字列の長さを調べる
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        public static void Strlen(List<string> sentence, int j, int k)
        {
            sentence[j] = sentence[j + 2] + ".Length";

            for (var i = j + 1; i <= k; i++)
            {
                sentence[i] = string.Empty;
            }
        }

        /// <summary>
        /// 文字列の一部を取り出す
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        public static void Strmid(List<string> sentence, int j, int k)
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
            if (int.Parse(p[1]) > -1)
            {
                sentence[j] = p[0] + ".Substring(" + p[1] + "," + p[2] + ")";
            }
            else
            {
                sentence[j] = p[0] + ".Substring(" + p[0] + ".Length - " + p[2] + "," + p[2] + ")";
            }
        }

        /// <summary>
        /// 指定した文字だけを取り除く
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        public static void Strtrim(List<string> sentence, int j, int k)
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
            if (p.Count() == 1)
            {
                sentence[j] = p[0] + ".Trim()";
            }
            else if (p.Count() == 2)
            {
                switch (p[1])
                {
                    case "0":
                        sentence[j] = p[0] + ".Trim()";
                        break;
                    case "1":
                        sentence[j] = p[0] + ".TrimStart()";
                        break;
                    case "2":
                        sentence[j] = p[0] + ".TrimEnd()";
                        break;
                    case "3":
                        sentence[j] = p[0] + ".Replace(\" \", \"\")";
                        break;
                }
            }
            else
            {
                switch (p[1])
                {
                    case "0":
                        sentence[j] = p[0] + ".Trim(" + p[2] + ")";
                        break;
                    case "1":
                        sentence[j] = p[0] + ".TrimStart(" + p[2] + ")";
                        break;
                    case "2":
                        sentence[j] = p[0] + ".TrimEnd(" + p[2] + ")";
                        break;
                    case "3":
                        sentence[j] = p[0] + ".Replace(" + p[2] + ", \"\")";
                        break;
                }
            }
        }

        /// <summary>
        /// 一定範囲内の整数を返す
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
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

            if (p.Count() > 3)
            {
                Console.WriteLine("関数のパラメーター記述が不正です");
            }
            else if (p.Count() < 3)
            {
                Console.WriteLine("パラメーターの省略はできません");
            }
            else
            {
                if (Program.VariableList.Contains(sentence[0]))
                {
                    sentence[0] = "if ((int)Math.Max(" + p[0] + "," + p[2] + ") == " + p[0] + ")\n" +
                        "{\n" + sentence[0] + " = (int)" + p[2] + ";\n}\nelse if ((int)Math.Min(" + p[0] + "," + p[1] + ") == " + p[0] + ")\n" +
                        "{\n" + sentence[0] + " = (int)" + p[1] + ";\n}\nelse\n{\n" + sentence[0] + " = (int)" + p[0] + ";\n}";
                }
                else
                {
                    sentence[0] = sentence[0] + ";\n" + "if ((int)Math.Max(" + p[0] + "," + p[2] + ") == " + p[0] + ")\n" +
                        "{\n" + sentence[0] + " = (int)" + p[2] + ";\n}\nelse if ((int)Math.Min(" + p[0] + "," + p[1] + ") == " + p[0] + ")\n" +
                        "{\n" + sentence[0] + " = (int)" + p[1] + ";\n}\nelse\n{\n" + sentence[0] + " = (int)" + p[0] + ";\n}";
                }
            }
            for (var i = 1; i <= k; i++)
            {
                sentence[i] = string.Empty;
            }
        }

        /// <summary>
        ///  一定範囲内の実数を返す
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
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

            if (p.Count() > 3)
            {
                Console.WriteLine("関数のパラメーター記述が不正です");
            }
            else if (p.Count() < 3)
            {
                Console.WriteLine("パラメーターの省略はできません");
            }
            else
            {
                if (Program.VariableList.Contains(sentence[0]))
                {
                    sentence[0] = "if (Math.Max(" + p[0] + "," + p[2] + ") == " + p[0] + ")\n" +
                        "{\n" + sentence[0] + " = " + p[2] + ";\n}\nelse if (Math.Min(" + p[0] + "," + p[1] + ") == " + p[0] + ")\n" +
                        "{\n" + sentence[0] + " = " + p[1] + ";\n}\nelse\n{\n" + sentence[0] + " = " + p[0] + ";\n}";
                }
                else
                {
                    sentence[0] = sentence[0] + ";\n" + "if (Math.Max(" + p[0] + "," + p[2] + ") == " + p[0] + ")\n" +
                        "{\n" + sentence[0] + " = " + p[2] + ";\n}\nelse if (Math.Min(" + p[0] + "," + p[1] + ") == " + p[0] + ")\n" +
                        "{\n" + sentence[0] + " = " + p[1] + ";\n}\nelse\n{\n" + sentence[0] + " = " + p[0] + ";\n}";
                }
            }
            for (var i = 1; i <= k; i++)
            {
                sentence[i] = string.Empty;
            }
        }

        /// <summary>
        /// 配列の1～4次元要素数を返す
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <param name="num"></param>
        public static void Length(List<string> sentence, int j, int k, int num)
        {
            sentence[j] = sentence[j + 2] + ".GetLength(" + num + " - 1)";

            for (var i = j + 1; i <= k; i++)
            {
                sentence[i] = string.Empty;
            }
        }

        /// <summary>
        /// 時間・日付を取得する
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="j"></param>
        public static void Gettime(List<string> sentence, int j)
        {
            sentence[j] = "DateTime.Now.ToString";
            switch (sentence[j + 2])
            {
                case "0":
                    sentence[j + 2] = "\"yyyy\"";
                    break;
                case "1":
                    sentence[j + 2] = "\"%M\"";
                    break;
                case "2":
                    var dayOfTheWeek = Program.__LocalVariableName("dayOfTheWeek");
                    Program.SubFunction += "static int " + dayOfTheWeek + "(string str)\n{\n"
                        + "switch(str)\n{\ncase \"日\":\nreturn 0;" + "\ncase \"月\":\nreturn 1;"
                        + "\ncase \"火\":\nreturn 2;" + "\ncase \"水\":\nreturn 3;"
                        + "\ncase \"木\":\nreturn 4;" + "\ncase \"金\":\nreturn 5;"
                        + "\ncase \"土\":\nreturn 6;" + "\ndefault:\nreturn -1;\n}\n}\n";
                    sentence[j] = dayOfTheWeek;
                    sentence[j + 2] = "DateTime.Now.ToString(\"ddd\")";
                    break;
                case "3":
                    sentence[j + 2] = "\"%d\"";
                    break;
                case "4":
                    sentence[j + 2] = "\"%H\"";
                    break;
                case "5":
                    sentence[j + 2] = "\"%m\"";
                    break;
                case "6":
                    sentence[j + 2] = "\"%s\"";
                    break;
                case "7":
                    sentence[j + 2] = "\"fff\"";
                    break;
            }
        }


        /*========================================
　　　　        　　　　マクロの定義
        ========================================*/

        
        public static void M_pi(List<string> sentence, int i)
        {
            sentence[i] = "Math.PI";
        }

        
        public static void And(List<string> sentence, int i)
        {
            sentence[i] = "&&";
        }

        
        public static void Not(List<string> sentence, int i)
        {
            sentence[i] = "!";
        }

        
        public static void Or(List<string> sentence, int i)
        {
            sentence[i] = "||";
        }

        
        public static void Xor(List<string> sentence, int i)
        {
            sentence[i] = "^";
        }

        public static void Dir_cur(List<string> sentence, int i)
        {
            if (!Program.Using.Contains("using System.IO;"))
            {
                Program.Using += "using System.IO;\n";
            }
            sentence[i] = "Directory.GetCurrentDirectory()";
        }
    }
}