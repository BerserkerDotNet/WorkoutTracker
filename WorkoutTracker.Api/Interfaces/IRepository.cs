using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkoutTracker.Models;

namespace WorkoutTracker.Api.Interfaces
{
    public interface IRepository<T>
        where T: EntityBase, new()
    {
        ValueTask<IEnumerable<T>> Get();

        ValueTask<T> Get(Guid id);

        ValueTask<T> Create(T entity);

        ValueTask<T> Update(T entity);

        ValueTask Delete(Guid id);
    }
}
