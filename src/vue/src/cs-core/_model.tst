${
    using Typewriter.Extensions.Types;

    Template(Settings settings)
    {
        settings.IncludeProject("SqlCrawler.Backend");

        settings.OutputFilenameFactory = file => 
        {
            var name = file.Name.Replace(".cs", ".ts");
            return Char.ToLowerInvariant(name[0]) + name.Substring(1);
        };
    }

    string Imports(Class c)
    {
        var types = c.Properties
            .Select(p => p.Type)
            .SelectMany(t => t.IsGeneric ? t.TypeArguments.Cast<Type>() : new [] { t })
            .Where(t => !t.IsPrimitive || t.IsEnum)
            .Select(t => t.name.EndsWith("[]") ? t.name.Substring(0, t.name.IndexOf("[]")) : t.name)
            .Distinct()
            .ToList();
            
        if (c.BaseClass != null) types.Add(c.BaseClass.name);

        return string.Join(Environment.NewLine, types.Select(name => $"import {{ {name} }} from './{name}';").Distinct());
    }

    string BaseClassIfExists(Class c)
    {
        return c.BaseClass != null ? "extends " + c.BaseClass.name + " " : "";
    }
}$Classes(c => c.FullName.StartsWith("SqlCrawler.Backend.Core"))[$Imports

export class $name $BaseClassIfExists{$Properties[
    public $name: $Type | null = $Type[$Default];]
}]
