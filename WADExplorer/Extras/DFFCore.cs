using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

// Hello, dear coder/viewer/programmer
//   Get ready to get lost because there are
//   Many classes in this code
// Sincerely, BuilderDemo7

namespace WADExplorer
{
    // vectors cores ;)
    public class Vector4
    {
        private float _x = 0;
        private float _y = 0;
        private float _z = 0;
        private float _w = 0;

        public float X { get { return _x; } }
        public float Y { get { return _y; } }
        public float Z { get { return _z; } }
        public float W { get { return _w; } }
        public Vector4() { }
        public Vector4(float x, float y, float z, float w) { _x = x; _y = y; _z = z; _w = w; }
        public void Load(Stream stream)
        {
            using (var f = new BinaryReader(stream, Encoding.UTF8, true))
            {
                _x = f.ReadSingle();
                _y = f.ReadSingle();
                _z = f.ReadSingle();
                _w = f.ReadSingle();
            }
        }
        public Vector4(Stream stream)
        {
            Load(stream);
        }

        /// <summary>
        /// Format Vector4 with 4 floats (X,Y,Z,W)
        /// </summary>
        /// <param name="format">The format to use</param>
        /// <returns></returns>
        public string Format(string format)
        {
            return String.Format(format, _x, _y, _z, _w);
        }

        public byte[] GetBytes()
        {
            byte[] bytes = new byte[16];

            MemoryStream stream = new MemoryStream(bytes.Length);

            using (var f = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                f.Write(_x);
                f.Write(_y);
                f.Write(_z);
                f.Write(_w);
            }

            bytes = stream.ToArray();
            stream.Dispose();
            return bytes;
        }
    }
    public class Vector3
    {
        private float _x = 0;
        private float _y = 0;
        private float _z = 0;

        public float X { get { return _x; } }
        public float Y { get { return _y; } }
        public float Z { get { return _z; } }
        public Vector3() { }
        public Vector3(float x, float y, float z) { _x = x; _y = y; _z = z; }
        public void Load(Stream stream)
        {
            using (var f = new BinaryReader(stream, Encoding.UTF8, true))
            {
                _x = f.ReadSingle();
                _y = f.ReadSingle();
                _z = f.ReadSingle();
            }
        }
        public Vector3(Stream stream)
        {
            Load(stream);
        }

        public string Format(string format)
        {
            return String.Format(format,_x,_y,_z);
        }

        public byte[] GetBytes()
        {
            byte[] bytes = new byte[12];

            MemoryStream stream = new MemoryStream(bytes.Length);

            using (var f = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                f.Write(_x);
                f.Write(_y);
                f.Write(_z);
            }

            bytes = stream.ToArray();
            stream.Dispose();
            return bytes;
        }
    }
    public class Vector2
    {
        private float _x = 0;
        private float _y = 0;

        public float X { get { return _x; } }
        public float Y { get { return _y; } }
        public Vector2() { }
        public Vector2(float x, float y) { _x = x; _y = y; }
        public void Load(Stream stream)
        {
            using (var f = new BinaryReader(stream, Encoding.UTF8, true))
            {
                _x = f.ReadSingle();
                _y = f.ReadSingle();
            }
        }
        public Vector2(Stream stream)
        {
            Load(stream);
        }

        public string Format(string format)
        {
            return String.Format(format, _x, _y);
        }

        public byte[] GetBytes()
        {
            byte[] bytes = new byte[8];

            MemoryStream stream = new MemoryStream(bytes.Length);

            using (var f = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                f.Write(_x);
                f.Write(_y);
            }

            bytes = stream.ToArray();
            stream.Dispose();
            return bytes;
        }
    }

    // classes from the vectors
    public class Matrix4x4
    {
        public Vector4 M1;
        public Vector4 M2;
        public Vector4 M3;
        public Vector4 M4;
        public void Load(Stream stream)
        {
                M1 = new Vector4(stream);
                M2 = new Vector4(stream);
                M3 = new Vector4(stream);
                M4 = new Vector4(stream);
        }
        public byte[] GetBytes()
        {
            byte[] bytes = new byte[64];

            MemoryStream stream = new MemoryStream(bytes.Length);

            using (var f = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                f.Write(M1.GetBytes());
                f.Write(M2.GetBytes());
                f.Write(M3.GetBytes());
                f.Write(M4.GetBytes());
            }

            bytes = stream.ToArray();
            stream.Dispose();
            return bytes;
        }
    }

