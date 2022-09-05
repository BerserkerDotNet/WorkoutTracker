using Microsoft.AspNetCore.Components;
using UnitsNet;
using WorkoutTracker.Models;

namespace WorkoutTracker.Components.Workout
{
    public partial class ExercisePanel
    {
        private TimeSpan _currentRestTime;
        private ExerciseViewModel _exercise;
        private SetStatus _exerciseStatus;
        private IEnumerable<Set> _completedSets;
        private PreviousLogRecordStats _previousSessionData;

        [Inject]
        public IDialogService DialogService { get; set; }

        [Inject]
        public IExerciseTimerService ExerciseTimer { get; set; }

        [Parameter]
        [EditorRequired]
        public ScheduleViewModel Schedule { get; set; }

        [Parameter]
        [EditorRequired]
        public ExerciseControlContext ControlContext { get; set; }

        [Parameter]
        public Dictionary<Guid, LogEntryViewModel> TodayLogByExercise { get; set; }

        [Parameter]
        public Dictionary<Guid, PreviousLogRecordStats> PreviousSessionLog { get; set; }

        protected override void OnParametersSet()
        {
            _exercise = Schedule.CurrentExercise;
            _previousSessionData = PreviousSessionLog.ContainsKey(_exercise.Id) ? PreviousSessionLog[_exercise.Id] : null;
            _completedSets = TodayLogByExercise.ContainsKey(_exercise.Id) ? TodayLogByExercise[_exercise.Id].Sets : Enumerable.Empty<Set>();
            _exerciseStatus = _completedSets.Count() == Schedule.TargetSets ? SetStatus.Completed : (_completedSets.Count() > 0 ? SetStatus.InProgress : SetStatus.NotStarted);
        }

        private async Task OnRemoveExercise()
        {
            await ControlContext.RemoveExercise(Schedule.Id);
        }

        private async Task OnStartSet(ExerciseViewModel exercise, WorkoutSet workoutSet)
        {
            _currentRestTime = ExerciseTimer.CurrentTime;
            ExerciseTimer.SetMode(ExerciseTimerMode.Exercising);
            await Task.Yield();
        }

        private async Task OnFinishSet(ExerciseViewModel exercise, WorkoutSet workoutSet)
        {
            var duration = ExerciseTimer.CurrentTime;
            ExerciseTimer.SetMode(ExerciseTimerMode.Resting);
            // TODO: Reps are not passed to a dialog
            // TODO: Show dialog when running set
            var model = TodayLogByExercise.ContainsKey(exercise.Id) ? TodayLogByExercise[exercise.Id] : LogEntryViewModel.New(exercise);
            var dialog = ShowSetCompletionDialog(workoutSet, 0, (int)duration.TotalSeconds, (int)_currentRestTime.TotalSeconds);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                var set = result.Data as Set;
                var newSets = model.Sets.Union(new[] { set });
                var newLog = model with { Sets = newSets };
                await ControlContext.SaveExercise(newLog);
            }
        }

        private async Task OnEditSet(ExerciseViewModel exercise, WorkoutSet workoutSet)
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
            }
        }

        private void OnAddSet()
        {
            ControlContext.SetScheduleTargetSets(Schedule.Id, Schedule.TargetSets + 1);
        }

        private void OnRemoveSet()
        {
            ControlContext.SetScheduleTargetSets(Schedule.Id, Schedule.TargetSets - 1);
        }

        private IDialogReference ShowSetCompletionDialog(WorkoutSet set, int reps, int duration, int restTime)
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