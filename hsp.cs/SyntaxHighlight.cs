/*===============================
             hsp.cs
  Created by @kkrnt && @ygcuber
===============================*/

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace hsp.cs
{
    public class Syntax
    {
        public string Code;
        public ConsoleColor Color;

        public Syntax(string code, ConsoleColor color)
        {
            Code = code;
            Color = color;
        }
    }

    /// <summary>
    /// C#のコードにシンタックスハイライトを付けて表示
    /// </summary>
    public class SyntaxHighlight : SyntaxWalker
    {
        public List<Syntax> view = new List<Syntax>(); 

        private SemanticModel semanticModel;
        private SyntaxTree tree;

        public SyntaxHighlight(Compilation compilation, SyntaxTree _tree)
        {
            tree = _tree;
            semanticModel = compilation.GetSemanticModel(tree);
        }

        public void highlight()
        {
            foreach (var token in tree.GetRoot().DescendantTokens())
            {
                this.VisitToken(token);
            }
        }

        protected override void VisitToken(SyntaxToken token)
        {
            if (token.HasLeadingTrivia)
            {
                foreach (var trivia in token.LeadingTrivia)
                {
                    VisitTrivia(trivia);
                }
            }

            bool isProcessed = false;

            // キーワードであるか
            if (token.IsKeyword())
            {
                view.Add(new Syntax(token.ValueText, ConsoleColor.Blue));
                isProcessed = true;

            }
            else
            {
                switch (token.Kind())
                {
                    // 各種リテラルであるか
                    case SyntaxKind.StringLiteralToken:
                        view.Add(new Syntax('"' + token.ValueText + '"', ConsoleColor.Red));
                        isProcessed = true;
                        break;
                    case SyntaxKind.CharacterLiteralToken:
                        view.Add(new Syntax(token.ValueText, ConsoleColor.Magenta));
                        isProcessed = true;
                        break;
                    case SyntaxKind.NumericLiteralToken:
                        view.Add(new Syntax(token.ValueText, ConsoleColor.DarkGreen));
                        isProcessed = true;
                        break;
                    case SyntaxKind.IdentifierToken:
                        // 何かの名前(変数等)を参照しようとした場合
                        if (token.Parent is SimpleNameSyntax)
                        {
                            var name = (SimpleNameSyntax)token.Parent;
                            // 参照先に関する情報を取得
                            var info = semanticModel.GetSymbolInfo(name);
                            if (info.Symbol != null && info.Symbol.Kind != SymbolKind.ErrorType)
                            {
                                switch (info.Symbol.Kind)
                                {
                                    case SymbolKind.NamedType:
                                        // クラスや列挙などの場合は色づけ
                                        view.Add(new Syntax(token.ValueText, ConsoleColor.Cyan));
                                        isProcessed = true;
                                        break;
                                    case SymbolKind.Namespace:
                                    case SymbolKind.Parameter:
                                    case SymbolKind.Local:
                                    case SymbolKind.Field:
                                    case SymbolKind.Property:
                                        // それ以外は通常の色
                                        view.Add(new Syntax(token.ValueText, ConsoleColor.White));
                                        isProcessed = true;
                                        break;
                                }
                            }
                        }
                        else if (token.Parent is TypeDeclarationSyntax)
                        {
                            // 宣言時のStatementがヒットした場合
                            var name = (TypeDeclarationSyntax)token.Parent;
                            var info = semanticModel.GetDeclaredSymbol(name);
                            if (info != null && info.Kind != SymbolKind.ErrorType)
                            {
                                switch (info.Kind)
                                {
                                    case SymbolKind.NamedType:
                                        view.Add(new Syntax(token.ValueText, ConsoleColor.Cyan));
                                        isProcessed = true;
                                        break;
                                }
                            }
                        }
                        break;
                }
            }

            // それ以外の項目 (今のところ、特殊例はすべて色づけしない)
            if (!isProcessed)
            {
                view.Add(new Syntax(token.ValueText, ConsoleColor.White));
            }

            if (token.HasTrailingTrivia)
            {
                foreach (var trivia in token.TrailingTrivia)
                {
                    VisitTrivia(trivia);
                }
            }

        }

        protected override void VisitTrivia(SyntaxTrivia trivia)
        {
            switch (trivia.Kind())
            {
                // コメント
                case SyntaxKind.MultiLineCommentTrivia:
                case SyntaxKind.SingleLineCommentTrivia:
                    view.Add(new Syntax(trivia.ToFullString(), ConsoleColor.Green));
                    break;
                // 無効になっているテキスト
                case SyntaxKind.DisabledTextTrivia:
                    view.Add(new Syntax(trivia.ToFullString(), ConsoleColor.DarkGreen));
                    break;
                // ドキュメントコメント
                case SyntaxKind.MultiLineDocumentationCommentTrivia:
                case SyntaxKind.SingleLineDocumentationCommentTrivia:
                case SyntaxKind.DocumentationCommentExteriorTrivia:
                    view.Add(new Syntax(trivia.ToFullString(), ConsoleColor.Green));
                    break;
                // #region
                case SyntaxKind.RegionDirectiveTrivia:
                case SyntaxKind.EndRegionDirectiveTrivia:
                    view.Add(new Syntax(trivia.ToFullString(), ConsoleColor.Gray));
                    break;
                // 空白
                case SyntaxKind.WhitespaceTrivia:
                    view.Add(new Syntax(trivia.ToFullString(), ConsoleColor.White));
                    break;
                // 改行
                case SyntaxKind.EndOfLineTrivia:
                    view.Add(new Syntax(trivia.ToFullString(), ConsoleColor.White));
                    break;
                // それ以外
                default:
                    view.Add(new Syntax(trivia.ToFullString(), ConsoleColor.White));
                    break;
            }
            base.VisitTrivia(trivia);
        }
    }
}
