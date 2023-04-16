using System.Collections.ObjectModel;
using WorkoutTracker.Models.Contracts;
using WorkoutTracker.Models.Entities;
using WorkoutTracker.Models.Presentation;
using WorkoutTracker.Models.Selectors;

namespace WorkoutTracker.MAUI.Controls.WorkoutDefinition;

public partial class ExerciseDefinitionEditor : ContentView
{
    public static readonly BindableProperty ExercisesProperty = BindableProperty.Create(nameof(Exercises), typeof(ObservableCollection<ExerciseViewModel>), typeof(ExerciseDefinitionEditor), defaultValue: null);
    public static readonly BindableProperty MusclesProperty = BindableProperty.Create(nameof(Muscles), typeof(ObservableCollection<MuscleViewModel>), typeof(ExerciseDefinitionEditor), defaultValue: null);

    private ExerciseDefinition _exerciseDefinition;

    public ObservableCollection<ExerciseViewModel> Exercises
    {
        get { return (ObservableCollection<ExerciseViewModel>)GetValue(ExercisesProperty); }
        set { SetValue(ExercisesProperty, value); }
    }
    
    public ObservableCollection<MuscleViewModel> Muscles
    {
        get { return (ObservableCollection<MuscleViewModel>)GetValue(MusclesProperty); }
        set { SetValue(ExercisesProperty, value); }
    }

    public ExerciseDefinitionEditor()
    {
        InitializeComponent();
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        var exerciseDef = BindingContext as ExerciseDefinition;
        if (_exerciseDefinition == exerciseDef)
        {
            return;
        }

        _exerciseDefinition = exerciseDef;
        content.BatchBegin();
        content.Clear();
        content.Add(new Label()
        {
            Text = $"{exerciseDef.ExerciseSelector.DisplayText} with {exerciseDef.OverloadFactor.DisplayText} overload"
        });
        content.Add(GetExerciseSelectorControl(exerciseDef.ExerciseSelector));
        content.Add(GetOverloadControl(exerciseDef.OverloadFactor));
        content.BatchCommit();
    }

    private ContentView GetOverloadControl(IProgressiveOverloadFactor overload)
    {
        return overload switch
        {
            PowerLadderOverloadFactor pl => new PowerLadderEditor(pl),
            RepetitionsLadderOverloadFactor rl => new RepetitionsLadderEditor(rl),
            OneRepMaxProgressiveOverloadFactor oneRM => new OneRepMaxEditor(oneRM),
            SteadyStateProgressiveOverloadFactor steadyState => new SteadyStateEditor(steadyState),
            _ => null
        };
    }

    private ContentView GetExerciseSelectorControl(IExerciseSelector selector)
    {
        return selector switch
        {
            SpecificExerciseSelector se => new SpecificExerciseSelectorEditor(se, Exercises),
            MuscleGroupExerciseSelector mg => new MuscleGroupExerciseSelectorEditor(mg, Muscles),
            MuscleExerciseSelector me => new SpecificMuscleExerciseSelectorEditor(me, Muscles),
            _ => null
        };
    }
}