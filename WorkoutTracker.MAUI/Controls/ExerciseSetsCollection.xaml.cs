using Microsoft.Maui.Graphics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using WorkoutTracker.MAUI.Extensions;
using WorkoutTracker.Models.Contracts;
using WorkoutTracker.Models.Entities;
using WorkoutTracker.Models.Presentation;

namespace WorkoutTracker.MAUI.Controls;

public partial class ExerciseSetsCollection : ContentView
{
    public static readonly BindableProperty OnHeaderMenuClickedCommandProperty = BindableProperty.Create(nameof(OnHeaderMenuClickedCommand), typeof(ICommand), typeof(ExerciseSetsCollection), defaultValue: null, propertyChanged: OnCommandChange);
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

    public ExerciseSetsCollection()
    {
        InitializeComponent();
    }

    private static void OnCommandChange(BindableObject bindable, object oldValue, object newValue)
    {

    }

    private void OnExpandCollapse(object sender, System.EventArgs e)
    {
        var fadeDirection = expandArea.Opacity == 0 ? 1 : 0;
        if (fadeDirection == 0)
        {
            expandArea.FadeTo(fadeDirection).ContinueWith((_) =>
            {
                Dispatcher.Dispatch(() =>
                {
                    expandArea.IsVisible = !expandArea.IsVisible;
                });

            });
        }
        else
        {
            expandArea.IsVisible = !expandArea.IsVisible;
            expandArea.FadeTo(fadeDirection);
        }
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        _viewModel = BindingContext as LogEntryViewModel;
        if (_viewModel is null)
        {
            return;
        }

        var observableSets = _viewModel.Sets as ObservableCollection<IExerciseSet>;
        observableSets.CollectionChanged += ObservableSets_CollectionChanged;

        RenderSets(_viewModel.Sets);
        UpdateSetsStatus();
    }

    private void ObservableSets_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        RenderSets(_viewModel.Sets);
        UpdateSetsStatus();
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
        this.expandArea.Clear();

        var idx = 1;
        foreach (var set in sets)
        {
            var wrapper = new SetWrapper(idx, set, _viewModel);
            var setView = new ExerciseSetView() { BindingContext = wrapper };
            setView.SetBinding(ExerciseSetView.SetClickedCommandProperty, new Binding(nameof(SetClickedCommand), source: this));
            this.expandArea.Add(setView);
            idx++;
        }
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

public record SetWrapper(int Number, IExerciseSet Set, LogEntryViewModel Model)
{
    public Color Color => Set.GetColor();
}