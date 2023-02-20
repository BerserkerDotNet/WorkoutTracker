using WorkoutTracker.Extensions;
using WorkoutTracker.Models.Contracts;

namespace WorkoutTracker.ViewModels;

public interface IShuffle
{
	IEnumerable<IExerciseSelector> Shuffle(IEnumerable<IExerciseSelector> selectors);
}

public record ShuffleAll() : IShuffle
{
	private readonly Random _random = new Random();

	public IEnumerable<IExerciseSelector> Shuffle(IEnumerable<IExerciseSelector> selectors)
	{
		return _random.Shuffle(selectors.ToArray()).ToArray();
	}
}

public record IndexBasedShuffle(int StartIndex) : IShuffle
{
	private readonly Random _random = new Random();

	public IEnumerable<IExerciseSelector> Shuffle(IEnumerable<IExerciseSelector> selectors)
	{
		var exerciseFiltersToShuffle = selectors.Skip(StartIndex + 1).ToArray();
		var shuffledFilters = _random.Shuffle(exerciseFiltersToShuffle);
		return selectors.Take(StartIndex + 1).Concat(shuffledFilters).ToArray();
	}
}

public record GroupShuffle(int StartIndex, int GroupSize) : IShuffle
{
	private readonly Random _random = new Random();

	public IEnumerable<IExerciseSelector> Shuffle(IEnumerable<IExerciseSelector> selectors)
	{
		var groupsToShuffle = new List<IExerciseSelector[]>();

		for (int i = StartIndex; i < selectors.Count(); i += GroupSize)
		{
			var group = selectors.Skip(i).Take(GroupSize).ToArray();
			groupsToShuffle.Add(group);
		}

		return _random.Shuffle(groupsToShuffle.ToArray()).SelectMany(g => g).ToArray();
	}
}

public class NoShuffle : IShuffle
{
	public IEnumerable<IExerciseSelector> Shuffle(IEnumerable<IExerciseSelector> selectors)
	{
		return selectors;
	}
}