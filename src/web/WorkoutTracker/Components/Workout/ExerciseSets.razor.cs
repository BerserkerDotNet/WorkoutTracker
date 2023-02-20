using Microsoft.AspNetCore.Components;
using UnitsNet;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.Components.Workout;

public partial class ExerciseSets
{
    private TimeSpan _currentRestTime;
    private IEnumerable<WorkoutExerciseSetViewModel> _sets;
    private WorkoutExerciseViewModel _exercise;

    [Inject]
    public IDialogService DialogService { get; set; }

    [Inject]
    public IExerciseTimerService ExerciseTimer { get; set; }

    [Parameter]
    [EditorRequired]
    public WorkoutViewModel Schedule { get; set; }

    protected override void OnAfterParametersSet()
    {
        _sets = Schedule.Exercise.Sets;
        _exercise = Schedule.Exercise;
    }

    private void OnWeightChanged(double newWeight, WorkoutExerciseSetViewModel set)
    {
        var newSet = set with { Weight = newWeight };
        Props.UpdateSet(Schedule.Id, newSet);
    }

    private void OnRepsChanged(int reps, WorkoutExerciseSetViewModel set)
    {
        var newSet = set with { Reps = reps };
        Props.UpdateSet(Schedule.Id, newSet);
    }

    private void OnStartSet(WorkoutExerciseSetViewModel workoutSet)
    {
        _currentRestTime = ExerciseTimer.CurrentTime;
        ExerciseTimer.SetMode(ExerciseTimerMode.Exercising);
        Props.StartSet(Schedule.Id, workoutSet.Index);
    }

    private async Task OnFinishSet(WorkoutExerciseSetViewModel workoutSet)
    {
        var duration = ExerciseTimer.CurrentTime;
        ExerciseTimer.SetMode(ExerciseTimerMode.Resting);
        // TODO: Show dialog when running set
        var model = Props.TodayLogByExercise.ContainsKey(_exercise.Id) ? Props.TodayLogByExercise[_exercise.Id] : LogEntryViewModel.New(_exercise);
        var dialog = ShowSetCompletionDialog(workoutSet, workoutSet.Reps, (int)duration.TotalSeconds, (int)_currentRestTime.TotalSeconds);
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            var set = result.Data as Set;
            var newSets = model.Sets.Union(new[] { set });
            model.Sets = newSets;
            await Props.Save(model);
            var finalSet = workoutSet with { Reps = set.Repetitions, Weight = set.WeightLB.Value, Duration = set.Duration, RestTime = set.RestTime, Status = SetStatus.Completed };
            Props.UpdateSet(Schedule.Id, finalSet);
        }
    }

    private async Task OnEditSet(WorkoutExerciseSetViewModel workoutSet)
    {
        // TODO: Find the set in the list and replace
        var model = Props.TodayLogByExercise.ContainsKey(_exercise.Id) ? Props.TodayLogByExercise[_exercise.Id] : LogEntryViewModel.New(_exercise);
        var dialog = ShowSetCompletionDialog(workoutSet, workoutSet.Reps, (int)workoutSet.Duration.TotalSeconds, (int)workoutSet.RestTime.TotalSeconds);
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            var set = result.Data as Set;
            var currentSets = model.Sets.ToArray();
            currentSets[workoutSet.Index] = set;
            model.Sets = currentSets;
            await Props.Save(model);
            var finalSet = workoutSet with { Reps = set.Repetitions, Weight = set.WeightLB.Value, Duration = set.Duration, RestTime = set.RestTime, Status = SetStatus.Completed };
            Props.UpdateSet(Schedule.Id, finalSet);
        }
    }

    private IDialogReference ShowSetCompletionDialog(WorkoutExerciseSetViewModel set, int reps, int duration, int restTime)
    {
        var parameters = new DialogParameters { { nameof(SetCompletionForm.Repetitions), reps }, { nameof(SetCompletionForm.CurrentDuration), duration }, { nameof(SetCompletionForm.CurrentRestTime), restTime }, { nameof(SetCompletionForm.CurrentWeight), Mass.FromPounds(set.Weight) } };
        return DialogService.Show<SetCompletionForm>($"Set details", parameters);
    }
}