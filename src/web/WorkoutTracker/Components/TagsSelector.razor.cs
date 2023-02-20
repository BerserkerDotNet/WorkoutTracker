using Microsoft.AspNetCore.Components;

namespace WorkoutTracker.Components;

public partial class TagsSelector 
{
	private const string AddNewTagItem = "Add new...";
	private HashSet<string> _selectedTags = new HashSet<string>();
	private string _selectedText = string.Empty;
	private MudAutocomplete<string> _autocomplete;

	[Parameter]
	[EditorRequired]
    public IEnumerable<string> Tags { get; set; }

	[Parameter]
	public IEnumerable<string> SelectedTags { get; set; }

	[Parameter]
	public EventCallback<IEnumerable<string>> SelectedTagsChanged { get; set; }

    protected override void OnParametersSet()
    {
		_selectedTags = new HashSet<string>(SelectedTags);
	}

    private async Task TagSelected(string tag)
	{
		if (string.IsNullOrEmpty(tag)) 
		{
			return;
		}

		if (string.Equals(tag, AddNewTagItem, StringComparison.OrdinalIgnoreCase))
		{
			tag = _selectedText; 
		}

		if (!_selectedTags.Contains(tag)) 
		{
			_selectedTags.Add(tag);
			_selectedText = string.Empty;
			_autocomplete.Reset();
			await SelectedTagsChanged.InvokeAsync(_selectedTags);
		}
	}

	private Task<IEnumerable<string>> SearchTags(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return Task.FromResult(Tags);
		}

		var tags = Tags.Where(x => x.Contains(value, StringComparison.InvariantCultureIgnoreCase));
		if (!tags.Any()) 
		{
			tags = new[] { AddNewTagItem };
		}

		return Task.FromResult(tags);
	}

	private async Task RemoveTag(string tag)
	{
		_selectedTags.Remove(tag);
		await SelectedTagsChanged.InvokeAsync(_selectedTags);
	}

	private void OnTextChanged(string text)
	{
		if (!string.Equals(text, AddNewTagItem, StringComparison.OrdinalIgnoreCase))
		{
			_selectedText = text;
		}
	}
}