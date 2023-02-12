using System;

namespace WorkoutTracker.MAUI.Exceptions;

[Serializable]
public class DataPersistanceException : Exception
{
    public DataPersistanceException() { }
    public DataPersistanceException(string message) : base(message) { }
    public DataPersistanceException(string message, Exception inner) : base(message, inner) { }
    protected DataPersistanceException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
