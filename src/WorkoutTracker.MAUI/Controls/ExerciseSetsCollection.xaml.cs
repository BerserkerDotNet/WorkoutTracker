using Microsoft.Maui.Graphics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using WorkoutTracker.MAUI.Extensions;
using WorkoutTracker.Models.Contracts;
using WorkoutTracker.Models.Entities;
using WorkoutTracker.Models.Presentation;
using WorkoutTracker.Services.Models;

namespace WorkoutTracker.MAUI.Controls;

public partial class ExerciseSetsCollection : ContentView
{
    public static readonly BindableProperty OnHeaderMenuClickedCommandProperty = BindableProperty.Create(nameof(OnHeaderMenuClickedCommand), typeof(ICommand), typeof(ExerciseSetsCollection), defaultValue: null);
    public static readonly BindableProperty SetClickedCommandProperty = BindableProperty.Create(nameof(SetClickedCommand), typeof(ICommand), typeof(ExerciseSetsCollection), defaultValue: null);
    public static readonly BindableProperty AddSetCommandProperty = BindableProperty.Create(nameof(AddSetCommand), typeof(ICommand), typeof(ExerciseSetsCollection), defaultValue: null);
    public static readonly BindableProperty ReduceSetsCommandProperty = BindableProperty.Create(nameof(ReduceSetsCommand), typeof(ICommand), typeof(ExerciseSetsCollection), defaultValue: null);
    public static readonly BindableProperty RemoveExerciseCommandProperty = BindableProperty.Create(nameof(RemoveExerciseCommand), typeof(ICommand), typeof(ExerciseSetsCollection), defaultValue: null);

    public ICommand RemoveExerciseCommand
    {
        get { return (ICommand)GetValue(RemoveExerciseCommandProperty); }
        set { SetValue(RemoveExerciseCommandProperty, value); }
    }

    public ICommand ReduceSetsCommand
    {
        get { return (ICommand)GetValue(ReduceSetsCommandProperty); }
        set { SetValue(ReduceSetsCommandProperty, value); }
    }

    public ICommand AddSetCommand
    {
        get { return (ICommand)GetValue(AddSetCommandProperty); }
        set { SetValue(AddSetCommandProperty, value); }
    }

    public ICommand SetClickedCommand
    {
        get { return (ICommand)GetValue(SetClickedCommandProperty); }
        set { SetValue(SetClickedCommandProperty, value); }
    }

    public ICommand OnHeaderMenuClickedCommand
    {
        get { return (ICommand)GetValue(OnHeaderMenuClickedCommandProperty); }
        set { SetValue(OnHeaderMenuClickedCommandProperty, value); }
    }

    private LogEntryViewModel _viewModel;

    private bool _isSetsRendered = false;

    public ExerciseSetsCollection()
    {
        InitializeComponent();
    }

    private void OnExpandCollapse(object sender, System.EventArgs e)
    {
        expandArea.IsVisible = !expandArea.IsVisible;
        if (!_isSetsRendered)
        {
            RenderSets(_viewModel.Sets);
        }
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        var ctx = BindingContext as LogEntryViewModel;
        if (ctx is null)
        {
            return;
        }

        if (_viewModel is not null)
        {
            var oldSets = _viewModel.Sets as ObservableCollection<IExerciseSet>;
            oldSets.CollectionChanged -= ObservableSets_CollectionChanged;
        }

        _viewModel = ctx;

        var observableSets = _viewModel.Sets as ObservableCollection<IExerciseSet>;
        observableSets.CollectionChanged += ObservableSets_CollectionChanged;

        _isSetsRendered = false;
        if (expandArea.IsVisible)
        {
            RenderSets(_viewModel.Sets);
        }

        UpdateSetsStatus();
    }

    private void ObservableSets_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        UpdateSetsStatus();

        if (!_isSetsRendered)
        {
            return;
        }

        if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
        {
            AddSet(e.NewItems[0] as IExerciseSet);
        }
        else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
        {
            this.expandArea.RemoveAt(e.OldStartingIndex);
        }
        else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
        {
            ReplaceSet(e.NewItems[0] as IExerciseSet, e.NewStartingIndex);
        }
    }

    private void UpdateSetsStatus()
    {
        var completedCount = _viewModel.Sets.OfType<CompletedSet>().Count();
        var proposedCount = _viewModel.Sets.OfType<ProposedSet>().Count();
        var totalCount = _viewModel.Sets.Count();
        this.setsStatus.Text = $"{completedCount}/{totalCount}";
        if (proposedCount == totalCount)
        {
            this.setsStatus.BackgroundColor = Colors.Transparent;
        }
        else
        {
            this.setsStatus.BackgroundColor = completedCount == totalCount ? Colors.LightGreen : Colors.LightSkyBlue;
        }

    }

    private void RenderSets(IEnumerable<IExerciseSet> sets)
    {
        this.expandArea.BatchBegin();
        this.expandArea.Clear();
        foreach (var set in sets)
        {
            AddSet(set);
        }

        this.expandArea.BatchCommit();

        _isSetsRendered = true;
    }

    private void AddSet(IExerciseSet set)
    {
        var number = this.expandArea.Children.Count + 1;
        var wrapper = new SetWrapper(number, set, _viewModel);
        var setView = new ExerciseSetView() { BindingContext = wrapper };
        setView.SetBinding(ExerciseSetView.SetClickedCommandProperty, new Binding(nameof(SetClickedCommand), source: this));
        this.expandArea.Add(setView);
    }

    private void ReplaceSet(IExerciseSet set, int idx)
    {
        var wrapper = new SetWrapper(idx + 1, set, _viewModel);
        ((ExerciseSetView)this.expandArea.Children[idx]).BindingContext = wrapper;
    }

    private void OnHeaderMenuClicked(object sender, System.EventArgs e)
    {
        contextMenu.IsOpen = true;
    }

    private void OnRemoveExerciseClicked(object sender, System.EventArgs e)
    {
        contextMenu.IsOpen = false;
        if (RemoveExerciseCommand.CanExecute(_viewModel))
        {
            RemoveExerciseCommand.Execute(_viewModel);
        }
    }

    private void OnReplaceExerciseClicked(object sender, System.EventArgs e)
    {
        contextMenu.IsOpen = false;
        if (OnHeaderMenuClickedCommand.CanExecute(_viewModel))
        {
            OnHeaderMenuClickedCommand.Execute(_viewModel);
        }
    }

    private void OnAddSetClicked(object sender, System.EventArgs e)
    {
        contextMenu.IsOpen = false;
        if (AddSetCommand.CanExecute(_viewModel))
        {
            AddSetCommand.Execute(_viewModel);
        }
    }

    private void OnReduceSetsClicked(object sender, System.EventArgs e)
    {
        contextMenu.IsOpen = false;
        if (ReduceSetsCommand.CanExecute(_viewModel))
        {
            ReduceSetsCommand.Execute(_viewModel);
        }
    }
}