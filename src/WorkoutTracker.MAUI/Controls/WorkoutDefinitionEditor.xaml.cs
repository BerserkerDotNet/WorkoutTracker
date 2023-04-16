using System.Windows.Input;

namespace WorkoutTracker.MAUI.Controls;

public partial class WorkoutDefinitionEditor : ContentView
{
    public static readonly BindableProperty SetClickedCommandProperty = BindableProperty.Create(nameof(SetClickedCommand), typeof(ICommand), typeof(ExerciseSetView), defaultValue: null);

    public ICommand SetClickedCommand
    {
        get { return (ICommand)GetValue(SetClickedCommandProperty); }
        set { SetValue(SetClickedCommandProperty, value); }
    }

    public WorkoutDefinitionEditor()
    {
        InitializeComponent();
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
    }

    private void OnTap(object sender, System.ComponentModel.HandledEventArgs e)
    {
        SetClickedCommand.Execute(BindingContext);
    }
}