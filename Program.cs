using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CSharp;

namespace CodeDom
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var compileUnit = new CodeCompileUnit();

            // Global namespace imports
            var codeGlobalNamespace = new CodeNamespace();
            var codeSystemImport = new CodeNamespaceImport("System");
            codeGlobalNamespace.Imports.Add(codeSystemImport);
            compileUnit.Namespaces.Add(codeGlobalNamespace);

            // CDTest namespace
            var codeCdtestNamespace = new CodeNamespace("CDTest");

            var codeProgramClass = new CodeTypeDeclaration("Program");
            codeProgramClass.IsClass = true;

            var codeMainMethod = new CodeMemberMethod();
            codeMainMethod.Name = "Main";
            codeMainMethod.Attributes |= MemberAttributes.Public;
            codeMainMethod.Attributes |= MemberAttributes.Static;

            var codeMainParam = new CodeParameterDeclarationExpression(
                typeof(string[]), "args");

            codeMainMethod.Parameters.Add(codeMainParam);

            var codeInvokeExpression = new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression("Console"),
                "WriteLine",
                new CodePrimitiveExpression("Hello, World!"));
            var codeMainStatement = new CodeExpressionStatement(codeInvokeExpression);
            codeMainMethod.Statements.Add(codeMainStatement);

            codeProgramClass.Members.Add(codeMainMethod);

            codeCdtestNamespace.Types.Add(codeProgramClass);

            compileUnit.Namespaces.Add(codeCdtestNamespace);

            var text = GenerateCode(compileUnit);
            Console.WriteLine(text);

            var results = GenerateAssembly("cdtest.exe", compileUnit);
            Console.WriteLine(results);
            Console.ReadKey();
        }

        static string GenerateCode(CodeCompileUnit compileUnit)
        {
            var codeProvider = new CSharpCodeProvider();

            using (var memStream = new MemoryStream())
            using (var outputWriter = new StreamWriter(memStream))
            {
                codeProvider.GenerateCodeFromCompileUnit(
                    compileUnit, 
                    outputWriter, 
                    new CodeGeneratorOptions());

                outputWriter.Flush();
                var bytes = memStream.ToArray();
                return Encoding.UTF8.GetString(bytes);
            }
        }

        static string GenerateAssembly(string assemblyName, CodeCompileUnit compileUnit)
        {
            var codeProvider = new CSharpCodeProvider();

            var results = codeProvider.CompileAssemblyFromDom(
                new CompilerParameters
                {
                    GenerateExecutable = true,
                    OutputAssembly = assemblyName
                }, 
                compileUnit);

            var errors = results.Errors.Cast<CompilerError>();

            var outputString = "";
            if (results.Errors.HasErrors)
            {
                outputString += "=== Errors ===";
                outputString += errors
                    .Where(error => !error.IsWarning)
                    .Select(e => e.ErrorText + "\n");
            }

            if (results.Errors.HasErrors)
            {
                outputString += "=== Warnings ===";
                outputString += errors
                    .Where(error => error.IsWarning)
                    .Select(e => e.ErrorText + "\n");
            }

            outputString += string.Join("\n", results.Output.Cast<string>());

            return outputString;
        }
    }
}
