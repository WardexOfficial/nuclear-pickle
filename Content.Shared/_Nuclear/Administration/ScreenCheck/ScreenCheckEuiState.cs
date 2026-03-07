// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

/*
 * This file is sublicensed under MIT License
 * https://github.com/space-wizards/space-station-14/blob/master/LICENSE.TXT
 */

using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared._Nuclear.Administration.ScreenCheck;

[Serializable, NetSerializable]
public enum ScreenCheckUiStatus : byte
{
    Pending,
    Success,
    TimedOut,
    TargetDisconnected,
    CaptureFailed,
    InvalidData,
}

[Serializable, NetSerializable]
public sealed partial class ScreenCheckEuiState(string targetName, ScreenCheckUiStatus status, byte[] imageData) : EuiStateBase
{
    public readonly string TargetName = targetName;
    public readonly ScreenCheckUiStatus Status = status;
    public readonly byte[] ImageData = imageData;
}
