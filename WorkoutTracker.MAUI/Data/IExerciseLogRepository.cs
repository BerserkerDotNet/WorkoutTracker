using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkoutTracker.Models;

namespace WorkoutTracker.MAUI.Data
{
    public interface IExerciseLogRepository : IRepository
    {
        Task<IEnumerable<ExerciseLogEntry>> GetByDate(DateTime date);

        Task<IEnumerable<string>> GetDates();
    }
}