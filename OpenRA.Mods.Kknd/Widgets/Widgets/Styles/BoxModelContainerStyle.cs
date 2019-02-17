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

namespace OpenRA.Mods.Kknd.Widgets.Widgets.Styles
{
    public class BoxModelContainerStyle : IContainerStyle
    {
        private Rectangle marginBounds;
        private Rectangle borderBounds;
        private Rectangle paddingBounds;
        private Rectangle contentBounds;

        public Color? BackgroundColor;
        public Color? BorderColor;

        public Int4 Margin;
        public Int4 Border;
        public Int4 Padding;

        public void Draw()
        {
            if (BorderColor.HasValue)
            {
                if (Border.Top > 0)
                    Game.Renderer.RgbaColorRenderer.DrawLine(
                        new float2(borderBounds.Left, borderBounds.Top + Border.Top / 2f),
                        new float2(borderBounds.Right, borderBounds.Top + Border.Top / 2f),
                        Border.Top,
                        BorderColor.Value
                    );

                if (Border.Right > 0)
                    Game.Renderer.RgbaColorRenderer.DrawLine(
                        new float2(borderBounds.Right - Border.Right / 2f, borderBounds.Top),
                        new float2(borderBounds.Right - Border.Right / 2f,borderBounds.Bottom),
                        Border.Right,
                        BorderColor.Value
                    );

                if (Border.Bottom > 0)
                    Game.Renderer.RgbaColorRenderer.DrawLine(
                        new float2(borderBounds.Left,borderBounds.Bottom - Border.Bottom / 2f),
                        new float2(borderBounds.Right,borderBounds.Bottom - Border.Bottom / 2f),
                        Border.Bottom,
                        BorderColor.Value
                    );

                if (Border.Left > 0)
                    Game.Renderer.RgbaColorRenderer.DrawLine(
                        new float2(borderBounds.Left + Border.Left / 2f, borderBounds.Top),
                        new float2(borderBounds.Left + Border.Left / 2f,borderBounds.Bottom),
                        Border.Left,
                        BorderColor.Value
                    );
            }

            if (BackgroundColor.HasValue)
                Game.Renderer.RgbaColorRenderer.FillRect(
                    new float2(paddingBounds.Left, paddingBounds.Top),
                    new float2(paddingBounds.Right, paddingBounds.Bottom),
                    BackgroundColor.Value
                );
        }

        public void Resize(Rectangle bounds)
        {
            marginBounds = new Rectangle(bounds.Location, bounds.Size);
            borderBounds = new Rectangle(
                marginBounds.Location + new Size(Margin.Left, Margin.Top),
                marginBounds.Size - new Size(Margin.Left + Margin.Right, Margin.Top + Margin.Bottom)
            );
            paddingBounds = new Rectangle(
                borderBounds.Location + new Size(Border.Left, Border.Top),
                borderBounds.Size - new Size(Border.Left + Border.Right, Border.Top + Border.Bottom)
            );
            contentBounds = new Rectangle(
                paddingBounds.Location + new Size(Padding.Left, Padding.Top),
                paddingBounds.Size - new Size(Padding.Left + Padding.Right, Padding.Top + Padding.Bottom)
            );
        }

        public Rectangle GetContentBounds(Rectangle bounds)
        {
            return new Rectangle(contentBounds.Location, contentBounds.Size);
        }
    }
}