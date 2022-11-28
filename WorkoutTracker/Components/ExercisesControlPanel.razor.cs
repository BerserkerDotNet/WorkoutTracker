using Microsoft.AspNetCore.Components;
using Plotly.Blazor;
using Plotly.Blazor.Traces;
using Domain = Plotly.Blazor.Traces.IndicatorLib.Domain;

namespace WorkoutTracker.Components;

public partial class ExercisesControlPanel : ComponentBase
{
    private PlotlyChart _indicatorChart;
    private Config _config = new Config() { DisplayModeBar = Plotly.Blazor.ConfigLib.DisplayModeBarEnum.False };
    private Layout _indicatorLayout = new Layout()
    {
        Title = new Plotly.Blazor.LayoutLib.Title { Text = "Exercises Control Panel" },
        Grid = new Plotly.Blazor.LayoutLib.Grid
        {
            Pattern = Plotly.Blazor.LayoutLib.GridLib.PatternEnum.Independent,
            Rows = 3,
            Columns = 3
        },
        Height = 700
    };

    private IList<ITrace> _indicatorData = null;

    [Parameter]
    [EditorRequired]
    public IEnumerable<WorkoutSummary> Summaries { get; set; }

    [Parameter]
    [EditorRequired]
    public IEnumerable<ExerciseIndicatorDescriptor> Exercises { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        if (_indicatorChart is null || (_indicatorChart is object && _indicatorData.Any()))
        {
            return;
        }

        await AddCharts();
    }

    private async Task AddCharts()
    {
        var summariesLookup = Summaries
            .GroupBy(s => s.ExerciseId)
            .ToDictionary(k => k.Key, v => v.ToArray());

        int column = 0;
        int row = 0;
        foreach (var exercise in Exercises)
        {
            if (!summariesLookup.ContainsKey(exercise.Id))
            {
                continue;
            }

            var summaries = summariesLookup[exercise.Id];
            var startOfThisWeek = DateTime.Today.StartOfWeek();
            var thisWeekMaxSummary = summaries
                .Where(s => s.Date >= startOfThisWeek)
                .MaxBy(s => exercise.ValueSelector(s.Max));
            var prevWeekMaxSummary = summaries
                .Where(s => s.Date > startOfThisWeek.AddDays(-7) && s.Date < startOfThisWeek)
                .MaxBy(s => exercise.ValueSelector(s.Max));

            var thisWeekMax = thisWeekMaxSummary is object ? exercise.ValueSelector(thisWeekMaxSummary.Max) : (double?)null;
            var prevWeekMax = prevWeekMaxSummary is object ? exercise.ValueSelector(prevWeekMaxSummary.Max) : (double?)null;

            if (prevWeekMax is null)
            {
                prevWeekMax = 0;
            }

            if (thisWeekMax is null)
            {
                thisWeekMax = prevWeekMax;
            }

            await _indicatorChart.AddTrace(new Indicator
            {
                Mode = Plotly.Blazor.Traces.IndicatorLib.ModeFlag.Number | Plotly.Blazor.Traces.IndicatorLib.ModeFlag.Delta | Plotly.Blazor.Traces.IndicatorLib.ModeFlag.Gauge,
                Title = new Plotly.Blazor.Traces.IndicatorLib.Title
                {
                    Text = exercise.Name
                },
                Value = (decimal)thisWeekMax,
                Domain = new Domain
                {
                    Row = row,
                    Column = column
                },
                Delta = new Plotly.Blazor.Traces.IndicatorLib.Delta
                {
                    Reference = (decimal)prevWeekMax,
                    Relative = true
                },
                Gauge = new Plotly.Blazor.Traces.IndicatorLib.Gauge
                {
                    Axis = new Plotly.Blazor.Traces.IndicatorLib.GaugeLib.Axis
                    {
                        Range = new List<object> { 0, exercise.Target }
                    }
                }
            });

            column++;
            if (column == 3)
            {
                column = 0;
                row++;
            }
        }
    }
}
