// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

/*
 * This file is sublicensed under MIT License
 * https://github.com/space-wizards/space-station-14/blob/master/LICENSE.TXT
 */

using Content.Server.EUI;
using Content.Shared._Nuclear.Administration.ScreenCheck;
using Content.Shared.Eui;

namespace Content.Server._Nuclear.Administration.ScreenCheck;

public sealed class ScreenCheckEui : BaseEui
{
    private readonly uint _requestId;
    private readonly ScreenCheckManager _manager;
    private ScreenCheckEuiState _state;

    public ScreenCheckEui(uint requestId, string targetName, ScreenCheckManager manager)
    {
        _requestId = requestId;
        _manager = manager;
        _state = new ScreenCheckEuiState(targetName, ScreenCheckUiStatus.Pending, Array.Empty<byte>());
    }

    public override EuiStateBase GetNewState()
    {
        return _state;
    }

    public void SetState(ScreenCheckUiStatus status, byte[]? imageData = null)
    {
        _state = new ScreenCheckEuiState(_state.TargetName, status, imageData ?? Array.Empty<byte>());

        if (Id != 0 && !IsShutDown)
            StateDirty();
    }

    public override void Closed()
    {
        base.Closed();
        _manager.OnEuiClosed(_requestId);
    }
}
