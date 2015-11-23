/*===============================
             hsp.cs
  Created by @kkrnt && @ygcuber
===============================*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace hsp.cs
{
    class Analyzer
    {
        /// <summary>
        /// コード中の""で括られた文字列をエスケープ
        /// </summary>
        /// <param name="hspArrayString"></param>
        /// <returns></returns>
        public static string StringEscape(string hspArrayString)
        {
            var hspStringData = hspArrayString;
            while (true)
            {
                var preIndex = hspArrayString.IndexOf("\"", StringComparison.OrdinalIgnoreCase);
                if (preIndex == -1 || hspArrayString[preIndex - 1] == '\\') break;
                var x = hspArrayString.Substring(preIndex + 1);
                var postIndex = x.IndexOf("\"", StringComparison.OrdinalIgnoreCase);
                if (postIndex == -1 || hspArrayString[postIndex - 1] == '\\') break;
                var midString = hspArrayString.Substring(preIndex, postIndex + 2);
                Program.StringList.Add(midString);
                hspArrayString = hspArrayString.Replace(midString, "");
                hspStringData = hspStringData.Replace(midString, "＠＋＠" + (Program.StringList.Count - 1) + "＠ー＠");
            }
            return hspStringData;
        }

        /// <summary>
        /// エスケープした文字列を元に戻す
        /// </summary>
        /// <param name="hspArrayData"></param>
        /// <returns></returns>
        public static List<string> StringUnEscape(List<string> hspArrayData)
        {
            for (var i = 0; i < hspArrayData.Count; i++)
            {
                var stringIndexCount = 0;
                while (true)
                {
                    var preStringIndex = hspArrayData[i].IndexOf("＠＋＠", StringComparison.OrdinalIgnoreCase);
                    if (preStringIndex != -1)
                    {
                        var postStringIndex = hspArrayData[i].IndexOf("＠ー＠", StringComparison.OrdinalIgnoreCase);
                        if (postStringIndex != -1)
                        {
                            var o = hspArrayData[i].Substring(preStringIndex, postStringIndex - preStringIndex + 3);
                            var index = int.Parse(o.Replace("＠＋＠", "").Replace("＠ー＠", ""));
                            hspArrayData[i] = hspArrayData[i].Replace(o, Program.StringList[index]);
                            stringIndexCount++;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return hspArrayData;
        } 

        /// <summary>
        /// 関数呼び出し
        /// </summary>
        /// <param name="hspArrayString"></param>
        /// <returns></returns>
        public static string Function(string hspArrayString)
        {
            var sentence = hspArrayString.Replace("  ", " ").Split(' ').ToList();
            for (var j = 0; j < sentence.Count; j++)
            {
                //余計なものは省く
                //関数は必ず関数名の後に"("が来るはず
                sentence[j] = sentence[j].Trim();
                if (sentence[j] == null ||
                    sentence[j].Equals("\n") ||
                    sentence[j].Equals("") ||
                    !Program.FunctionList.Contains(sentence[j]) ||
                    sentence[j + 1][0] != '(')
                    continue;

                //初めに")"が来る行と, それまでに"("が幾つ出てくるか数える
                var bracketStartCount = 0;
                int k;
                for (k = j + 1; k < sentence.Count; k++)
                {
                    if (sentence[k].Equals("("))
                    {
                        bracketStartCount++;
                    }
                    if (sentence[k].Equals(")"))
                    {
                        break;
                    }
                }

                //"("の数だけ該当する")"をズラす
                for (var l = 0; l < bracketStartCount - 1; l++)
                {
                    var flag = false;
                    for (var m = k + 1; m < sentence.Count; m++)
                    {
                        if (sentence[m].Equals(")"))
                        {
                            k = m;
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        /*============================
                        //カッコの数がオカシイのでエラー
                        =============================*/
                        Console.WriteLine("Error");
                    }
                }

                //sentence[j]が関数名
                //sentence[k]が関数の")"
                //sentence[j + 1]～sentence[k]で"("～")"まで
                switch (sentence[j])
                {
                    case "int":
                        HSP.Int(sentence, j);
                        break;
                    case "double":
                        HSP.Double(sentence, j);
                        break;
                    case "str":
                        HSP.Str(sentence, j, k);
                        break;
                    case "abs":
                        HSP.Abs(sentence, j, k);
                        break;
                    case "absf":
                        HSP.Absf(sentence, j, k);
                        break;
                    case "sin":
                        HSP.Sin(sentence, j);
                        break;
                    case "cos":
                        HSP.Cos(sentence, j);
                        break;
                    case "tan":
                        HSP.Tan(sentence, j);
                        break;
                    case "atan":
                        HSP.Atan(sentence, j);
                        break;
                    case "expf":
                        HSP.Expf(sentence, j);
                        break;
                    case "logf":
                        HSP.Logf(sentence, j);
                        break;
                    case "powf":
                        HSP.Powf(sentence, j);
                        break;
                    case "sqrt":
                        HSP.Sqrt(sentence, j);
                        break;
                    case "instr":
                        HSP.Instr(sentence, j, k);
                        break;
                    case "strlen":
                        HSP.Strlen(sentence, j, k);
                        break;
                    case "limit":
                        HSP.Limit(sentence, j, k);
                        break;
                    case "limitf":
                        HSP.Limitf(sentence, j, k);
                        break;
                    case "length":
                        HSP.Length(sentence, j, 0);
                        break;
                    case "length2":
                        HSP.Length(sentence, j, 1);
                        break;
                    case "length3":
                        HSP.Length(sentence, j, 2);
                        break;
                    case "length4":
                        HSP.Length(sentence, j, 3);
                        break;
                    case "gettime":
                        HSP.Gettime(sentence, j);
                        break;
                }
            }
            //結果を反映
            return string.Join(" ", sentence);
        }
    }
}
