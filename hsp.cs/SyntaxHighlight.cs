/*===============================
             hsp.cs
  Created by @kkrnt && @ygcuber
===============================*/

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace hsp.cs
{
    /// <summary>
    /// C#のコードにシンタックスハイライトを付けて表示
    /// </summary>
    public class SyntaxHighlight : SyntaxWalker
    {
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
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write(token.ValueText);
                Console.ResetColor();
                isProcessed = true;

            }
            else
            {
                switch (token.Kind())
                {
                    // 各種リテラルであるか
                    case SyntaxKind.StringLiteralToken:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write('"' + token.ValueText + '"');
                        isProcessed = true;
                        break;
                    case SyntaxKind.CharacterLiteralToken:
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.Write(token.ValueText);
                        isProcessed = true;
                        break;
                    case SyntaxKind.NumericLiteralToken:
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write(token.ValueText);
                        isProcessed = true;
                        break;
                    case SyntaxKind.IdentifierToken:
                        // 何かの名前(変数等)を参照しようとした場合
                        var syntax = token.Parent as SimpleNameSyntax;
                        if (syntax != null)
                        {
                            var name = syntax;
                            // 参照先に関する情報を取得
                            var info = ModelExtensions.GetSymbolInfo(semanticModel, name);
                            if (info.Symbol != null && info.Symbol.Kind != SymbolKind.ErrorType)
                            {
                                switch (info.Symbol.Kind)
                                {
                                    case SymbolKind.NamedType:
                                        // クラスや列挙などの場合は色づけ
                                        Console.ForegroundColor = ConsoleColor.Cyan;
                                        Console.Write(token.ValueText);
                                        isProcessed = true;
                                        break;
                                    case SymbolKind.Namespace:
                                    case SymbolKind.Parameter:
                                    case SymbolKind.Local:
                                    case SymbolKind.Field:
                                    case SymbolKind.Property:
                                        // それ以外は通常の色
                                        Console.ForegroundColor = ConsoleColor.White;
                                        Console.Write(token.ValueText);
                                        isProcessed = true;
                                        break;
                                }
                            }
                        }
                        else if (token.Parent is TypeDeclarationSyntax)
                        {
                            // 宣言時のStatementがヒットした場合
                            var name = (TypeDeclarationSyntax)token.Parent;
                            var info = ModelExtensions.GetDeclaredSymbol(semanticModel, name);
                            if (info != null && info.Kind != SymbolKind.ErrorType)
                            {
                                switch (info.Kind)
                                {
                                    case SymbolKind.NamedType:
                                        Console.ForegroundColor = ConsoleColor.Cyan;
                                        Console.Write(token.ValueText);
                                        Console.ResetColor();
                                        isProcessed = true;
                                        break;
                                }
                            }
                        }
                        break;
                }
                Console.ResetColor();
            }

            if (!isProcessed)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(token.ValueText);
                Console.ResetColor();
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
                // ドキュメントコメント
                case SyntaxKind.MultiLineDocumentationCommentTrivia:
                case SyntaxKind.SingleLineDocumentationCommentTrivia:
                case SyntaxKind.DocumentationCommentExteriorTrivia:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(trivia.ToFullString());
                    break;
                // 無効になっているテキスト
                case SyntaxKind.DisabledTextTrivia:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(trivia.ToFullString());
                    break;
                // #region
                case SyntaxKind.RegionDirectiveTrivia:
                case SyntaxKind.EndRegionDirectiveTrivia:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(trivia.ToFullString());
                    break;
                // それ以外
                default:
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(trivia.ToFullString());
                    break;
            }
            Console.ResetColor();
            base.VisitTrivia(trivia);
        }
    }
}
