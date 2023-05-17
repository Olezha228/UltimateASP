using System.Dynamic;

namespace Contracts.Contracts;

public interface IDataShaper<in T>
{
    IEnumerable<ExpandoObject> ShapeData(IEnumerable<T> entities, string?
        fieldsString);

    ExpandoObject ShapeData(T entity, string? fieldsString);
}
