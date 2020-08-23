
using System.Collections;
using System.IO;
using System.Linq;
using Zu.TypeScript;
using Zu.TypeScript.TsTypes;

namespace resource.preview
{
    public class JS : cartridge.AnyPreview
    {
        protected override void _Execute(atom.Trace context, string url)
        {
            var a_Context = (new TypeScriptAST(File.ReadAllText(url), url)).RootNode as SourceFile;
            var a_IsFound = GetProperty(NAME.PROPERTY.DEBUGGING_SHOW_PRIVATE) != 0;
            if (a_Context == null)
            {
                return;
            }
            {
                context.
                    SetState(NAME.STATE.HEADER).
                    SetState(NAME.STATE.EXPAND).
                    Send(NAME.PATTERN.FOLDER, 1, "[[Info]]");
                {
                    context.
                        SetValue(url).
                        Send(NAME.PATTERN.VARIABLE, 2, "[[File name]]");
                    context.
                        SetValue(a_Context.SourceStr.Length.ToString()).
                        Send(NAME.PATTERN.VARIABLE, 2, "[[File size]]");
                    context.
                        SetValue("JavaScript").
                        Send(NAME.PATTERN.VARIABLE, 2, "[[Language]]");
                }
            }
            if (a_Context.GetDescendants().OfType<ImportDeclaration>().Any())
            {
                context.
                    SetComment(__GetArraySize(a_Context.GetDescendants().OfType<ImportDeclaration>())).
                    Send(NAME.PATTERN.FOLDER, 1, "[[Imports]]");
                foreach (var a_Context1 in a_Context.GetDescendants().OfType<ImportDeclaration>())
                {
                    __Execute(a_Context1, 2, context, url);
                }
            }
            if (a_Context.GetDescendants().OfType<ExportDeclaration>().Any())
            {
                context.
                    SetComment(__GetArraySize(a_Context.GetDescendants().OfType<ExportDeclaration>())).
                    Send(NAME.PATTERN.FOLDER, 1, "[[Exports]]");
                foreach (var a_Context1 in a_Context.GetDescendants().OfType<ExportDeclaration>())
                {
                    __Execute(a_Context1, 2, context, url);
                }
            }
            if (a_Context.GetDescendants().OfType<ClassDeclaration>().Any())
            {
                context.
                    SetComment(__GetArraySize(a_Context.GetDescendants().OfType<ClassDeclaration>())).
                    Send(NAME.PATTERN.FOLDER, 1, "[[Classes]]");
                foreach (var a_Context1 in a_Context.GetDescendants().OfType<ClassDeclaration>())
                {
                    __Execute(a_Context1, 2, context, url, a_IsFound);
                }
            }
            if (a_Context.GetDescendants().OfType<InterfaceDeclaration>().Any())
            {
                context.
                    SetComment(__GetArraySize(a_Context.GetDescendants().OfType<InterfaceDeclaration>())).
                    Send(NAME.PATTERN.FOLDER, 1, "[[Interfaces]]");
                foreach (var a_Context1 in a_Context.GetDescendants().OfType<InterfaceDeclaration>())
                {
                    __Execute(a_Context1, 2, context, url, a_IsFound);
                }
            }
            if (a_Context.GetDescendants().OfType<EnumDeclaration>().Any())
            {
                context.
                    SetComment(__GetArraySize(a_Context.GetDescendants().OfType<EnumDeclaration>())).
                    Send(NAME.PATTERN.FOLDER, 1, "[[Enums]]");
                foreach (var a_Context1 in a_Context.GetDescendants().OfType<EnumDeclaration>())
                {
                    __Execute(a_Context1, 2, context, url, a_IsFound);
                }
            }
            if (a_Context.GetDescendants().OfType<FunctionDeclaration>().Any())
            {
                context.
                    SetComment(__GetArraySize(a_Context.GetDescendants().OfType<FunctionDeclaration>())).
                    Send(NAME.PATTERN.FOLDER, 1, "[[Functions]]");
                foreach (var a_Context1 in a_Context.GetDescendants().OfType<FunctionDeclaration>())
                {
                    __Execute(a_Context1, 2, context, url, true, a_IsFound);
                }
            }
            if (a_Context.Statements.OfType<VariableStatement>().Any())
            {
                context.
                    SetComment(__GetArraySize(a_Context.Statements.OfType<VariableStatement>())).
                    Send(NAME.PATTERN.FOLDER, 1, "[[Variables]]");
                foreach (var a_Context1 in a_Context.Statements.OfType<VariableStatement>())
                {
                    __Execute(a_Context1, 2, context, url, a_IsFound);
                }
            }
            if ((a_Context.ParseDiagnostics != null) && a_Context.ParseDiagnostics.Any())
            {
                context.
                    SetComment(__GetArraySize(a_Context.ParseDiagnostics)).
                    Send(NAME.PATTERN.FOLDER, 1, "[[Diagnostics]]");
                foreach (var a_Context2 in a_Context.ParseDiagnostics)
                {
                    __Execute(a_Context2, 2, context, url);
                }
            }
            if (GetState() == STATE.CANCEL)
            {
                context.
                    SendWarning(1, NAME.WARNING.TERMINATED);
            }
        }

