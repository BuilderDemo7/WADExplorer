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
    public enum ValueSignal : int
    {
        None = 0,
        Unk1 = 1,
        Equals = 2
    }
    public class BinaryConfigurationItem
    {
        /// <summary>
        /// The name of this binary configuration item.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// An additional line of this binary configuration item
        /// </summary>
        public string Value { get; set; } // can be either a number, enum, etc. (because it's not encrypted/in binary)
        public ValueSignal Signal;

        /// <summary>
        /// An identificator perhaps?
        /// </summary>
        public int Serial { get; set; }

        /*
        private readonly static int Alignment1 = 6;
        private readonly static int Alignment2 = 8;
        private readonly static int Alignment3 = 12;
        private readonly static int Alignment4 = 16;
        private readonly static int Alignment5 = 24;
        private readonly static int Alignment6 = 64;
        */

        private readonly static int Alignment = 4;
        // TODO: Find out how the hell do we find out the alignment to this crap
        private static int GetStringAlign(string str)
        {
            int size = str.Length+1;
            int v = size / Alignment;
            v += 1;
            int r = v * Alignment;
            if (r==64) { r += 4; }
            return r;
            /*
            Debug.WriteLine(str);
            int r = str.Length+(str.Length / 2);
            Debug.WriteLine("  THEN -> " + r);
            if (r < Alignment2) r = Alignment2;
            if (r > Alignment2 & r < Alignment3)
                r = 12;
            if (r > Alignment5)
            {
                r = str.Length + (str.Length / Alignment1) - 1;
            }
            if (r > Alignment4 & r < Alignment5)
            {
                r = Alignment4;
            }
            if (r > Alignment6)
            {
                r += 1;
            }
            if (r > Alignment3 & r < Alignment4) r = Alignment4;
            if (r>74) { r -= 3; }
            Debug.WriteLine("  NOW -> " + r);
            return r;
            */
        }

        public void Load(Stream stream)
        {
            using (var f = new BinaryReader(stream, Encoding.UTF8, true)) {
                int nameLen = f.ReadInt32();
                Name = Encoding.UTF8.GetString(f.ReadBytes(nameLen-1));
                int nAlign = GetStringAlign(Name);

                Debug.WriteLine(" Align of Name ->"+nAlign);
                stream.Position += nAlign - nameLen;
                stream.Position += 1;

                Serial = f.ReadInt32();
                Debug.WriteLine(" Read Value's len At ->"+stream.Position);
                int subLen = f.ReadInt32();
                Value = Encoding.UTF8.GetString(f.ReadBytes(subLen - 1));
                Debug.WriteLine(" Align of Value ->" + GetStringAlign(Value));

                stream.Position -= (subLen - 1);

                stream.Position += GetStringAlign(Value);
                Debug.WriteLine(" Read Signal At ->"+stream.Position);
                Signal = (ValueSignal)f.ReadInt32();
            }
        }

        public byte[] GetBytes()
        {
            int nameLen = Name.Length + 1;
            int addiLen = Value.Length + 1;
            int namePad = GetStringAlign(Name);
            int subPad = GetStringAlign(Value);
            byte[] buffer = new byte[8 + (namePad + subPad)];

            MemoryStream stream = new MemoryStream(buffer);
            using (var f = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                f.Write(nameLen);
                f.Write(Encoding.UTF8.GetBytes(Name));

                stream.Position += namePad - Name.Length; // skip some bytes padding left

                f.Write(Serial);

                f.Write(addiLen);
                f.Write(Encoding.UTF8.GetBytes(Value));
            }

            buffer = stream.ToArray();
            stream.Dispose();
            return buffer;
        }

        public BinaryConfigurationItem() { }
        public BinaryConfigurationItem(string name, string subLine, int serial = 0 ) { Name = name; Value = subLine; Serial = serial; }
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
                    Debug.WriteLine("Reading at: "+stream.Position);
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
                cfg.WriteLine(item.Value);
            }

            return cfg.ToString();
        }

        public BinaryConfiguration() { }
        public BinaryConfiguration(Stream stream) { Load(stream); }
    }
}
