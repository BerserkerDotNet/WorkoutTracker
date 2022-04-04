using Microsoft.AspNetCore.Components;

namespace WorkoutTracker.Data.Props;

public class MusclesListProps
{
    public IEnumerable<MuscleViewModel> Muscles { get; set; }

    public Action<Guid> Edit { get; set; }
}
