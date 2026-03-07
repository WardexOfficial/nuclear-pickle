// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

/*
 * This file is sublicensed under MIT License
 * https://github.com/space-wizards/space-station-14/blob/master/LICENSE.TXT
 */

using Content.Client.Eui;
using Content.Shared._Nuclear.Administration.ScreenCheck;
using Content.Shared.Eui;
using JetBrains.Annotations;

namespace Content.Client._Nuclear.Administration.ScreenCheck;

[UsedImplicitly]
public sealed class ScreenCheckEui : BaseEui
{
    private readonly ScreenCheckWindow _window;

    public ScreenCheckEui()
    {
        _window = new ScreenCheckWindow();
        _window.OnClose += () => SendMessage(new CloseEuiMessage());
    }

    public override void Opened()
    {
        base.Opened();
        _window.OpenCentered();
    }

    public override void Closed()
    {
        base.Closed();
        _window.Close();
        _window.Cleanup();
    }

    public override void HandleState(EuiStateBase state)
    {
        if (state is not ScreenCheckEuiState cast)
            return;

        _window.UpdateState(cast);
    }
}
