using System.Linq.Expressions;
using Yestino.Common.Domain;

namespace Yestino.Common.Infrastructure.Persistence;

public interface IAggregateRepository<TAggregateRoot>
    where TAggregateRoot : AggregateRoot
{
    Task<TAggregateRoot?> GetById(Guid id, CancellationToken cancellationToken);
    Task GetByIds(ICollection<Guid> ids, CancellationToken cancellationToken);
    Task<TAggregateRoot?> FirstOrDefault(Expression<Func<TAggregateRoot, bool>> filterExpression, CancellationToken cancellationToken);
    Task<ICollection<TAggregateRoot>> FilterBy(Expression<Func<TAggregateRoot, bool>> filterExpression, CancellationToken cancellationToken);
    void Add(TAggregateRoot entity);
    void AddRange(ICollection<TAggregateRoot> entities);
    void Remove(TAggregateRoot entity);
    void RemoveRange(ICollection<TAggregateRoot> entities);
}