        private static void __Execute(Diagnostic node, int level, atom.Trace context, string url)
        {
            if (string.IsNullOrEmpty(node.MessageText?.ToString()) == false)
            {
                context.
                    SetState(__GetFlag(node)).
                    SetUrlLine(__GetLine(node.File, node.Start)).
                    SetUrlPosition(__GetPosition(node.File, node.Start)).
                    SetUrl(url).
                    SetLink((node.Code > 0) ? ("https://www.bing.com/search?q=JavaScript+error+code+" + node.Code.ToString()) : "").
                    Send(NAME.PATTERN.ELEMENT, level, node.MessageText.ToString() == "localizedDiagnosticMessages" ? "[[Syntax error]]" : node.MessageText.ToString());
            }
        }

        private static void __Execute(ImportDeclaration node, int level, atom.Trace context, string url)
        {
            context.
                SetComment("import").
                SetCommentHint("[[Data type]]").
                SetUrlLine(__GetLine(node, node.Pos.Value)).
                SetUrlPosition(__GetPosition(node, node.Pos.Value)).
                SetUrl(url).
                Send(NAME.PATTERN.ELEMENT, level, node.GetText());
        }

        private static void __Execute(ExportDeclaration node, int level, atom.Trace context, string url)
        {
            context.
                SetComment("export").
                SetCommentHint("[[Data type]]").
                SetUrlLine(__GetLine(node, node.Pos.Value)).
                SetUrlPosition(__GetPosition(node, node.Pos.Value)).
                SetUrl(url).
                Send(NAME.PATTERN.ELEMENT, level, node.GetText());
        }

        private static void __Execute(EnumDeclaration node, int level, atom.Trace context, string url, bool isShowPrivate)
        {
            if (__IsEnabled(node, isShowPrivate))
            {
                context.
                    SetComment(__GetType(node, "enum")).
                    SetCommentHint("[[Data type]]").
                    SetUrlLine(__GetLine(node, node.Name.Pos.Value)).
                    SetUrlPosition(__GetPosition(node, node.Name.Pos.Value)).
                    SetUrl(url).
                    Send(NAME.PATTERN.CLASS, level, __GetName(node.Name, true));
                foreach (var a_Context in node.Members.OfType<EnumMember>())
                {
                    context.
                        SetComment("int").
                        SetCommentHint("[[Data type]]").
                        SetUrlLine(__GetLine(a_Context, a_Context.Name.Pos.Value)).
                        SetUrlPosition(__GetPosition(a_Context, a_Context.Name.Pos.Value)).
                        SetUrl(url).
                        Send(NAME.PATTERN.ELEMENT, level + 1, __GetName(a_Context.Name, false));
                }
            }
        }

