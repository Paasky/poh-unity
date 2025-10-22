using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace PohLibrary;

public static class PohLib
{
    public static string FormatClass(string className)
    {
        var baseName = className.Contains('.')
            ? className[(className.LastIndexOf('.') + 1)..]
            : className;
        
        return char.ToUpper(baseName[0]) + baseName[1..];
    }
    
    public static string FormatId(string input)
    {
        // Normalize (remove diacritics like ä → a)
        var normalized = input.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var c in normalized)
        {
            var cat = CharUnicodeInfo.GetUnicodeCategory(c);
            if (cat != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }
        var ascii = sb.ToString().Normalize(NormalizationForm.FormC);

        // Replace any non-letter/digit with space
        ascii = IrregularChars.Replace(ascii, " ").Trim();

        if (ascii.Length == 0) return string.Empty;

        // Split and apply camelCase
        var words = ascii.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (words.Length == 0) return string.Empty;

        var result = new StringBuilder(words[0].ToLowerInvariant());
        for (var i = 1; i < words.Length; i++)
        {
            var w = words[i];
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

    public static Dictionary<string, List<Exception>> InitTryToErrors()
    {
        return new Dictionary<string, List<Exception>>();
    }
    
    public static T? TryTo<T>(
        Func<T> fn,
        Dictionary<string, List<Exception>> errors,
        [CallerMemberName] string context = "Default"
    ) {
        try
        {
            return fn();
        }
        catch (AggregateException e)
        {
            if (!errors.ContainsKey(context)) errors[context] = [];
            foreach (var e2 in e.Flatten().InnerExceptions)
            {
                errors[context].Add(e2);
            }
        }
        catch (Exception e)
        {
            if (!errors.ContainsKey(context)) errors[context] = [];
            errors[context].Add(e);
        }
        
        return default;
    }
    
    public static void TryTo(
        Action fn,
        Dictionary<string, List<Exception>> errors,
        [CallerMemberName] string context = "Default"
    ) {
        TryTo<object?>(() =>
        {
            fn();
            return null;
        }, errors, context);
    }

    public static void TryToForeach<T>(IEnumerable<T> list, Action<T> fn, string context)
    {
        var errors = InitTryToErrors();
        var i = 0;
        foreach (var itemElem in list)
        {
            TryTo(() => fn(itemElem), errors, $"{context}[{i}]");
            i++;
        }
        ThrowIfErrors(errors);
    }

    public static void ThrowIfErrors(Dictionary<string, List<Exception>> errors)
    {
        if (errors.Count == 0) return;
        
        var eList = new List<Exception>();
        foreach (var exceptions in errors.Values)
        {
            eList.AddRange(exceptions);
        }
        if (eList.Count == 0) return;
        
        throw new AggregateException(eList);
    }
    
    private static readonly Regex IrregularChars = new Regex(@"[^A-Za-z0-9]+", RegexOptions.Compiled);
}