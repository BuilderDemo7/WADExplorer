using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

namespace WADExplorer
{
    public class Package
    {
        /// <summary>
        /// [WADH] - The magic value to each .WAD file.
        /// </summary>
        public static readonly int Magic = 0x48444157;

        /// <summary>
        /// The stream this package is from, otherwise null if the stream does not exist yet.
        /// </summary>
        public Stream StreamPackage;

        public string FileName;

        private BinaryReader f;

        /// <summary>
        /// The base offset to each item's data.
        /// </summary>
        public uint BaseOffset;

        /// <summary>
        /// The base offset to each item's name.
        /// </summary>
        public uint NamesOffset;

        /// <summary>
        /// The buffer size of the names of the package.
        /// </summary>
        public uint NamesSize;

        public List<InsideItem> Items;

        public string GetItemName(int index)
        {
            InsideItem item = Items[index];
            if (index < 1)
            {
                item.Name = "";
                return "(Root)";
            }

            if (item.Name != null)
                return item.Name;

            string name = "";
            string judgeName = "\0";
            using (var f = new BinaryReader(StreamPackage, Encoding.UTF8, true)) {
                StreamPackage.Position = NamesOffset+item.NameOffset;
                judgeName = Encoding.UTF8.GetString(f.ReadBytes(255));
            }
            int magia = judgeName.IndexOf("\0", 0);
            if (magia > 0)
            {
                name = judgeName.Substring(0, magia);
            }
            item.Name = name; // update name

            return name;
        }
        public byte[] GetItemBuffer(int index)
        {
            if (StreamPackage == null)
                throw new NullReferenceException("The Package stream returned null, closed or unavailable.");
            InsideItem item = Items[index];
            byte[] buffer = new byte[item.Size];

            if (item.Buffer == null)
            {
                int off = (int)(BaseOffset + item.Offset);
                StreamPackage.Position = off;
                for (int byteI = 0; byteI < item.Size; byteI++)
                {
                    buffer[byteI] = f.ReadByte();
                }
                item.Buffer = buffer; // update buffer
            }
            else
            {
                buffer = item.Buffer; // simple :)
            }
            return buffer;
        }
        public static string GetItemFullPath(InsideItem item, string seperator = "/")
        {
            string fullpath = item.Name;
            InsideItem lastItem = item.Parent;
            while (lastItem!=null && lastItem.Index!=0)
            {
                fullpath = lastItem.Name + seperator + fullpath;
                lastItem = lastItem.Parent;
            }
            return fullpath;
        }
        public static string GetItemDirectory(InsideItem item, string seperator = "/")
        {
            string fullpath = "";
            bool first = true;
            InsideItem lastItem = item.Parent;
            while (lastItem != null && lastItem.Index != 0)
            {
                fullpath = lastItem.Name + ( first ? "" : seperator ) + fullpath;
                lastItem = lastItem.Parent;
                first = false;
            }
            return fullpath;
        }

        public virtual void ParseParents()
        {
            foreach (InsideItem item in Items)
            {
                if (item.IsFolder)
                {
                    item.Children = new List<InsideItem>();
                    int id = item.Unk2;

                    if (id != -1)
                    {
                        while (id != -1)
                        {
                            item.Children.Add(Items[id]);
                            Items[id].Parent = item;
                            if (id == Items[id].Unk3)
                                break;

                            id = Items[id].Unk3;
                        }
                    }
                    /*
                    for (int id = item.Unk2; id < Items.Count; id++)
                    {
                        // add if parent corresponds to this
                        if (Items[id].ParentId == item.Index)
                        {
                            item.Children.Add(Items[id]);
                        }
                    }
                    */
                }
            }
        }
        public virtual void RecastParentChainsForItem(InsideItem item, bool loopChain = false)
        {
            if (!item.IsFolder)
                return;

            item.Unk2 = item.Children[0].Index;
            int idx = 0;
            foreach (InsideItem child in item.Children)
            {
                if (idx != item.Children.Count-1)
                {
                    child.Unk3 = item.Children[idx + 1].Index;
                    if (child.IsFolder & loopChain)
                        RecastParentChainsForItem(child);

                    idx++;
                }
                else
                {
                    child.Unk3 = -1;
                    break;
                }
            }
        }
        public static int GetNextLineOf(int from)
        {
            return 16 * ((from / 16) + 1);
        }
        public virtual void Load(Stream s)
        {
            if (s!=null)
                StreamPackage = s;

            f = new BinaryReader(s, Encoding.UTF8, true);
                s.Position += 4; // Skip magic

                BaseOffset = f.ReadUInt32();
                uint NumberOfItems = f.ReadUInt32();
                NamesSize = f.ReadUInt32();

                Items = new List<InsideItem>();
            for (int itemId = 0; itemId < NumberOfItems; itemId++)
            {
                InsideItem item = new InsideItem(
                    false, // Is Folder
                    f.ReadInt32(), // Offset to Name
                    f.ReadInt32(), // UID
                    f.ReadUInt32(), // Offset
                    f.ReadUInt32(), // Total Size
                    f.ReadUInt32(),  // Size

                    // Unknown
                    f.ReadInt32(),
                    f.ReadInt32(),
                    f.ReadInt32()
                );
                // read buffer
                var old = StreamPackage.Position; // return to value

                int off = (int)(BaseOffset + item.Offset);
                item.Buffer = new byte[item.Size];
                StreamPackage.Position = off;
                for (int byteI = 0; byteI < item.Size; byteI++)
                {
                    item.Buffer[byteI] = f.ReadByte();
                }
                /*
                StreamPackage.Position = old;

                // name cache
                old = StreamPackage.Position; // return to value
                int o = (int)(NamesOffset + item.NameOffset);
                StreamPackage.Position = o;

                string name = "";
                string judgeName = "\0";
                using (var f = new BinaryReader(StreamPackage, Encoding.UTF8, true))
                {
                    judgeName = Encoding.UTF8.GetString(f.ReadBytes(255));
                }
                int magia = judgeName.IndexOf("\0", 0);
                if (magia > 0)
                {
                    name = judgeName.Substring(0, magia);
                }
                item.Name = name; // update name
                */
                // indexation
                // etc

                item.Index = itemId;
                Items.Add(item);
                StreamPackage.Position = old; // return to
            }
            NamesOffset = (uint) s.Position;
            // process parents
            ParseParents();
        }