        private static void __Execute(ClassDeclaration node, int level, atom.Trace context, string url, bool isShowPrivate)
        {
            if (__IsEnabled(node, isShowPrivate))
            {
                context.
                    SetComment(__GetType(node, "class")).
                    SetCommentHint("[[Data type]]").
                    SetUrlLine(__GetLine(node, node.Name.Pos.Value)).
                    SetUrlPosition(__GetPosition(node, node.Name.Pos.Value)).
                    SetUrl(url).
                    Send(NAME.PATTERN.CLASS, level, __GetName(node.Name, true));
                foreach (var a_Context in node.Members.OfType<MethodDeclaration>())
                {
                    __Execute(a_Context, level + 1, context, url, false, isShowPrivate);
                }
                foreach (var a_Context in node.Members.OfType<PropertyDeclaration>())
                {
                    __Execute(a_Context, level + 1, context, url, isShowPrivate);
                }
                foreach (var a_Context in node.Members.OfType<VariableDeclaration>())
                {
                    __Execute(a_Context, level + 1, context, url, isShowPrivate);
                }
            }
        }

        private static void __Execute(InterfaceDeclaration node, int level, atom.Trace context, string url, bool isShowPrivate)
        {
            if (__IsEnabled(node, isShowPrivate))
            {
                context.
                    SetComment(__GetType(node, "interface")).
                    SetCommentHint("[[Data type]]").
                    SetUrlLine(__GetLine(node, node.Name.Pos.Value)).
                    SetUrlPosition(__GetPosition(node, node.Name.Pos.Value)).
                    SetUrl(url).
                    Send(NAME.PATTERN.CLASS, level, __GetName(node.Name, true));
                foreach (var a_Context in node.Members.OfType<MethodSignature>())
                {
                    __Execute(a_Context, level + 1, context, url, false, isShowPrivate);
                }
            }
        }

        private static void __Execute(FunctionDeclaration node, int level, atom.Trace context, string url, bool isFullName, bool isShowPrivate)
        {
            if (__IsEnabled(node, isShowPrivate))
            {
                context.
                    SetComment(__GetType(node, "function")).
                    SetCommentHint("[[Function type]]").
                    SetUrlLine(__GetLine(node, node.Name.Pos.Value)).
                    SetUrlPosition(__GetPosition(node, node.Name.Pos.Value)).
                    SetUrl(url).
                    Send(NAME.PATTERN.FUNCTION, level, __GetName(node.Name, isFullName) + __GetParams(node.Parameters));
            }
        }

        private static void __Execute(MethodDeclaration node, int level, atom.Trace context, string url, bool isFullName, bool isShowPrivate)
        {
            if (__IsEnabled(node, isShowPrivate))
            {
                context.
                    SetComment(__GetType(node, "method")).
                    SetCommentHint("[[Method type]]").
                    SetUrlLine(__GetLine(node, node.Name.Pos.Value)).
                    SetUrlPosition(__GetPosition(node, node.Name.Pos.Value)).
                    SetUrl(url).
                    Send(NAME.PATTERN.FUNCTION, level, __GetName(node.Name, isFullName) + __GetParams(node.Parameters));
            }
        }

        private static void __Execute(MethodSignature node, int level, atom.Trace context, string url, bool isFullName, bool isShowPrivate)
        {
            if (__IsEnabled(node, isShowPrivate))
            {
                context.
                    SetComment(__GetType(node, "method")).
                    SetCommentHint("[[Method type]]").
                    SetUrlLine(__GetLine(node, node.Name.Pos.Value)).
                    SetUrlPosition(__GetPosition(node, node.Name.Pos.Value)).
                    SetUrl(url).
                    Send(NAME.PATTERN.FUNCTION, level, __GetName(node.Name, isFullName) + __GetParams(node.Parameters));
            }
        }

        private static void __Execute(PropertyDeclaration node, int level, atom.Trace context, string url, bool isShowPrivate)
        {
            if (__IsEnabled(node, isShowPrivate))
            {
                context.
                    SetComment(__GetType(node, "property")).
                    SetCommentHint("[[Data type]]").
                    SetUrlLine(__GetLine(node, node.Name.Pos.Value)).
                    SetUrlPosition(__GetPosition(node, node.Name.Pos.Value)).
                    SetUrl(url).
                    SetValue(__GetValue(node.Initializer)).
                    Send(NAME.PATTERN.PARAMETER, level, node.IdentifierStr);
            }
        }

