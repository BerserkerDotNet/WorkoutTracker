namespace WorkoutTracker.Models
{
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class PluralNameAttribute : System.Attribute
    {
        public PluralNameAttribute(string pluralName)
        {
            PluralName = pluralName;
        }

        public string PluralName { get; }
    }
}
