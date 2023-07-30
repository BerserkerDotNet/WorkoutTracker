using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;
using WorkoutTracker.Services.Models;

namespace WorkoutTracker.MAUI.Services.Data.Entities
{
    public class WorkoutStatisticsEntity : BaseDbEntity
    {
        [TextBlob(nameof(SummaryBlobbed))]
        public WorkoutsSummary Summary { get; set; }
        
        public string SummaryBlobbed { get; set; }
        
        [TextBlob(nameof(TimeMetricsBlobbed))]
        public WorkoutTimeMetrics TimeMetrics { get; set; }
        
        public string TimeMetricsBlobbed { get; set; }

        [TextBlob(nameof(PercentageByMuscleGroupBlobbed))]
        public IEnumerable<DataSeriesItem> PercentageByMuscleGroup { get; set; }
        
        public string PercentageByMuscleGroupBlobbed { get; set; }
    }
}