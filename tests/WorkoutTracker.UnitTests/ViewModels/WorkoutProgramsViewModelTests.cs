using FluentAssertions;
using NSubstitute;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using WorkoutTracker.Models.Entities;
using WorkoutTracker.Services;
using WorkoutTracker.Services.Interfaces;
using WorkoutTracker.Services.ViewModels;
using INavigation = WorkoutTracker.Services.Interfaces.INavigation;

namespace WorkoutTracker.UnitTests.ViewModels;

[TestFixture]
[ExcludeFromCodeCoverage]
public class WorkoutProgramsViewModelTests
{
    private WorkoutProgramsViewModel _vm;
    private IWorkoutDataProvider _db;
    private INavigation _navigation;
    
    [SetUp]
    public void Init()
    {
        _db = Substitute.For<IWorkoutDataProvider>();
        _navigation = Substitute.For<INavigation>();
        _vm = new WorkoutProgramsViewModel(_db, _navigation);
    }

    [Test]
    public void DeleteShouldRemoveProgramFromList()
    {
        var program1 = new WorkoutProgram
        {
            Name = "program 1",
            Id = Guid.NewGuid(),
            Schedule = Schedule.Default
        };
        var program2 = new WorkoutProgram
        {
            Name = "program 2",
            Id = Guid.NewGuid(),
            Schedule = Schedule.Default
        };
        var program3 = new WorkoutProgram
        {
            Name = "program 3",
            Id = Guid.NewGuid(),
            Schedule = Schedule.Default
        };
        
        _vm.Programs = new ObservableCollection<WorkoutProgram>(new[]
        {
            program1,
            program2,
            program3
        });
        
        _vm.DeleteProgram(program2);
        
        _vm.Programs.Should().HaveCount(2);
        _vm.Programs.Should().Contain(program1);
        _vm.Programs.Should().NotContain(program2);
        _vm.Programs.Should().Contain(program3);
        
        _db.Received(1).DeleteViewModel(program2);
    }

    [Test]
    public void SetCurrentWorkoutDoesNothingIfSelectingAlreadySelectedProgram()
    {
        var id = Guid.NewGuid();
        _vm.SelectedProgram = id;
        
        _vm.SetCurrentWorkout(new WorkoutProgram() { Id = id, Name = "Test", Schedule = Schedule.Default });

        _vm.SelectedProgram.Should().Be(id);
        _db.DidNotReceive().SetCurrentWorkout(Arg.Any<Guid>());
    }
    
    [Test]
    public void SetCurrentWorkoutUpdatesSelectedProgram()
    {
        var id = Guid.NewGuid();
        var newId = Guid.NewGuid();
        _vm.SelectedProgram = id;
        
        _vm.SetCurrentWorkout(new WorkoutProgram() { Id = newId, Name = " New Test", Schedule = Schedule.Default });

        _vm.SelectedProgram.Should().Be(newId);
        _db.Received(1).SetCurrentWorkout(newId);
    }

    [Test]
    public async Task EditProgramShouldNotFailIfProgramIsNull()
    {        
        await _vm.EditProgram(null);

        await _navigation.Received(1).GoTo("EditWorkoutProgram",
            Arg.Is<Dictionary<string, object>>(d => d.ContainsKey(nameof(EditWorkoutProgramViewModel.WorkoutProgram))));
    }

    [Test]
    public async Task EditProgramShouldNavigateToEditPage()
    {
        await _vm.EditProgram(WorkoutProgramProvider.Default);

        await _navigation.Received(1).GoTo("EditWorkoutProgram",
            Arg.Is<Dictionary<string, object>>(d => d.ContainsKey(nameof(EditWorkoutProgramViewModel.WorkoutProgram))));
    }

    [Test]
    public void LoadProgramsPopulatesData()
    {
        var currentWorkoutId = Guid.NewGuid();
        _db.GetPrograms().Returns(new[] { WorkoutProgramProvider.Default, WorkoutProgramProvider.Default });
        _db.GetProfile().Returns(new Profile(){CurrentWorkout = currentWorkoutId});

        var monitor = _vm.Monitor();
        _vm.LoadPrograms();

        monitor.Should().RaisePropertyChangeFor(p => p.IsLoadingData);
        _vm.SelectedProgram.Should().Be(currentWorkoutId);
        _vm.Programs.Should().HaveCount(2);
    }
    
    [Test]
    public void LoadProgramsShouldHandleSelectedProgramBeingNull()
    {
        _db.GetPrograms().Returns(new[] { WorkoutProgramProvider.Default });
        _db.GetProfile().Returns(new Profile());
        
        var monitor = _vm.Monitor();
        _vm.LoadPrograms();

        monitor.Should().RaisePropertyChangeFor(p => p.IsLoadingData);
        _vm.SelectedProgram.Should().BeNull();
        _vm.Programs.Should().HaveCount(1);
    }
}