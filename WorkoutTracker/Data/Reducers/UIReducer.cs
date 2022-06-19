using WorkoutTracker.Data.Actions;

namespace WorkoutTracker.Data.Reducers;

public record UIState(Stack<ProgressIndicatorViewModel> ProgressIndicators);
public class UIReducer : IReducer<UIState>
{
    public UIState Reduce(UIState state, IAction action)
    {
        state = state ?? new UIState(new Stack<ProgressIndicatorViewModel>());

        switch (action)
        {
            case ShowProgressIndicator a:
                state.ProgressIndicators.Push(new ProgressIndicatorViewModel(a.Text));
                return state;
            case HideProgressIndicator _:
                state.ProgressIndicators.Pop();
                return state;
            default:
                return state;
        }
    }
}