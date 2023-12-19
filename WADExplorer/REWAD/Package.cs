using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

namespace WADExplorer
{
    public class PackageLoadFileBufferFailureEventArgs : EventArgs
    {
        public Package SourcePackage;
        public InsideItem File;

        public PackageLoadFileBufferFailureEventArgs() { }
        public PackageLoadFileBufferFailureEventArgs(InsideItem file, Package sourcePackage) { File = file; SourcePackage = sourcePackage; }
    }
    public delegate void OnPackageLoadFileBufferFailure(PackageLoadFileBufferFailureEventArgs e);
    public class PackageFileLoadedEventArgs : EventArgs
    {
        public Package SourcePackage;
        public InsideItem File;
        public int ToLoad = 0;

        public PackageFileLoadedEventArgs() { }
        public PackageFileLoadedEventArgs(InsideItem file, Package sourcePackage, int toLoad = 0) { File = file; SourcePackage = sourcePackage; ToLoad = toLoad; }
    }
    public delegate void OnPackageFileLoaded(PackageFileLoadedEventArgs e);
    public class PackageDoneLoadingEventArgs : EventArgs
    {
        public Package SourcePackage;

        public PackageDoneLoadingEventArgs() { }
        public PackageDoneLoadingEventArgs(Package sourcePackage) { SourcePackage = sourcePackage; }
    }
    public delegate void OnPackageDoneLoading(PackageDoneLoadingEventArgs e);
    public class Package
    {
        // Events
        public event OnPackageLoadFileBufferFailure OnLoadFileBufferFail;
        public event OnPackageFileLoaded FileLoaded;
        public event OnPackageDoneLoading Loaded;

        protected void OnLoadFileFail(PackageLoadFileBufferFailureEventArgs e)
        {
            if (OnLoadFileBufferFail != null)
                OnLoadFileBufferFail(e);
        }

        protected void OnFileLoad(PackageFileLoadedEventArgs e)
        {
            if (FileLoaded != null)
                FileLoaded(e);
        }

        protected void DoneLoading(PackageDoneLoadingEventArgs e)
        {
            if (Loaded != null)
                Loaded(e);
        }

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
                    int id = item.FolderStartIndex;