        public virtual byte[] RegenerateAndReturnBuffer(bool saveInNewFormat = true)
        {
            if (saveInNewFormat==false)
            {
                PackageOld oldFormatPackage = new PackageOld();
                oldFormatPackage.Items = Items;
                oldFormatPackage.BaseOffset = BaseOffset;
                return oldFormatPackage.RegenerateAndReturnBuffer(false);
            }
            int bufferSize = 0x10;
            int stringBufferSize = 0;

            foreach (InsideItem item in Items)
            {
                bufferSize += 0x20;
            }
            //int stringOffset = bufferSize;

            // strings size etc.
            foreach (InsideItem item in Items)
            {
                if (item.Name == "")
                    continue;

                bufferSize += item.Name.Length + 1;
            }

            // base offset
            BaseOffset = (uint)bufferSize;


            // AGAIN... but calculate size from the buffers...
            foreach (InsideItem item in Items)
            {
                bufferSize += (int)item.Buffer.Length;
            }

            MemoryStream dataStream = new MemoryStream();
            dataStream.SetLength(bufferSize);

            // regenerate parent chain
            RecastParentChainsForItem(Items[0], true);
            using (var f = new BinaryWriter(dataStream, Encoding.UTF8, false))
            {
                f.Write(Magic);
                f.Write(BaseOffset);
                f.Write(Items.Count);
                f.Write(stringBufferSize);

                // regenerate names offsets before everything
                int noff = 0;
                foreach (InsideItem item in Items)
                {
                    if (item.Name == "")
                        continue;

                    item.NameOffset = noff;
                    noff += item.Name.Length+1;
                }
                
                // write most data
                int lastOffset = 0;
                foreach (InsideItem item in Items)
                {
                    f.Write(item.NameOffset);

                    f.Write(item.CRC);
                    if (item.Offset != 0)
                    {
                        item.Offset = (uint)lastOffset;
                    }
                    f.Write(item.Offset);
                    f.Write(item.Buffer.Length);
                    f.Write(item.Buffer.Length);

                    f.Write(item.ParentId);

                    f.Write(item.Unk2);
                    f.Write(item.Unk3);

                    lastOffset += (int)item.Buffer.Length;
                }
                // write names
                foreach (InsideItem item in Items)
                {
                    if (item.Name == "")
                        continue;

                    f.Write(Encoding.UTF8.GetBytes(item.Name));
                    f.Write((byte)(0));
                }
                // and finally... write buffer data
                foreach (InsideItem item in Items)
                {
                    f.Write(item.Buffer);
                }
            }

            return dataStream.GetBuffer();
        }

        public virtual void Dispose()
        {
            StreamPackage.Dispose();
            f.Dispose();
        }

        public Package() { }
        public Package(string filename) {
            FileStream fileS = new FileStream(filename, FileMode.Open, FileAccess.Read);
            Load(fileS);
            FileName = filename;
        }
    }
    // Seems to be mostly used in old version of DDI games
    // Such as MT: AE, Rig Racer 2, etc...
    
