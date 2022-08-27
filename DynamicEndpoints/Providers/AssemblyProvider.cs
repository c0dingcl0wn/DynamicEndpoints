using System.Reflection;
using DynamicEndpoints.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace DynamicEndpoints.Providers;

public static class AssemblyProvider
{
    public static Assembly CreateOrGetAssembly(GenerationOptions generationOptions, out List<Diagnostic> diagnostic)
    {
        diagnostic = new List<Diagnostic>();
        var assemblyName = $"{generationOptions.ControllerName}.dll";
        var outputDll = Path.Join("GeneratedAssemblies", assemblyName);

        if (File.Exists(outputDll))
        {
            return Assembly.LoadFrom(outputDll);
        }

        var code = GenerateCode(generationOptions);
        if (string.IsNullOrEmpty(code))
        {
            return null;
        }

        // get assembly references
        var references = new List<PortableExecutableReference>
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
        };
        // load all referenced assemblies from current context
        Assembly.GetEntryAssembly()?.GetReferencedAssemblies()
            .ToList()
            .ForEach(a =>
            {
                var reference = MetadataReference.CreateFromFile(Assembly.Load(a).Location);
                references.Add(reference);
            });
        
        // parse code from edited template
        var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest);
        
        // IMPORTANT: Do not insert a path into Create(). It will fail and complain  with CS8203 about illegal characters in assembly name.
        var compilation = CSharpCompilation.Create(assemblyName,
            new[]
            {
                CSharpSyntaxTree.ParseText(code, parseOptions)
            }, 
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        // compile
        var emitResult = compilation.Emit(outputDll);
        diagnostic = emitResult.Diagnostics.ToList();

        if (emitResult.Success) return Assembly.LoadFrom(outputDll);
        
        // if compilation fails, delete the generated dll. 
        // To avoid creating it before knowing if it worked, one could emit it to a memory stream instead of a file.
        File.Delete(outputDll);
        return null;
    }

    private static string GenerateCode(GenerationOptions generationOptions)
    {
        string content;
        // read template
        using (var reader = new StreamReader(Path.Join("Templates", "default.txt")))
        {
            content = reader.ReadToEnd();
        }

        // replace placeholders in template
        var code =  content.Replace("{ControllerName}", generationOptions.ControllerName)
                                .Replace("{ControllerRoute}", generationOptions.ControllerRoute)
                                .Replace("{Code}", generationOptions.Code);
        return code;
    }
}