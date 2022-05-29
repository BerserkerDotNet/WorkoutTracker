using BlazorApplicationInsights;
using System.Runtime.ExceptionServices;

namespace WorkoutTracker.Data.Actions;

public abstract class TrackableAction<TProperty> : IAsyncAction<TProperty>
{
    public TrackableAction(ApplicationContext context)
    {
        Context = context;
    }

    protected ApplicationContext Context { get; }

    public async Task Execute(IDispatcher dispatcher, TProperty property)
    {
        var trackableProperties = new Dictionary<string, string>();
        trackableProperties.Add("Parameter", property.ToString());

        await Context.StartTrackEvent(GetType().Name);
        try
        {
            await Execute(dispatcher, property, trackableProperties);
        }
        catch (Exception ex) 
        {
            await Context.TrackException(new Error 
            {
                Name = $"Error executing {GetType().Name} action.",
                Message = ex.Message,
                Stack = ex.StackTrace
            });

            Context.ShowError(ex.Message);
            ExceptionDispatchInfo.Capture(ex).Throw();
        }
        finally
        {
            await Context.StopTrackEvent(GetType().Name, trackableProperties);
            await Context.Flush();
        }
    }

    protected abstract Task Execute(IDispatcher dispatcher, TProperty property, Dictionary<string, string> trackableProperties);
}

public abstract class TrackableAction : IAsyncAction
{
    public TrackableAction(ApplicationContext context)
    {
        Context = context;
    }

    protected ApplicationContext Context { get; }

    public async Task Execute(IDispatcher dispatcher)
    {
        var trackableProperties = new Dictionary<string, string>();
        await Context.StartTrackEvent(GetType().Name);
        try
        {
            await Execute(dispatcher, trackableProperties);
        }
        catch (Exception ex)
        {
            await Context.TrackException(new Error
            {
                Name = $"Error executing {GetType().Name} action.",
                Message = ex.Message,
                Stack = ex.StackTrace
            });

            Context.ShowError(ex.Message);
            ExceptionDispatchInfo.Capture(ex).Throw();
        }
        finally
        {
            await Context.StopTrackEvent(GetType().Name, trackableProperties);
            await Context.Flush();
        }
    }

    protected abstract Task Execute(IDispatcher dispatcher, Dictionary<string, string> trackableProperties);
}