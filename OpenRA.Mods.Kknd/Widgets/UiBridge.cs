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

using System.Collections.Generic;
using System.Drawing;
using OpenRA.Graphics;
using OpenRA.Mods.Kknd.Widgets.Widgets;
using OpenRA.Mods.Kknd.Widgets.Widgets.Layouts;
using OpenRA.Mods.Kknd.Widgets.Widgets.Styles;
using OpenRA.Mods.Kknd.Widgets.Widgets.Widgets;

namespace OpenRA.Mods.Kknd.Widgets
{
    public class UiBridge : OpenRA.Widgets.Widget
    {
        private ContainerWidget NewUiRoot;

        public Dictionary<string, Dictionary<int, SpriteFont>> fonts = new Dictionary<string, Dictionary<int, SpriteFont>>();

        public static UiBridge instance;

        public UiBridge()
        {
            instance = this;

            NewUiRoot = new ContainerWidget
            {
                ContainerStyle = new BoxModelContainerStyle
                {
                    BackgroundColor = Color.Red,
                    BorderColor = Color.HotPink,
                    Border = new Int4(30, 20, 10, 0),
                    Margin = new Int4(50),
                    Padding = new Int4(10)
                }
            };

            var topLeft = new ContainerWidget
            {
                ContainerStyle = new BoxModelContainerStyle
                {
                    BackgroundColor = Color.Blue
                }
            };

            var bottomRight = new ContainerWidget
            {
                ContainerStyle = new BoxModelContainerStyle
                {
                    BackgroundColor = Color.Green
                }
            };

            var centerText = new LabelWidget
            {
                Text = "You're talking to me?",
                TextStyle = new TrueTypeTextStyle
                {
                    Color = Color.CornflowerBlue,
                    Font = "Rajdhani-SemiBold",
                    Size = 48
                }
            };

            var layout = new GridLayout(new int?[] {250, null, 250}, new int?[] {250, null, 250});
            layout.Add(topLeft, new Rectangle(0, 0, 1, 1));
            layout.Add(bottomRight, new Rectangle(2, 2, 1, 1));
            layout.Add(centerText, new Rectangle(1, 1, 1, 1));
            NewUiRoot.Layout = layout;

            NewUiRoot.Resize(new Rectangle(Point.Empty, Game.Renderer.Resolution));
        }

        public override void Draw()
        {
            NewUiRoot.Draw();
        }

        public SpriteFont GetFont(string fontFamily, int fontSize)
        {
            if (!fonts.ContainsKey(fontFamily))
                fonts.Add(fontFamily, new Dictionary<int, SpriteFont>());

            if (!fonts[fontFamily].ContainsKey(fontSize))
            {
                fonts[fontFamily][fontSize] = new SpriteFont(
                    fontFamily,
                    Game.ModData.DefaultFileSystem.Open("kknd|assets/" + fontFamily + ".ttf").ReadAllBytes(),
                    fontSize,
                    1,
                    new SheetBuilder(SheetType.BGRA, 512)
                );
            }

            return fonts[fontFamily][fontSize];
        }


        /*private int currentMobdId = 0;
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