using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace EDUCACOOPERN.Helpers;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum val) =>
        val.GetType().GetMember(val.ToString()).FirstOrDefault()?.GetCustomAttribute<DisplayAttribute>(false)?.Name ?? val.ToString();

    public static List<int> GetValues<T>()
    {
        List<int> values = [];

        foreach (var itemType in Enum.GetValues(typeof(T)))
        {
            values.Add((int)itemType);
        }

        return values;
    }
}
