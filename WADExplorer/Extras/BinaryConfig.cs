using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

// Work in progress
namespace WADExplorer
{
    public class BinaryConfigurationItem
    {
        /// <summary>
        /// The name of this binary configuration item.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// An additional line of this binary configuration item
        /// </summary>
        public string SubLine { get; set; } // can be either a number, enum, etc. (because it's not encrypted/in binary)

        /// <summary>
        /// An identificator perhaps?
        /// </summary>
        public int Serial { get; set; }

        private readonly int Padding = 4;
        // TODO: Find out how the hell do we find out the padding to this crap
        private int GetPad(string str)
        {
            int size = str.Length + 1;
            float v = (float)size / (float)Padding;
            v += 1;
            if ( Math.Floor(v) - v >= 0.49f )
            {
                v += 1;
            }
            float r = (float)v * (float)Padding;
            return (int)r-1;
        }

        public void Load(Stream stream)
        {
            using (var f = new BinaryReader(stream, Encoding.UTF8, true)) {
                int nameLen = f.ReadInt32();
                Name = Encoding.UTF8.GetString(f.ReadBytes(nameLen-1));

                stream.Position += GetPad(Name) - nameLen;
                stream.Position += 1;

                Serial = f.ReadInt32();
                int subLen = f.ReadInt32();
                SubLine = Encoding.UTF8.GetString(f.ReadBytes(subLen - 1));

                stream.Position += GetPad(SubLine) - subLen;
                stream.Position += 1;
            }
        }

        public byte[] GetBytes()
        {
            int nameLen = Name.Length + 1;
            int addiLen = SubLine.Length + 1;
            int namePad = GetPad(Name);
            int subPad = GetPad(SubLine);
            byte[] buffer = new byte[8 + (namePad + subPad)];

            MemoryStream stream = new MemoryStream(buffer);
            using (var f = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                f.Write(nameLen);
                f.Write(Encoding.UTF8.GetBytes(Name));

                stream.Position += namePad - Name.Length; // skip some bytes padding left

                f.Write(Serial);

                f.Write(addiLen);
                f.Write(Encoding.UTF8.GetBytes(SubLine));
            }

            buffer = stream.ToArray();
            stream.Dispose();
            return buffer;
        }

        public BinaryConfigurationItem() { }
        public BinaryConfigurationItem(string name, string subLine, int serial = 0 ) { Name = name; SubLine = subLine; Serial = serial; }
        public BinaryConfigurationItem(Stream stream) { Load(stream); }
    }

    public class BinaryConfiguration
    {
        public List<BinaryConfigurationItem> Items = new List<BinaryConfigurationItem>();

        public void Load(Stream stream)
        {
            using (var f = new BinaryReader(stream, Encoding.UTF8, true))
            {
                Items = new List<BinaryConfigurationItem>();
                while (stream.Position < stream.Length)
                {
                    Debug.WriteLine(stream.Position);
                    Items.Add(new BinaryConfigurationItem(stream));
                }
            }
        }

        public byte[] GetBytes()
        {
            int bufferSize = 0;
            foreach(BinaryConfigurationItem item in Items)
            {
                bufferSize += item.GetBytes().Length;
            }

            byte[] buffer = new byte[bufferSize];

            MemoryStream stream = new MemoryStream(buffer);
            using (var f = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                foreach(BinaryConfigurationItem item in Items)
                {
                    f.Write(item.GetBytes());
                }
            }

            buffer = stream.ToArray();
            stream.Dispose();
            return buffer;
        }

        public override string ToString()
        {
            StringWriter cfg = new StringWriter();

            foreach (BinaryConfigurationItem item in Items)
            {
                cfg.WriteLine(item.Name);
                cfg.WriteLine(item.SubLine);
            }

            return cfg.ToString();
        }

        public BinaryConfiguration() { }
        public BinaryConfiguration(Stream stream) { Load(stream); }
    }
}