        private static void __Execute(VariableDeclaration node, int level, atom.Trace context, string url, bool isShowPrivate)
        {
            if (__IsEnabled(node, isShowPrivate))
            {
                context.
                    SetComment(__GetType(node, "variable")).
                    SetCommentHint("[[Data type]]").
                    SetUrlLine(__GetLine(node, node.Name.Pos.Value)).
                    SetUrlPosition(__GetPosition(node, node.Name.Pos.Value)).
                    SetUrl(url).
                    SetValue(__GetValue(node.Initializer)).
                    Send(NAME.PATTERN.VARIABLE, level, node.IdentifierStr);
            }
        }

        private static void __Execute(VariableStatement node, int level, atom.Trace context, string url, bool isShowPrivate)
        {
            if (__IsEnabled(node, isShowPrivate))
            {
                var a_Context = node.GetDescendants().OfType<Identifier>()?.First();
                if (a_Context != null)
                {
                    context.
                        SetComment(__GetType(node, "variable")).
                        SetCommentHint("[[Data type]]").
                        SetUrlLine(__GetLine(a_Context, a_Context.Pos.Value)).
                        SetUrlPosition(__GetPosition(a_Context, a_Context.Pos.Value)).
                        SetUrl(url).
                        SetValue("...").
                        Send(NAME.PATTERN.VARIABLE, level, a_Context.IdentifierStr);
                }
            }
        }


