using FluentAssertions;
using NSubstitute;
using System.Diagnostics.CodeAnalysis;
using WorkoutTracker.Models.Entities;
using WorkoutTracker.Services;
using WorkoutTracker.Services.Interfaces;
using WorkoutTracker.Services.ViewModels;
using INavigation = WorkoutTracker.Services.Interfaces.INavigation;

namespace WorkoutTracker.UnitTests.ViewModels;

[TestFixture]
[ExcludeFromCodeCoverage]
public class EditWorkoutProgramViewModelTests
{
    private EditWorkoutProgramViewModel _vm;
    private INavigation _navigation;
    private IWorkoutDataProvider _db;
    
    [SetUp]
    public void Init()
    {
        _navigation = Substitute.For<INavigation>();
        _db = Substitute.For<IWorkoutDataProvider>();
        _vm = new EditWorkoutProgramViewModel(_db, _navigation);
    }

    [Test]
    public async Task SaveViewModelAndGoBack()
    {        
        var program =  new WorkoutProgram { Name = "Test program", Id = Guid.NewGuid(), Schedule = Schedule.Default };
        _vm.WorkoutProgram = program;
        
        await _vm.Save();

        _vm.WorkoutProgram.Should().BeNull();
        _db.Received(1).UpdateViewModel(program);
        await _navigation.Received(1).GoBack();
    }

    [Test]
    public async Task NavigatesBackToParentView()
    {
        _vm.WorkoutProgram =  new WorkoutProgram { Name = "Test program", Id = Guid.NewGuid(), Schedule = Schedule.Default };

        await _vm.Back();

        _vm.WorkoutProgram.Should().BeNull();
        await _navigation.Received(1).GoBack();
    }

    [Test]
    public void ApplyQueryAttributeSetsViewModelIfNull()
    {
        var program = new WorkoutProgram { Name = "Test program", Id = Guid.NewGuid(), Schedule = Schedule.Default };
        _vm.ApplyQueryAttributes(new Dictionary<string, object>
        {
            { nameof(WorkoutProgram) , program}
        });

        _vm.WorkoutProgram.Should().Be(program);
    }
    
    [Test]
    public void ApplyQueryAttributeSetsDoesNotOverridesViewModel()
    {
         var originalProgram = new WorkoutProgram { Name = "Test program", Id = Guid.NewGuid(), Schedule = Schedule.Default };
         _vm.WorkoutProgram = originalProgram;
        var program = new WorkoutProgram { Name = "Test program 2", Id = Guid.NewGuid(), Schedule = Schedule.Default };
        _vm.ApplyQueryAttributes(new Dictionary<string, object>
        {
            { nameof(WorkoutProgram) , program}
        });

        _vm.WorkoutProgram.Should().Be(originalProgram);
    }
    
    [Test]
    public void ApplyQueryAttributeShouldNotFailIfProgramIsNull()
    {
        _vm.ApplyQueryAttributes(new Dictionary<string, object>
        {
            { nameof(WorkoutProgram) , null}
        });

        _vm.WorkoutProgram.Should().BeNull();
    }
    
    [Test]
    public void ApplyQueryAttributeShouldNotFailIfParameterMissing()
    {
        _vm.ApplyQueryAttributes(new Dictionary<string, object>());
        _vm.WorkoutProgram.Should().BeNull();
    }

    [Test]
    public void RefreshShouldCloneWorkoutDefinition()
    {
        var originalProgram = WorkoutProgramProvider.Default;
        _vm.WorkoutProgram = originalProgram;
        
        _vm.Refresh();

        _vm.WorkoutProgram.Should().NotBe(originalProgram);
        _vm.WorkoutProgram.Should().BeEquivalentTo(originalProgram);
    }
    
    [Test]
    public void NavigatesToEditDefinitionPage()
    {
        _vm.EditDefinition(new AssignedWorkoutDefinition(DayOfWeek.Monday, "test", WorkoutDefinition.Rest));

        _navigation.Received(1).GoTo("EditWorkoutDefinition",
            Arg.Is<Dictionary<string, object>>(d => d.ContainsKey("WorkoutDefinition")));
    }
}