    // mesh data stuff
    public class TexCoords
    {
        public Vector2 Coords;
        public void Load(Stream stream)
        {
            Coords = new Vector2(stream);
        }
        public byte[] GetBytes()
        {
            byte[] bytes = new byte[8];

            MemoryStream stream = new MemoryStream(bytes.Length);

            using (var f = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                f.Write(Coords.GetBytes());
            }

            bytes = stream.ToArray();
            stream.Dispose();
            return bytes;
        }
        public TexCoords() { }
        public TexCoords(Vector2 uv) { Coords = uv; }
        public TexCoords(Stream stream) { Load(stream); }
    }
    public class Vertex
    {
        public Vector3 Position;
        public void Load(Stream stream)
        {
                Position = new Vector3(stream);
        }
        public byte[] GetBytes()
        {
            byte[] bytes = new byte[12];

            MemoryStream stream = new MemoryStream(bytes.Length);

            using (var f = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                f.Write(Position.GetBytes());
            }

            bytes = stream.ToArray();
            stream.Dispose();
            return bytes;
        }
        // why?
        public static explicit operator Vertex(Vector3 vector)
        {
            return new Vertex(vector);
        }
        public Vertex() { }
        public Vertex(Vector3 position) { Position = position; }
        public Vertex(Stream stream) { Load(stream); }
    }
    public class TriangleIndex
    {
        public short Vert1;
        public short Vert2;
        public short Material;
        public short Vert3;
        public void Load(Stream stream)
        {
            using (var f = new BinaryReader(stream, Encoding.UTF8, true))
            {
                Vert2 = f.ReadInt16();
                Vert1 = f.ReadInt16();
                Material = f.ReadInt16();
                Vert3 = f.ReadInt16();
            }
        }
        public byte[] GetBytes()
        {
            byte[] bytes = new byte[16];

            MemoryStream stream = new MemoryStream(bytes.Length);

            using (var f = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                f.Write(Vert2);
                f.Write(Vert1);
                f.Write(Material);
                f.Write(Vert3);
            }

            bytes = stream.ToArray();
            stream.Dispose();
            return bytes;
        }
        public TriangleIndex() { }
        public TriangleIndex(short v1, short v2, short v3, short mat) { Vert1 = v1; Vert2 = v2; Vert3 = v3; Material = mat;  }
        public TriangleIndex(Stream stream) { Load(stream); }
    }
    public abstract class Attribute
    {
        public static readonly int OldMagic = 0x1400FFFF; 
        public static readonly int NewMagic = 0x1803FFFF; 
        public static int Magic = NewMagic; // not a word
        public virtual int Type { get; }
        public int Parameter { get; set; }

        public void Load(Stream stream)
        {
            stream.Position += 4; // Skip type
            using (var f = new BinaryReader(stream, Encoding.UTF8, true))
            {
                Parameter = f.ReadInt32();
            }
            stream.Position += 4; // Skip magic
            AttributeLoad(stream);
        }
        protected virtual void AttributeLoad(Stream stream)
        {
            throw new NotImplementedException();
        }
        public virtual byte[] GetAttributeBytes()
        {
            throw new NotImplementedException();
        }
        public byte[] GetBytes()
        {
            byte[] materialBytes = GetAttributeBytes();
            byte[] buffer = new byte[12+materialBytes.Length];

            MemoryStream ms = new MemoryStream(buffer);
            using (var f = new BinaryWriter(ms, Encoding.UTF8, true))
            {
                f.Write(Type);
                f.Write(Parameter);
                f.Write(Magic);

                f.Write(materialBytes);
            }

            buffer = ms.ToArray();
            ms.Dispose();
            return buffer;
        }
        public Attribute() { }
        public Attribute(Stream stream)
        {
            Load(stream);
        }
    }
    public class MainChunk : Attribute
    {
        public override int Type
        {
            get { return 20; }
        }

        protected override void AttributeLoad(Stream stream)
        {
            // sorry, I don't want to use binary reader and stream stuff here because most of code will change 
            // and memory overload, etc.
        }

        public override byte[] GetAttributeBytes()
        {
            // well, not really necessary I suppose so return a empty byte array.
            return new byte[0];
        }

