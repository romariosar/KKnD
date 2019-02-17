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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace OpenRA.Mods.Kknd.Widgets.Widgets.Layouts
{
    public class GridLayout : ILayout
    {
        private int?[] columns;
        private int minWidth;
        private int fillColumns;
        private int?[] rows;
        private int minHeight;
        private int fillRows;

        private Dictionary<Widget, Rectangle> children = new Dictionary<Widget, Rectangle>();

        public IEnumerable<Widget> Children
        {
            get { return children.Keys; }
        }

        public GridLayout(int?[] columns, int?[] rows)
        {
            this.columns = columns;
            minWidth = columns.Where(width => width.HasValue).Sum(width => width.Value);
            fillColumns = columns.Count(width => !width.HasValue);
            this.rows = rows;
            minHeight = columns.Where(height => height.HasValue).Sum(height => height.Value);
            fillRows = columns.Count(height => !height.HasValue);
        }

        public void Add(Widget widget, Rectangle position)
        {
            if (position.Width < 1 || position.Height < 1)
                throw new Exception("Invalid size!");

            if (position.X < 0 || position.X + position.Width > columns.Length)
                throw new Exception("Out of bounds!");

            if (position.Y < 0 || position.Y + position.Height > rows.Length)
                throw new Exception("Out of bounds!");

            if (children.ContainsKey(widget))
                throw new Exception("Widget already a child!");

            foreach (var bounds in children.Values)
                if (bounds.IntersectsWith(position))
                    throw new Exception("Grid area not empty!");

            children.Add(widget, new Rectangle(position.Location, position.Size));
        }

        public void AlignChildren(Rectangle contentBounds)
        {
            var autoColumnWidth = (contentBounds.Width - minWidth) / fillColumns;
            var autoRowHeight = (contentBounds.Height - minHeight) / fillRows;

            foreach (var entry in children)
            {
                var x = columns.Take(entry.Value.X).Sum(width => width ?? autoColumnWidth);
                var y = rows.Take(entry.Value.Y).Sum(height => height ?? autoRowHeight);
                var w = columns.Skip(entry.Value.X).Take(entry.Value.Width).Sum(width => width ?? autoColumnWidth);
                var h = rows.Skip(entry.Value.Y).Take(entry.Value.Height).Sum(height => height ?? autoRowHeight);
                entry.Key.Resize(new Rectangle(contentBounds.X + x, contentBounds.Y + y, w, h));
            }
        }

        public void Draw()
        {
            foreach (var child in children.Keys)
                child.Draw();
        }
    }
}