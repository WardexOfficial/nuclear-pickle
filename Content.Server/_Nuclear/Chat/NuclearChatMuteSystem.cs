// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Collections.Generic;
using Content.Server.Chat.Managers;
using Content.Shared.Administration.Managers;
using Content.Shared.Chat;
using Content.Shared._Nuclear.Chat;
using Robust.Shared.Network;

namespace Content.Server._Nuclear.Chat;

public sealed class NuclearChatMuteSystem : EntitySystem
{
    [Dependency] private readonly ISharedAdminManager _admin = default!;
    [Dependency] private readonly IChatManager _chat = default!;

    private readonly Dictionary<NetUserId, NuclearChatMuteFlags> _mutes = new();

    public override void Initialize()
    {
        SubscribeLocalEvent<NuclearChatSendAttemptEvent>(OnSendAttempt);
    }

    public NuclearChatMuteFlags GetFlags(NetUserId userId)
    {
        return _mutes.GetValueOrDefault(userId, NuclearChatMuteFlags.None);
    }

    public void SetFlags(NetUserId userId, NuclearChatMuteFlags flags, bool enabled)
    {
        var current = GetFlags(userId);
        var updated = enabled ? current | flags : current & ~flags;

        if (updated == NuclearChatMuteFlags.None)
        {
            _mutes.Remove(userId);
            return;
        }

        _mutes[userId] = updated;
    }

    private void OnSendAttempt(NuclearChatSendAttemptEvent args)
    {
        if (_admin.IsAdmin(args.Player))
            return;

        var muted = GetMutedFlags(args.Channel);
        if (muted == NuclearChatMuteFlags.None)
            return;

        if (!IsMuted(args.Player.UserId, muted))
            return;

        args.Cancel();

        _chat.DispatchServerMessage(args.Player,
            Loc.GetString("nchatmute-feedback-send-blocked",
                ("channel", GetChannelName(args.Channel))));
    }

    public bool IsMuted(NetUserId userId, NuclearChatMuteFlags flags)
    {
        if (flags == NuclearChatMuteFlags.None)
            return false;

        return (GetFlags(userId) & flags) != 0;
    }

    private static NuclearChatMuteFlags GetMutedFlags(ChatChannel channel)
    {
        if ((channel & ChatChannel.LOOC) != 0)
            return NuclearChatMuteFlags.Looc;

        if ((channel & ChatChannel.OOC) != 0)
            return NuclearChatMuteFlags.Ooc;

        if ((channel & ChatChannel.Dead) != 0)
            return NuclearChatMuteFlags.Dead;

        return NuclearChatMuteFlags.None;
    }

    private string GetChannelName(ChatChannel channel)
    {
        if ((channel & ChatChannel.LOOC) != 0)
            return Loc.GetString("nchatmute-channel-looc");

        if ((channel & ChatChannel.OOC) != 0)
            return Loc.GetString("nchatmute-channel-ooc");

        if ((channel & ChatChannel.Dead) != 0)
            return Loc.GetString("nchatmute-channel-dead");

        return channel.ToString();
    }
}
