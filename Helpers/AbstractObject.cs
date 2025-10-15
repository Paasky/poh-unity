using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using PohLibrary.GenericObjects;

namespace PohLibrary.Helpers;

public abstract partial class AbstractObject
{
    public string Id { get; }
    public string Class { get; }
    public string Key { get; }
    public string? Name { get; set; }
    public string? Description { get; set; }

    public AbstractObject(string? id = null, string? name = null, string? description = null)
    {
        Class = FormatClass();
        Id = string.IsNullOrWhiteSpace(id) ? Guid.NewGuid().ToString() : id;
        Key = KeyFromClassAndId(Class, Id);
        Name = name;
        Description = description;
    }

    public ObjectRef Ref()
    {
        return ObjectRef.Get(this);
    }
    
    public bool Is(ObjectRef obj)
    {
        return Key == obj.Key;
    }

    public bool Is(AbstractObject obj)
    {
        return Key == obj.Key;
    }
    
    public static string FormatClass(string? className = null)
    {
        if (string.IsNullOrWhiteSpace(className)) {
            className = System.Reflection.MethodBase.GetCurrentMethod()!.DeclaringType!.Name;            
        }
        
        var baseName = className.Contains('.')
            ? className[(className.LastIndexOf('.') + 1)..]
            : className;
        
        return char.ToUpper(baseName[0]) + baseName[1..];
    }
    
    public static string FormatId(string input)
    {
        // Normalize (remove diacritics like ä → a)
        string normalized = input.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (char c in normalized)
        {
            var cat = CharUnicodeInfo.GetUnicodeCategory(c);
            if (cat != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }
        string ascii = sb.ToString().Normalize(NormalizationForm.FormC);

        // Replace any non-letter/digit with space
        ascii = IrregularCharsRegex().Replace(ascii, " ").Trim();

        if (ascii.Length == 0) return string.Empty;

        // Split and apply camelCase
        var words = ascii.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (words.Length == 0) return string.Empty;

        var result = new StringBuilder(words[0].ToLowerInvariant());
        for (int i = 1; i < words.Length; i++)
        {
            string w = words[i];
            if (w.Length == 0) continue;
            result.Append(char.ToUpperInvariant(w[0]));
            if (w.Length > 1)
                result.Append(w[1..].ToLowerInvariant());
        }

        return result.ToString();
    }

    public static string KeyFromClassAndId(string className, string id)
    {
        return $"{className}:{id}";
    }

    [GeneratedRegex(@"[^A-Za-z0-9]+")]
    private static partial Regex IrregularCharsRegex();
}