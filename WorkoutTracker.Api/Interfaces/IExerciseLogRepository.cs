using System.Collections.Generic;
using System.Threading.Tasks;
using WorkoutTracker.Models;

namespace WorkoutTracker.Api.Interfaces
{
    public interface IExerciseLogRepository : IRepository<ExerciseLogEntry>
    {
        ValueTask<IEnumerable<ExerciseLogEntry>> GetByDate(string date);

        ValueTask<IEnumerable<string>> GetAllDates();
    }
}
