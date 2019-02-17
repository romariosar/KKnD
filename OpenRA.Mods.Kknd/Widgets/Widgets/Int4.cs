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

namespace OpenRA.Mods.Kknd.Widgets.Widgets
{
    public struct Int4
    {
        public int Top;
        public int Right;
        public int Bottom;
        public int Left;

        public Int4(int value)
        {
            Top = value;
            Right = value;
            Bottom = value;
            Left = value;
        }

        public Int4(int? top = null, int? right = null, int? bottom = null, int? left = null)
        {
            Top = Right = Bottom = Left = 0;
            Set(top, right, bottom, left);
        }

        public void Set(int? top = null, int? right = null, int? bottom = null, int? left = null)
        {
            Top = top ?? Top;
            Right = right ?? Right;
            Bottom = bottom ?? Bottom;
            Left = left ?? Left;
        }
    }
}