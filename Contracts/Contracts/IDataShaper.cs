using Entities.Models;

namespace Contracts.Contracts;

public interface IDataShaper<in T>
{
    IEnumerable<ShapedEntity> ShapeData(IEnumerable<T> entities, string? fieldsString);

    // ReSharper disable once UnusedMember.Global
    ShapedEntity ShapeData(T entity, string? fieldsString);
}
