using BlazorState.Redux.Blazor;
using Microsoft.AspNetCore.Components;
using WorkoutTracker.Components;
using WorkoutTracker.Data.Actions;
using static WorkoutTracker.Data.Selectors.ExerciseHistorySelectors;

namespace WorkoutTracker.Pages
{
	public class LogRecordDetailsConnected : ConnectedComponent<ExerciseDetails, RootState, LogRecordDetailsProps>
	{
		[Inject]
		public NavigationManager Navigation { get; set; }

		[Parameter]
		[EditorRequired]
		public Guid RecordId { get; set; }

		[Parameter]
		[EditorRequired]
		public DateTime Date { get; set; }

		protected override void MapDispatchToProps(IStore<RootState> store, LogRecordDetailsProps props)
		{
			props.OnSave = CreateCallback<LogEntryViewModel>(async e =>
			{
				await store.Dispatch<SaveExerciseLogEntryAction, LogEntryViewModel>(e);
			});

			props.OnBack = CreateCallback(() => Navigation.NavigateTo("/log"));
		}

		protected override void MapStateToProps(RootState state, LogRecordDetailsProps props)
		{
			props.LogRecord = SelectExerciseLog(state, Date, RecordId); // TODO: Handle null and also load exercise initally
		}
	}
}