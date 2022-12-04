using System.Collections;
using System.IO;
using System.Linq;
using Zu.TypeScript;
using Zu.TypeScript.TsTypes;

namespace resource.preview
{
    internal class VSPreview : extension.AnyPreview
    {
        protected override void _Execute(atom.Trace context, int level, string url, string file)
        {
            var a_Context = (new TypeScriptAST(File.ReadAllText(file), file)).RootNode as SourceFile;
            var a_IsFound = GetProperty(NAME.PROPERTY.DEBUGGING_SHOW_PRIVATE, true) != 0;
            if (a_Context == null)
            {
                return;
            }
            {
                context.Send(NAME.SOURCE.PREVIEW, NAME.EVENT.HEADER, level, "[[[Info]]]");
                {
                    context.Send(NAME.SOURCE.PREVIEW, NAME.EVENT.PARAMETER, level + 1, "[[[File Name]]]", url);
                    context.Send(NAME.SOURCE.PREVIEW, NAME.EVENT.PARAMETER, level + 1, "[[[File Size]]]", a_Context.SourceStr.Length.ToString());
                    context.Send(NAME.SOURCE.PREVIEW, NAME.EVENT.PARAMETER, level + 1, "[[[Language]]]", "JavaScript");
                }
            }
            if (a_Context.GetDescendants().OfType<ImportDeclaration>().Any())
            {
                context.
                    SetComment(__GetArraySize(a_Context.GetDescendants().OfType<ImportDeclaration>())).
                    Send(NAME.SOURCE.PREVIEW, NAME.EVENT.FOLDER, level, "[[[Imports]]]");
                foreach (var a_Context1 in a_Context.GetDescendants().OfType<ImportDeclaration>())
                {
                    __Execute(context, level + 1, a_Context1, file);
                }
            }
            if (a_Context.GetDescendants().OfType<ExportDeclaration>().Any())
            {
                context.
                    SetComment(__GetArraySize(a_Context.GetDescendants().OfType<ExportDeclaration>())).
                    Send(NAME.SOURCE.PREVIEW, NAME.EVENT.FOLDER, level, "[[[Exports]]]");
                foreach (var a_Context1 in a_Context.GetDescendants().OfType<ExportDeclaration>())
                {
                    __Execute(context, level + 1, a_Context1, file);
                }
            }
            if (a_Context.GetDescendants().OfType<ClassDeclaration>().Any())
            {
                context.
                    SetComment(__GetArraySize(a_Context.GetDescendants().OfType<ClassDeclaration>())).
                    Send(NAME.SOURCE.PREVIEW, NAME.EVENT.FOLDER, level, "[[[Classes]]]");
                foreach (var a_Context1 in a_Context.GetDescendants().OfType<ClassDeclaration>())
                {
                    __Execute(context, level + 1, a_Context1, file, a_IsFound);
                }
            }
            if (a_Context.GetDescendants().OfType<InterfaceDeclaration>().Any())
            {
                context.
                    SetComment(__GetArraySize(a_Context.GetDescendants().OfType<InterfaceDeclaration>())).
                    Send(NAME.SOURCE.PREVIEW, NAME.EVENT.FOLDER, level, "[[[Interfaces]]]");
                foreach (var a_Context1 in a_Context.GetDescendants().OfType<InterfaceDeclaration>())
                {
                    __Execute(context, level + 1, a_Context1, file, a_IsFound);
                }
            }
            if (a_Context.GetDescendants().OfType<EnumDeclaration>().Any())
            {
                context.
                    SetComment(__GetArraySize(a_Context.GetDescendants().OfType<EnumDeclaration>())).
                    Send(NAME.SOURCE.PREVIEW, NAME.EVENT.FOLDER, level, "[[[Enums]]]");
                foreach (var a_Context1 in a_Context.GetDescendants().OfType<EnumDeclaration>())
                {
                    __Execute(context, level + 1, a_Context1, file, a_IsFound);
                }
            }
            if (a_Context.GetDescendants().OfType<FunctionDeclaration>().Any())
            {
                context.
                    SetComment(__GetArraySize(a_Context.GetDescendants().OfType<FunctionDeclaration>())).
                    Send(NAME.SOURCE.PREVIEW, NAME.EVENT.FOLDER, level, "[[[Functions]]]");
                foreach (var a_Context1 in a_Context.GetDescendants().OfType<FunctionDeclaration>())
                {
                    __Execute(context, level + 1, a_Context1, file, true, a_IsFound);
                }
            }
            if (a_Context.Statements.OfType<VariableStatement>().Any())
            {
                context.
                    SetComment(__GetArraySize(a_Context.Statements.OfType<VariableStatement>())).
                    Send(NAME.SOURCE.PREVIEW, NAME.EVENT.FOLDER, level, "[[[Variables]]]");
                foreach (var a_Context1 in a_Context.Statements.OfType<VariableStatement>())
                {
                    __Execute(context, level + 1, a_Context1, file, a_IsFound);
                }
            }
            if ((a_Context.ParseDiagnostics != null) && a_Context.ParseDiagnostics.Any())
            {
                context.
                    SendPreview(NAME.EVENT.ERROR, url).
                    SetComment(__GetArraySize(a_Context.ParseDiagnostics)).
                    Send(NAME.SOURCE.PREVIEW, NAME.EVENT.ERROR, level, "[[[Diagnostics]]]");
                foreach (var a_Context1 in a_Context.ParseDiagnostics)
                {
                    __Execute(context, level + 1, a_Context1, file);
                }
            }
        }