    // The file header's size is lowered to 24 bytes and the file headers are slightly changed.
    public class PackageOld : Package
    {
        private BinaryReader f;

        public override void Load(Stream s)
        {
            if (s != null)
                StreamPackage = s;

            f = new BinaryReader(s, Encoding.UTF8, true);
            s.Position += 4; // Skip magic

            BaseOffset = f.ReadUInt32();
            uint NumberOfItems = f.ReadUInt32();
            NamesSize = f.ReadUInt32();

            Items = new List<InsideItem>();
            for (int itemId = 0; itemId < NumberOfItems; itemId++)
            {
                InsideItem item = new InsideItem(
                    false, // Is Folder
                    f.ReadInt32(), // Offset to Name
                    0, // CRC is removed
                    f.ReadUInt32(), // Offset
                    0, // Total Size is also removed
                    f.ReadUInt32(),  // Size

                    // Unknown
                    f.ReadInt32(),
                    f.ReadInt32(),
                    f.ReadInt32()
                );
                // read buffer
                var old = StreamPackage.Position; // return to value

                int off = (int)(BaseOffset + item.Offset);
                item.Buffer = new byte[item.Size];
                StreamPackage.Position = off;
                for (int byteI = 0; byteI < item.Size; byteI++)
                {
                    item.Buffer[byteI] = f.ReadByte();
                }
                /*
                StreamPackage.Position = old;

                // name cache
                old = StreamPackage.Position; // return to value
                int o = (int)(NamesOffset + item.NameOffset);
                StreamPackage.Position = o;

                string name = "";
                string judgeName = "\0";
                using (var f = new BinaryReader(StreamPackage, Encoding.UTF8, true))
                {
                    judgeName = Encoding.UTF8.GetString(f.ReadBytes(255));
                }
                int magia = judgeName.IndexOf("\0", 0);
                if (magia > 0)
                {
                    name = judgeName.Substring(0, magia);
                }
                item.Name = name; // update name
                */
                // indexation
                // etc

                item.Index = itemId;
                Items.Add(item);
                StreamPackage.Position = old; // return to
            }
            NamesOffset = (uint)s.Position;
            // process parents
            ParseParents();
        }

        public virtual byte[] RegenerateAndReturnBuffer(bool saveInNewFormat = false)
        {
            if (saveInNewFormat)
            {
                return base.RegenerateAndReturnBuffer(true);
            }
            int bufferSize = 0x10;
            int stringBufferSize = 0;

            foreach (InsideItem item in Items)
            {
                bufferSize += 0x18;
            }
            //int stringOffset = bufferSize;

            // strings size etc.
            foreach (InsideItem item in Items)
            {
                if (item.Name == "")
                    continue;

                bufferSize += item.Name.Length + 1;
            }

            // base offset
            BaseOffset = (uint)bufferSize;


            // AGAIN... but calculate size from the buffers...
            foreach (InsideItem item in Items)
            {
                bufferSize += (int)item.Buffer.Length;
            }

            MemoryStream dataStream = new MemoryStream();
            dataStream.SetLength(bufferSize);

            // regenerate parent chain
            RecastParentChainsForItem(Items[0], true);
            using (var f = new BinaryWriter(dataStream, Encoding.UTF8, false))
            {
                f.Write(Magic);
                f.Write(BaseOffset);
                f.Write(Items.Count);
                f.Write(stringBufferSize);

                // regenerate names offsets before everything
                int noff = 0;
                foreach (InsideItem item in Items)
                {
                    if (item.Name == "")
                        continue;

                    item.NameOffset = noff;
                    noff += item.Name.Length + 1;
                }

                // write most data
                int lastOffset = 0;
                foreach (InsideItem item in Items)
                {
                    f.Write(item.NameOffset);

                    //f.Write(item.CRC);
                    if (item.Offset != 0)
                    {
                        item.Offset = (uint)lastOffset;
                    }
                    f.Write(item.Offset);
                    //f.Write(item.Buffer.Length);
                    f.Write(item.Buffer.Length);

                    f.Write(item.ParentId);

                    f.Write(item.Unk2);
                    f.Write(item.Unk3);

                    lastOffset += (int)item.Buffer.Length;
                }
                // write names
                foreach (InsideItem item in Items)
                {
                    if (item.Name == "")
                        continue;

                    f.Write(Encoding.UTF8.GetBytes(item.Name));
                    f.Write((byte)(0));
                }
                // and finally... write buffer data
                foreach (InsideItem item in Items)
                {
                    f.Write(item.Buffer);
                }
            }

            return dataStream.GetBuffer();
        }

        public override void Dispose()
        {
            StreamPackage.Dispose();
            f.Dispose();
        }

        public PackageOld() { }
        public PackageOld(string filename) : base(filename) { }
    }
}
