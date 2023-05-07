using WorkoutTracker.Models.Entities;
using WorkoutTracker.Models.Selectors;

namespace WorkoutTracker.Services
{
    public sealed class WorkoutProgramProvider
    {
        private static readonly Guid BarbellRow = Guid.Parse("b1ce8082-2b1a-4956-b3de-e687e3e16902");
        private static readonly Guid LatPullDown = Guid.Parse("d6846bae-ccf8-4073-8825-e9fb28def637");
        private static readonly Guid BarbellCurl = Guid.Parse("93b66b46-74bc-484f-bc52-844d5facba69");
        private static readonly Guid Facepulls = Guid.Parse("d4355b45-ac08-4ed6-8401-6a7cf9d91491");

        public static WorkoutProgram Default => new WorkoutProgram
        {
            Id = Guid.Parse("5184f670-ad0d-4062-a936-6a16c9e14069"),
            Name = "Default",
            Schedule = new Schedule
            {
                Monday = PullWorkout(),
                Tuesday = LegsDay(),
                Wednesday = PushDay(),
                Thursday = LegsDay(),
                Friday = UpperBodyDay(),
                Saturday = WorkoutDefinition.Rest,
                Sunday = WorkoutDefinition.Rest,
            }
        };

        public static WorkoutDefinition PullWorkout()
        {
            return new WorkoutDefinition
            {
                Name = "Pull day",
                Exercises = new List<ExerciseDefinition>
                {
                    new ExerciseDefinition
                    {
                        ExerciseSelector = new SpecificExerciseSelector(BarbellRow),
                        OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: true)
                    },
                    new ExerciseDefinition
                    {
                        ExerciseSelector = new SpecificExerciseSelector(LatPullDown),
                        OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: false)
                    },
                    new ExerciseDefinition
                    {
                        ExerciseSelector = new MuscleGroupExerciseSelector("Back"),
                        OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: false)
                    },
                    new ExerciseDefinition
                    {
                        ExerciseSelector = new SpecificExerciseSelector(BarbellCurl),
                        OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: true)
                    },
                    new ExerciseDefinition
                    {
                        ExerciseSelector = new MuscleGroupExerciseSelector("Arm"),
                        OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: false),
                    },
                    new ExerciseDefinition
                    {
                        ExerciseSelector = new MuscleGroupExerciseSelector("Arm"),
                        OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: false),
                    },
                    new ExerciseDefinition
                    {
                        ExerciseSelector = new SpecificExerciseSelector(Facepulls),
                        OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: false),
                    }
                }
            };
        }

        public static WorkoutDefinition LegsDay()
        {
            return new WorkoutDefinition
            {
                Name = "Legs day",
                Exercises = new List<ExerciseDefinition>
                {
                    new ExerciseDefinition
                    {
                        ExerciseSelector = new MuscleGroupExerciseSelector("Quads"),
                        OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: true),
                    },
                    new ExerciseDefinition
                    {
                        ExerciseSelector = new MuscleGroupExerciseSelector("Glutes"),
                        OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: false),
                    },
                    new ExerciseDefinition
                    {
                        ExerciseSelector = new MuscleGroupExerciseSelector("Hamstrings"),
                        OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: false),
                    },
                    new ExerciseDefinition
                    {
                        ExerciseSelector = new MuscleGroupExerciseSelector("Quads"),
                        OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: false),
                    },
                    new ExerciseDefinition
                    {
                        ExerciseSelector = new MuscleGroupExerciseSelector("Calves"),
                        OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: false, workingReps: 15),
                    },
                    new ExerciseDefinition
                    {
                        ExerciseSelector = new SpecificExerciseSelector(Facepulls),
                        OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: false),
                    }
                }
            };
        }

        public static WorkoutDefinition PushDay()
        {
            return new WorkoutDefinition
            {
                Name = "Push day",
                Exercises = new List<ExerciseDefinition>
                {
                    new ExerciseDefinition
                    {
                        ExerciseSelector = new MuscleGroupExerciseSelector("Chest"),
                        OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: true),
                    },
                    new ExerciseDefinition
                    {
                        ExerciseSelector = new MuscleGroupExerciseSelector("Chest"),
                        OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: false),
                    },
                    new ExerciseDefinition
                    {
                        ExerciseSelector = new MuscleGroupExerciseSelector("Shoulder"),
                        OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: true),
                    },
                    new ExerciseDefinition
                    {
                        ExerciseSelector = new MuscleGroupExerciseSelector("Shoulder"),
                        OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: false),
                    },
                    new ExerciseDefinition
                    {
                        ExerciseSelector = new MuscleGroupExerciseSelector("Triceps"),
                        OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: false),
                    },
                    new ExerciseDefinition
                    {
                        ExerciseSelector = new SpecificExerciseSelector(Facepulls),
                        OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: false),
                    }
                }
            };
        }

        public static WorkoutDefinition UpperBodyDay()
        {
            return new WorkoutDefinition
            {
                Name = "Upper body day",
                Exercises = new List<ExerciseDefinition>
                {
                    new ExerciseDefinition
                    {
                        ExerciseSelector = new MuscleGroupExerciseSelector("Back"),
                        OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: true)
                    },
                    new ExerciseDefinition
                    {
                        ExerciseSelector = new MuscleGroupExerciseSelector("Back"),
                        OverloadFactor = new OneRepMaxProgressiveOverloadFactor(80, 3)
                    },
                    new ExerciseDefinition
                    {
                        ExerciseSelector = new MuscleGroupExerciseSelector("Chest"),
                        OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: true),
                    },
                    new ExerciseDefinition
                    {
                        ExerciseSelector = new MuscleGroupExerciseSelector("Shoulder"),
                        OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: false),
                    },
                    new ExerciseDefinition
                    {
                        ExerciseSelector = new MuscleGroupExerciseSelector("Arm"),
                        OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: false),
                    },
                    new ExerciseDefinition
                    {
                        ExerciseSelector = new MuscleGroupExerciseSelector("Arm"),
                        OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: false),
                    },
                    new ExerciseDefinition
                    {
                        ExerciseSelector = new MuscleGroupExerciseSelector("Triceps"),
                        OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: false),
                    },
                    new ExerciseDefinition
                    {
                        ExerciseSelector = new SpecificExerciseSelector(Facepulls),
                        OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: false),
                    }
                }
            };
        }
    }
}