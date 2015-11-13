/*===============================
             hsp.cs
  Created by @kkrnt && @ygcuber
===============================*/

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CSharp;

namespace hsp.cs
{
    public class Program
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
            "default"
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
            "expf",
            "logf",
            "powf",
            "sqrt",
            "instr",
            "strlen",
            "limit",
            "limitf",
            "length",
            "length2",
            "length3",
            "length4",
            "gettime"
        };

        //コマンドリスト
        public static readonly List<string> CommandList = new List<string>()
        {
            "print",
            "mes",
            "exist",
            "delete",
            "mkdir",
            "split"
        };

        //変数リスト
        public static List<string> VariableList = new List<string>()
        {
            "strsize",
            "stat",
            "cnt"
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
        private static bool commentFlag = false;

        //switch文の中にいるかどうか
        private static bool switchFlag = false;
        //switch文の行数を入れるためのリスト
        private static List<int> switchList = new List<int>(); 
        //1つ目のcase文
        private static bool firstCase = true;

        //変数名の先頭として存在してはいけない文字
        private static List<char> VariableNameRule =
            "0123456789!\"#$%&'()-^\\=~|@[`{;:]+*},./<>?".ToCharArray().ToList();

        /// <summary>
        /// ローカル変数名を作成する関数
        /// GUIDを生成し, 変数名の末尾に追加する
        /// </summary>
        /// <param name="variableName"></param>
        /// <returns></returns>
        public static string __LocalVariableName(string variableName)
        {
            return variableName + "_" + Guid.NewGuid().ToString("N");
        }

        private static void Main(string[] args)
        {
            //コマンドライン引数が足りない場合の処理
            if (args == null || args.Length < 1)
            {
                Console.WriteLine("hsp.cs [.hsp] [option]");
                Console.WriteLine("Option:");
                Console.WriteLine("     -o  --output <file>       output executive file(.exe)");
                Console.WriteLine("     -c  --cs     <file>       output csharp source file(.cs)");
                Console.WriteLine("Example:");
                Console.WriteLine("     hsp.cs.exe sample.hsp");
                Console.WriteLine("     hsp.cs.exe sample.hsp -o sample.exe -c sample.cs\n");
                return;
            }

            //コマンドラインオプションを取得
            var commandLineOption = new bool[]
            {
                false,  //-o --output   output executive file
                false,  //-c --cs       output csharp file
            };
            var hspFileName = args[0];
            var executiveFileName = string.Empty;
            var csharpFileName = string.Empty;
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i][0] != '-') continue;
                switch (args[i])
                {
                    case "-o":
                    case "--output":
                        commandLineOption[0] = true;
                        executiveFileName = args[i + 1];
                        break;
                    case "-c":
                    case "--cs":
                        commandLineOption[1] = true;
                        csharpFileName = args[i + 1];
                        break;
                }
            }

            //hspファイルを読み込み
            var sr = new StreamReader(hspFileName, Encoding.UTF8);
            //全て小文字にして全角スペースとタブを半角スペースに変換し, 改行でスプリット
            var hspArrayData = sr.ReadToEnd().Split('\n').Where(i => i.Length != 0).ToList();
            sr.Close();

            //デバッグ用の出力
            Console.WriteLine("\n<HSP Code>");
            Console.WriteLine(string.Join("\n", hspArrayData));
            Console.WriteLine("\n========================\n");

            //文字列を_stringListに格納し, エスケープする
            for (var i = 0; i < hspArrayData.Count; i++)
            {
                if (hspArrayData[i].Equals("{") || hspArrayData[i].Equals("}")) continue;

                //データの整形
                //前後の空白文字を削除
                hspArrayData[i] = hspArrayData[i].Trim();

                //直前にエスケープのないダブルクオーテーションが存在した場合
                //文字列部分をStringListに格納し
                //その部分を＠＋＠StringListのindex＠ー＠で置換する
                //Example: hoge = "fu" + "ga"
                //         hoge = ＠＋＠0＠ー＠ + ＠＋＠1＠ー＠
                //StringListには"fu"と"ga"が格納される
                //このときダブルクオーテーションも含まれているので注意
                var hspStringData = hspArrayData[i];
                while (true)
                {
                    var preIndex = hspArrayData[i].IndexOf("\"", StringComparison.OrdinalIgnoreCase);
                    if (preIndex == -1 || hspArrayData[i][preIndex - 1] == '\\') break;
                    var x = hspArrayData[i].Substring(preIndex + 1);
                    var postIndex = x.IndexOf("\"", StringComparison.OrdinalIgnoreCase);
                    if (postIndex == -1 || hspArrayData[i][postIndex - 1] == '\\') break;
                    var midString = hspArrayData[i].Substring(preIndex, postIndex + 2);
                    StringList.Add(midString);
                    hspArrayData[i] = hspArrayData[i].Replace(midString, "");
                    hspStringData = hspStringData.Replace(midString, "＠＋＠" + (StringList.Count - 1) + "＠ー＠");
                }
                hspArrayData[i] = hspStringData;

                //コメントを取り除く
                //スラッシュ2つによるコメントアウトを取り除く
                var commentIndex = hspArrayData[i].IndexOf("//", StringComparison.Ordinal);
                if (commentIndex > -1)
                {
                    hspArrayData[i] = hspArrayData[i].Substring(0, commentIndex).Trim();
                }
                //スラッシュとアスタリスクによるコメントアウトをエスケープする
                commentIndex = hspArrayData[i].IndexOf("/*", StringComparison.Ordinal);
                if (commentIndex > -1)
                {
                    hspArrayData[i] = hspArrayData[i].Substring(0, commentIndex).Trim();
                    commentFlag = true;
                }
                if (commentFlag)
                {
                    commentIndex = hspArrayData[i].IndexOf("*/", StringComparison.Ordinal);
                    if (commentIndex > -1)
                    {
                        hspArrayData[i] = hspArrayData[i].Substring(commentIndex + "*/".Length).Trim();
                        commentFlag = false;
                    }
                    else
                    {
                        continue;
                    }
                }

                hspArrayData[i] = hspArrayData[i]
                    //データ中の空白文字を全て半角スペースに変換
                    .Replace('　', ' ')
                    .Replace('\t', ' ')
                    //関数部分で正確にスプリットするために()の直前後に半角スペースを追加
                    .Replace("(", " ( ")
                    .Replace(")", " ) ")
                    .Replace("=", " = ")
                    .Replace("+", " + ")
                    .Replace("-", " - ")
                    .Replace("*", " * ")
                    .Replace("/", " / ")
                    //連続する演算子を修正
                    .Replace("=  =", "==")
                    .Replace("!  =", "!=")
                    .Replace("+  =", "+=")
                    .Replace("-  =", "-=")
                    .Replace("*  =", "*=")
                    .Replace("/  =", "/=")
                    .Replace("+  +", "++")
                    .Replace("-  -", "--")
                    .Replace("\\  =", "\\=")
                    .Trim();

                //関数処理
                //要素単位で分解するために半角スペースでスプリット
                var sentence = hspArrayData[i].Replace("  ", " ").Split(' ').ToList();
                for (var j = 0; j < sentence.Count; j++)
                {
                    //余計なものは省く
                    //関数は必ず関数名の後に"("が来るはず
                    sentence[j] = sentence[j].Trim();
                    if (sentence[j] == null ||
                        sentence[j].Equals("\n") ||
                        sentence[j].Equals("") ||
                        !FunctionList.Contains(sentence[j]) ||
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
                hspArrayData[i] = string.Join(" ", sentence);

                //１番最初のsentenceを抜き出す
                var spaceIndex = hspArrayData[i].IndexOf(" ", StringComparison.OrdinalIgnoreCase);
                var firstSentence = spaceIndex < 0
                    ? hspArrayData[i].Trim()
                    : hspArrayData[i].Substring(0, spaceIndex).Trim();

                //基本文法の処理
                if (BasicList.Contains(firstSentence))
                {
                    switch (firstSentence)
                    {
                        //if文の処理
                        case "if":
                            //"{}"を使って複数行で書かれている場合
                            //必ず文中に"{"が入っている
                            var bracketIndex = hspArrayData[i].IndexOf("{", StringComparison.Ordinal);
                            if (bracketIndex < 0)
                            {
                                //処理が1行で書かれている場合
                                //ifと条件文の間に"()"を入れる
                                var coronIndex = hspArrayData[i].IndexOf(":", StringComparison.Ordinal);
                                if (coronIndex < 0)
                                {
                                    //条件文の後に処理が書かれていないため無効な文
                                    //エラーとして吐き出すよりも警告として表示したほうが良い？
                                    Console.WriteLine("条件文の後に実行すべき処理が書かれていません");
                                }
                                else
                                {
                                    var tmpString = hspArrayData[i].Substring(coronIndex + 1);
                                    hspArrayData[i] = "if (" +
                                                      hspArrayData[i].Substring("if ".Length,
                                                          coronIndex - "if ".Length - 1) +
                                                      ")\n{";

                                    //":"以降をhspArrayDataにInsertする
                                    var tmpArray = tmpString.Split(':');
                                    var index = i + 1;
                                    if (tmpArray.Length > 0)
                                    {
                                        for (var j = 0; j < tmpArray.Length; j++)
                                        {
                                            hspArrayData.Insert(i + j + 1, tmpArray[j].Trim());
                                            index = i + j + 1;
                                        }
                                    }

                                    //末尾に"}"を付けるためのフラグ
                                    ifFlag.Add(index);
                                }
                            }
                            else
                            {
                                //複数行の処理
                                hspArrayData[i] = "if (" +
                                                  hspArrayData[i].Substring("if ".Length,
                                                      bracketIndex - "if ".Length - 1) + ")\n" +
                                                  hspArrayData[i].Substring(bracketIndex);
                            }
                            break;

                        //elseの処理
                        case "else":
                            hspArrayData[i] = "}\n else \n{";
                            break;
                        
                        //forの処理
                        case "for":
                            var forConditionalSentence =
                                hspArrayData[i].Substring(spaceIndex).Split(',').Select(_ => _.Trim()).ToList();
                            if (forConditionalSentence.Count() != 4)
                            {
                                //要素数がオカシイのでエラー
                                Console.WriteLine("for文の要素数がオカシイです");
                                Console.WriteLine("現在の要素数 = " + forConditionalSentence.Count());
                            }
                            else
                            {
                                hspArrayData[i] = "for (var " + forConditionalSentence[0] + " = " + forConditionalSentence[1] +
                                                  "; " + forConditionalSentence[0] + " != " + forConditionalSentence[2] + "; " +
                                                  forConditionalSentence[0] + " += " + forConditionalSentence[3] + " )\n{";
                            }
                            break;
                        
                        //breakの処理
                        case "_break":
                            hspArrayData[i] = "break";
                            break;
                        
                        //continueの処理
                        case "_continue":
                            hspArrayData[i] = "continue";
                            break;

                        //whileの処理
                        case "while":
                            var whileConditionalSentence = hspArrayData[i].Substring(spaceIndex).Trim();
                            hspArrayData[i] = "while (" + whileConditionalSentence + ")\n{";
                            break;

                        //repeatの処理
                        case "repeat":
                            var repeatConditionalSentence = hspArrayData[i].Substring(spaceIndex).Trim();
                            int counter;
                            if (int.TryParse(repeatConditionalSentence, out counter))
                            {
                                hspArrayData[i] = "for (cnt=0; cnt<" + counter + "; cnt++)\n{";

                                //システム変数cntが定義されていない場合は定義
                                if (!VariableDefinition.Contains("int cnt = 0;"))
                                {
                                    VariableDefinition += "int cnt = 0;\n";
                                }
                            }
                            else
                            {
                                //repeatに渡されている値が数字ではないのでエラー
                                Console.WriteLine("repeatに渡されている値("+ repeatConditionalSentence + ")は数字ではありません");
                            }
                            break;

                        //switchの処理
                        case "switch":
                            var switchSpaceIndex = hspArrayData[i].IndexOf(" ");
                            if (switchSpaceIndex < 0)
                            {
                                //switchの条件文としてオカシイのでエラー
                                Console.WriteLine("switch文の条件文が書かれていません");
                            }
                            else
                            {
                                var switchConditionalSentence = hspArrayData[i].Substring(switchSpaceIndex).Trim();
                                var switchTmpString = __LocalVariableName("switchTmpString");
                                hspArrayData[i] = "string " + switchTmpString + " = " + switchConditionalSentence +
                                                  ".ToString();\n" + "switch (" + switchTmpString + ") \n{";
                            }
                            switchFlag = true;
                            break;
                        case "swend":
                            //1つ目の要素はswitch文なので取り除く
                            switchList.RemoveAt(0);

                            for (var j = 0; j < switchList.Count; j++)
                            {
                                if (hspArrayData[switchList[j]].Equals("default:"))
                                {
                                    if (!hspArrayData[switchList[j] - 1].Contains("break;"))
                                    {
                                        var defaultString = "";
                                        for (var k = switchList[j]+1; k < switchList[switchList.Count - 1]; k++)
                                        {
                                            defaultString += hspArrayData[k] + "\nbreak;";
                                        }
                                        hspArrayData[switchList[j] - 1] += defaultString;
                                    }
                                }

                                var endIndex = hspArrayData[switchList[j]].IndexOf(" ", StringComparison.Ordinal);
                                if (endIndex < 0)
                                {
                                    //
                                }
                                else
                                {
                                    var caseName = hspArrayData[switchList[j]].Substring(endIndex).Trim();
                                    var first = hspArrayData[switchList[j]].Substring(0, endIndex).Trim();
                                    if (firstCase)
                                    {
                                        firstCase = false;
                                    }
                                    else if (first.Equals("case"))
                                    {
                                        if (!hspArrayData[switchList[j] - 1].Contains("break;"))
                                        {
                                            hspArrayData[switchList[j] - 1] += "\ngoto case " +
                                                                               caseName.Substring(0, caseName.Length - 1) +
                                                                               ";";
                                        }
                                    }
                                }
                            }

                            switchFlag = false;
                            hspArrayData[i] = "}";
                            break;
                        case "swbreak":
                            hspArrayData[i] = "break;";
                            break;
                        case "case":
                            var caseSpaceIndex = hspArrayData[i].IndexOf(" ", StringComparison.Ordinal);
                            if (caseSpaceIndex < 0)
                            {
                                //case文の値が不正なのでエラー
                                Console.WriteLine("case文の値が不正です");
                            }
                            else
                            {
                                hspArrayData[i] = "case \"" + hspArrayData[i].Substring(caseSpaceIndex).Trim() + "\"";
                            }
                            hspArrayData[i] += ":";
                            break;
                        case "default":
                            hspArrayData[i] += ":";
                            break;

                        //色々な後処理
                        case "next":
                        case "wend":
                        case "loop":
                            hspArrayData[i] = "}";
                            break;
                    }
                }

                //コマンド処理
                //sentenceがコマンドかどうか
                else if (CommandList.Contains(firstSentence))
                {
                    //コマンドの引数部分を取得
                    var commandArguments = hspArrayData[i].Substring(spaceIndex + 1);
                    switch (firstSentence)
                    {
                        case "print":
                            hspArrayData[i] = HSP.Print(commandArguments);
                            break;
                        case "mes":
                            hspArrayData[i] = HSP.Mes(commandArguments);
                            break;
                        case "exist":
                            hspArrayData[i] = HSP.Exist(commandArguments);
                            break;
                        case "delete":
                            hspArrayData[i] = HSP.Delete(commandArguments);
                            break;
                        case "mkdir":
                            hspArrayData[i] = HSP.Mkdir(commandArguments);
                            break;
                        case "split":
                            hspArrayData[i] = HSP.Split(commandArguments);
                            break;
                    }

                    //if文の後処理
                    if (ifFlag.Count > 0)
                    {
                        foreach (var t in ifFlag.Where(t => t == i))
                        {
                            hspArrayData[i] += "\n}";
                        }
                    }
                }

                //基本文法でもコマンドでもないものは変数
                else if (!BasicList.Contains(firstSentence) && !FunctionList.Contains(firstSentence))
                {
                    //変数名として正しいか
                    if (VariableNameRule.Contains(firstSentence[0]))
                    {
                        //変数名ではない
                    }
                    else
                    {
                        //変数リストに含まれていない場合
                        if (!VariableList.Contains(firstSentence))
                        {
                            //変数宣言
                            hspArrayData[i] = "dynamic " + hspArrayData[i];
                            //変数リストに追加
                            VariableList.Add(firstSentence);
                        }
                    }
                }

                //HSPではmodを￥で表記するので%に置換
                hspArrayData[i] = hspArrayData[i].Replace("\\", "%");

                if (switchFlag)
                {
                    switchList.Add(i);
                }
            }

            //文字列のエスケープを元に戻す
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
                            hspArrayData[i] = hspArrayData[i].Replace(o, StringList[index]);
                            stringIndexCount++;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }


            //各行の末尾にセミコロンを追加
            for (var i = 0; i < hspArrayData.Count; i++)
            {
                if (hspArrayData[i].Equals("") || hspArrayData[i].Equals("{") || hspArrayData[i].Equals("}") ||
                    hspArrayData[i][hspArrayData[i].Length - 1].Equals(':')) continue;

                if (hspArrayData[i][hspArrayData[i].Length - 1] != '{' &&
                    hspArrayData[i][hspArrayData[i].Length - 1] != ';')
                {
                    hspArrayData[i] += ';';
                }
            }

            //C#のコードを完成
            var code = Using + Header + SubFunction + MainFunction + VariableDefinition +
                       string.Join("\n", hspArrayData) + Footer;

            //エラー判定
            var error = true;

            var references = new[]
            {
                //microlib.dll
                MetadataReference.CreateFromFile(typeof (object).Assembly.Location),
                //System.dll
                MetadataReference.CreateFromFile(
                    typeof (System.Collections.ObjectModel.ObservableCollection<>).Assembly.Location),
                //System.Core.dll
                MetadataReference.CreateFromFile(typeof (System.Linq.Enumerable).Assembly.Location),
            };

            var tree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("code", syntaxTrees: new[] { tree }, references: references);

            Console.WriteLine("<C# Code>");
            //コードをハイライト付けて表示
            var syntax = new SyntaxHighlight(compilation, tree);
            syntax.highlight();

            Console.WriteLine("\n========================\n");

            var syntaxDiagnostics = tree.GetDiagnostics().ToList();
            if (syntaxDiagnostics.Count > 0)
            {
                Console.WriteLine("<構文エラー>");
                foreach (var diagnostics in syntaxDiagnostics)
                {
                    Console.WriteLine(diagnostics);
                }
            }
            else
            {
                error = false;
                Console.WriteLine("構文エラーなし");
            }


            Console.WriteLine("\n========================\n");

            var semanticDiagnostics = compilation.GetDiagnostics().ToList();
            if (semanticDiagnostics.Count > 0)
            {
                Console.WriteLine("<意味エラー>");
                foreach (var diagnostics in semanticDiagnostics)
                {
                    Console.WriteLine(diagnostics);
                }
            }
            else
            {
                error = false;
                Console.WriteLine("意味エラーなし");
            }

            if (!error)
            {
                Console.WriteLine("\n========================\n");

                Console.WriteLine("<実行結果>");

                var param = commandLineOption[0]
                    ? new CompilerParameters()
                    : new CompilerParameters { GenerateExecutable = true, OutputAssembly = executiveFileName };

                param.ReferencedAssemblies.AddRange(new string[]
                {
                    "mscorlib.dll", "System.dll", "System.Core.dll", "Microsoft.CSharp.dll", "System.IO.dll"
                });
                new CSharpCodeProvider()
                    .CompileAssemblyFromSource(param, code)
                    .CompiledAssembly
                    .GetType("Program")
                    .GetMethod("Main")
                    .Invoke(null, null);

                Console.WriteLine("\n========================\n");
            }
            else
            {
                Console.WriteLine("エラーのためコンパイルを中止しました");
            }

            if (!commandLineOption[1]) return;
            var sw = new StreamWriter(csharpFileName, false, Encoding.UTF8);
            sw.WriteLine(code);
            sw.Close();
        }
    }
}