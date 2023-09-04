using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkoutTracker.Services.Models;

namespace WorkoutTracker.MAUI.Controls.Charts;

public partial class DonutChart : ContentView
{
    public static readonly BindableProperty DataProperty = BindableProperty.Create(nameof(Data), typeof(IEnumerable<DataSeriesItem>), typeof(DonutChart), defaultValue: null);
    public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(DonutChart), defaultValue: null);

    public IEnumerable<DataSeriesItem> Data
    {
        get { return (IEnumerable<DataSeriesItem>)GetValue(DataProperty); }
        set { SetValue(DataProperty, value); }
    }
    
    public string Title
    {
        get { return (string)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public DonutChart()
    {
        InitializeComponent();
    }
}