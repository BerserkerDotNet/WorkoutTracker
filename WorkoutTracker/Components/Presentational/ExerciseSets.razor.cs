using Microsoft.AspNetCore.Components;
using WorkoutTracker.Models;

namespace WorkoutTracker.Components.Presentational;

public partial class ExerciseSets
{
    [Parameter]
    public ExerciseSetsProps Props { get; set; }

    [Inject]
    public IDialogService DialogService { get; set; }

    private async Task SaveLog()
    {
        await Props.Save(Props.Log);
    }

    private async Task OnDeleteSet(Set set)
    {
        var result = await DialogService.ShowMessageBox("Delete set", $"Are you sure you want to delete set?", "Yes", "No");
        if (result.HasValue && result.Value)
        {
            Props.Log.Sets = Props.Log.Sets.Where(s => s != set).ToArray();
            await SaveLog();
        }
    }
}