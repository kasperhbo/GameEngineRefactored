using System.Numerics;
using Accord.Math;
using GameEngine.Engine.Components;
using GameEngine.Engine.Core;
using GameEngine.Engine.Inputs;
using GameEngine.Engine.Utilities;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Matrix4x4 = Accord.Math.Matrix4x4;
using Vector2 = System.Numerics.Vector2;
using Vector3 = OpenTK.Mathematics.Vector3;
using Vector4 = OpenTK.Mathematics.Vector4;

namespace GameEngine.Engine.Rendering
{
    /// <summary>
    /// TODO: REFACTOR THIS TO MY OWN CODE,
    /// HAD THIS LEFT FROM FOLLOWING GAMES WITH GABE TUTORIALS 
    /// </summary>
    public class RenderBatch : IComparable<RenderBatch>
    {
        private readonly int _colorOffset;
        private readonly int _colorSize = 4;
        private readonly int _maxBatchSize;
        private readonly int _positionOffset;

        //Vertex
        //======
        //Pos                       //Color                     //TexCoords     //texid
        //float, float,             float,float,float,float      float,float    float
        private readonly int _positionSize = 2;
        private readonly Shader _shader;

        private readonly SpriteRenderer[] _sprites;
        private readonly int _texCoordSize = 2;
        private readonly int _texCoorOffset;
        private readonly int _texIdOffset;
        private readonly int _texIdSize = 1;
        private readonly int[] _texSlots = { 0, 1, 2, 3, 4, 5, 6, 7 };

        private readonly int _vertexSize;
        private readonly int _vertexSizeInBytes;
        private readonly float[] _vertices;

        private readonly List<Texture> textures = new();
        private int _numSprites;

        private int _vaoId, _vboId;

        public bool HasRoom { get; private set; }
        public bool TextureRoom => textures.Count < 8;
        public int ZIndex { get; }
        
        public RenderBatch(int zIndex, int maxBatchSize = 100)
        {
            this.ZIndex = zIndex;
            _positionOffset = 0;
            _colorOffset = _positionOffset + _positionSize * sizeof(float);
            _texCoorOffset = _colorOffset + _colorSize * sizeof(float);
            _texIdOffset = _texCoorOffset + _texCoordSize * sizeof(float);

            _vertexSize = 9;
            _vertexSizeInBytes = _vertexSize * sizeof(float);

            _shader = AssetPool.GetShader(new ShaderSource("../../../default.vert", "../../../default.frag"));

            _sprites = new SpriteRenderer[maxBatchSize];

            _maxBatchSize = maxBatchSize;

            //4 vert qauds
            _vertices = new float[maxBatchSize * 4 * _vertexSize];

            _numSprites = 0;
            HasRoom = true;
        }

        

        public void Start()
        {
            //Generate and bind vao
            _vaoId = GL.GenVertexArray();
            GL.BindVertexArray(_vaoId);

            //Allocate space for verts
            _vboId = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboId);
            GL.BufferData(BufferTarget.ArrayBuffer,
                _vertices.Length * sizeof(float),
                _vertices,
                BufferUsageHint.DynamicDraw);

