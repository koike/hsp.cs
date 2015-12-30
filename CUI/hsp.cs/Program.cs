/*===============================
             hsp.cs
  Created by @kkrnt && @ygcuber
===============================*/

using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CSharp;

namespace hsp.cs
{
    public partial class Program
    {
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
            var sr = new StreamReader(hspFileName, Encoding.Default);
            //全て小文字にして全角スペースとタブを半角スペースに変換し, 改行でスプリット
            var hspArrayData = sr.ReadToEnd().Split('\n').Where(i => i.Length != 0).ToList();
            sr.Close();

            //デバッグ用の出力
            Console.WriteLine("\n<HSP Code>");
            Console.WriteLine(string.Join("\n", hspArrayData));
            Console.WriteLine("\n========================\n");

            for (var i = 0; i < hspArrayData.Count; i++)
            {
                if (hspArrayData[i].Equals("{") || hspArrayData[i].Equals("}")) continue;

                //データの整形
                //前後の空白文字を削除
                hspArrayData[i] = hspArrayData[i].Trim();

                if (hspArrayData[i].Equals(""))
                {
                    continue;
                }

                //直前にエスケープのないダブルクオーテーションが存在した場合
                //文字列部分をStringListに格納し
                //その部分を＠＋＠StringListのindex＠ー＠で置換する
                //Example: hoge = "fu" + "ga"
                //         hoge = ＠＋＠0＠ー＠ + ＠＋＠1＠ー＠
                //StringListには"fu"と"ga"が格納される
                //このときダブルクオーテーションも含まれているので注意
                hspArrayData[i] = Analyzer.StringEscape(hspArrayData[i]);

                //コメントを取り除く
                var commentOutIndex = hspArrayData[i].IndexOf("//", StringComparison.Ordinal);
                if (commentOutIndex > -1)
                {
                    hspArrayData[i] = hspArrayData[i].Substring(0, commentOutIndex).Trim();
                }

                //スラッシュとアスタリスクによるコメントアウトをエスケープする
                commentOutIndex = hspArrayData[i].IndexOf("/*", StringComparison.Ordinal);
                if (commentOutIndex > -1)
                {
                    hspArrayData[i] = hspArrayData[i].Substring(0, commentOutIndex).Trim();
                    commentFlag = true;
                }
                if (commentFlag)
                {
                    commentOutIndex = hspArrayData[i].IndexOf("*/", StringComparison.Ordinal);
                    if (commentOutIndex > -1)
                    {
                        hspArrayData[i] = hspArrayData[i].Substring(commentOutIndex + "*/".Length).Trim();
                        commentFlag = false;
                    }
                    else
                    {
                        continue;
                    }
                }

                if (hspArrayData[i].Equals(""))
                {
                    continue;
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

                //ラベルの定義
                if (hspArrayData[i][0] == '*')
                {
                    hspArrayData[i] = hspArrayData[i].Substring(1);
                    hspArrayData[i] = hspArrayData[i].Trim();

                    hspArrayData[i] += ":";
                }

                //１番最初のsentenceを抜き出す
                var spaceIndex = hspArrayData[i].IndexOf(" ", StringComparison.OrdinalIgnoreCase);
                var firstSentence = spaceIndex < 0
                    ? hspArrayData[i].Trim()
                    : hspArrayData[i].Substring(0, spaceIndex).Trim();

                //マクロ処理
                hspArrayData[i] = Analyzer.Macro(hspArrayData[i]);

                //関数処理
                hspArrayData[i] = Analyzer.Function(hspArrayData[i]);

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

                            //if文の条件における"="の好意的解釈
                            //"="が1つでも"=="として扱う
                            hspArrayData[i] = hspArrayData[i]
                                .Replace(" ", "")
                                .Replace("=", "==")
                                .Replace("====", " == ")
                                .Replace("!==", " != ")
                                .Replace("+==", " += ")
                                .Replace("-==", " -= ")
                                .Replace("*==", " *= ")
                                .Replace("/==", " /= ");

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
                                var switchTmpString = __LocalName("switchTmpString");
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

                        //gotoの処理
                        case "goto":
                            hspArrayData[i] = hspArrayData[i].Replace("*", "");
                            break;

                        case "gosub":
                            hspArrayData[i] = "goto " + hspArrayData[i].Substring("gosub".Length).Replace("*", "");
                            var label = __LocalName("label");
                            hspArrayData.Insert(i+1, label + ":");
                            ReturnLabelList.Add(label);
                            break;
                    }
                }

