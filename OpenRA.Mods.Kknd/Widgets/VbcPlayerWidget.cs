#region Copyright & License Information

/*
 * Copyright 2016-2019 The KKnD Developers (see AUTHORS)
 * This file is part of KKnD, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 3 of
 * the License, or (at your option) any later version. For more
 * information, see COPYING.
 */

#endregion

using System;
using System.Drawing;
using System.IO;
using System.Linq;
using OpenRA.Graphics;
using OpenRA.Mods.Kknd.FileFormats;
using OpenRA.Widgets;

// TODO refactor this to use the new ui system as soon as its usable! :)
namespace OpenRA.Mods.Kknd.Widgets
{
    public class VbcPlayerWidget : Widget
    {
        private readonly Vbc video;
        private readonly bool fullscreen;
        private readonly Action onComplete;
        private readonly Action<string> onText;

        private readonly Sprite sprite;
        private readonly uint[] palette = new uint[256];

        private byte[] frameData;
        private int currentFrame;
        private long currentFrameStart;

        public VbcPlayerWidget(Vbc video, bool fullscreen, Action<string> onText, Action onComplete)
        {
            this.video = video;
            this.fullscreen = fullscreen;
            this.onText = onText;
            this.onComplete = onComplete;

            sprite = new Sprite(
                new Sheet(SheetType.BGRA, new Size(video.Size.X, video.Size.Y)),
                new Rectangle(0, 0, video.Size.X, video.Size.Y),
                TextureChannel.RGBA
            );

            sprite.Sheet.GetTexture().ScaleFilter = TextureScaleFilter.Linear;
            frameData = new byte[video.Size.X * video.Size.Y];

            // TODO see aud stream. Make VBC an audio stream, and use draw to keepup to current frame.
            // Playing audio per frame results in choppy sound. So play everything at once.
            var audio = new MemoryStream();
            foreach (var frame in video.Frames.Where(f => f.Audio != null))
                audio.WriteArray(frame.Audio);

            Game.Sound.PlayVideo(audio.ToArray(), 1, video.SampleBits, video.SampleRate);
        }

        public override bool HandleKeyPress(KeyInput keyInput)
        {
            if (keyInput.Key != Keycode.ESCAPE)
                return false;

            Game.Sound.StopVideo();
            onComplete();
            return true;
        }

        private void UpdateFrame()
        {
            if (currentFrame == 0 && currentFrameStart == 0)
                currentFrameStart = Game.RunTime;
            else
            {
                if (currentFrameStart + video.Frames[currentFrame].Duration >= Game.RunTime)
                    return;

                if (currentFrame + 1 == video.NumFrames)
                {
                    onComplete();
                    return;
                }

                currentFrameStart = currentFrameStart + video.Frames[currentFrame].Duration;
                currentFrame++;
            }

            var frame = video.Frames[currentFrame];

            frameData = frame.GetFrame(frameData, palette, video.Size);
            var data = new uint[sprite.Sheet.Size.Height, sprite.Sheet.Size.Width];

            for (var i = 0; i < frameData.Length; i++)
                data[i / video.Size.X, i % video.Size.X] = palette[frameData[i]];

            sprite.Sheet.GetTexture().SetData(data);

            /*if (frame.Audio != null)
                Game.Sound.PlayVideo(frame.Audio, 1, video.SampleBits, video.SampleRate);*/

            if (frame.Text != null)
                onText(frame.Text);
        }

        public override void Draw()
        {
            UpdateFrame();

            var resolution = Game.Renderer.Resolution;
            var area = fullscreen ? new Rectangle(0, 0, resolution.Width, resolution.Height) : Parent.RenderBounds;
            var scale = Math.Min(area.Width / (float) video.Size.X, area.Height / (float) (video.Size.Y * 2));
            var size = new float2(video.Size.X * scale, video.Size.Y * 2 * scale);
            var offset = new float2((area.Width - size.X) / 2, (area.Height - size.Y) / 2);

            Game.Renderer.RgbaSpriteRenderer.DrawSprite(sprite, new float2(area.X + offset.X, area.Y + offset.Y), size);
        }
    }
}