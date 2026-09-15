using CounterStrikeSharp.API.Core;

namespace NameGuard;

public class NameGuardConfig : BasePluginConfig
{
    public bool RevertName { get; set; } = true;
    public bool RevertClanTag { get; set; } = true;
    public float PollIntervalSeconds { get; set; } = 1.0f;
}