        public MainChunk() { }
        public MainChunk(int chunkSize) { Parameter = chunkSize; }
        public MainChunk(Stream stream) : base(stream) { }
    }
    public class HeaderInfo : Struct
    {
        public int FileVersion;
        public int ExtraData;
        public int Reserved1;
        public int Reserved2;

        protected override void AttributeLoad(Stream stream)
        {
            using (var f = new BinaryReader(stream, Encoding.UTF8, true))
            {
                FileVersion = f.ReadInt32();
                ExtraData = f.ReadInt32();
                Reserved1 = f.ReadInt32();
                Reserved2 = f.ReadInt32();
            }
        }

        public override byte[] GetAttributeBytes()
        {
            byte[] buffer = new byte[16];

            MemoryStream ms = new MemoryStream(buffer);
            using (var f = new BinaryWriter(ms, Encoding.UTF8, true))
            {
                Parameter = 16;
                f.Write(FileVersion);
                f.Write(ExtraData);
                f.Write(Reserved1);
                f.Write(Reserved2);
            }

            buffer = ms.ToArray();
            ms.Dispose();
            return buffer;
        }

        public HeaderInfo() { }
        public HeaderInfo(int version, int extraData, int reserved1 = 5, int reserved2 = 0) { FileVersion = version; ExtraData = extraData; Reserved1 = reserved1; Reserved2 = reserved2; }
        public HeaderInfo(Stream stream) : base(stream) { }
    }
    public class Geometry : Attribute
    {
        public override int Type
        {
            get { return 15; }
        }

        protected override void AttributeLoad(Stream stream)
        {
            // none yet again!!!
            // screw streams now for DFF!
            // I RULE THIS!
        }

        public override byte[] GetAttributeBytes()
        {
            // ;)
            return new byte[0];
        }

        public Geometry() { }
        public Geometry(int size) { Parameter = size; }
        public Geometry(Stream stream) : base(stream) { }
    }
    public class GeometryInfo : Struct
    {
        public static new int NeutralFormat = 0x0001004D; // most used in MTEOE's original edition
        public static new int NewFormat = 0x0001004C; // most used in MTEOE's Nestle Edition objects
        public static new int DynamicFormat = 0x0001007D; // seems to be commonly used in the vehicle models

        public int Format = NeutralFormat;
        public int FacesCount;
        public int VerticesCount;
        public int Morph; // never really used in this game

        protected override void AttributeLoad(Stream stream)
        {
            using (var f = new BinaryReader(stream, Encoding.UTF8, true))
            {
                Format = f.ReadInt32();
                FacesCount = f.ReadInt32();
                VerticesCount = f.ReadInt32();
                Morph = f.ReadInt32();
            }
        }

        public override byte[] GetAttributeBytes()
        {
            byte[] buffer = new byte[16];

            MemoryStream ms = new MemoryStream(buffer);
            using (var f = new BinaryWriter(ms, Encoding.UTF8, true))
            {
                if (Format == null)
                    Format = NeutralFormat;

                f.Write(Format);
                f.Write(FacesCount);
                f.Write(VerticesCount);
                f.Write(Morph);
            }

            buffer = ms.ToArray();
            ms.Dispose();
            return buffer;
        }

        public GeometryInfo() { }
        public GeometryInfo(int faces, int vertices, int morph = 1) { FacesCount = faces; VerticesCount = vertices; Morph = morph; }
        public GeometryInfo(Stream stream) : base(stream) { }
    }
    public class MeshData
    {
        public uint NumberOfIndices;
        public uint MaterialIndex;
        public List<uint> Indices;

        public void Load(Stream stream)
        {
            using (var f = new BinaryReader(stream, Encoding.UTF8, true))
            {
                NumberOfIndices = f.ReadUInt32();
                MaterialIndex = f.ReadUInt32();
                Indices = new List<uint>();
                for (int id = 0; id<NumberOfIndices; id++)
                {
                    Indices.Add(f.ReadUInt32());
                }
            }
        }

        public byte[] GetBytes()
        {
            byte[] buffer = new byte[8 + (Indices.Count*4)];

            MemoryStream stream = new MemoryStream(buffer);
            using (var f = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                NumberOfIndices = (uint)Indices.Count;
                f.Write(NumberOfIndices);
                f.Write(MaterialIndex);
                foreach (int ind in Indices)
                {
                    f.Write(ind);
                }
            }

            buffer = stream.ToArray();
            stream.Dispose();
            return buffer;
        }

