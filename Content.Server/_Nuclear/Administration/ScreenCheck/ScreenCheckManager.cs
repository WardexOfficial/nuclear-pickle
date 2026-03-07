// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

/*
 * This file is sublicensed under MIT License
 * https://github.com/space-wizards/space-station-14/blob/master/LICENSE.TXT
 */

using Content.Server.EUI;
using Content.Shared._Nuclear.Administration.ScreenCheck;
using Robust.Server.Player;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Server._Nuclear.Administration.ScreenCheck;

public sealed class ScreenCheckManager
{
    private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(10);

    [Dependency] private readonly EuiManager _euis = default!;
    [Dependency] private readonly ILogManager _logs = default!;
    [Dependency] private readonly IPlayerManager _players = default!;
    [Dependency] private readonly IServerNetManager _net = default!;

    private readonly Dictionary<uint, PendingScreenCheck> _pendingChecks = new();
    private ISawmill _sawmill = default!;
    private uint _nextRequestId;

    private sealed record PendingScreenCheck(NetUserId AdminUserId, NetUserId TargetUserId, ScreenCheckEui Ui);

    public void Initialize()
    {
        _sawmill = _logs.GetSawmill("screencheck");

        _net.RegisterNetMessage<MsgScreenCheckRequest>();
        _net.RegisterNetMessage<MsgScreenCheckResponse>(OnScreenCheckResponse);
        _net.Disconnect += OnDisconnect;
    }

    public void StartScreenCheck(ICommonSession admin, ICommonSession target)
    {
        var requestId = unchecked(++_nextRequestId);
        if (requestId == 0)
            requestId = unchecked(++_nextRequestId);

        var ui = new ScreenCheckEui(requestId, target.Name, this);
        _euis.OpenEui(ui, admin);
        ui.StateDirty();

        _pendingChecks[requestId] = new PendingScreenCheck(admin.UserId, target.UserId, ui);
        _net.ServerSendMessage(new MsgScreenCheckRequest { RequestId = requestId }, target.Channel);

        Timer.Spawn(RequestTimeout, () => OnTimeout(requestId));
    }

    public void OnEuiClosed(uint requestId)
    {
        _pendingChecks.Remove(requestId);
    }

    private void OnScreenCheckResponse(MsgScreenCheckResponse message)
    {
        if (!_pendingChecks.TryGetValue(message.RequestId, out var pending))
            return;

        if (pending.TargetUserId != message.MsgChannel.UserId)
        {
            _sawmill.Warning(
                "Received screencheck response for request {RequestId} from unexpected user {UserId}.",
                message.RequestId,
                message.MsgChannel.UserId);
            return;
        }

        _pendingChecks.Remove(message.RequestId);

        if (pending.Ui.IsShutDown)
            return;

        if (!message.Success)
        {
            pending.Ui.SetState(ScreenCheckUiStatus.CaptureFailed);
            return;
        }

        if (!ScreenCheckImageValidator.IsValidEncodedJpeg(message.ImageData))
        {
            pending.Ui.SetState(ScreenCheckUiStatus.InvalidData);
            return;
        }

        var imageData = message.ImageData!;
        pending.Ui.SetState(ScreenCheckUiStatus.Success, imageData);
    }

    private void OnTimeout(uint requestId)
    {
        if (!_pendingChecks.Remove(requestId, out var pending) || pending.Ui.IsShutDown)
            return;

        pending.Ui.SetState(ScreenCheckUiStatus.TimedOut);
    }

    private void OnDisconnect(object? sender, NetDisconnectedArgs args)
    {
        List<(uint RequestId, PendingScreenCheck Pending, ScreenCheckUiStatus? Status)>? completed = null;

        foreach (var (requestId, pending) in _pendingChecks)
        {
            if (pending.AdminUserId == args.Channel.UserId)
            {
                completed ??= new();
                completed.Add((requestId, pending, null));
                continue;
            }

            if (pending.TargetUserId == args.Channel.UserId)
            {
                completed ??= new();
                completed.Add((requestId, pending, ScreenCheckUiStatus.TargetDisconnected));
            }
        }

        if (completed == null)
            return;

        foreach (var (requestId, pending, status) in completed)
        {
            _pendingChecks.Remove(requestId);

            if (status == null || pending.Ui.IsShutDown)
                continue;

            pending.Ui.SetState(status.Value);
        }
    }
}
