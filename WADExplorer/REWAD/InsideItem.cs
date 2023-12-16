using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace WADExplorer
{
    // QUICK BINARY INFO:
    // EACH INSIDE ITEM HEADER BUFFER SIZE IS 0x20

    public class InsideItem
    {
        /// <summary>
        /// A boolean confirming if the item is a folder or not.
        /// </summary>
        [ReadOnly(true)]
        [Category("Header")]
        [DisplayName("Is Folder")]
        public bool IsFolder { get; set; }
        /// <summary>
        /// The inside item that this item is parent of.
        /// </summary>
        [ReadOnly(true)]
        [Browsable(false)]
        public InsideItem Parent { get; set; }
        /// <summary>
        /// The name of this item.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// List of items that this item has inside.
        /// </summary>
        public List<InsideItem> Children { get; set; }

        /// <summary>
        /// The real index of this item.
        /// </summary>
        [ReadOnly(true)]
        [Browsable(false)]
        public int Index = 0;

        /// <summary>
        /// The offset relative to the base offset to the name of files.
        /// </summary>
        [ReadOnly(true)]
        [Browsable(false)]
        public int NameOffset { get; set; }

        /// <summary>
        /// Supposedly a virtual identification number.
        /// </summary>
        [Category("Data")]
        [DisplayName("CRC")]
        public int CRC { get; set; }

        /// <summary>
        /// A offset to the buffer of this file is indicated to. relative to base offset from the package.
        /// </summary>
        [Category("Data")]
        [DisplayName("Offset")]
        [ReadOnly(true)]
        public uint Offset { get; set; }
        [ReadOnly(true)]
        [Browsable(false)]
        public uint TotalSize { get; set; }
        [Category("Data")]
        [DisplayName("Size")]
        [ReadOnly(true)]
        public uint Size { get; set; }

        //[ReadOnly(true)]
        //[Browsable(false)]
        public int ParentId { get; set; }

        public int Unk2 { get; set; }
        public int Unk3 { get; set; }

        [ReadOnly(true)]
        [Browsable(false)]
        public byte[] Buffer { get; set; }

        public InsideItem FindFirstChildByName(string name, bool checkSiblings = false)
        {
            foreach(InsideItem child in Children)
            {
                if (child.Name.ToLower() == name.ToLower())
                    return child;
            }
            return null;
        }

        public InsideItem() { }
        public InsideItem(
            bool folder,
            int nameOffset,
            int crc,

            uint offset,
            uint totalsize,
            uint size,

            int parentID,
            int unk2,
            int unk3,

            byte[] buffer = null,
            InsideItem parent = null
        )
        {
            IsFolder = folder;

            NameOffset = nameOffset;
            CRC = crc;

            Offset = offset;
            TotalSize = totalsize;
            Size = size;

            ParentId = parentID;
            Unk2 = unk2;
            Unk3 = unk3;

            if (buffer != null)
               Buffer = buffer;
            if (parent != null)
                Parent = parent;

            // force this to be a folder if
            if (Offset == 0 && Size == 0 && Unk2!=-1)
                IsFolder = true;
        }
    }
}