        public MeshData() { }
        public MeshData(Stream stream) { Load(stream); }
    }
    public class MeshContainer : Struct
    {
        public override int Type
        {
            get { return 0x050E; }
        }

        public int Flags = 1;
        public uint NumberOfMeshes;
        public uint TotalNumberOfIndices;
        public List<MeshData> Meshes;

        protected override void AttributeLoad(Stream stream)
        {
            using (var f = new BinaryReader(stream, Encoding.UTF8, true))
            {
                Flags = f.ReadInt32();
                NumberOfMeshes = f.ReadUInt32();
                TotalNumberOfIndices = f.ReadUInt32();
                Meshes = new List<MeshData>();
                for (int id = 0; id<NumberOfMeshes; id++)
                {
                    Meshes.Add(new MeshData(stream));
                }
            }
        }

        public override byte[] GetAttributeBytes()
        {
            int bufferSize = 0;
            foreach(MeshData mesh in Meshes)
            {
                bufferSize += 8 + (mesh.Indices.Count * 4);
            }
            byte[] buffer = new byte[12 + bufferSize];

            MemoryStream stream = new MemoryStream(buffer);
            using (var f = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                f.Write(Flags);
                f.Write(Meshes.Count);
                uint total = 0;
                foreach(MeshData md in Meshes)
                {
                    total += (uint)md.Indices.Count;
                }
                f.Write(total);
                foreach(MeshData md in Meshes)
                {
                    f.Write(md.GetBytes());
                }
            }

            Parameter = buffer.Length;
            return buffer;
        }

        public static MeshContainer ChronologicalOrder(int verticesCount)
        {
            MeshContainer container = new MeshContainer();
            container.Meshes = new List<MeshData>();

            MeshData meshData = new MeshData();
            meshData.Indices = new List<uint>();
            for (uint id = 0; id < verticesCount; id++)
            {
                meshData.Indices.Add(id);
            }

            container.Meshes.Add(meshData);

            return container;
        }

        public MeshContainer() { }
        public MeshContainer(List<MeshData> meshes) { Meshes = meshes; }
        public MeshContainer(Stream stream) : base(stream) { } 
    }
    public class Extension : Struct
    {
        public override int Type
        {
            get { return 3; }
        }

        /*
        protected override void AttributeLoad(Stream stream)
        {
            // Don't worry, these extensions usually doesn't really have anything at all
        }

        public override byte[] GetAttributeBytes()
        {
            // told you about it ;)
            return new byte[0];
        }
        */

        public Extension() { Data = new byte[0]; }
        public Extension(byte[] data) { Data = data; }
        public Extension(Stream stream) : base(stream) { }
    }
    public class Material : Attribute
    {
        public override int Type
        {
            get { return 7; }
        }

        public MaterialDataStruct Data;
        public Texture Texture = new Texture();
        public Extension Extension = new Extension();
        //public Extension Extension2 = new Extension();

        protected override void AttributeLoad(Stream stream)
        {
            using (var f = new BinaryReader(stream, Encoding.UTF8, true))
            {
                Data = new MaterialDataStruct(stream);
                if (Data.isTextured)
                {
                    Texture = new Texture(stream);
                }
                Extension = new Extension(stream);
            }
        }

        public override byte[] GetAttributeBytes()
        {
            byte[] data1 = Data.GetBytes();
            byte[] data2 = new byte[0];
            if (Data.isTextured)
                data2 = Texture.GetBytes();
            //byte[] data3 = AlphaLayer.GetBytes();
            byte[] data3 = Extension.GetBytes();
            //byte[] data5 = Extension2.GetBytes();
            byte[] buffer = new byte[data1.Length + data2.Length + data3.Length /*+ data5.Length*/];

            if (!Data.isTextured)
                buffer = new byte[data1.Length + data3.Length /*+ data4.Length + data5.Length*/];

            MemoryStream ms = new MemoryStream(buffer);
            using (var f = new BinaryWriter(ms, Encoding.UTF8, true))
            {
                f.Write(data1);
                if (Data.isTextured)
                {
                    f.Write(data2);
                }
                f.Write(data3);
            }

            buffer = ms.ToArray();
            ms.Dispose();
            return buffer;
        }

        public Material() { }
        public Material(Stream stream) : base(stream) { }
    }

