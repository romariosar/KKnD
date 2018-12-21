#region Copyright & License Information
/*
 * Copyright 2007-2019 The OpenRA Developers (see AUTHORS)
 * This file is part of OpenRA, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 3 of
 * the License, or (at your option) any later version. For more
 * information, see COPYING.
 */
#endregion

using System.Collections.Generic;
using System.Drawing;
using OpenRA.Graphics;
using OpenRA.Mods.Kknd.FileFormats;
using OpenRA.Mods.Kknd.Widgets;
using OpenRA.Widgets;

namespace OpenRA.Mods.Kknd.LoadScreens
{
    public class LoadScreen : ILoadScreen
    {
        private Sprite logo;

        public void Init(ModData modData, Dictionary<string, string> info)
        {
            using (var stream = modData.DefaultFileSystem.Open("assets/logo.png"))
            {
                var sheet = new Sheet(SheetType.BGRA, stream);
                logo = new Sprite(sheet, new Rectangle(0, 0, sheet.Size.Width, sheet.Size.Height), TextureChannel.RGBA);
            }
        }

        public bool BeforeLoad()
        {
            return true;
        }

        public void Display()
        {
            if (Game.Renderer == null)
                return;

            var center = new float3(
                (Game.Renderer.Resolution.Width - logo.Size.X) / 2,
                (Game.Renderer.Resolution.Height - logo.Size.Y) / 2,
                0);

            Game.Renderer.BeginFrame(int2.Zero, 1f);
            Game.Renderer.RgbaSpriteRenderer.DrawSprite(logo, center);
            Game.Renderer.EndFrame(new NullInputHandler());
        }

        public void StartGame(Arguments args)
        {
            Ui.Root = new VbcPlayerWidget(
                new Vbc(Game.ModData.ModFiles.Open("kknd|assets/intro.vbc")), 
                true, 
                text => {},
                () => Ui.Root = new UiBridge());
        }

        public void Dispose()
        {
            logo.Sheet.Dispose();
        }
    }
}
