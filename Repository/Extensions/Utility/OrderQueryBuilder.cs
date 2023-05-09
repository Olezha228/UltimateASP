using System.Reflection;
using System.Text;

namespace Repository.Extensions.Utility;

public static class OrderQueryBuilder
{
    public static string CreateOrderQuery<T>(string orderByQueryString)
    {
        var orderParams = GetParamsArray(orderByQueryString);

        var propertyInfos = GetPropertiesOfType<T>();

        var orderQueryBuilder = new StringBuilder();

        BuildOrderQuery<T>(orderParams, propertyInfos, orderQueryBuilder);

        var orderQuery = TrimOrderQueryBuilderEnd(orderQueryBuilder);

        return orderQuery;
    }

    private static void BuildOrderQuery<T>(string[] orderParams, PropertyInfo[] propertyInfos,
        StringBuilder orderQueryBuilder)
    {
        foreach (var param in orderParams)
        {
            if (string.IsNullOrWhiteSpace(param))
            {
                continue;
            }

            var propertyFromQueryName = GetPropertyFromQueryName(param);

            var objectProperty = GetObjectProperty(propertyInfos, propertyFromQueryName);

            if (objectProperty == null)
            {
                continue;
            }

            var direction = DetermineDirection(param);

            orderQueryBuilder.Append($"{objectProperty.Name} {direction}, ");
        }
    }

    private static string TrimOrderQueryBuilderEnd(StringBuilder orderQueryBuilder)
    {
        return orderQueryBuilder.ToString().TrimEnd(',', ' ');
    }

    private static string DetermineDirection(string param)
    {
        return param.EndsWith(" desc") ? "descending" : "ascending";
    }

    private static PropertyInfo? GetObjectProperty(PropertyInfo[] propertyInfos, string propertyFromQueryName)
    {
        return propertyInfos.FirstOrDefault(pi =>
            pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));
    }

    private static string GetPropertyFromQueryName(string param)
    {
        return param.Split(" ")[0];
    }

    private static PropertyInfo[] GetPropertiesOfType<T>()
    {
        return typeof(T).GetProperties(BindingFlags.Public |
                                       BindingFlags.Instance);
    }

    private static string[] GetParamsArray(string orderByQueryString)
    {
        return orderByQueryString.Trim().Split(',');
    }
}
