using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace WADExplorer
{
    /// <summary>
    /// DDI's .dff file format
    /// </summary>
    public class DFF
    {
        // ignore these 2 days stuff....
        /*
        public int Unk1;    // 0x00
        public int Unk2;    // 0x04
        public short Unk3;  // 0x08
        public int Unk4;    // 0x0A
        public short Unk5;  // 0x0E
        public int Unk6;    // 0x10
        public short Unk7;  // 0x14
        public int Unk8;    // 0x16
        public short Unk9;  // 0x1A
        public int Unk10;   // 0x1C
        public long Unk11;  // 0x20
        public int Unk12;   // 0x28
        public int Unk13;   // 0x2C
        public short Unk14; // 0x30
        public int Unk15;   // 0x32
        public short Unk16; // 0x36
        public int Unk17;   // 0x38
        public short Unk18; // 0x38
        public int Unk19;   // 0x3E
        public short Unk20; // 0x42
        */

        public MainChunk Main;
        public HeaderInfo HeaderInfo;
        public Geometry GeometryChunk;
        public GeometryInfo GeometryInfo;

        public static int GeometryChunkMagic = 0x0001004C;

        public static int OldVersionGeneric = 0x004EEF1D;
        public static int NewVersionGeneric = 0x00126B48;
        public static int OldVersionExtra =   0x007AC150;
        public static int NewVersionExtra =   0x2;
        // Triangles count       0x44 (No need to be stored as int)
        // Vertices count      0x48 (No need to be stored as int)
        public int Unk21; // 0x4C

        public List<VertexColor> VerticesColors;
        public List<Vertex> Vertices;
        public List<TexCoords> TextureCoordinates;
        public List<TriangleIndex> TriangleIndices;

        public MaterialList Materials;

        public Extension MeshDataExtension = new Extension();
        public MeshContainer Meshes = new MeshContainer();
        public Extension EndExtension = new Extension();

        public Vector4 BoundingBox = new Vector4(1,1,1,1);

        //public Vector3 Position = new Vector3();

        public void Load(Stream stream)
        {
            using (var f = new BinaryReader(stream, Encoding.UTF8, true))
            {
                // again, ignore it!
                /*
                Unk1 = f.ReadInt32();
                Unk2 = f.ReadInt32();
                Unk3 = f.ReadInt16();
                Unk4 = f.ReadInt32();
                Unk5 = f.ReadInt16();
                Unk6 = f.ReadInt32();
                Unk7 = f.ReadInt16();
                Unk8 = f.ReadInt32();
                Unk9 = f.ReadInt16();
                Unk10 = f.ReadInt32();
                Unk11 = f.ReadInt64();
                Unk12 = f.ReadInt32();
                Unk13 = f.ReadInt32();
                Unk14 = f.ReadInt16();
                Unk15 = f.ReadInt32();
                Unk16 = f.ReadInt16();
                Unk17 = f.ReadInt32();
                Unk18 = f.ReadInt16();
                Unk19 = f.ReadInt32();
                Unk20 = f.ReadInt16();
                */
                Main = new MainChunk(stream);
                HeaderInfo = new HeaderInfo(stream);
                GeometryChunk = new Geometry(stream);
                GeometryInfo = new GeometryInfo(stream);

                var trianglesCount = GeometryInfo.FacesCount; //f.ReadInt32();
                var verticesCount = GeometryInfo.VerticesCount; //f.ReadInt32();
                //Unk21 = f.ReadInt32();
                // skip 0xFF bytes
                VerticesColors = new List<VertexColor>();
                for (int unkintid = 0; unkintid < verticesCount; unkintid++)
                {
                    VerticesColors.Add(new VertexColor(stream));
                }
                TextureCoordinates = new List<TexCoords>();
                for (int id = 0; id < verticesCount; id++)
                {
                    TextureCoordinates.Add(new TexCoords(stream));
                }
                TriangleIndices = new List<TriangleIndex>();
                for (int id = 0; id < trianglesCount; id++)
                {
                    TriangleIndices.Add(new TriangleIndex(stream));
                }
                stream.Position += 0x18;
                Vertices = new List<Vertex>();
                for (int id = 0; id < verticesCount; id++)
                {
                    Vertices.Add(new Vertex(stream));

                }
                if (this.GeometryInfo.Format>=WADExplorer.GeometryInfo.DynamicFormat)
                {
                    // test
                    for (int id = 0; id < verticesCount; id++)
                    {
                        Vertices.Add(new Vertex(stream));
                    }
                }
                Materials = new MaterialList(stream);
            }
        }

        public byte[] GetBytes()
        {
            byte[] data1 = Materials.GetBytes();

            //Meshes = MeshContainer.ChronologicalOrder(Vertices.Count);
            MeshData mainmesh = new MeshData() { Indices = new List<uint>() };
            foreach (TriangleIndex triangleIndex in TriangleIndices)
            {
                mainmesh.Indices.Add((uint)triangleIndex.Vert1);
                mainmesh.Indices.Add((uint)triangleIndex.Vert2);
                mainmesh.Indices.Add((uint)triangleIndex.Vert3);
            }
            Meshes = new MeshContainer( new List<MeshData>() { mainmesh } );
            byte[] meshData = Meshes.GetBytes();

            byte[] buffer = new byte[80 + (VerticesColors.Count*4) + (Vertices.Count*12) + (TextureCoordinates.Count*8) + (TriangleIndices.Count * 8) + data1.Length + (meshData.Length) + MeshDataExtension.GetBytes().Length + 0x18 + 0x12 - 6];

            MemoryStream ms = new MemoryStream(buffer);
            using (var f = new BinaryWriter(ms, Encoding.UTF8, true))
            {
                Main = new MainChunk(buffer.Length-12);
                GeometryChunk = new Geometry(buffer.Length-52);
                GeometryInfo = new GeometryInfo(TriangleIndices.Count, Vertices.Count);
                GeometryInfo.Parameter = (VerticesColors.Count * 4) + (Vertices.Count * 12) + (TextureCoordinates.Count * 8) + (TriangleIndices.Count * 8) + 0x18 + 0x10;
                f.Write(Main.GetBytes());
                f.Write(HeaderInfo.GetBytes());

                // I kinda forgot to link them
                f.Write(GeometryChunk.GetBytes());
                f.Write(GeometryInfo.GetBytes());

                foreach(VertexColor vertexColor in VerticesColors)
                {
                    f.Write(vertexColor.GetBytes());
                }
                foreach (TexCoords texCoord in TextureCoordinates)
                {
                    f.Write(texCoord.GetBytes());
                }
                foreach (TriangleIndex triangle in TriangleIndices)
                {
                    f.Write(triangle.GetBytes());
                }
                f.Write(BoundingBox.GetBytes());
                f.Write((long)1);
                foreach (Vertex vert in Vertices)
                {
                    f.Write(vert.GetBytes());
                }

                // materials
                f.Write(data1);

                // mesh data
                MeshDataExtension.Parameter = meshData.Length;
                f.Write(MeshDataExtension.GetBytes());

                f.Write(meshData);

                // end
                f.Write(EndExtension.GetBytes());
            }

            buffer = ms.ToArray();
            ms.Dispose();
            return buffer;
        }

        public static string invalidMatErr = "Materials name must start with 'Mat_' and have material ID from a range of 0 to the ID in the end.";

        public static DFF FromOBJ(string filename)
        {
            DFF dff = new DFF();
            dff.Vertices = new List<Vertex>();
            dff.TextureCoordinates = new List<TexCoords>();
            dff.TriangleIndices = new List<TriangleIndex>();

            dff.Materials = new MaterialList();
            dff.Materials.Materials = new List<Material>();

            short matId = 0;
            string mtllibFileName = null;
            using (StreamReader text = File.OpenText(filename))
            {
                string entry = text.ReadLine();
                while (entry != null)
                {
                    if (!entry.StartsWith("#"))
                    {
                        string[] parameters = entry.Split(' ');
                        string command = parameters[0];
                        switch (command)
                        {
                            case "mtllib":
                                mtllibFileName = parameters[1];
                                break;
                            case "v":
                                float vx = Convert.ToSingle(parameters[1].Replace('.',','));
                                float vy = Convert.ToSingle(parameters[2].Replace('.', ','));
                                float vz = Convert.ToSingle(parameters[3].Replace('.', ','));
                                dff.Vertices.Add( new Vertex(new Vector3(vx,vy,vz)) );
                                break;
                            case "vt":
                                float x = Convert.ToSingle(parameters[1].Replace('.', ','));
                                float y = Convert.ToSingle(parameters[2].Replace('.', ','));
                                dff.TextureCoordinates.Add( new TexCoords(new Vector2(x, y)) );
                                break;
                            case "f":
                                short vert1 = 0; 
                                short vert2 = 0; 
                                short vert3 = 0; 
                                if (parameters[1].Contains('/'))
                                {
                                    vert1 = Convert.ToInt16(parameters[1].Split('/')[0]);
                                    vert2 = Convert.ToInt16(parameters[2].Split('/')[0]);
                                    vert3 = Convert.ToInt16(parameters[3].Split('/')[0]);
                                }
                                else
                                {
                                    vert1 = Convert.ToInt16(parameters[1].Replace('/', ' '));
                                    vert2 = Convert.ToInt16(parameters[2].Replace('/', ' '));
                                    vert3 = Convert.ToInt16(parameters[3].Replace('/', ' '));
                                }
                                dff.TriangleIndices.Add(new TriangleIndex((short)(vert1 - 1),(short)(vert2 - 1),(short)(vert3 - 1 ), matId));
                                break;
                            case "usemtl":
                                string matName = parameters[1];
                                if (matName.StartsWith("Mat_"))
                                {
                                    matId = Convert.ToInt16(matName.Split('_')[1]);
                                }
                                else
                                {
                                    throw new InvalidOperationException(invalidMatErr);
                                }
                                break;
                        }
                    }
                    entry = text.ReadLine();
                }

                text.Close();
            }
            
            // no texture coordinates included?
            // well..
            if (dff.TextureCoordinates.Count==0)
            {
                dff.TextureCoordinates = new List<TexCoords>(dff.Vertices.Count);
                for (int id = 0; id<dff.Vertices.Count; id++)
                {
                    dff.TextureCoordinates.Add(new TexCoords(new Vector2()));
                }
            }

            dff.VerticesColors = VertexColor.WhiteList(dff.Vertices.Count);
            dff.HeaderInfo = new HeaderInfo(NewVersionGeneric, NewVersionExtra);

            string MTLfileName = Path.GetDirectoryName(filename) + @"\" + mtllibFileName;
            if (mtllibFileName != null & File.Exists(MTLfileName))
            {
                Material selectedMaterial = null;
                using (StreamReader text = File.OpenText(MTLfileName))
                {
                    string entry = text.ReadLine();
                    while (entry != null)
                    {
                        if (!entry.StartsWith("#"))
                        {
                            string[] parameters = entry.Split(' ');
                            string command = parameters[0];
                            switch (command)
                            {
                                case "newmtl":
                                    dff.Materials.Materials.Add(new Material() { Data = new MaterialDataStruct() });
                                    selectedMaterial = dff.Materials.Materials[dff.Materials.Materials.Count - 1];
                                    break;
                                // Diffuse
                                case "Kd":
                                    float dr = Convert.ToSingle(parameters[1].Replace('.', ','));
                                    float dg = Convert.ToSingle(parameters[2].Replace('.', ','));
                                    float db = Convert.ToSingle(parameters[3].Replace('.', ','));
                                    selectedMaterial.Data.MaterialColor = new Color((byte)(dr * 255), (byte)(dg * 255), (byte)(db * 255), 255);
                                    selectedMaterial.Data.Diffuse = 1f;
                                    break;
                                // Specular and Ambient
                                case "Ks":
                                    float sr = Convert.ToSingle(parameters[1].Replace('.', ','));
                                    float sg = Convert.ToSingle(parameters[2].Replace('.', ','));
                                    float sb = Convert.ToSingle(parameters[3].Replace('.', ','));
                                    selectedMaterial.Data.Specular = (sr + sg + sb)/3;
                                    break;
                                case "Ka":
                                    float ar = Convert.ToSingle(parameters[1].Replace('.', ','));
                                    float ag = Convert.ToSingle(parameters[2].Replace('.', ','));
                                    float ab = Convert.ToSingle(parameters[3].Replace('.', ','));
                                    selectedMaterial.Data.Specular = (ar + ag + ab) / 3;
                                    break;
                                // applied if ANY texture of all kinds is used
                                // (Diffuse, Specular, Ambient)
                                case "map_Kd":
                                case "map_Ks":
                                case "map_Ka":
                                    string textureName = parameters[1];
                                    selectedMaterial.Data.isTextured = true;
                                    selectedMaterial.Texture = new Texture(textureName);
                                    selectedMaterial.Texture.Structure = new TextureStruct();
                                    break;
                            }
                        }
                        entry = text.ReadLine();
                    }

                    text.Close();
                }
            }



            return dff;
        }

        public static TriangleIndex GetWhatTriangleUsesVertex(int vert, List<TriangleIndex> triangles)
        {
            foreach(TriangleIndex triangleIndex in triangles)
            {
                if (triangleIndex.Vert1 == vert)
                    return triangleIndex;
                else if (triangleIndex.Vert2 == vert)
                    return triangleIndex;
                else if (triangleIndex.Vert3 == vert)
                    return triangleIndex;
            }
            return null;
        }

        public string AsOBJ(string mtlFileName=null)
        {
            string obj = "# Automatically generated by WADExplorer\n";
            string vertices = "";
            string texcoords = "";
            string faces = "";
            if (mtlFileName!=null)
               obj += $"mtllib {mtlFileName}\n";

            vertices += $"\n# Vertices ({Vertices.Count})\n";
            foreach(Vertex vert in Vertices)
            {
                vertices += $"v {vert.Position.X:F8} {vert.Position.Y:F8} {vert.Position.Z:F8}\n".Replace(",",".");
            }
            texcoords += $"\n# Texture Coordinates ({TextureCoordinates.Count})\n";
            int texcoordandvertid = 0;
            int tex_matId = -1;
            foreach (TexCoords texcoord in TextureCoordinates)
            {
                TriangleIndex query = GetWhatTriangleUsesVertex(texcoordandvertid, TriangleIndices);
                if (query != null)
                {
                    if (tex_matId!=query.Material)
                        tex_matId = query.Material;
                }
                bool coordinatesFlippedVertically = false;
                if (tex_matId != -1) {
                    Material mat = Materials.Materials[tex_matId];
                    if (mat != null)
                    {
                        if (mat.Data.isTextured)
                        {
                            if (mat.Texture.Structure.FlippedVertically == true)
                                coordinatesFlippedVertically = true;
                        }
                    }
                }
                float x = texcoord.Coords.X;
                float y = texcoord.Coords.Y;
                if (coordinatesFlippedVertically)
                {
                    y = 1 - y;
                }

                texcoords += $"vt {x:F8} {y:F8}\n".Replace(",", ".");
                texcoordandvertid++;
            }
            faces += $"\n# There is {Materials.Data.MaterialsCount} materials from the source DDI RenderWare DFF stream";
            faces += $"\n# Triangle indices ({TriangleIndices.Count})\n";
            #region Old_Code
            /*
            int matId = -1;
            foreach (TriangleIndex tidx in TriangleIndices)
            {
                if (matId != tidx.Material) {
                    matId = tidx.Material;
                    obj += $"usemtl Mat_{matId}\n";
                }
                obj += $"f {tidx.Vert1+1} {tidx.Vert2+1} {tidx.Vert3+1}\n";
            }
            */
            #endregion
            if (Materials.Materials.Count != 0)
            {
                for (int matId = 0; matId < Materials.Materials.Count; matId++)
                {
                    faces += $"usemtl Mat_{matId}\n";

                    foreach (TriangleIndex tidx in TriangleIndices)
                    {
                        List<TriangleIndex> currentMaterialTIs = new List<TriangleIndex>();
                        if (tidx.Material == matId)
                        {
                            currentMaterialTIs.Add(tidx);
                        }

                        foreach (TriangleIndex triangle in currentMaterialTIs)
                        {
                            int vert1 = triangle.Vert1 + 1;
                            int vert2 = triangle.Vert2 + 1;
                            int vert3 = triangle.Vert3 + 1;
                            faces += $"f {vert1}/{vert1} {vert2}/{vert2} {vert3}/{vert3}\n";
                        }
                    }
                }
            }
            else
            {
                foreach (TriangleIndex tidx in TriangleIndices)
                {
                    int vert1 = tidx.Vert1 + 1;
                    int vert2 = tidx.Vert2 + 1;
                    int vert3 = tidx.Vert3 + 1;
                    faces += $"f {vert1}/{vert1} {vert2}/{vert2} {vert3}/{vert3}\n";
                }
            }

            return obj+vertices+texcoords+faces;
        }

        public string AsOBJMTL()
        {
            string mtl = "# Automatically generated by WADExplorer\n";
            int matId = 0;
            foreach (Material mat in Materials.Materials)
            {
                float R = mat.Data.MaterialColor.R / 255;
                float G = mat.Data.MaterialColor.G / 255;
                float B = mat.Data.MaterialColor.B / 255;
                float A = mat.Data.MaterialColor.A / 255;

                mtl += $"newmtl Mat_{matId}\n";

                mtl += $"Ka {R * mat.Data.Ambient:F8} {G * mat.Data.Ambient:F8} {B * mat.Data.Ambient:F8}\n";
                mtl += $"Ks {R * mat.Data.Specular:F8} {G * mat.Data.Specular:F8} {B * mat.Data.Specular:F8}\n";
                mtl += $"Kd {R * mat.Data.Diffuse:F8} {G * mat.Data.Diffuse:F8} {B * mat.Data.Diffuse:F8}\n";

                if (mat.Data.isTextured)
                {
                    mtl += $"map_Ka {mat.Texture.Name.Value.Replace("\0","")}\n";
                    mtl += $"map_Kd {mat.Texture.Name.Value.Replace("\0","")}\n";
                }

                mtl += "\n";
                matId++;
            }
            return mtl.Replace(",",".");
        }

        public DFF() { }
        public DFF(Stream stream) { Load(stream); }
    }
}