        private static void __Execute(atom.Trace context, int level, Diagnostic data, string file)
        {
            if (string.IsNullOrEmpty(data.MessageText?.ToString()) == false)
            {
                context.
                    SetUrl(file, __GetLine(data.File, data.Start), __GetPosition(data.File, data.Start)).
                    SetUrlInfo((data.Code > 0) ? ("https://www.bing.com/search?q=JavaScript+error+code+" + data.Code.ToString()) : "").
                    Send(NAME.SOURCE.PREVIEW, __GetType(data), level, data.MessageText.ToString() == "localizedDiagnosticMessages" ? "[[[Syntax error]]]" : __GetText(data.MessageText.ToString()));
            }
        }

        private static void __Execute(atom.Trace context, int level, ImportDeclaration data, string file)
        {
            context.
                SetComment("import", "[[[Data Type]]]").
                SetUrl(file, __GetLine(data, data.Pos.Value), __GetPosition(data, data.Pos.Value)).
                Send(NAME.SOURCE.PREVIEW, NAME.EVENT.PARAMETER, level, __GetText(data.GetText()));
        }

        private static void __Execute(atom.Trace context, int level, ExportDeclaration data, string file)
        {
            context.
                SetComment("export", "[[[Data Type]]]").
                SetUrl(file, __GetLine(data, data.Pos.Value), __GetPosition(data, data.Pos.Value)).
                Send(NAME.SOURCE.PREVIEW, NAME.EVENT.PARAMETER, level, __GetText(data.GetText()));
        }

        private static void __Execute(atom.Trace context, int level, EnumDeclaration data, string file, bool isShowPrivate)
        {
            if (__IsEnabled(data, isShowPrivate))
            {
                context.
                    SetComment(__GetType(data, "enum"), "[[[Data Type]]]").
                    SetUrl(file, __GetLine(data, data.Name.Pos.Value), __GetPosition(data, data.Name.Pos.Value)).
                    Send(NAME.SOURCE.PREVIEW, NAME.EVENT.CLASS, level, __GetName(data.Name, true));
                foreach (var a_Context in data.Members.OfType<EnumMember>())
                {
                    context.
                        SetComment("int", "[[[Data Type]]]").
                        SetUrl(file, __GetLine(a_Context, a_Context.Name.Pos.Value), __GetPosition(a_Context, a_Context.Name.Pos.Value)).
                        Send(NAME.SOURCE.PREVIEW, NAME.EVENT.PARAMETER, level + 1, __GetName(a_Context.Name, false));
                }
            }
        }

