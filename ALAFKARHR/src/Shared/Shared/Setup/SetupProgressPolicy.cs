namespace Shared.Setup;

public static class SetupProgressPolicy
{
    public static IReadOnlySet<string> ExpandTrackDependencies(
        IEnumerable<string> selectedTrackKeys,
        IEnumerable<SetupTrackDefinition> tracks)
    {
        var byKey = tracks.ToDictionary(x => x.Key, StringComparer.OrdinalIgnoreCase);
        var selected = selectedTrackKeys.Where(byKey.ContainsKey).ToHashSet(StringComparer.OrdinalIgnoreCase);
        selected.Add("core");
        var pending = new Queue<string>(selected);
        while (pending.TryDequeue(out var key))
        {
            if (!byKey.TryGetValue(key, out var track)) continue;
            foreach (var dependency in track.DependencyTrackKeys ?? [])
            {
                if (byKey.ContainsKey(dependency) && selected.Add(dependency)) pending.Enqueue(dependency);
            }
        }
        return selected;
    }

    public static int CompletionPercent(IEnumerable<bool> completionStates)
    {
        var states = completionStates.ToList();
        return states.Count == 0 ? 0 : (int)Math.Round(states.Count(x => x) * 100d / states.Count);
    }
}
