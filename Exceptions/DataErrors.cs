namespace PohLibrary.Exceptions;

public class DataErrors : Exception
{
    public Dictionary<string, Dictionary<string, List<Exception>>> Errors { get; } = new();

    public Dictionary<string, List<Exception>> InitGroup(string groupName)
    {
        if (!Errors.TryGetValue(groupName, out var fileErrors))
        {
            fileErrors = new Dictionary<string, List<Exception>>();
            Errors.Add(groupName, fileErrors);
        }
        
        return fileErrors;
    }

    public void AddError(string groupName, string itemName, Exception exception)
    {
        if (!Errors.TryGetValue(groupName, out var groupErrors))
        {
            groupErrors = new Dictionary<string, List<Exception>>();
            Errors.Add(groupName, groupErrors);
        }

        if (!groupErrors.ContainsKey(itemName))
        {
            groupErrors.Add(itemName, []);
        }

        groupErrors[itemName].Add(exception);
    }

    public Dictionary<string, Dictionary<string, List<string>>> Messages()
    {
        var messages = new Dictionary<string, Dictionary<string, List<string>>>();
        
        foreach (var groupKeyVal in Errors)
        {
            if (!messages.TryGetValue(groupKeyVal.Key, out var groupMessages))
            {
                groupMessages = new Dictionary<string, List<string>>();
                messages.Add(groupKeyVal.Key, groupMessages);
            }
            
            foreach (var itemKeyVal in groupKeyVal.Value)
            {
                if (!groupMessages.TryGetValue(itemKeyVal.Key, out var itemMessages))
                {
                    itemMessages = [];
                    groupMessages.Add(itemKeyVal.Key, itemMessages);
                }
                
                foreach (var exception in itemKeyVal.Value)
                {
                    itemMessages.Add(exception.Message);
                }
            }
        }
        
        return messages;
    }
}