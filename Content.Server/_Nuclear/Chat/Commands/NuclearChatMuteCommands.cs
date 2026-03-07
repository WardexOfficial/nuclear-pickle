// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Diagnostics.CodeAnalysis;
using Content.Server._Nuclear.Chat;
using Content.Server.Administration;
using Content.Shared.Administration;
using Content.Shared._Nuclear.Chat;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.Player;

namespace Content.Server._Nuclear.Chat.Commands;

[AdminCommand(AdminFlags.Moderator)]
public sealed class NuclearChatMuteCommand : LocalizedCommands
{
    [Dependency] private readonly IPlayerManager _players = default!;
    [Dependency] private readonly IEntitySystemManager _systems = default!;

    public override string Command => "chatmute";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length != 3)
        {
            shell.WriteError(Loc.GetString("shell-wrong-arguments-number"));
            shell.WriteLine(Help);
            return;
        }

        if (!TryGetSession(args[0], out var session))
        {
            shell.WriteError(Loc.GetString("nchatmute-error-player-not-found", ("player", args[0])));
            return;
        }

        if (!TryParseChannel(args[1], out var channelFlags))
        {
            shell.WriteError(Loc.GetString("nchatmute-error-channel", ("channel", args[1])));
            return;
        }

        if (!TryParseState(args[2], out var enabled))
        {
            shell.WriteError(Loc.GetString("nchatmute-error-state", ("state", args[2])));
            return;
        }

        _systems.GetEntitySystem<NuclearChatMuteSystem>().SetFlags(session.UserId, channelFlags, enabled);

        shell.WriteLine(Loc.GetString("nchatmute-set",
            ("player", session.Name),
            ("target", ChannelFlagsToString(channelFlags)),
            ("state", enabled ? Loc.GetString("nchatmute-flag-on") : Loc.GetString("nchatmute-flag-off"))));
    }

    public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        if (args.Length == 1)
        {
            return CompletionResult.FromHintOptions(
                CompletionHelper.SessionNames(players: _players),
                "<player>");
        }

        if (args.Length == 2)
        {
            return CompletionResult.FromHintOptions(
                new[] { "looc", "ooc", "dead", "all" },
                "<looc|ooc|dead|all>");
        }

        if (args.Length == 3)
        {
            return CompletionResult.FromHintOptions(
                new[] { "on", "off" },
                "<on|off>");
        }

        return CompletionResult.Empty;
    }

    private bool TryGetSession(string usernameOrId, [NotNullWhen(true)] out ICommonSession? session)
    {
        if (_players.TryGetSessionByUsername(usernameOrId, out session))
            return true;

        if (Guid.TryParse(usernameOrId, out var guid))
            return _players.TryGetSessionById(new NetUserId(guid), out session);

        session = null;
        return false;
    }

    private static bool TryParseChannel(string value, out NuclearChatMuteFlags flags)
    {
        switch (value.ToLowerInvariant())
        {
            case "looc":
                flags = NuclearChatMuteFlags.Looc;
                return true;
            case "ooc":
                flags = NuclearChatMuteFlags.Ooc;
                return true;
            case "dead":
                flags = NuclearChatMuteFlags.Dead;
                return true;
            case "all":
                flags = NuclearChatMuteFlags.All;
                return true;
            default:
                flags = NuclearChatMuteFlags.None;
                return false;
        }
    }

    private static bool TryParseState(string value, out bool enabled)
    {
        switch (value.ToLowerInvariant())
        {
            case "on":
            case "true":
            case "1":
                enabled = true;
                return true;
            case "off":
            case "false":
            case "0":
                enabled = false;
                return true;
            default:
                enabled = false;
                return false;
        }
    }

    private static string ChannelFlagsToString(NuclearChatMuteFlags flags)
    {
        if (flags == NuclearChatMuteFlags.All)
            return "all";

        if (flags == NuclearChatMuteFlags.Looc)
            return "looc";

        if (flags == NuclearChatMuteFlags.Ooc)
            return "ooc";

        if (flags == NuclearChatMuteFlags.Dead)
            return "dead";

        return flags.ToString();
    }
}

[AdminCommand(AdminFlags.Moderator)]
public sealed class NuclearChatMuteStatusCommand : LocalizedCommands
{
    [Dependency] private readonly IPlayerManager _players = default!;
    [Dependency] private readonly IEntitySystemManager _systems = default!;

    public override string Command => "chatmute_status";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length != 1)
        {
            shell.WriteError(Loc.GetString("shell-need-exactly-one-argument"));
            shell.WriteLine(Help);
            return;
        }

        if (!TryGetSession(args[0], out var session))
        {
            shell.WriteError(Loc.GetString("nchatmute-error-player-not-found", ("player", args[0])));
            return;
        }

        var flags = _systems.GetEntitySystem<NuclearChatMuteSystem>().GetFlags(session.UserId);
        shell.WriteLine(Loc.GetString("nchatmute-status",
            ("player", session.Name),
            ("looc", FlagToText(flags, NuclearChatMuteFlags.Looc)),
            ("ooc", FlagToText(flags, NuclearChatMuteFlags.Ooc)),
            ("dead", FlagToText(flags, NuclearChatMuteFlags.Dead))));
    }

    public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        if (args.Length == 1)
        {
            return CompletionResult.FromHintOptions(
                CompletionHelper.SessionNames(players: _players),
                "<player>");
        }

        return CompletionResult.Empty;
    }

    private bool TryGetSession(string usernameOrId, [NotNullWhen(true)] out ICommonSession? session)
    {
        if (_players.TryGetSessionByUsername(usernameOrId, out session))
            return true;

        if (Guid.TryParse(usernameOrId, out var guid))
            return _players.TryGetSessionById(new NetUserId(guid), out session);

        session = null;
        return false;
    }

    private string FlagToText(NuclearChatMuteFlags current, NuclearChatMuteFlags expected)
    {
        return (current & expected) != 0
            ? Loc.GetString("nchatmute-flag-on")
            : Loc.GetString("nchatmute-flag-off");
    }
}
