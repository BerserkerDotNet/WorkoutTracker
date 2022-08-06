using Microsoft.AspNetCore.Components;
using System.Diagnostics;
using UnitsNet;
using UnitsNet.Units;
using WorkoutTracker.Models;

namespace WorkoutTracker.Components.Presentational;

public partial class TrackExerciseForm : IDisposable
{
    private Stopwatch _timeTracker;
    private bool _isRunning = false;
    private bool _isDetailsShown = false;
    private CancellationTokenSource _source;

    private int _currentRestTime = 0;
    private Mass _weight = new Mass(0, MassUnit.Pound);

    [Parameter]
    public TrackExerciseFormProps Props { get; set; }

    [Inject]
    public IDialogService DialogService { get; set; }

    [Inject]
    public ISnackbar Snackbar { get; set; }

    private bool DisablePrevious => Props.PreviousExerciseId is null || _isRunning;

    private bool DisableSwap => Props.NextExerciseId is null || _isRunning;

    private bool DisableNext => Props.NextExerciseId is null || _isRunning;

    private bool DisableReplace => _isRunning;

    public void Dispose()
    {
        _source.Cancel();
        _source = null;
    }

    protected override void OnInitialized()
    {
        _timeTracker = Stopwatch.StartNew();
        _source = new CancellationTokenSource();
        Task.Factory.StartNew(async () => await StartCounting(_source.Token));
    }

    private void ToggleExerciseDetails()
    {
        _isDetailsShown = !_isDetailsShown;
    }

    private async Task SaveAndNew(Set set)
    {
        try
        {
            var newSets = Props.Log.Sets.Union(new[] { set });
            var newLog = Props.Log with { Sets = newSets };

            await SaveLog(newLog);
        }
        catch
        {
            await RetryCompleteSet(set);
        }
    }

    private async Task SaveLog(LogEntryViewModel model)
    {
        await Props.Save(model);
    }

    private async Task SaveCurrentLog()
    {
        await Props.Save(Props.Log);
    }

    private async Task OnDeleteSet(Set set)
    {
        var result = await DialogService.ShowMessageBox("Delete set", $"Are you sure you want to delete set?", "Yes", "No");
        if (result.HasValue && result.Value)
        {
            var newSets = Props.Log.Sets.Where(s => s != set).ToArray();
            var newLog = Props.Log with { Sets = newSets };
            await SaveLog(newLog);
        }
    }

    private void StartSet()
    {
        _currentRestTime = (int)_timeTracker.Elapsed.TotalSeconds;
        _timeTracker.Restart();
        _isRunning = true;
    }

    private async Task EndSet()
    {
        _isRunning = false;
        var restTime = _currentRestTime;
        var exerciseDuration = (int)_timeTracker.Elapsed.TotalSeconds;
        _currentRestTime = 0;
        _timeTracker.Restart();
        await CompleteSet(exerciseDuration, restTime);
    }

    private async Task CompleteSet(int exerciseDuration, int restTime)
    {
        var saveDialog = ShowSetCompletionDialog(exerciseDuration, restTime);
        var result = await saveDialog.Result;
        if (result.Cancelled)
        {
            return;
        }

        await SaveAndNew(result.Data as Set);
    }

    private async Task RetryCompleteSet(Set set)
    {
        var saveDialog = ShowSetCompletionDialog(set);
        var result = await saveDialog.Result;
        if (result.Cancelled)
        {
            return;
        }

        await SaveAndNew(result.Data as Set);
    }

    private async Task StartCounting(CancellationToken token)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
        while (await timer.WaitForNextTickAsync())
        {
            if (token.IsCancellationRequested)
            {
                break;
            }

            await InvokeAsync(() => StateHasChanged());
        }
    }

    private IDialogReference ShowSetCompletionDialog(int duration, int restTime)
    {
        var parameters = new DialogParameters
            {
                { nameof(SetCompletionForm.CurrentDuration), duration},
                { nameof(SetCompletionForm.CurrentRestTime), restTime},
                { nameof(SetCompletionForm.CurrentWeight), _weight},
            };
        return DialogService.Show<SetCompletionForm>($"Details of set {Props.SetNumber}", parameters);
    }

    private IDialogReference ShowSetCompletionDialog(Set set)
    {
        var parameters = new DialogParameters
            {
                { nameof(SetCompletionForm.CurrentDuration), (int)set.Duration.TotalSeconds},
                { nameof(SetCompletionForm.CurrentRestTime), (int)set.RestTime.TotalSeconds},
                { nameof(SetCompletionForm.CurrentWeight), _weight},
                { nameof(SetCompletionForm.Repetitions), set.Repetitions},
                { nameof(SetCompletionForm.Notes), set.Note},
            };
        return DialogService.Show<SetCompletionForm>($"Details of set {Props.SetNumber}", parameters);
    }
}