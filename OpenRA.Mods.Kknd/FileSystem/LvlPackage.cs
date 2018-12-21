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
using System.IO;
using System.Linq;
using OpenRA.FileSystem;
using OpenRA.Primitives;
using FS = OpenRA.FileSystem.FileSystem;

namespace OpenRA.Mods.Kknd.FileSystem
{
    public enum Version
    {
        KKND1,
        KKND2_ALPHA,
        KKND2,
        UNKNOWN
    }

    public class NonDisposingSegmentStream : SegmentStream
    {
        public NonDisposingSegmentStream(Stream stream, long offset, long count) : base(stream, offset, count)
        {
        }

        protected override void Dispose(bool disposing)
        {
            // TODO get rid of this class!
        }
    }

    public class LvlPackageLoader : IPackageLoader
    {
        public sealed class LvlPackage : IReadOnlyPackage
        {
            public string Name { get; private set; }
            public Version Version { get; private set; }

            public IEnumerable<string> Contents
            {
                get { return index.Keys; }
            }

            private readonly Dictionary<string, uint[]> index = new Dictionary<string, uint[]>();
            private readonly SegmentStream stream;

            public LvlPackage(SegmentStream stream, string name, Version version,
                System.Collections.Generic.IReadOnlyDictionary<string, MiniYaml> filenames)
            {
                Name = name;
                Version = version;
                this.stream = stream;

                var fileTypesOffset = stream.ReadUInt32();
                stream.Position = fileTypesOffset + 4;
                var firstOffset = stream.ReadUInt32();
                stream.Position = fileTypesOffset;

                while (stream.Position < stream.Length - 8)
                {
                    var fileType = stream.ReadASCII(4);
                    var fileTableOffset = stream.ReadUInt32();

                    var continueTypePosition = stream.Position;

                    stream.Position += 4;

                    var fileTableEndOffset = stream.ReadUInt32();
                    if (fileTableEndOffset == 0)
                        fileTableEndOffset = fileTypesOffset;

                    stream.Position = fileTableOffset;

                    for (var fileId = 0; stream.Position < fileTableEndOffset; fileId++)
                    {
                        var fileOffset = stream.ReadUInt32();
                        if (fileOffset == 0x00)
                            continue;

                        var continueFilePosition = stream.Position;

                        var fileEndOffset = (uint) 0;
                        while (fileEndOffset == 0)
                            fileEndOffset = stream.Position == fileTypesOffset ? firstOffset : stream.ReadUInt32();

                        stream.Position = continueFilePosition;

                        var fileName = fileId + "." + fileType.ToLower();
                        if (filenames.ContainsKey(fileName))
                            fileName = filenames[fileName].Value;

                        index.Add(fileName, new[] {fileOffset, fileEndOffset - fileOffset});
                    }

                    stream.Position = continueTypePosition;
                }
            }

            public IReadOnlyPackage OpenPackage(string filename, FS context)
            {
                // Not implemented
                return null;
            }

            public bool Contains(string filename)
            {
                return index.ContainsKey(filename);
            }

            public Stream GetStream(string filename)
            {
                uint[] entry;

                if (index.TryGetValue(filename, out entry))
                {
                    return new NonDisposingSegmentStream(stream, entry[0], entry[1]);
                }

                return null;
            }

            public void Dispose()
            {
                stream.Dispose();
            }
        }

        public bool TryParsePackage(Stream stream, string filename, FS context, out IReadOnlyPackage package)
        {
            SegmentStream data;
            Version version;

            if (filename.EndsWith(".lpk") || filename.EndsWith(".spk"))
            {
                var versionId = stream.ReadUInt32();

                if (versionId == 0x91)
                    version = Version.KKND2_ALPHA;
                else
                {
                    stream.ReadInt32(); // TODO unk. Possibly build related.
                    version = Version.KKND2;
                }

                var decompressedSize = int2.Swap(stream.ReadUInt32());
                var compressedSize = stream.ReadUInt32();
                data = Decompress(new SegmentStream(stream, stream.Position, compressedSize), decompressedSize);
            }
            else if (filename.EndsWith(".lvl") || filename.EndsWith(".slv"))
            {
                version = Version.KKND1;
                stream.ReadASCII(4); // DATA
                var dataSize = int2.Swap(stream.ReadUInt32());
                data = new SegmentStream(stream, 8, dataSize);
            }
            else
            {
                package = null;
                return false;
            }

            Stream lvlLookup;

            if (context.TryOpen(filename + ".yaml", out lvlLookup))
                package = new LvlPackage(data, filename, version,
                    MiniYaml.FromStream(lvlLookup).ToDictionary(x => x.Key, x => x.Value));
            else
                package = new LvlPackage(data, filename, version, new Dictionary<string, MiniYaml>());

            return true;
        }

        private static SegmentStream Decompress(Stream compressedStream, uint decompressedSize)
        {
            var decompressedStream = new MemoryStream(new byte[decompressedSize]);

            while (decompressedStream.Position < decompressedStream.Capacity)
            {
                var chunkDecompressedSize = compressedStream.ReadUInt32();
                var chunkCompressedSize = compressedStream.ReadUInt32();

                if (chunkCompressedSize == chunkDecompressedSize)
                    decompressedStream.WriteArray(compressedStream.ReadBytes((int) chunkCompressedSize));
                else
                {
                    var chunkEndOffset = compressedStream.Position + chunkCompressedSize;

                    while (compressedStream.Position < chunkEndOffset)
                    {
                        var bitmask = compressedStream.ReadBytes(2);

                        for (var i = 0; i < 16; i++)
                        {
                            if ((bitmask[i / 8] & 1 << i % 8) == 0)
                                decompressedStream.WriteArray(compressedStream.ReadBytes(1));
                            else
                            {
                                var metaBytes = compressedStream.ReadBytes(2);
                                var readSize = 1 + (metaBytes[0] & 0x000F);
                                var readOffset = (metaBytes[0] & 0x00F0) << 4 | metaBytes[1];
                                var substitutes = new byte[readSize];
                                var returnPosition = decompressedStream.Position;

                                for (var j = 0; j < readSize; j++)
                                {
                                    decompressedStream.Position = returnPosition - readOffset + j % readOffset;
                                    substitutes[j] = decompressedStream.ReadUInt8();
                                }

                                decompressedStream.Position = returnPosition;
                                decompressedStream.WriteArray(substitutes);
                            }

                            if (compressedStream.Position == chunkEndOffset)
                                break;
                        }
                    }
                }
            }

            return new SegmentStream(decompressedStream, 0, decompressedSize);
        }
    }
}