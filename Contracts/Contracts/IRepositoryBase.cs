using System.Linq.Expressions;
// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable UnusedMember.Global

namespace Contracts.Contracts;

public interface IRepositoryBase<T>
{
    IQueryable<T> FindAll(bool trackChanges);

    IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression,
        bool trackChanges);

    void Create(T entity);

    void Update(T entity);

    void Delete(T entity);
}