            //Create And Upload indices
            var eboId = GL.GenBuffer();
            var indices = GenerateIndices();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, eboId);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices,
                BufferUsageHint.StaticDraw);

            //Enable attrib pointers
            GL.VertexAttribPointer(0, _positionSize, VertexAttribPointerType.Float, false, _vertexSizeInBytes,
                _positionOffset);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, _colorSize, VertexAttribPointerType.Float, false, _vertexSizeInBytes,
                _colorOffset);
            GL.EnableVertexAttribArray(1);

            GL.VertexAttribPointer(2, _texCoordSize, VertexAttribPointerType.Float, false, _vertexSizeInBytes,
                _texCoorOffset);
            GL.EnableVertexAttribArray(2);

            GL.VertexAttribPointer(3, _texIdSize, VertexAttribPointerType.Float, false, _vertexSizeInBytes,
                _texIdOffset);
            GL.EnableVertexAttribArray(3);
        }


        public void Render()
        {
            var rebufferData = false;

            for (var i = 0; i < _numSprites; i++)
            {
                var spr = _sprites[i];

                if (spr.IsDirty)
                {
                    rebufferData = true;
                    LoadVertexProperties(i);
                    spr.IsDirty = false;
                }
            }

            if (rebufferData)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vboId);
                var ptr = IntPtr.Zero;
                GL.BufferSubData(BufferTarget.ArrayBuffer, ptr, _vertices.Length * sizeof(float), _vertices);
            }

            _shader.Use();
            
            _shader.UploadMat4F("uProjection", Window.GetInstance().CurrentScene.CurrentCamera.GetProjectionMatrix());
            _shader.UploadMat4F("uView", Window.GetInstance().CurrentScene.CurrentCamera.GetViewMatrix());
            
            for (var i = 0; i < textures.Count; i++)
            {
                GL.ActiveTexture(TextureUnit.Texture0 + i + 1);
                textures[i].Bind();
            }

            _shader.UploadIntArray("uTextures", _texSlots);

            GL.BindVertexArray(_vaoId);

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);

            GL.DrawElements(PrimitiveType.Triangles, _numSprites * 6, DrawElementsType.UnsignedInt, 0);

            // Unbind everything
            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);

            GL.BindVertexArray(0);

            for (var i = 0; i < textures.Count; i++) textures[i].Unbind();

            _shader.Detach();
        }

        public void AddSprite(SpriteRenderer spriteRenderer)
        {
            //Get index and render object
            var index = _numSprites;
            _sprites[index] = spriteRenderer;
            _numSprites++;

            if (spriteRenderer.Texture != null)
                if (!textures.Contains(spriteRenderer.Texture))
                    textures.Add(spriteRenderer.Texture);

            //Add properties to vert array
            LoadVertexProperties(index);

            if (_numSprites >= _maxBatchSize) HasRoom = false;
            // Console.WriteLine("Creating new Batch");
        }

        private void LoadVertexProperties(int index)
        {
            SpriteRenderer spriteRenderer = this._sprites[index];

            int offset = index * 4 * this._vertexSize;

            Vector4 color = new Vector4(spriteRenderer.Color.X, spriteRenderer.Color.Y, spriteRenderer.Color.Z,
                spriteRenderer.Color.W);
            Vector2[] texCoords = spriteRenderer.TextureCoords;

            int texID = 0;

            if (spriteRenderer.Texture != null)
            {
                for (int i = 0; i < this.textures.Count; i++)
                {
                    if (this.textures[i].Equals(spriteRenderer.Texture))
                    {
                        texID = i + 1;
                        break;
                    }
                }
            }

            bool rotated = spriteRenderer.Parent.Transform.Rotation.Z != 0;
            
            Transform transform = spriteRenderer.Parent.Transform;
            
            //TAKEN FROM GAMES WITH GABE TUTORIAL, Mario Engine in Java/LWJGL
            //https://www.youtube.com/c/GamesWithGabe
            //
            
            Vector2 pos = transform.Position;
            Vector2 scale = transform.Scale;
            System.Numerics.Matrix4x4 t = System.Numerics.Matrix4x4.Identity;
            t =     System.Numerics.Matrix4x4.CreateTranslation(pos.X, pos.Y,0);
            t = t * System.Numerics.Matrix4x4.CreateRotationZ(MathHelper.DegreesToRadians(transform.Rotation.Z));
            t.M41 = pos.X; 
            t.M42 = pos.Y;
            
            var m11 = t.M11 * scale.X;
            var m12 = t.M12 * scale.X;
            var m13 = t.M13 * scale.X;
            var m14 = t.M14 * scale.X;
            
            var m21 = t.M21 * scale.Y;
            var m22 = t.M22 * scale.Y;
            var m23 = t.M23 * scale.Y;
            var m24 = t.M24 * scale.Y;

            var m31 = t.M31 * 1;
            var m32 = t.M32 * 1;
            var m33 = t.M33 * 1;
            var m34 = t.M34 * 1;
            
            var m41 = t.M41;
            var m42 = t.M42;
            var m43 = t.M43;
            var m44 = t.M44;
                              
            
            t = new System.Numerics.Matrix4x4(
                m11,m12,m13,m14,
                m21,m22,m23,m24,
                m31,m32,m33,m34,
                m41,m42,m43,m44
            );
            
            Console.WriteLine(t);
            Console.WriteLine("////////////");
            // _Matrix4.CreateScale();
            

            float xAdd = 0.5f;
            float yAdd = 0.5f;
            
            for (int i=0; i < 4; i++) {
                if (i == 1) {
                    yAdd = -0.5f;
                } else if (i == 2) {
                    xAdd = -0.5f;
                } else if (i == 3) {
                    yAdd = 0.5f;
                }
                
                Vector4 currentPos = new Vector4(transform.Position.X + (xAdd * transform.Scale.X),
                    transform.Position.Y + (yAdd * transform.Scale.Y),
                    0, 1);

                if(rotated)
                    currentPos =  Input.Temp(t, new Vector4(xAdd, yAdd, 0, 1));
                
                Console.WriteLine(currentPos.X + " "  + currentPos.Y);
                Console.WriteLine(currentPos.Z + " " + currentPos.W);
                
                _vertices[offset] =     currentPos.X;
                _vertices[offset + 1] = currentPos.Y;
                
                _vertices[offset + 2] = color.X;
                _vertices[offset + 3] = color.Y;
                _vertices[offset + 4] = color.Z;
                _vertices[offset + 5] = color.W;
                
                _vertices[offset + 6] = texCoords[i].X;
                _vertices[offset + 7] = texCoords[i].Y;
                
                _vertices[offset + 8] = texID;
                
                offset += _vertexSize;
            }
            Console.WriteLine("/////////////////////////");
        }

        private int[] GenerateIndices()
        {
            //6 indices per qaud
            var elements = new int[6 * _maxBatchSize];

            for (var i = 0; i < _maxBatchSize; i++) LoadElementIndices(elements, i);

            return elements;
        }

        private void LoadElementIndices(int[] elements, int index)
        {
            var offsetArrayIndex = 6 * index;
            var offset = 4 * index;

            // 3, 2, 0, 0, 2, 1        7, 6, 4, 4, 6, 5
            // Triangle 1
            elements[offsetArrayIndex] = offset + 3;
            elements[offsetArrayIndex + 1] = offset + 2;
            elements[offsetArrayIndex + 2] = offset + 0;

            // Triangle 2
            elements[offsetArrayIndex + 3] = offset + 0;
            elements[offsetArrayIndex + 4] = offset + 2;
            elements[offsetArrayIndex + 5] = offset + 1;
        }


        public bool HasTexture(Texture tex)
        {
            return textures.Contains(tex);
        }

        

        public int CompareTo(RenderBatch other)
        {
            if (this.ZIndex < other.ZIndex)
                return -1;
            
            if (this.ZIndex == other.ZIndex)
                return 0;
            
            return 1;
        }
    }

   
}