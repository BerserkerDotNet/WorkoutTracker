using Microsoft.AspNetCore.Components;


namespace WorkoutTracker.Components.Presentational;

public partial class WorkoutStatistics
{
    private ExerciseIndicatorDescriptor[] _exercisesToShow = new ExerciseIndicatorDescriptor[]
    {
        new (Guid.Parse("b33af80a-8608-4ef9-a792-a18c7eadb676"), "Deadlift", 450, s => s.WeightLb),
        new (Guid.Parse("6ebba228-4205-4d17-b51f-b870fd061332"), "Bench Press", 270, s => s.WeightLb),
        new (Guid.Parse("b71b20c9-0fb4-4351-aa6f-41cffc75daed"), "Squats", 350, s => s.WeightLb),
        new (Guid.Parse("f9585d34-46bd-4044-8221-ed3ce0b7a7fc"), "Military Press", 175, s => s.WeightLb),
        new (Guid.Parse("04ad6bd8-5dec-4264-b05d-5182429a1ec9"), "Pull-ups", 15, s => s.Repetitions),
        new (Guid.Parse("b1ce8082-2b1a-4956-b3de-e687e3e16902"), "Barbell Row", 230, s => s.WeightLb),
        new (Guid.Parse("93b66b46-74bc-484f-bc52-844d5facba69"), "Barbell Curl", 127, s => s.WeightLb)
    };

    [Parameter]
    public WorkoutSummaryProps Props { get; set; }
}