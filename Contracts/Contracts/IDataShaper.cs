using System.Dynamic;

namespace Contracts.Contracts;

public interface IDataShaper<in T>
{
    IEnumerable<ExpandoObject> ShapeData(IEnumerable<T> entities, string?
        fieldsString);

    // ReSharper disable once UnusedMember.Global
    ExpandoObject ShapeData(T entity, string? fieldsString);
}
