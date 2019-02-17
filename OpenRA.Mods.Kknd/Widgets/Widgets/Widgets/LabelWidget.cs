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

namespace OpenRA.Mods.Kknd.Widgets.Widgets.Widgets
{
    public class LabelWidget : Widget
    {
        public ITextStyle TextStyle;
        public string Text;

        public override void Draw()
        {
            if (Text == null)
                return;
            
            if (TextStyle == null)
                throw new Exception("Textstyle not found!");
            
            TextStyle.Draw(Text);
        }

        public override void Resize(Rectangle renderArea)
        {
            base.Resize(renderArea);

            if (TextStyle != null)
                TextStyle.Resize(Bounds);
        }
    }
}