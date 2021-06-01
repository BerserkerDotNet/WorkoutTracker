using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkoutTracker.Models;

namespace WorkoutTracker.MAUI.Data
{
    public interface IRepository
    {
        Task Create<T>(T entity)
            where T : EntityBase;

        Task Update<T>(T entity)
            where T : EntityBase;

        Task<IEnumerable<T>> GetAll<T>()
            where T : EntityBase;

        Task<T> GetById<T>(Guid id)
            where T : EntityBase;

        Task Delete<T>(Guid id)
            where T : EntityBase;
    }
}