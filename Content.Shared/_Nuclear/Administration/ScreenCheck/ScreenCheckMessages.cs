// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

/*
 * This file is sublicensed under MIT License
 * https://github.com/space-wizards/space-station-14/blob/master/LICENSE.TXT
 */

using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using System.IO;

namespace Content.Shared._Nuclear.Administration.ScreenCheck;

public sealed class MsgScreenCheckRequest : NetMessage
{
    public override MsgGroups MsgGroup => MsgGroups.Command;

    public uint RequestId;

    public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
    {
        RequestId = buffer.ReadUInt32();
    }

    public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
    {
        buffer.Write(RequestId);
    }
}

public sealed class MsgScreenCheckResponse : NetMessage
{
    public override MsgGroups MsgGroup => MsgGroups.Command;

    public uint RequestId;
    public bool Success;
    public byte[]? ImageData;

    public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
    {
        RequestId = buffer.ReadUInt32();
        Success = buffer.ReadBoolean();

        if (!buffer.ReadBoolean())
        {
            ImageData = null;
            return;
        }

        var length = buffer.ReadVariableInt32();
        if (length <= 0 || length > ScreenCheckImageValidator.MaxImageBytes)
            throw new InvalidDataException($"Invalid screencheck image length: {length}.");

        ImageData = new byte[length];
        buffer.ReadBytes(ImageData, 0, length);
    }

    public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
    {
        buffer.Write(RequestId);
        buffer.Write(Success);
        buffer.Write(ImageData != null);

        if (ImageData == null)
            return;

        buffer.WriteVariableInt32(ImageData.Length);
        buffer.Write(ImageData);
    }
}
