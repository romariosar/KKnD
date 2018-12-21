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

using System.Drawing;

namespace OpenRA.Mods.Kknd.Widgets.Widgets
{
    public class Widget
    {
        public Rectangle Rectangle;
        public Color BackgroundColor;
        public int BorderWidth;
        public Color BorderColor;

        public Widget()
        {
            Rectangle = new Rectangle(new Point(100, 100), new Size(50, 50));
            BackgroundColor = Color.CornflowerBlue;
            BorderWidth = 5;
            BorderColor = Color.DarkRed;
        }

        public virtual void Draw()
        {
            Game.Renderer.RgbaColorRenderer.DrawRect(
                new float2(Rectangle.Left, Rectangle.Top),
                new float2(Rectangle.Right, Rectangle.Bottom),
                BorderWidth,
                BorderColor
            );
            
            Game.Renderer.RgbaColorRenderer.FillRect(
                new float2(Rectangle.Left + BorderWidth, Rectangle.Top + BorderWidth),
                new float2(Rectangle.Right - BorderWidth, Rectangle.Bottom - BorderWidth),
                BackgroundColor
            );
        }
    }
}