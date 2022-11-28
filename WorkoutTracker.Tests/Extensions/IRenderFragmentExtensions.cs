using AngleSharp.Dom;
using Bunit;
using Microsoft.AspNetCore.Components;
using System.Linq;

namespace WorkoutTracker.Tests.Extensions;

public static class IRenderFragmentExtensions
{
    public static IRenderedComponent<TComponent> FindComponent<TComponent>(this IRenderedFragment renderedFragment, string cssSelector)
        where TComponent : IComponent
    {
        var components = renderedFragment.FindComponents<TComponent>();
        return components.SingleOrDefault(b => b.Nodes.QuerySelector(cssSelector) is object);
    }
}