    public class StringAttribute : Attribute
    {
        public override int Type
        {
            get { return 2; }
        }

        public string Value { get; set; }
        public bool LongNamePadding = false;

        protected override void AttributeLoad(Stream stream)
        {
            using (var f = new BinaryReader(stream, Encoding.ASCII, true))
            {
                Value = Encoding.ASCII.GetString(f.ReadBytes(Parameter));
            }
        }

        public override byte[] GetAttributeBytes()
        {
            int pad = (Value.Length < 32 ? 32 : 64);
            if (Value.Length > 64)
                pad = 128;

            if (Value.Length > 128)
            {
                throw new InvalidDataException("The string is longer than 128 characters");
            }
            byte[] data = new byte[LongNamePadding ? pad : Value.Length];
            MemoryStream stream = new MemoryStream(data);
            using (var f = new BinaryWriter(stream, Encoding.ASCII, true))
            {
                f.Write(Encoding.ASCII.GetBytes(Value));
            }

            Parameter = data.Length;
            return data;
        }

        public StringAttribute() { }
        public StringAttribute(string value) { Value = value; }
        public StringAttribute(Stream stream) : base(stream) { }
    }

    public class TextureStruct : Struct
    {
        public override int Type
        {
            get { return 1; }
        }

        public byte Unk1 { get; set; }
        public byte Unk2 { get; set; }
        public bool FlippedVertically { get; set; }
        public byte Unk3 { get; set; }

        protected override void AttributeLoad(Stream stream)
        {
            using (var f = new BinaryReader(stream, Encoding.UTF8, true))
            {
                Unk1 = f.ReadByte();
                Unk2 = f.ReadByte();
                FlippedVertically = f.ReadByte() >= 1 ? true : false;
                Unk3 = f.ReadByte();
            }
        }

        public override byte[] GetAttributeBytes()
        {
            byte[] bytes = new byte[4];
            Parameter = bytes.Length;

            MemoryStream stream = new MemoryStream(bytes.Length);

            using (var f = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                f.Write(Unk1);
                f.Write(Unk2);
                f.Write(FlippedVertically == true ? (byte)1 : (byte)0);
                f.Write(Unk3);
            }

            bytes = stream.ToArray();
            stream.Dispose();
            return bytes;
        }

        public TextureStruct() { }
        public TextureStruct(bool flippedVertically, byte unk1 = 6, byte unk2 = 17, byte unk3 = 0) { FlippedVertically = flippedVertically; Unk1 = unk1; Unk2 = unk2; Unk3 = unk3; }
        public TextureStruct(Stream stream) { Load(stream); }
    }
    public class Texture : Attribute
    {
        public override int Type
        {
            get { return 6; }
        }

        public TextureStruct Structure { get; set; }
        public StringAttribute Name { get; set; }
        public StringAttribute AlphaLayer = new StringAttribute("\0\0\0\0");
        public Extension Extension = new Extension();

        protected override void AttributeLoad(Stream stream)
        {
            using (var f = new BinaryReader(stream, Encoding.ASCII, true))
            {
                Structure = new TextureStruct(stream);
                Name = new StringAttribute(stream) { LongNamePadding = true };
                AlphaLayer = new StringAttribute(stream);
                Extension = new Extension(stream);
            }
        }

        public override byte[] GetAttributeBytes()
        {
            byte[] data1 = Structure.GetBytes();
            byte[] data2 = Name.GetBytes();
            byte[] data3 = AlphaLayer.GetBytes();
            byte[] data4 = Extension.GetBytes();
            byte[] buffer = new byte[data1.Length + data2.Length + data3.Length + data4.Length];

            MemoryStream ms = new MemoryStream(buffer);
            using (var f = new BinaryWriter(ms, Encoding.UTF8, true))
            {
                Parameter = buffer.Length;
                f.Write(data1);
                f.Write(data2);
                f.Write(data3);
                f.Write(data4);
            }

            buffer = ms.ToArray();
            ms.Dispose();
            return buffer;
        }

        public Texture() { }
        public Texture(string textureName) { Name = new StringAttribute(textureName) { LongNamePadding = true }; }
        public Texture(Stream stream) : base(stream) { }
    }

