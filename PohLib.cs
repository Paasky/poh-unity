using System.Runtime.CompilerServices;

namespace PohLibrary;

public static class PohLib
{
    public static T? TryTo<T>(Func<T> fn, Dictionary<string, List<Exception>> errors, [CallerMemberName] string context = "Default")
    {
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
    
    // For void functions
    public static void TryTo(
        Action fn,
        Dictionary<string, List<Exception>> errors,
        [CallerMemberName] string context = "Default")
    {
        TryTo<object?>(() =>
        {
            fn();
            return null;
        }, errors, context);
    }
}