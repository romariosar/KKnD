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
using OpenRA.Primitives;
using Version = OpenRA.Mods.Kknd.FileSystem.Version;

namespace OpenRA.Mods.Kknd.FileFormats
{
    public class Mobd
    {
        public readonly MobdAnimation[] directionalAnimations;
        public readonly MobdAnimation[] staticAnimations;

        public Mobd(SegmentStream stream, Version version)
        {
            var fileOffset = (uint) stream.BaseOffset;
            var firstFrameStart = stream.Length;
            var justReadFrameOffset = false;

            var animationOffsets = new List<uint>();
            var animations = new List<MobdAnimation>();
            var hardcodedAnimations = new List<MobdAnimation>();

            while (stream.Position < firstFrameStart)
            {
                var value = stream.ReadUInt32();

                // This parsing method is trash, because animation offsets are hardcoded in .exe but it seems to work.
                if ((value == 0xffffffff || value == 0x00000000) && justReadFrameOffset)
                {
                    // terminator
                    justReadFrameOffset = false;
                    // TODO that justReadFrameOffset thing is weird! figure that out!
                    // TODO when is terminator 0 and when -1?
                }
                else if (value - fileOffset > stream.Position && value - fileOffset < stream.Length)
                {
                    // frame
                    justReadFrameOffset = true;
                    firstFrameStart = Math.Min(firstFrameStart, value - fileOffset);
                    // TODO why do we need that "if" here? SHouldnt it always be unk1, [frames], terminator ?
                }
                else if (value - fileOffset < stream.Position && value >= fileOffset)
                {
                    // animation pointer
                    animationOffsets.Remove(value - fileOffset);
                    var returnPosition = stream.Position;
                    stream.Position = value - fileOffset;
                    animations.Add(new MobdAnimation(stream, version));
                    stream.Position = returnPosition;
                }
                else if (value == 0)
                {
                    Log.Write("debug", "MOBD FILLER");
                    // TODO filler ? Sprite Invisible? Does this happen at all?! Maybe the block ends here?
                }
                else
                    animationOffsets.Add((uint) (stream.Position - 4));
                // TODO when does the last one fire?
            }

            foreach (var animationOffset in animationOffsets)
            {
                try
                {
                    stream.Position = animationOffset;
                    hardcodedAnimations.Add(new MobdAnimation(stream, Version.KKND2));
                }
                catch (Exception)
                {
                    Log.Write("debug", "MOBD CRASH");
                    // TODO crashes on kknd2, fix me!
                }
            }

            directionalAnimations = animations.ToArray();
            staticAnimations = hardcodedAnimations.ToArray();
        }
    }
}