        private static void __Execute(atom.Trace context, int level, ClassDeclaration data, string file, bool isShowPrivate)
        {
            if (__IsEnabled(data, isShowPrivate))
            {
                context.
                    SetComment(__GetType(data, "class"), "[[[Data Type]]]").
                    SetUrl(file, __GetLine(data, data.Name.Pos.Value), __GetPosition(data, data.Name.Pos.Value)).
                    Send(NAME.SOURCE.PREVIEW, NAME.EVENT.CLASS, level, __GetName(data.Name, true));
                foreach (var a_Context in data.Members.OfType<MethodDeclaration>())
                {
                    __Execute(context, level + 1, a_Context, file, false, isShowPrivate);
                }
                foreach (var a_Context in data.Members.OfType<PropertyDeclaration>())
                {
                    __Execute(context, level + 1, a_Context, file, isShowPrivate);
                }
                foreach (var a_Context in data.Members.OfType<VariableDeclaration>())
                {
                    __Execute(context, level + 1, a_Context, file, isShowPrivate);
                }
            }
        }

        private static void __Execute(atom.Trace context, int level, InterfaceDeclaration data, string file, bool isShowPrivate)
        {
            if (__IsEnabled(data, isShowPrivate))
            {
                context.
                    SetComment(__GetType(data, "interface"), "[[[Data Type]]]").
                    SetUrl(file, __GetLine(data, data.Name.Pos.Value), __GetPosition(data, data.Name.Pos.Value)).
                    Send(NAME.SOURCE.PREVIEW, NAME.EVENT.CLASS, level, __GetName(data.Name, true));
                foreach (var a_Context in data.Members.OfType<MethodSignature>())
                {
                    __Execute(context, level + 1, a_Context, file, false, isShowPrivate);
                }
            }
        }

        private static void __Execute(atom.Trace context, int level, FunctionDeclaration data, string file, bool isFullName, bool isShowPrivate)
        {
            if (__IsEnabled(data, isShowPrivate))
            {
                context.
                    SetComment(__GetType(data, "function"), "[[[Function Type]]]").
                    SetUrl(file, __GetLine(data, data.Name.Pos.Value), __GetPosition(data, data.Name.Pos.Value)).
                    Send(NAME.SOURCE.PREVIEW, NAME.EVENT.FUNCTION, level, __GetName(data.Name, isFullName) + __GetParams(data.Parameters));
            }
        }

        private static void __Execute(atom.Trace context, int level, MethodDeclaration data, string file, bool isFullName, bool isShowPrivate)
        {
            if (__IsEnabled(data, isShowPrivate))
            {
                context.
                    SetComment(__GetType(data, "method"), "[[[Method Type]]]").
                    SetUrl(file, __GetLine(data, data.Name.Pos.Value), __GetPosition(data, data.Name.Pos.Value)).
                    Send(NAME.SOURCE.PREVIEW, NAME.EVENT.FUNCTION, level, __GetName(data.Name, isFullName) + __GetParams(data.Parameters));
            }
        }

        private static void __Execute(atom.Trace context, int level, MethodSignature data, string file, bool isFullName, bool isShowPrivate)
        {
            if (__IsEnabled(data, isShowPrivate))
            {
                context.
                    SetComment(__GetType(data, "method"), "[[[Method Type]]]").
                    SetUrl(file, __GetLine(data, data.Name.Pos.Value), __GetPosition(data, data.Name.Pos.Value)).
                    Send(NAME.SOURCE.PREVIEW, NAME.EVENT.FUNCTION, level, __GetName(data.Name, isFullName) + __GetParams(data.Parameters));
            }
        }

        private static void __Execute(atom.Trace context, int level, PropertyDeclaration data, string file, bool isShowPrivate)
        {
            if (__IsEnabled(data, isShowPrivate))
            {
                context.
                    SetComment(__GetType(data, "property"), "[[[Data Type]]]").
                    SetUrl(file, __GetLine(data, data.Name.Pos.Value), __GetPosition(data, data.Name.Pos.Value)).
                    SetValue(__GetValue(data.Initializer)).
                    Send(NAME.SOURCE.PREVIEW, NAME.EVENT.PARAMETER, level, __GetText(data.IdentifierStr));
            }
        }

