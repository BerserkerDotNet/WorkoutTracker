namespace WorkoutTracker.Models.Entities
{
    [PluralName(EndpointNames.WorkoutProgramPluralName)]
    public class WorkoutProgram : EntityBase
    {
        public required string Name { get; set; }

        public required Schedule Schedule { get; set; }
    }
}