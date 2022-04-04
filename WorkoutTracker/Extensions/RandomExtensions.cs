namespace WorkoutTracker.Extensions;
public static class RandomExtensions
{
    public static T[] Shuffle<T>(this Random rng, T[] array)
    {
        var arrayClone = array.Clone() as T[];
        int n = arrayClone.Length;
        while (n > 1)
        {
            int k = rng.Next(n--);
            T temp = arrayClone[n];
            arrayClone[n] = arrayClone[k];
            arrayClone[k] = temp;
        }

        return arrayClone;
    }
}
