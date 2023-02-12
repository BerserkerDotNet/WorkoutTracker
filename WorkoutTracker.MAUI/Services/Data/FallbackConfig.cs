using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;

namespace WorkoutTracker.MAUI.Services.Data;

public class FallbackConfig : IConfiguration
{
    private Dictionary<string, string> _data = new Dictionary<string, string>();

    public string this[string key] { get => _data[key]; set => _data[key] = value; }

    public IEnumerable<IConfigurationSection> GetChildren()
    {
        throw new NotImplementedException();
    }

    public IChangeToken GetReloadToken()
    {
        throw new NotImplementedException();
    }

    public IConfigurationSection GetSection(string key)
    {
        throw new NotImplementedException();
    }
}