    public class Color
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public byte A { get; set; }
        public void Load(Stream stream)
        {
            using (var f = new BinaryReader(stream, Encoding.UTF8, true))
            {
                R = f.ReadByte();
                G = f.ReadByte();
                B = f.ReadByte();
                A = f.ReadByte();
            }
        }
        public byte[] GetBytes()
        {
            byte[] bytes = new byte[4];

            MemoryStream stream = new MemoryStream(bytes.Length);

            using (var f = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                f.Write(R);
                f.Write(G);
                f.Write(B);
                f.Write(A);
            }

            bytes = stream.ToArray();
            stream.Dispose();
            return bytes;
        }
        public Color() { }
        public Color(byte r, byte g, byte b, byte a) { R = r; G = g; B = b; A = a; }
        public static explicit operator Color(VertexColor vertexColor)
        {
            return new Color(vertexColor.R, vertexColor.G, vertexColor.B, vertexColor.A);
        }
        public Color(Stream stream) { Load(stream); }

        public static Color Black = new Color(0, 0, 0, 255);
        public static Color White = new Color(255, 255, 255, 255);
        public static Color WhiteSemiTransparent = new Color(255, 255, 255, 128);
        public static Color WhiteTransparent = new Color(255, 255, 255, 0);
    }
    public class Struct : Attribute
    {
        public override int Type
        {
            get { return 1; }
        }

        public virtual byte[] Data { get; set; }

        protected override void AttributeLoad(Stream stream)
        {
            using (var f = new BinaryReader(stream, Encoding.UTF8, true))
            {
                if (Parameter > 0)
                    Data = f.ReadBytes(Parameter);
                else
                    Data = new byte[0];
            }
        }

        public override byte[] GetAttributeBytes()
        {
            return Data;
        }

        public Struct() { }
        public Struct(Stream stream) : base(stream) { }
    }
    public class ArrayStruct : Attribute
    {
        public override int Type
        {
            get { return 1; }
        }

        public List<int> Data { get; set; }

        protected override void AttributeLoad(Stream stream)
        {
            using (var f = new BinaryReader(stream, Encoding.UTF8, true))
            {
                Data = new List<int>();
                var count = f.ReadInt32();
                for (int id = 0; id<count; id++)
                {
                    Data.Add(f.ReadInt32());
                }
            }
        }

        public override byte[] GetAttributeBytes()
        {
            byte[] bytes = new byte[4+(Data.Count*4)];
            Parameter = bytes.Length;

            MemoryStream stream = new MemoryStream(bytes.Length);

            using (var f = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                f.Write(Data.Count);
                foreach(int integer in Data)
                {
                    f.Write(integer);
                }
            }

            bytes = stream.ToArray();
            stream.Dispose();
            return bytes;
        }

        public ArrayStruct() { }
        public ArrayStruct(Stream stream) : base(stream) { }
    }

    public class MaterialDataStruct : Attribute
    {
        public override int Type
        {
            get { return 1; }
        }

        public static int UnusedMagic = 0x026DDF20;
        public static int UnusedMagic2 = 0x00E4E2DC;

        public int Flags { get; set; }
        public Color MaterialColor { get; set; }
        public int UNUSED { get; set; }
        public bool isTextured { get; set; }

        public float Ambient { get; set; }
        public float Specular { get; set; }
        public float Diffuse { get; set; }

        protected override void AttributeLoad(Stream stream)
        {
            using (var f = new BinaryReader(stream, Encoding.UTF8, true))
            {
                Flags = f.ReadInt32();
                MaterialColor = new Color(stream);
                UNUSED = f.ReadInt32();
                isTextured = f.ReadInt32() >= 1 ? true : false;

                Ambient = f.ReadSingle();
                Specular = f.ReadSingle();
                Diffuse = f.ReadSingle();
            }
        }

        public override byte[] GetAttributeBytes()
        {
            byte[] bytes = new byte[28];
            Parameter = bytes.Length;

            MemoryStream stream = new MemoryStream(bytes.Length);

            using (var f = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                f.Write(Flags);
                f.Write(MaterialColor.GetBytes());
                f.Write(UnusedMagic);
                f.Write(isTextured == true ? (int)1 : (int)0);

                f.Write(Ambient);
                f.Write(Specular);
                f.Write(Diffuse);
            }

            bytes = stream.ToArray();
            stream.Dispose();
            return bytes;
        }

        public MaterialDataStruct() { }
        public MaterialDataStruct(Stream stream) { Load(stream); }
    }

    public class MaterialListStruct : ArrayStruct
    {
        public int MaterialsCount { get { return Data.Count; }  }
        public MaterialListStruct() { }
        public MaterialListStruct(Stream stream) : base(stream) { }
    }
    public class MaterialList : Attribute
    {
        public override int Type
        {
            get { return 8; }
        }

        public MaterialListStruct Data;
        public List<Material> Materials { get; set; }

        protected override void AttributeLoad(Stream stream)
        {
            using (var f = new BinaryReader(stream, Encoding.UTF8, true))
            {
                Data = new MaterialListStruct(stream);
                var count = Data.MaterialsCount;
                Materials = new List<Material>();
                for (int matId = 0; matId<count; matId++)
                {
                    Materials.Add(new Material(stream));
                }
            }
        }

        public override byte[] GetAttributeBytes()
        {
            Data = new MaterialListStruct();
            Data.Data = new List<int>();
            for (int id = 0; id<Materials.Count; id++)
            {
                Data.Data.Add(-1);
            }

            Data.Parameter = Data.GetAttributeBytes().Length;
            byte[] data1 = Data.GetBytes();

            var bufferSize = data1.Length;
            foreach(Material mat in Materials)
            {
                bufferSize += mat.GetBytes().Length;
                mat.Parameter = mat.GetAttributeBytes().Length;
            }

            byte[] buffer = new byte[bufferSize];

            MemoryStream ms = new MemoryStream(buffer);
            using (var f = new BinaryWriter(ms, Encoding.UTF8, true))
            {
                f.Write(data1);
                foreach (Material mat in Materials)
                {
                    f.Write(mat.GetBytes());
                }
            }

            buffer = ms.ToArray();
            ms.Dispose();
            Parameter = buffer.Length;
            return buffer;
        }

        public MaterialList() { }
        public MaterialList(Stream stream) : base(stream) { }
    }
    public class VertexColor
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;
        public void Load(Stream stream)
        {
            using (var f = new BinaryReader(stream, Encoding.UTF8, true))
            {
                R = f.ReadByte();
                G = f.ReadByte();
                B = f.ReadByte();
                A = f.ReadByte();
            }
        }
        public byte[] GetBytes()
        {
            byte[] bytes = new byte[4];

            MemoryStream stream = new MemoryStream(bytes.Length);

            using (var f = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                f.Write(R);
                f.Write(G);
                f.Write(B);
                f.Write(A);
            }

            bytes = stream.ToArray();
            stream.Dispose();
            return bytes;
        }
        public VertexColor() { }
        public VertexColor(byte r,byte g,byte b,byte a) { R = r; G = g; B = b; A = a; }
        public static explicit operator VertexColor(Color color)
        {
            return new VertexColor(color.R, color.G, color.B, color.A);
        }
        public VertexColor(Stream stream) { Load(stream); }

        public string Format(string format)
        {
            return String.Format(format, R, G, B, A);
        }

        public static List<VertexColor> BlackList(int count)
        {
            
            List<VertexColor> list = new List<VertexColor>();
            for(int i=0;i<count;i++)
            {
                list.Add(Black);
            }
            return list;
        }
        public static List<VertexColor> WhiteList(int count)
        {

            List<VertexColor> list = new List<VertexColor>();
            for (int i = 0; i < count; i++)
            {
                list.Add(White);
            }
            return list;
        }

        public readonly static VertexColor Black = new VertexColor(0, 0, 0, 255);
        public readonly static VertexColor White = new VertexColor(255, 255, 255, 255);
        public readonly static VertexColor WhiteSemiTransparent = new VertexColor(255, 255, 255, 128);
        public readonly static VertexColor WhiteTransparent = new VertexColor(255, 255, 255, 0);
    }
    public class Normal
    {
        //public Vector3 Position { get; set; }
        public Vector2 Unknown { get; set; }

        public void Load(Stream stream)
        {
            //Position = new Vector3(stream);
            Unknown = new Vector2(stream);
        }
        public byte[] GetBytes()
        {
            byte[] bytes = new byte[8];

            MemoryStream stream = new MemoryStream(bytes.Length);

            using (var f = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                //f.Write(Position.GetBytes());
                f.Write(Unknown.GetBytes());
            }

            bytes = stream.ToArray();
            stream.Dispose();
            return bytes;
        }
        public Normal() { }
        public Normal(Stream stream) { Load(stream); }
    }
}