        private static bool __IsEnabled(Node node, bool isShowPrivate)
        {
            if (GetState() == STATE.CANCEL)
            {
                return false;
            }
            if (node.Flags.HasFlag(NodeFlags.ThisNodeHasError))
            {
                return false;
            }
            if ((isShowPrivate == false) && (node?.Children != null))
            {
                foreach (var a_Context in node?.Children.OfType<Modifier>())
                {
                    if (a_Context.Kind == SyntaxKind.PrivateKeyword)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static string __GetType(Node node, string typeName)
        {
            var a_Result = typeName;
            if (node?.Children != null)
            {
                foreach (var a_Context in node.Children.OfType<Modifier>())
                {
                    switch (a_Context.Kind)
                    {
                        case SyntaxKind.ConstKeyword: a_Result = "const " + a_Result; break;
                        case SyntaxKind.DefaultKeyword: a_Result = "default " + a_Result; break;
                        case SyntaxKind.EnumKeyword: a_Result = "enum " + a_Result; break;
                        case SyntaxKind.SuperKeyword: a_Result = "super " + a_Result; break;
                        case SyntaxKind.InterfaceKeyword: a_Result = "interface " + a_Result; break;
                        case SyntaxKind.PackageKeyword: a_Result = "package " + a_Result; break;
                        case SyntaxKind.PrivateKeyword: a_Result = "private " + a_Result; break;
                        case SyntaxKind.ProtectedKeyword: a_Result = "protected " + a_Result; break;
                        case SyntaxKind.PublicKeyword: a_Result = "public " + a_Result; break;
                        case SyntaxKind.StaticKeyword: a_Result = "static " + a_Result; break;
                        case SyntaxKind.YieldKeyword: a_Result = "yield " + a_Result; break;
                        case SyntaxKind.AbstractKeyword: a_Result = "abstract" + a_Result; break;
                        case SyntaxKind.AsyncKeyword: a_Result = "async " + a_Result; break;
                        case SyntaxKind.ConstructorKeyword: a_Result = "constructor " + a_Result; break;
                        case SyntaxKind.DeclareKeyword: a_Result = "declare " + a_Result; break;
                        case SyntaxKind.ModuleKeyword: a_Result = "module " + a_Result; break;
                        case SyntaxKind.NamespaceKeyword: a_Result = "namespace " + a_Result; break;
                        case SyntaxKind.NeverKeyword: a_Result = "never " + a_Result; break;
                        case SyntaxKind.ReadonlyKeyword: a_Result = "readonly " + a_Result; break;
                        case SyntaxKind.RequireKeyword: a_Result = "require " + a_Result; break;
                        case SyntaxKind.UndefinedKeyword: a_Result = "undefined " + a_Result; break;
                        case SyntaxKind.GlobalKeyword: a_Result = "global " + a_Result; break;
                    }
                }
            }
            return a_Result;
        }

        internal static string __GetArraySize(IEnumerable value)
        {
            var a_Result = 0;
            foreach (var a_Context in value)
            {
                a_Result++;
            }
            return "[[Found]]: " + a_Result.ToString();
        }

        private static string __GetFlag(Diagnostic node)
        {
            switch (node.Category)
            {
                case DiagnosticCategory.Message: return NAME.STATE.NONE;
                case DiagnosticCategory.Error: return NAME.STATE.ERROR;
            }
            return NAME.STATE.WARNING;
        }

        private static string __GetName(INode node)
        {
            if (node is Identifier)
            {
                return (node as Identifier).IdentifierStr;
            }
            return "";
        }

        private static string __GetName(INode node, bool isFullName)
        {
            var a_Result = "";
            if (isFullName)
            {
                var a_Context = node?.Parent?.Parent;
                while (a_Context != null)
                {
                    if (a_Context is NamespaceDeclaration)
                    {
                        a_Result += __GetName((a_Context as NamespaceDeclaration)?.Name);
                        a_Result += string.IsNullOrEmpty(a_Result) ? "" : ".";
                    }
                    if (a_Context is ModuleDeclaration)
                    {
                        a_Result += __GetName((a_Context as ModuleDeclaration)?.Name);
                        a_Result += string.IsNullOrEmpty(a_Result) ? "" : ".";
                    }
                    if (a_Context is ClassDeclaration)
                    {
                        a_Result += __GetName((a_Context as ClassDeclaration)?.Name);
                        a_Result += string.IsNullOrEmpty(a_Result) ? "" : ".";
                    }
                    {
                        a_Context = a_Context.Parent;
                    }
                }
            }
            return (a_Result + __GetName(node)).Trim();
        }

        private static string __GetParams(NodeArray<ParameterDeclaration> node)
        {
            if (node != null)
            {
                var a_Result = "";
                foreach (var a_Context in node)
                {
                    if (string.IsNullOrEmpty(a_Result) == false)
                    {
                        a_Result += ", ";
                    }
                    {
                        a_Result += a_Context.IdentifierStr;
                    }
                }
                return "(" + a_Result + ")";
            }
            return "";
        }

        private static string __GetValue(IExpression node)
        {
            if (node is BinaryExpression)
            {
                return "...";
            }
            if (node is LiteralExpression)
            {
                return (node as LiteralExpression).Text;
            }
            return "";
        }

        private static int __GetLine(Node node, int position)
        {
            var a_Context = node.SourceStr;
            var a_Result = 1;
            var a_Size = a_Context.Length;
            for (var i = 0; i < a_Size; i++)
            {
                if (a_Context[i] == '\n')
                {
                    a_Result++;
                }
                if (i >= position)
                {
                    switch (a_Context[i])
                    {
                        case ' ':
                        case '\r':
                        case '\n':
                        case '\t':
                            continue;
                    }
                    break;
                }
            }
            return a_Result;
        }

        private static int __GetPosition(Node node, int position)
        {
            var a_Context = node.SourceStr;
            var a_Index = 0;
            var a_Result = 0;
            var a_Size = a_Context.Length;
            for (var i = 0; i < a_Size; i++)
            {
                if (a_Context[i] == '\n')
                {
                    a_Result = i;
                }
                if (i >= position)
                {
                    switch (a_Context[i])
                    {
                        case ' ':
                        case '\r':
                        case '\n':
                        case '\t':
                            a_Index = i + 1;
                            continue;
                    }
                    break;
                }
            }
            if (a_Index > 0)
            {
                return a_Index - a_Result;
            }
            else
            {
                return position - a_Result;
            }
        }
    };
}
