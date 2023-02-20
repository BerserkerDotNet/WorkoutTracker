using Microsoft.AspNetCore.Components;

namespace WorkoutTracker.Components.Workout
{
    public partial class ExercisePanel
    {
        private WorkoutExerciseViewModel _exercise;
        private SetStatus _exerciseStatus;
        private PreviousLogRecordStats _previousSessionData;

        [Parameter]
        [EditorRequired]
        public WorkoutViewModel Schedule { get; set; }

        protected override void OnAfterParametersSet()
        {
            _exercise = Schedule.Exercise;
            _previousSessionData = Props.PreviousSessionLog.ContainsKey(_exercise.Id) ? Props.PreviousSessionLog[_exercise.Id] : null;
            _exerciseStatus = _exercise.Sets.All(s => s.Status == SetStatus.Completed) ? SetStatus.Completed : _exercise.Sets.Any(s => s.Status == SetStatus.Completed) ? SetStatus.InProgress : SetStatus.NotStarted;
        }
    }
}