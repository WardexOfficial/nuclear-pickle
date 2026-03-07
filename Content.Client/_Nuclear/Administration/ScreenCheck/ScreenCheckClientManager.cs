// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

/*
 * This file is sublicensed under MIT License
 * https://github.com/space-wizards/space-station-14/blob/master/LICENSE.TXT
 */

using System.IO;
using System.Threading.Tasks;
using Content.Shared._Nuclear.Administration.ScreenCheck;
using Robust.Client.Graphics;
using Robust.Shared.Network;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Content.Client._Nuclear.Administration.ScreenCheck;

public sealed class ScreenCheckClientManager
{
    [Dependency] private readonly IClyde _clyde = default!;
    [Dependency] private readonly ILogManager _logs = default!;
    [Dependency] private readonly IClientNetManager _net = default!;

    private ISawmill _sawmill = default!;

    public void Initialize()
    {
        _sawmill = _logs.GetSawmill("screencheck");

        _net.RegisterNetMessage<MsgScreenCheckRequest>(OnRequestReceived);
        _net.RegisterNetMessage<MsgScreenCheckResponse>();
    }

    private void OnRequestReceived(MsgScreenCheckRequest message)
    {
        try
        {
            _clyde.Screenshot(ScreenshotType.Final, screenshot => _ = ProcessScreenshotAsync(message.RequestId, screenshot));
        }
        catch (Exception e)
        {
            _sawmill.Error("Failed to queue screencheck screenshot for request {RequestId}: {Error}", message.RequestId, e);
            SendFailure(message.RequestId);
        }
    }

    private async Task ProcessScreenshotAsync(uint requestId, Image<Rgb24> screenshot)
    {
        using (screenshot)
        {
            try
            {
                var imageData = await Task.Run(() => EncodeScreenshot(screenshot));
                _net.ClientSendMessage(new MsgScreenCheckResponse
                {
                    RequestId = requestId,
                    Success = true,
                    ImageData = imageData,
                });
            }
            catch (Exception e)
            {
                _sawmill.Error("Failed to encode screencheck screenshot for request {RequestId}: {Error}", requestId, e);
                SendFailure(requestId);
            }
        }
    }

    private static byte[] EncodeScreenshot(Image<Rgb24> screenshot)
    {
        if (!ScreenCheckImageValidator.IsAllowedDimensions(screenshot.Width, screenshot.Height))
            throw new InvalidOperationException($"Screencheck screenshot dimensions exceed limit: {screenshot.Width}x{screenshot.Height}.");

        using var stream = new MemoryStream();
        screenshot.SaveAsJpeg(stream);

        if (stream.Length <= 0 || stream.Length > ScreenCheckImageValidator.MaxImageBytes)
            throw new InvalidOperationException($"Screencheck screenshot size exceeds limit: {stream.Length} bytes.");

        return stream.ToArray();
    }

    private void SendFailure(uint requestId)
    {
        _net.ClientSendMessage(new MsgScreenCheckResponse
        {
            RequestId = requestId,
            Success = false,
        });
    }
}