        private static void __Execute(atom.Trace context, int level, VariableDeclaration data, string file, bool isShowPrivate)
        {
            if (__IsEnabled(data, isShowPrivate))
            {
                context.
                    SetComment(__GetType(data, "variable"), "[[[Data Type]]]").
                    SetUrl(file, __GetLine(data, data.Name.Pos.Value), __GetPosition(data, data.Name.Pos.Value)).
                    SetValue(__GetValue(data.Initializer)).
                    Send(NAME.SOURCE.PREVIEW, NAME.EVENT.VARIABLE, level, __GetText(data.IdentifierStr));
            }
        }

        private static void __Execute(atom.Trace context, int level, VariableStatement data, string file, bool isShowPrivate)
        {
            if (__IsEnabled(data, isShowPrivate))
            {
                var a_Context = data.GetDescendants().OfType<Identifier>()?.First();
                if (a_Context != null)
                {
                    context.
                        SetComment(__GetType(data, "variable"), "[[[Data Type]]]").
                        SetUrl(file, __GetLine(a_Context, a_Context.Pos.Value), __GetPosition(a_Context, a_Context.Pos.Value)).
                        Send(NAME.SOURCE.PREVIEW, NAME.EVENT.VARIABLE, level, __GetText(a_Context.IdentifierStr), "...");
                }
            }
        }


        private static bool __IsEnabled(Node data, bool isShowPrivate)
        {
            if (GetState() == NAME.STATE.CANCEL)
            {
                return false;
            }
            if (data.Flags.HasFlag(NodeFlags.ThisNodeHasError))
            {
                return false;
            }
            if ((isShowPrivate == false) && (data?.Children != null))
            {
                foreach (var a_Context in data?.Children.OfType<Modifier>())
                {
                    if (a_Context.Kind == SyntaxKind.PrivateKeyword)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static string __GetType(Node data, string typeName)
        {
            var a_Result = typeName;
            if (data?.Children != null)
            {
                foreach (var a_Context in data.Children.OfType<Modifier>())
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

        internal static string __GetArraySize(IEnumerable data)
        {
            var a_Result = 0;
            foreach (var a_Context in data)
            {
                a_Result++;
            }
            return "[[[Found]]]: " + a_Result.ToString();
        }

        private static string __GetText(string value)
        {
            var a_Result = (value != null) ? value.Trim() : "";
            if (a_Result.Contains("\t"))
            {
                a_Result = a_Result.Replace("\t", " ");
            }
            if (a_Result.Contains("\r"))
            {
                a_Result = a_Result.Replace("\r", " ");
            }
            if (a_Result.Contains("\n"))
            {
                a_Result = a_Result.Replace("\n", " ");
            }
            while (a_Result.Contains("  "))
            {
                a_Result = a_Result.Replace("  ", " ");
            }
            return a_Result;
        }

        private static string __GetType(Diagnostic data)
        {
            switch (data.Category)
            {
                case DiagnosticCategory.Message: return NAME.EVENT.PARAMETER;
                case DiagnosticCategory.Error: return NAME.EVENT.ERROR;
            }
            return NAME.EVENT.WARNING;
        }

        private static string __GetName(INode data)
        {
            if (data is Identifier)
            {
                return (data as Identifier).IdentifierStr;
            }
            return "";
        }

        private static string __GetName(INode data, bool isFullName)
        {
            var a_Result = "";
            if (isFullName)
            {
                var a_Context = data?.Parent?.Parent;
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
            return (a_Result + __GetName(data)).Trim();
        }

        private static string __GetParams(NodeArray<ParameterDeclaration> data)
        {
            if (data != null)
            {
                var a_Result = "";
                foreach (var a_Context in data)
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

        private static string __GetValue(IExpression data)
        {
            if (data is BinaryExpression)
            {
                return "...";
            }
            if (data is LiteralExpression)
            {
                return (data as LiteralExpression).Text;
            }
            return "";
        }

        private static int __GetLine(Node data, int position)
        {
            var a_Context = data.SourceStr;
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

        private static int __GetPosition(Node data, int position)
        {
            var a_Context = data.SourceStr;
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