                    if (id != -1)
                    {
                        while (id != -1)
                        {
                            item.Children.Add(Items[id]);
                            Items[id].Parent = item;
                            if (id == Items[id].FolderNextItemIndex)
                                break;

                            id = Items[id].FolderNextItemIndex;
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

            item.FolderStartIndex = item.Children[0].Index;

            if (item.Children.Count == 0)
                item.FolderStartIndex = -1;

            int idx = 0;
            foreach (InsideItem child in item.Children)
            {
                child.Priority = item.Priority;
                if (idx != item.Children.Count-1)
                {
                    child.FolderNextItemIndex = item.Children[idx + 1].Index;
                    if (child.IsFolder & loopChain)
                        RecastParentChainsForItem(child, true);

                    idx++;
                }
                else
                {
                    child.FolderNextItemIndex = -1;
                    break;
                }
            }
        }
        public static int GetNextLineOf(int from, int range = 16)
        {
            return range * ((from / range) + 1);
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
                
                if (off < StreamPackage.Length & (off+item.Size)<StreamPackage.Length)
                {
                    for (int byteI = 0; byteI < item.Size; byteI++)
                    {
                        item.Buffer[byteI] = f.ReadByte();
                    }
                }
                else
                {
                    OnLoadFileFail(new PackageLoadFileBufferFailureEventArgs(item,this));
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
                OnFileLoad(new PackageFileLoadedEventArgs(item,this,(int)NumberOfItems));
            }
            NamesOffset = (uint) s.Position;
            // process parents
            ParseParents();
            DoneLoading(new PackageDoneLoadingEventArgs(this));
        }

        public void Load(string filename)
        {
            FileStream fileS = new FileStream(filename, FileMode.Open, FileAccess.Read);
            Load(fileS);
        }

        // Example: Data/[Cache]
        public InsideItem GetChildByPath(string path)
        {
            char sep = '/';
            if (path.Contains(@"\"))
                sep = (char)(byte)(0x5C);

            

            string[] pathNames = path.Split(sep);

            if (pathNames[0].Contains(":"))
            {
                throw new InvalidOperationException("The path cannot be a path to a hard drive");
            }

            InsideItem r = Items[0].FindFirstChildByName(pathNames[0].Replace("\0", ""));
            for (int id = 1; id < pathNames.Length; id++)
            {
                string p = pathNames[id];

                r = r.FindFirstChildByName(p);
            }
            return r;
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
                stringBufferSize += item.Name.Length + 1;
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
            //RecastParentChainsForItem(Items[0], true);
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

                    f.Write(item.Priority);

                    f.Write(item.FolderStartIndex);
                    f.Write(item.FolderNextItemIndex);

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

        public virtual void DisposeStreams()
        {
            if (StreamPackage!=null)
               StreamPackage.Dispose();
            if (f!=null)
               f.Dispose();
        }

        /// <summary>
        /// Adds items from a directory.
        /// </summary>
        /// <param name="directory">The directory to add items as files or folders.</param>
        /// <param name="relativeTo">The item where the result is added, will be attached to the root of this package if null.</param>
        /// <param name="addSiblings">Specifies if it will add more items from each folder like a chain.</param>
        public void AddItemsFromDirectory(string directory, InsideItem relativeTo = null, bool addSiblings = true)
        {
            if (!Directory.Exists(directory))
                throw new InvalidOperationException("The directory does not exist");

            string[] files = Directory.GetFiles(directory);
            string[] directories = Directory.GetDirectories(directory);

            InsideItem _r = Items[0]; // root
            if (relativeTo != null)
                _r = relativeTo;

            // files
            foreach (string entry in files)
            {
                InsideItem item = new InsideItem()
                {
                    Name = Path.GetFileName(entry),

                    Offset = 0,
                    Size = 0,
                    Buffer = new byte[0],
                    Children = new List<InsideItem>()
                };
                FileStream file = new FileStream(entry, FileMode.Open, FileAccess.Read);
                using (var f = new BinaryReader(file, Encoding.UTF8, false))
                {
                    item.Buffer = f.ReadBytes((int)file.Length);
                    item.Size = (uint)item.Buffer.Length;
                }

                item.Index = Items.Count;
                Items.Add(item);
                _r.Children.Add(item);
            }
            // directories
            foreach (string entry in directories)
            {
                InsideItem item = new InsideItem()
                {
                    Name = Path.GetFileName(entry),

                    Offset = 0,
                    Size = 0,
                    Buffer = new byte[0],
                    Children = new List<InsideItem>(),
                    IsFolder = true
                };
                if (addSiblings)
                   AddItemsFromDirectory(entry, item, true);

                item.Index = Items.Count;
                Items.Add(item);
                _r.Children.Add(item);
            }
        }

        public static Package FromDirectory(string directory)
        {
            Package pkg = new Package(new List<InsideItem>(1) {

                new InsideItem()
                {
                    Children = new List<InsideItem>(),
                    Offset = 0,
                    Size = 0,
                    CRC = -1,
                    Buffer = new byte[0]
                }

                }

            );

            // add the items
            pkg.AddItemsFromDirectory(directory, null, true);

            pkg.RecastParentChainsForItem(pkg.Items[0],true);

            return pkg;
        }

        public Package() { }
        public Package(List<InsideItem> items) { Items = items; }
        public Package(string filename) {
            FileStream fileS = new FileStream(filename, FileMode.Open, FileAccess.Read);
            Load(fileS);
            FileName = filename;
        }
    }
    // Seems to be mostly used in old version of DDI games
    // Such as MTAE, Rig Racer 2, etc...
    
    // And it seems it is required to have padding? weird.

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

                if (off < StreamPackage.Length & (off + item.Size) < StreamPackage.Length)
                {
                    for (int byteI = 0; byteI < item.Size; byteI++)
                    {
                        item.Buffer[byteI] = f.ReadByte();
                    }
                }
                else
                {
                    this.OnLoadFileFail(new PackageLoadFileBufferFailureEventArgs(item, this));
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
                this.OnFileLoad(new PackageFileLoadedEventArgs(item, this, (int)NumberOfItems));
            }
            NamesOffset = (uint)s.Position;
            // process parents
            ParseParents();
            this.DoneLoading(new PackageDoneLoadingEventArgs(this));
        }

        public override byte[] RegenerateAndReturnBuffer(bool saveInNewFormat = false)
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
                stringBufferSize += item.Name.Length + 1;
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
            //RecastParentChainsForItem(Items[0], true);
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

                    f.Write(item.Priority);

                    f.Write(item.FolderStartIndex);
                    f.Write(item.FolderNextItemIndex);

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
                    //dataStream.Position = BaseOffset+item.Offset; // prepare position
                    f.Write(item.Buffer);
                }
            }

            return dataStream.GetBuffer();
        }

        public override void DisposeStreams()
        {
            StreamPackage.Dispose();
            f.Dispose();
        }

        public PackageOld() { }
        public PackageOld(string filename) : base(filename) { }
    }
}
