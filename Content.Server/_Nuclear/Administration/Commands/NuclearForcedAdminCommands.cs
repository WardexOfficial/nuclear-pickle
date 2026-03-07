// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Diagnostics.CodeAnalysis;
using Content.Server.Administration;
using Content.Server.Administration.Managers;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.Player;

namespace Content.Server._Nuclear.Administration.Commands;

internal abstract class BaseForcedAdminCommand : LocalizedCommands
{
    [Dependency] protected readonly IAdminManager AdminManager = default!;
    [Dependency] protected readonly IPlayerManager Players = default!;

    public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        if (args.Length == 1)
        {
            return CompletionResult.FromHintOptions(
                CompletionHelper.SessionNames(players: Players),
                "<username/userId>");
        }

        return CompletionResult.Empty;
    }

    protected bool TryGetSession(string usernameOrId, [NotNullWhen(true)] out ICommonSession? session)
    {
        if (Players.TryGetSessionByUsername(usernameOrId, out session))
            return true;

        if (Guid.TryParse(usernameOrId, out var guid))
            return Players.TryGetSessionById(new NetUserId(guid), out session);

        session = null;
        return false;
    }
}

[AdminCommand(AdminFlags.Permissions)]
internal sealed class ForceDeAdminCommand : BaseForcedAdminCommand
{
    public override string Command => "forcedeadmin";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length != 1)
        {
            shell.WriteError(Loc.GetString("shell-need-exactly-one-argument"));
            shell.WriteLine(Help);
            return;
        }

        if (!TryGetSession(args[0], out var target))
        {
            shell.WriteError(Loc.GetString("force-admin-player-not-found", ("player", args[0])));
            return;
        }

        var adminData = AdminManager.GetAdminData(target, includeDeAdmin: true);
        if (adminData == null)
        {
            shell.WriteError(Loc.GetString("force-admin-target-not-admin", ("player", target.Name)));
            return;
        }

        if (!adminData.Active)
        {
            shell.WriteError(Loc.GetString("force-admin-already-deadmin", ("player", target.Name)));
            return;
        }

        AdminManager.DeAdmin(target);
        shell.WriteLine(Loc.GetString("force-admin-deadmin-success", ("player", target.Name)));
    }
}

[AdminCommand(AdminFlags.Permissions)]
internal sealed class ForceReAdminCommand : BaseForcedAdminCommand
{
    public override string Command => "forcereadmin";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length != 1)
        {
            shell.WriteError(Loc.GetString("shell-need-exactly-one-argument"));
            shell.WriteLine(Help);
            return;
        }

        if (!TryGetSession(args[0], out var target))
        {
            shell.WriteError(Loc.GetString("force-admin-player-not-found", ("player", args[0])));
            return;
        }

        var adminData = AdminManager.GetAdminData(target, includeDeAdmin: true);
        if (adminData == null)
        {
            shell.WriteError(Loc.GetString("force-admin-target-not-admin", ("player", target.Name)));
            return;
        }

        if (adminData.Active)
        {
            shell.WriteError(Loc.GetString("force-admin-already-readmin", ("player", target.Name)));
            return;
        }

        AdminManager.ReAdmin(target);
        shell.WriteLine(Loc.GetString("force-admin-readmin-success", ("player", target.Name)));
    }
}
