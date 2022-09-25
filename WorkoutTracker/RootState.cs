using BlazorState.Redux.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using WorkoutTracker.Data.Actions;
using WorkoutTracker.Data.Reducers;
using WorkoutTracker.Data.Selectors;

namespace WorkoutTracker;

public class RootState
{
    public ExercisesState Exercises { get; set; }

    public LogEntriesState ExerciseLogs { get; set; }

    public ExerciseScheduleState ExerciseSchedule { get; set; }

    public UserPreferencesState Preferences { get; set; }

    public UIState UI { get; set; }
}

public class ComponentWithProps<TProps> : ComponentBase
    where TProps : class
{
    [CascadingParameter]
    public PropsProvider Provider { get; set; }

    public TProps Props { get; private set; }

    protected sealed override void OnParametersSet()
    {
        Props = Provider.Get<TProps>();
        OnAfterParametersSet();
    }

    protected virtual void OnAfterParametersSet()
    {
    }
}

public class PropsProvider : ComponentBase, IDisposable
{
    private PropsContext _propsContext;
    private Dictionary<Type, object> _mappedProps;

    [Inject]
    public IStore<RootState> Store { get; set; }

    [Inject]
    public IPropsConfig PropsConfig { get; set; }

    [Parameter]
    public RenderFragment ChildContent { get; set; }

    protected override void OnInitialized()
    {
        _mappedProps = new Dictionary<Type, object>();
        Store.OnStateChanged += OnStateChanged;
        _propsContext = new PropsContext();

        PropsConfig.Configure(_propsContext);
    }

    public TProps Get<TProps>()
        where TProps : class
    {
        var key = typeof(TProps);

        if (_mappedProps.ContainsKey(key))
        {
            return _mappedProps[key] as TProps;
        }

        var mapper = _propsContext.GetMapper<TProps>();
        var props = mapper(Store.State, Store);
        _mappedProps[key] = props;

        return props;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<CascadingValue<PropsProvider>>(1);
        builder.AddAttribute(2, "Value", this);
        builder.AddAttribute(3, "ChildContent", ChildContent);
        builder.CloseComponent();
    }

    public void Dispose()
    {
        Store.OnStateChanged -= OnStateChanged;
    }

    private void OnStateChanged(object sender, EventArgs e)
    {
        StateHasChanged();
        _mappedProps.Clear();
    }
}

public class PropsContext
{
    private Dictionary<Type, Func<RootState, IDispatcher, object>> _propsMappers;

    public PropsContext()
    {
        _propsMappers = new Dictionary<Type, Func<RootState, IDispatcher, object>>();
    }

    public void Register<TProps>(Func<RootState, IDispatcher, TProps> mapper) where TProps : class
    {
        _propsMappers.Add(typeof(TProps), mapper);
    }

    public Func<RootState, IDispatcher, TProps> GetMapper<TProps>() where TProps : class
    {
        return _propsMappers[typeof(TProps)] as Func<RootState, IDispatcher, TProps>;
    }
}

public interface IPropsConfig
{
    void Configure(PropsContext context);
}

public class PropsConfigurations : IPropsConfig
{
    public void Configure(PropsContext context)
    {
        context.Register(ExerciseScheduleProps);
        context.Register(ExerciseSchedulePanelProps);
        context.Register(ExerciseActionBarProps);
        context.Register(ExerciseSetsProps);
    }

    public ExerciseScheduleProps ExerciseScheduleProps(RootState state, IDispatcher dispatcher)
    {
        var props = new ExerciseScheduleProps();
        props.AllExercises = state.SelectExercises();
        props.Schedule = state.SelectSchedule();
        props.AddExercise = CallbackAsync<IExerciseSelector>(async (selector) =>
        {
            var request = new InsertNewExerciseRequest(selector, state.SelectSchedule());
            await dispatcher.Dispatch<InsertNewExerciseAction, InsertNewExerciseRequest>(request);
        });

        return props;
    }

    public ExerciseSchedulePanelProps ExerciseSchedulePanelProps(RootState state, IDispatcher dispatcher)
    {
        var schedule = state.SelectSchedule();
        var props = new ExerciseSchedulePanelProps();
        props.PreviousSessionLog = state.SelectLastLogByExercise(schedule);
        return props;
    }

    public ExerciseActionBarProps ExerciseActionBarProps(RootState state, IDispatcher dispatcher)
    {
        var removeExercise = CallbackAsync<Guid>(async id => await dispatcher.Dispatch<RemoveExerciseFromSchedule, RemoveExerciseFromScheduleRequest>(new RemoveExerciseFromScheduleRequest(state.SelectSchedule(), id)));
        Action<Guid, ExerciseViewModel> replaceExercise = (id, exercise) => CallbackAsync(async () => await dispatcher.Dispatch<ReplaceExerciseInSchedule, ReplaceExerciseInScheduleRequest>(new ReplaceExerciseInScheduleRequest(id, exercise)))();
        var increaseSets = Callback<Guid>((id) => dispatcher.Dispatch(new IncreaseSets(id)));
        var decreaseSets = Callback<Guid>((id) => dispatcher.Dispatch(new DecreaseSets(id)));

        return new ExerciseActionBarProps(state.SelectExercises(), increaseSets, decreaseSets, replaceExercise, removeExercise);
    }

    public ExerciseSetsProps ExerciseSetsProps(RootState state, IDispatcher dispatcher)
    {
        var schedule = state.SelectSchedule();
        var todayLogByExercise = state.SelectTodayExerciseLogLookup();
        var save = CallbackAsync<LogEntryViewModel>(async e =>
        {
            await dispatcher.Dispatch<SaveExerciseLogEntryAction, LogEntryViewModel>(e);
        });
        Action<Guid, int> startSet = (id, index) => Callback(() => dispatcher.Dispatch(new UpdateSetStatus(id, index, SetStatus.InProgress)))();
        Action<Guid, int> finishSet = (id, index) => Callback(() => dispatcher.Dispatch(new UpdateSetStatus(id, index, SetStatus.Completed)))();
        Action<Guid, WorkoutExerciseSetViewModel> updateSet = (id, set) => Callback(() => dispatcher.Dispatch(new UpdateSet(id, set)))();

        return new ExerciseSetsProps(todayLogByExercise, startSet, finishSet, updateSet, save);
    }

    protected Action Callback(Action callback)
    {
        EventCallback callbackDelegate = EventCallback.Factory.Create(this, callback);
        return delegate
        {
            callbackDelegate.InvokeAsync(null);
        };
    }

    protected Action<T> Callback<T>(Action<T> callback)
    {
        EventCallback<T> callbackDelegate = EventCallback.Factory.Create(this, callback);
        return delegate (T value)
        {
            callbackDelegate.InvokeAsync(value);
        };
    }

    protected AsyncAction CallbackAsync(Func<Task> callback)
    {
        EventCallback callbackDelegate = EventCallback.Factory.Create(this, callback);
        return () => callbackDelegate.InvokeAsync(null);
    }

    protected AsyncAction<T> CallbackAsync<T>(Func<T, Task> callback)
    {
        EventCallback<T> callbackDelegate = EventCallback.Factory.Create(this, callback);
        return (T value) => callbackDelegate.InvokeAsync(value);
    }
}
