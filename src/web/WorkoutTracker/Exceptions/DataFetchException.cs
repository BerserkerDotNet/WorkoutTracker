namespace WorkoutTracker.Exceptions
{
    [Serializable]
    public class DataFetchException : Exception
    {
        public DataFetchException() { }
        public DataFetchException(string message) : base(message) { }
        public DataFetchException(string message, Exception inner) : base(message, inner) { }
        protected DataFetchException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
