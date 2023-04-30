using FluentAssertions;
using NSubstitute;
using WorkoutTracker.Models.Entities;
using WorkoutTracker.Models.Selectors;
using WorkoutTracker.Services.Interfaces;
using WorkoutTracker.Services.ViewModels;

namespace WorkoutTracker.UnitTests.ViewModels;

[TestFixture]
public class EditWorkoutDefinitionViewModelTests
{
    private EditWorkoutDefinitionViewModel _viewModel;
    
    [SetUp]
    public void Init()
    {
        var dataProvider = Substitute.For<IWorkoutDataProvider>();
        _viewModel = new EditWorkoutDefinitionViewModel(dataProvider);
        _viewModel.ApplyQueryAttributes(new Dictionary<string, object>()
            {
                { nameof(WorkoutDefinition), new WorkoutDefinition(){ Name = "Test", Exercises = new List<ExerciseDefinition>()} }
            }
        );
    }

    [Test]
    public void AddsExerciseDefinition()
    {
        _viewModel.ShowNewExerciseMenu(null);
        _viewModel.AddExercise(new NewExerciseOptions(ExerciseSelectorType.SpecificExercise,
            ProgressiveOverloadType.PowerLadder));

        _viewModel.WorkoutDefinition.Exercises.Count.Should().Be(1);
        _viewModel.IsNewExerciseMenuVisible.Should().BeFalse();
    }
    
    [Test]
    public void AddExerciseDefinitionToTheEndOfTheList()
    {
        _viewModel.WorkoutDefinition.Exercises.Add(new ExerciseDefinition
        {
            OverloadFactor = new PowerLadderOverloadFactor(5, false),
            ExerciseSelector = new MuscleGroupExerciseSelector("Test")
        });
        
        _viewModel.ShowNewExerciseMenu(null);
        _viewModel.AddExercise(new NewExerciseOptions(ExerciseSelectorType.SpecificExercise,
            ProgressiveOverloadType.PowerLadder));

        _viewModel.WorkoutDefinition.Exercises.Count.Should().Be(2);
        _viewModel.WorkoutDefinition.Exercises.Last().ExerciseSelector.Should().BeOfType<SpecificExerciseSelector>();
        _viewModel.WorkoutDefinition.Exercises.Last().OverloadFactor.Should().BeOfType<PowerLadderOverloadFactor>();
    }

    [Test]
    public void ReplacesSelectedExerciseDefinition()
    {
        _viewModel.WorkoutDefinition.Exercises.Add(new ExerciseDefinition
        {
            OverloadFactor = new PowerLadderOverloadFactor(5, false),
            ExerciseSelector = new MuscleGroupExerciseSelector("Test")
        });
        
        _viewModel.ShowNewExerciseMenu(_viewModel.WorkoutDefinition.Exercises.First());
        _viewModel.AddExercise(new NewExerciseOptions(ExerciseSelectorType.SpecificExercise,
            ProgressiveOverloadType.PowerLadder));

        _viewModel.WorkoutDefinition.Exercises.Count.Should().Be(1);
        _viewModel.WorkoutDefinition.Exercises.First().ExerciseSelector.Should().BeOfType<SpecificExerciseSelector>();
        _viewModel.WorkoutDefinition.Exercises.First().OverloadFactor.Should().BeOfType<PowerLadderOverloadFactor>();
        _viewModel.IsNewExerciseMenuVisible.Should().BeFalse();
    }
    
    [Test]
    public void ReplacesSelectedExerciseDefinitionOnlyOnce()
    {
        _viewModel.WorkoutDefinition.Exercises.Add(new ExerciseDefinition
        {
            OverloadFactor = new PowerLadderOverloadFactor(5, false),
            ExerciseSelector = new MuscleGroupExerciseSelector("Test")
        });
        
        _viewModel.ShowNewExerciseMenu(_viewModel.WorkoutDefinition.Exercises.First());
        _viewModel.AddExercise(new NewExerciseOptions(ExerciseSelectorType.SpecificExercise,
            ProgressiveOverloadType.PowerLadder));
        _viewModel.AddExercise(new NewExerciseOptions(ExerciseSelectorType.MuscleGroup,
            ProgressiveOverloadType.OneRepMaxPercentage));

        _viewModel.WorkoutDefinition.Exercises.Count.Should().Be(2);
        var replacedDefinition = _viewModel.WorkoutDefinition.Exercises.First();
        replacedDefinition.ExerciseSelector.Should().BeOfType<SpecificExerciseSelector>();
        replacedDefinition.OverloadFactor.Should().BeOfType<PowerLadderOverloadFactor>();
        
        var addedDefinition = _viewModel.WorkoutDefinition.Exercises.Last();
        addedDefinition.ExerciseSelector.Should().BeOfType<MuscleGroupExerciseSelector>();
        addedDefinition.OverloadFactor.Should().BeOfType<OneRepMaxProgressiveOverloadFactor>();
    }
    
    [Test]
    public void ReplacesSelectedExerciseDefinitionInTheMiddle()
    {
        _viewModel.WorkoutDefinition.Exercises.Add(new ExerciseDefinition
        {
            OverloadFactor = new PowerLadderOverloadFactor(1, false),
            ExerciseSelector = new MuscleGroupExerciseSelector("First")
        });
        _viewModel.WorkoutDefinition.Exercises.Add(new ExerciseDefinition
        {
            OverloadFactor = new PowerLadderOverloadFactor(2, false),
            ExerciseSelector = new MuscleGroupExerciseSelector("Second")
        });
        _viewModel.WorkoutDefinition.Exercises.Add(new ExerciseDefinition
        {
            OverloadFactor = new PowerLadderOverloadFactor(3, false),
            ExerciseSelector = new MuscleGroupExerciseSelector("Third")
        });
        
        _viewModel.ShowNewExerciseMenu(_viewModel.WorkoutDefinition.Exercises[1]);
        _viewModel.AddExercise(new NewExerciseOptions(ExerciseSelectorType.SpecificExercise,
            ProgressiveOverloadType.OneRepMaxPercentage));

        _viewModel.WorkoutDefinition.Exercises.Count.Should().Be(3);
        _viewModel.WorkoutDefinition.Exercises[0].ExerciseSelector.Should().BeOfType<MuscleGroupExerciseSelector>();
        _viewModel.WorkoutDefinition.Exercises[0].OverloadFactor.Should().BeOfType<PowerLadderOverloadFactor>();
        
        _viewModel.WorkoutDefinition.Exercises[1].ExerciseSelector.Should().BeOfType<SpecificExerciseSelector>();
        _viewModel.WorkoutDefinition.Exercises[1].OverloadFactor.Should().BeOfType<OneRepMaxProgressiveOverloadFactor>();
        
        _viewModel.WorkoutDefinition.Exercises[2].ExerciseSelector.Should().BeOfType<MuscleGroupExerciseSelector>();
        _viewModel.WorkoutDefinition.Exercises[2].OverloadFactor.Should().BeOfType<PowerLadderOverloadFactor>();
    }
}