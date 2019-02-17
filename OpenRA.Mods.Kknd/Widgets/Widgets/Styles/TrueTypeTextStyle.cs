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
using OpenRA.Graphics;

namespace OpenRA.Mods.Kknd.Widgets.Widgets.Styles
{
    public class TrueTypeTextStyle : ITextStyle
    {
        private string font;
        private int size;
        private SpriteFont spriteFont;
        private Rectangle contentBounds;

        public Color Color;
        public string Font;
        public int Size;

        public void Draw(string text)
        {
            if (font != Font || size != Size)
            {
                spriteFont = UiBridge.instance.GetFont(Font, Size);
                font = Font;
                size = Size;
            }

            if (spriteFont == null)
                throw new Exception("Invalid font!");

            spriteFont.DrawText(text, new float2(contentBounds.X, contentBounds.Y), Color);
        }

        public void Resize(Rectangle bounds)
        {
            contentBounds = new Rectangle(bounds.Location, bounds.Size);
        }
    }
}