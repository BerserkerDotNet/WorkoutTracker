using Microsoft.AspNetCore.Components;
using UnitsNet;
using WorkoutTracker.Models;

namespace WorkoutTracker.Components.Workout
{
    public partial class ExercisePanel
    {
        private TimeSpan _currentRestTime;
        private WorkoutExerciseViewModel _exercise;
        private SetStatus _exerciseStatus;
        private PreviousLogRecordStats _previousSessionData;
        private bool _isReplacingExercise;

        [Inject]
        public IDialogService DialogService { get; set; }

        [Inject]
        public IExerciseTimerService ExerciseTimer { get; set; }

        [Parameter]
        [EditorRequired]
        public WorkoutViewModel Schedule { get; set; }

        [Parameter]
        [EditorRequired]
        public IEnumerable<ExerciseViewModel> AllExercises { get; set; }

        [Parameter]
        [EditorRequired]
        public ExerciseControlContext ControlContext { get; set; }

        [Parameter]
        public Dictionary<Guid, LogEntryViewModel> TodayLogByExercise { get; set; }

        [Parameter]
        public Dictionary<Guid, PreviousLogRecordStats> PreviousSessionLog { get; set; }

        protected override void OnParametersSet()
        {
            _exercise = Schedule.Exercise;
            _previousSessionData = PreviousSessionLog.ContainsKey(_exercise.Id) ? PreviousSessionLog[_exercise.Id] : null;
            _exerciseStatus = _exercise.Sets.Any(s => s.Status == SetStatus.Completed) ? SetStatus.InProgress : _exercise.Sets.All(s => s.Status == SetStatus.Completed) ? SetStatus.Completed : SetStatus.NotStarted;
        }

        private async Task OnRemoveExercise()
        {
            await ControlContext.RemoveExercise(Schedule.Id);
        }

        private void OnBeginReplaceExercise()
        {
            _isReplacingExercise = true;
        }

        private void OnCompleteReplaceExercise(ExerciseViewModel model)
        {
            ControlContext.ReplaceExercise(Schedule.Id, model);
            _isReplacingExercise = false;
        }

        private void OnCancelReplaceExercise()
        {
            _isReplacingExercise = false;
        }

        private async Task OnStartSet(WorkoutExerciseViewModel exercise, WorkoutExerciseSetViewModel workoutSet)
        {
            _currentRestTime = ExerciseTimer.CurrentTime;
            ExerciseTimer.SetMode(ExerciseTimerMode.Exercising);
            ControlContext.StartSet(Schedule.Id, workoutSet.Index);
            await Task.Yield();
        }

        private async Task OnFinishSet(WorkoutExerciseViewModel exercise, WorkoutExerciseSetViewModel workoutSet)
        {
            var duration = ExerciseTimer.CurrentTime;
            ExerciseTimer.SetMode(ExerciseTimerMode.Resting);
            // TODO: Show dialog when running set
            var model = TodayLogByExercise.ContainsKey(exercise.Id) ? TodayLogByExercise[exercise.Id] : LogEntryViewModel.New(exercise);
            var dialog = ShowSetCompletionDialog(workoutSet, workoutSet.Reps, (int)duration.TotalSeconds, (int)_currentRestTime.TotalSeconds);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                var set = result.Data as Set;
                var newSets = model.Sets.Union(new[] { set });
                var newLog = model with { Sets = newSets };
                await ControlContext.SaveExercise(newLog);
                var finalSet = workoutSet with { Reps = set.Repetitions, Weight = set.WeightLB.Value, Duration = set.Duration, RestTime = set.RestTime, Status = SetStatus.Completed };
                ControlContext.UpdateSet(Schedule.Id, finalSet);
            }
        }

        private async Task OnEditSet(WorkoutExerciseViewModel exercise, WorkoutExerciseSetViewModel workoutSet)
        {
            // TODO: Find the set in the list and replace
            var model = TodayLogByExercise.ContainsKey(exercise.Id) ? TodayLogByExercise[exercise.Id] : LogEntryViewModel.New(exercise);
            var dialog = ShowSetCompletionDialog(workoutSet, workoutSet.Reps, (int)workoutSet.Duration.TotalSeconds, (int)workoutSet.RestTime.TotalSeconds);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                var set = result.Data as Set;

                var currentSets = model.Sets.ToArray();
                currentSets[workoutSet.Index] = set;

                var newLog = model with { Sets = currentSets };
                await ControlContext.SaveExercise(newLog);
                var finalSet = workoutSet with { Reps = set.Repetitions, Weight = set.WeightLB.Value, Duration = set.Duration, RestTime = set.RestTime, Status = SetStatus.Completed };
                ControlContext.UpdateSet(Schedule.Id, finalSet);
            }
        }

        private async Task OnSetUpdated(WorkoutExerciseSetViewModel workoutSet)
        {
            ControlContext.UpdateSet(Schedule.Id, workoutSet);
            await Task.Yield();
        }

        private void OnAddSet()
        {
            ControlContext.IncreaseSets(Schedule.Id);
        }

        private void OnRemoveSet()
        {
            ControlContext.DecreaseSets(Schedule.Id);
        }

        private IDialogReference ShowSetCompletionDialog(WorkoutExerciseSetViewModel set, int reps, int duration, int restTime)
        {
            var parameters = new DialogParameters
            {
                { nameof(SetCompletionForm.Repetitions), reps },
                { nameof(SetCompletionForm.CurrentDuration), duration },
                { nameof(SetCompletionForm.CurrentRestTime), restTime },
                { nameof(SetCompletionForm.CurrentWeight), Mass.FromPounds(set.Weight) }
            };
            return DialogService.Show<SetCompletionForm>($"Set details", parameters);
        }
    }
}