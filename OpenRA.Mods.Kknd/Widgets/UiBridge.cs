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

using OpenRA.Mods.Kknd.Widgets.Widgets;

namespace OpenRA.Mods.Kknd.Widgets
{
    public class UiBridge : OpenRA.Widgets.Widget
    {
        private Widget NewUiRoot;

        public UiBridge()
        {
            NewUiRoot = new Widget();
        }

        public override void Draw()
        {
            NewUiRoot.Draw();
        }


        /*private int currentMobdId = 422;
        private Mobd currentMobd;
        private Sprite currentSprite;

        private Color[] kknd1Palette;
        private uint[] palette;

        private uint maxX;
        private uint maxY;
        private int total;

        public UiBridge()
        {
            // TODO this class basically holds the whole new ui system and acts as a wrapper.

            kknd1Palette = new Png(Game.ModData.DefaultFileSystem.Open("kknd|assets/kknd1.png")).Palette.ToArray();
            SelectMobd();
        }

        private void SelectMobd()
        {
            var i = 0;

            if (currentMobd != null)
            {
                currentSprite.Sheet.Dispose();
            }
            
            currentMobd = null;
            palette = null;
            
            foreach (var package in Game.ModData.ModFiles.MountedPackages)
            foreach (var packageContent in package.Contents)
            {
                if (!packageContent.EndsWith(".mobd"))
                    continue;

                if (i++ != currentMobdId)
                    continue;

                currentMobd = new Mobd(package.GetStream(packageContent) as SegmentStream, ((LvlPackageLoader.LvlPackage) package).Version);
                UpdateSprite();
            }
        }

        public override bool HandleKeyPress(KeyInput keyInput)
        {
            if (keyInput.Event != KeyInputEvent.Down || keyInput.IsRepeat)
                return false;
            
            if (keyInput.Key == Keycode.A)
            {
                currentMobdId--;
                SelectMobd();
            }
            else if (keyInput.Key == Keycode.D)
            {
                currentMobdId++;
                SelectMobd();
            }
            else if (keyInput.Key == Keycode.W)
            {
                // TODO next SOUN
            }
            else if (keyInput.Key == Keycode.W)
            {
                // TODO prev SOUN
            }

            return true;
        }

        private void UpdateSprite()
        {
            maxX = 0;
            maxY = 0;
            total = 0;

            Prepare(currentMobd.directionalAnimations);
            Prepare(currentMobd.staticAnimations);

            if (total == 0)
                return;

            // TODO fix these!!!!
            if (currentMobdId == 384 || currentMobdId == 385)
                return;

            var cols = (int) Math.Sqrt(total);
            var rows = Math.Ceiling((float) total / cols);

            var sheet = new Sheet(SheetType.BGRA, new Size((int) (cols * maxX), (int) (rows * maxY)));
            var data = new uint[sheet.Size.Height, sheet.Size.Width];

            var current = 0;
            current = Draw(currentMobd.directionalAnimations, data, current, cols);
            Draw(currentMobd.staticAnimations, data, current, cols);

            sheet.GetTexture().SetData(data);
            currentSprite = new Sprite(sheet, new Rectangle(Point.Empty, sheet.Size), TextureChannel.RGBA);
        }

        private int Draw(IEnumerable<MobdAnimation> animations, uint[,] data, int current, int cols)
        {
            foreach (var animation in animations)
            foreach (var frame in animation.Frames)
            {
                var startX = current % cols * maxX;
                var startY = current / cols * maxY;

                for (var y = 0; y < frame.RenderFlags.Image.Height; y++)
                for (var x = 0; x < frame.RenderFlags.Image.Width; x++)
                {
                    var index = frame.RenderFlags.Image.Pixels[x + y * frame.RenderFlags.Image.Width];
                    data[startY + y, startX + x] = palette == null ? (uint) kknd1Palette[index].ToArgb() : palette[index];
                }

                current++;
            }

            return current;
        }

        private void Prepare(IEnumerable<MobdAnimation> animations)
        {
            foreach (var animation in animations)
            foreach (var frame in animation.Frames)
            {
                maxX = Math.Max(maxX, frame.RenderFlags.Image.Width);
                maxY = Math.Max(maxY, frame.RenderFlags.Image.Height);
                total++;

                if (frame.RenderFlags.Palette != null)
                    palette = frame.RenderFlags.Palette;
            }
        }

        public override void Draw()
        {
            if (currentMobd == null)
                return;

            Game.Renderer.RgbaSpriteRenderer.DrawSprite(currentSprite, new float2(
                (Game.Renderer.Resolution.Width - currentSprite.Size.X) / 2f,
                (Game.Renderer.Resolution.Height - currentSprite.Size.Y) / 2f
            ));
        }*/
    }
}