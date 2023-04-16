namespace WorkoutTracker.Services.Extensions;

public static class IntExtensions
{
    public static int RoundToNearestFive(this double weight)
    {
        return RoundToNearestFive((int)Math.Ceiling(weight));
    }

    public static int RoundToNearestFive(this int weight)
    {

        var lastDigit = weight % 10;
        if (lastDigit <= 2)
        {
            return weight - lastDigit;
        }
        else if (lastDigit <= 7)
        {
            return weight + (5 - lastDigit);
        }
        else
        {
            return weight + (10 - lastDigit);
        }
    }
}
