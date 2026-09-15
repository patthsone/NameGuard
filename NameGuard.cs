using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Timers;

namespace NameGuard;

public class NameGuard : BasePlugin, IPluginConfig<NameGuardConfig>
{
    public override string ModuleName => "NameGuard";
    public override string ModuleAuthor => "PattHs";
    public override string ModuleVersion => "1.0.0";

    public NameGuardConfig Config { get; set; } = new();

    private readonly Dictionary<ulong, (string Name, string Clan)> _originals = new();

    public void OnConfigParsed(NameGuardConfig config)
    {
        Config = config;
    }

    public override void Load(bool hotReload)
    {
        RegisterEventHandler<EventPlayerConnectFull>(OnPlayerConnectFull);
        RegisterEventHandler<EventPlayerChangename>(OnPlayerChangename);
        RegisterListener<Listeners.OnClientDisconnect>(OnClientDisconnect);

        AddTimer(Config.PollIntervalSeconds, CheckPlayers, TimerFlags.REPEAT);

        if (hotReload)
        {
            foreach (var controller in Utilities.GetPlayers())
            {
                if (!IsValidHuman(controller))
                    continue;

                _originals[controller.SteamID] = (controller.PlayerName, controller.Clan);
            }
        }
    }

    private HookResult OnPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
    {
        var controller = @event.Userid;
        if (!IsValidHuman(controller))
            return HookResult.Continue;

        _originals[controller.SteamID] = (controller.PlayerName, controller.Clan);

        return HookResult.Continue;
    }

    private HookResult OnPlayerChangename(EventPlayerChangename @event, GameEventInfo info)
    {
        if (!Config.RevertName)
            return HookResult.Continue;

        var controller = @event.Userid;
        if (!IsValidHuman(controller))
            return HookResult.Continue;

        if (_originals.TryGetValue(controller.SteamID, out var original) &&
            controller.PlayerName != original.Name)
        {
            controller.PlayerName = original.Name;
            Utilities.SetStateChanged(controller, "CBasePlayerController", "m_iszPlayerName");
        }

        return HookResult.Continue;
    }

    private void OnClientDisconnect(int playerSlot)
    {
        var controller = Utilities.GetPlayerFromSlot(playerSlot);
        if (controller == null || !controller.IsValid)
            return;

        _originals.Remove(controller.SteamID);
    }

    private void CheckPlayers()
    {
        foreach (var controller in Utilities.GetPlayers())
        {
            if (!IsValidHuman(controller))
                continue;

            if (!_originals.TryGetValue(controller.SteamID, out var original))
                continue;

            if (Config.RevertName && controller.PlayerName != original.Name)
            {
                controller.PlayerName = original.Name;
                Utilities.SetStateChanged(controller, "CBasePlayerController", "m_iszPlayerName");
            }

            if (Config.RevertClanTag && controller.Clan != original.Clan)
            {
                controller.Clan = original.Clan;
                Utilities.SetStateChanged(controller, "CCSPlayerController", "m_szClan");
            }
        }
    }

    private static bool IsValidHuman([System.Diagnostics.CodeAnalysis.NotNullWhen(true)] CCSPlayerController? controller)
    {
        return controller != null
            && controller.IsValid
            && !controller.IsBot
            && !controller.IsHLTV
            && controller.SteamID != 0;
    }
}