                //コマンド処理
                //sentenceがコマンドかどうか
                else if (CommandList.Contains(firstSentence))
                {
                    //コマンドの引数部分を取得
                    var commandArguments = "";
                    if (spaceIndex > -1)
                    {
                        commandArguments = hspArrayData[i].Substring(spaceIndex + 1);
                    }

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
                        case "bcopy":
                            hspArrayData[i] = HSP.Bcopy(commandArguments);
                            break;
                        case "mkdir":
                            hspArrayData[i] = HSP.Mkdir(commandArguments);
                            break;
                        case "chdir":
                            hspArrayData[i] = HSP.Chdir(commandArguments);
                            break;
                        case "split":
                            hspArrayData[i] = HSP.Split(commandArguments);
                            break;
                        case "strrep":
                            hspArrayData[i] = HSP.Strrep(commandArguments);
                            break;
                        case "dim":
                            hspArrayData[i] = HSP.Dim(commandArguments);
                            break;
                        case "ddim":
                            hspArrayData[i] = HSP.Ddim(commandArguments);
                            break;
                        case "end":
                            hspArrayData[i] = HSP.End(commandArguments);
                            break;
                        case "stop":
                            hspArrayData[i] = HSP.Stop(commandArguments);
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
                        if (!VariableList.Contains(firstSentence) && hspArrayData[i][hspArrayData[i].Length-1] != ':')
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

            //returnの処理
            for (var i = 0; i < hspArrayData.Count; i++)
            {
                if (hspArrayData[i].Equals("return"))
                {
                    var returnLabel = ReturnLabelList[ReturnLabelList.Count - 1];
                    ReturnLabelList.RemoveAt(ReturnLabelList.Count - 1);
                    hspArrayData[i] = "goto " + returnLabel;
                }
            }

            //文字列をアンエスケープ
            hspArrayData = Analyzer.StringUnEscape(hspArrayData);


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

            //コードを整形する
            code = Formatter.Format(tree.GetRoot(), MSBuildWorkspace.Create()).ToFullString();

            //ハイライト用の前処理
            tree = CSharpSyntaxTree.ParseText(code);
            var compilation = CSharpCompilation.Create("code", syntaxTrees: new[] { tree }, references: references);

            Console.WriteLine("<C# Code>");
            //コードをハイライト付けて表示
            var syntax = new SyntaxHighlight(compilation, tree);
            syntax.highlight();
            var syntaxDiagnostics = tree.GetDiagnostics().ToList();

            if (syntaxDiagnostics.Count > 0)
            {
                foreach (var diagnostics in syntaxDiagnostics)
                {
                    var e =
                        diagnostics.ToString().Split(':')[0].Replace("(", "")
                            .Replace(")", "")
                            .Split(',')
                            .Select(i => int.Parse(i.Trim()))
                            .ToArray();
                    errorLine.Add(e);
                }
            }

            var line = 0;
            for (var i = 0; i < syntax.view.Count; i++)
            {
                for (var j = 0; j < errorLine.Count; j++)
                {
                    if (errorLine[j][0] == line + 1)
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                    }
                }
                Console.ForegroundColor = syntax.view[i].Color;
                Console.Write(syntax.view[i].Code);
                Console.ResetColor();
                line += syntax.view[i].Code.Length - syntax.view[i].Code.Replace("\n", "").Length;
            }

            Console.WriteLine("\n========================\n");

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
                    ? new CompilerParameters {GenerateExecutable = true, OutputAssembly = executiveFileName}
                    : new CompilerParameters();

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