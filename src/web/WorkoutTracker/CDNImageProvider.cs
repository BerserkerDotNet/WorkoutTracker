namespace WorkoutTracker;

public class CDNImageProvider
{
    private readonly Uri _basePath;

    public CDNImageProvider(Uri basePath)
    {
        this._basePath = basePath;
    }

    public string GetFullPath(string imagePath)
    {
        if (string.IsNullOrEmpty(imagePath))
        {
            return _basePath.ToString();
        }

        return new Uri(_basePath, imagePath.TrimStart('/', '\\')).ToString();
    }
}