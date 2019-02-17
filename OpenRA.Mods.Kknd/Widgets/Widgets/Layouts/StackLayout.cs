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

namespace OpenRA.Mods.Kknd.Widgets.Widgets.Layouts
{
    public class StackLayout : ILayout
    {
        private List<Widget> children = new List<Widget>();

        public IEnumerable<Widget> Children
        {
            get { return children.ToArray(); }
        }

        public StackLayout Add(Widget child)
        {
            children.Add(child);
            return this;
        }

        public void AlignChildren(Rectangle contentBounds)
        {
            foreach (var child in Children)
                child.Resize(contentBounds);
        }

        public void Draw()
        {
            foreach (var child in Children)
                child.Draw();
        }
    }
}