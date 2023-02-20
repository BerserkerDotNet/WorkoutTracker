using Microsoft.AspNetCore.Components;
using Plotly.Blazor;
using Plotly.Blazor.Traces;
using Plotly.Blazor.Traces.ScatterLib;

namespace WorkoutTracker.Components;

public partial class ExercisesChart : ComponentBase
{
    private PlotlyChart _chart;
    private Config _config = new Config() { DisplayModeBar = Plotly.Blazor.ConfigLib.DisplayModeBarEnum.False };
    private Layout _layout = new Layout()
    {
        Title = new Plotly.Blazor.LayoutLib.Title { Text = "Compound Exercises" },
        Height = 500
    };

    private IList<ITrace> _data = null;

    [Parameter]
    [EditorRequired]
    public IEnumerable<WorkoutSummary> Summaries { get; set; }

    [Parameter]
    [EditorRequired]
    public IEnumerable<ExerciseIndicatorDescriptor> Exercises { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        if (_chart is null || (_data is object && _data.Any()))
        {
            return;
        }

        await AddCharts();
    }

    private async Task AddCharts()
    {
        var summariesLookup = Summaries
            .GroupBy(s => s.ExerciseId)
            .ToDictionary(k => k.Key, v => v.ToArray().OrderBy(d => d.Date));

        foreach (var exercise in Exercises)
        {
            if (!summariesLookup.ContainsKey(exercise.Id))
            {
                continue;
            }

            var summaries = summariesLookup[exercise.Id];
            var count = summaries.Count();

            var dates = new List<object>(count);
            var points = new List<object>(count);
            var labels = new List<string>(count);
            var sizes = new List<decimal?>(count);
            var errorPlus = new List<object>(count);
            var errorMinus = new List<object>(count);

            foreach (var summary in summaries)
            {
                var maxValue = exercise.ValueSelector(summary.Max);
                var avgValue = exercise.ValueSelector(summary.Avg);
                var minValue = exercise.ValueSelector(summary.Min);
                dates.Add(summary.Date.ToString("O"));
                points.Add(avgValue);
                labels.Add($"Max: {summary.Max.WeightLb}lb / {summary.Max.Repetitions} reps; Avg: {summary.Avg.WeightLb}lb / {summary.Avg.Repetitions} reps; Min: {summary.Min.WeightLb}lb / {summary.Min.Repetitions} reps;");
                sizes.Add(summary.Avg.Repetitions * 10);
                errorPlus.Add(maxValue - avgValue);
                errorMinus.Add(avgValue - minValue);
            }

            await _chart.AddTrace(new Scatter
            {
                Name = exercise.Name,
                Mode = ModeFlag.Markers | ModeFlag.Lines,
                X = dates,
                Y = points,
                TextArray = labels,
                Marker = new Marker
                {
                    SizeArray = sizes,
                    SizeMode = Plotly.Blazor.Traces.ScatterLib.MarkerLib.SizeModeEnum.Area
                },
                ErrorY = new ErrorY
                {
                    Array = errorPlus,
                    ArrayMinus = errorMinus
                }
            });
        }
    }
}
