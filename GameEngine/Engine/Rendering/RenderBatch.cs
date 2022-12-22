using System.Numerics;
using GameEngine.Engine.Components;
using GameEngine.Engine.Core;
using GameEngine.Engine.Inputs;
using GameEngine.Engine.Utilities;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Vector2 = System.Numerics.Vector2;
using Vector3 = OpenTK.Mathematics.Vector3;
using Vector4 = OpenTK.Mathematics.Vector4;

namespace GameEngine.Engine.Rendering
{
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
        
        public RenderBatch(int zIndex, int maxBatchSize = 10000)
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
            // var sprite = _sprites[index];
            //
            // //Find offset in array
            // var offset = index * 4 * _vertexSize;
            //
            // float xAdd = 1;
            // float yAdd = 1;
            //
            // var color = sprite.Color;
            // var texCoords = sprite.TextureCoords;
            //
            // var texId = 0;
            //
            // //Find texture in textures list
            // if (sprite.Texture != null)
            //     for (var i = 0; i < textures.Count; i++)
            //         if (textures[i].Equals(sprite.Texture))
            //         {
            //             texId = i + 1;
            //             break;
            //         }
            //
            // Transform transform = sprite.Parent.Transform;
            // float rotZ = transform.Rotation.Z;
            //
            // bool isRotated = rotZ != 0.0f;
            //
            // Matrix4 transformMatrix = Matrix4.Identity;
            //
            // if (isRotated)
            // {
            //     Console.WriteLine("is Rotated");
            //     transformMatrix = transformMatrix * Matrix4.CreateTranslation(transform.Position.X, transform.Position.Y, 0f);    
            //     transformMatrix = transformMatrix * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotZ));
            //     transformMatrix = transformMatrix * Matrix4.CreateScale(transform.Scale.X, transform.Scale.Y, 0);
            // }
            //
            // for (var i = 0; i < 4; i++)
            // {
            //     if (i == 1)
            //         yAdd = 0.0f;
            //     else if (i == 2)
            //         xAdd = 0.0f;
            //     else if (i == 3)
            //         yAdd = 1.0f;
            //     
            //     
            //
            //     
            //     // sprite.Parent.Transform.Position.X +
            //     //     xAdd * sprite.Parent.Transform.Scale.X;
            //     //
            //     // sprite.Parent.Transform.Position.Y +
            //     //     yAdd * sprite.Parent.Transform.Scale.Y;
            //     
            //     Vector4 currentPos = new Vector4(sprite.Parent.Transform.Position.X + (xAdd * sprite.Parent.Transform.Scale.X),
            //         sprite.Parent.Transform.Position.Y + (yAdd * sprite.Parent.Transform.Scale.Y),
            //         0, 1);
            //     
            //     if (isRotated)
            //     {
            //         currentPos = Vector4.Transform(currentPos, transformMatrix);
            //     }
            //
            //     // uf*()
            //     
            //     _vertices[offset] = sprite.Parent.Transform.Position.X +
            //              xAdd * sprite.Parent.Transform.Scale.X;
            //     
            //        //Calculate transformation matrix
            //    
            //         //
            //         // sprite.Parent.Transform.Position.Y +
            //         //     yAdd * sprite.Parent.Transform.Scale.Y;
            //                         
            //
            //     _vertices[offset + 1] =                      sprite.Parent.Transform.Position.Y +
            //              yAdd * sprite.Parent.Transform.Scale.Y;
            //
            //     //Load color
            //     _vertices[offset + 2] = color.X;
            //     _vertices[offset + 3] = color.Y;
            //     _vertices[offset + 4] = color.Z;
            //     _vertices[offset + 5] = color.W;
            //
            //     //load tex coord
            //     _vertices[offset + 6] = texCoords[i].X;
            //     _vertices[offset + 7] = texCoords[i].Y;
            //
            //     //load tex id
            //     _vertices[offset + 8] = texId;
            //
            //     offset += _vertexSize;
            // }
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
            Matrix4 transformMatrix = Matrix4.Identity;
            
            if (rotated)
            {
                Transform transform = spriteRenderer.Parent.Transform;
                
                transformMatrix = transformMatrix * Matrix4.CreateTranslation(
                    transform.Position.X, transform.Position.Y,0
                    );
                
                transformMatrix = transformMatrix * Matrix4.CreateFromAxisAngle(new Vector3(0, 0, 1), 0f);
                
                transformMatrix = transformMatrix * Matrix4.CreateScale(transform.Scale.X, transform.Scale.Y, 1);
            }
            
            float xAdd = .5f;
            float yAdd = .5f;

            for (int i = 0; i < 4; i++)
            {
                if (i == 1)
                {
                    yAdd = -0.5f;
                }else if (i ==2)
                {
                    xAdd = -0.5f;
                }else if (i == 3)
                {
                    yAdd = 0.5f;
                }
                
                Vector4 currentPos = new Vector4(spriteRenderer.Parent.Transform.Position.X + (xAdd * spriteRenderer.Parent.Transform.Scale.X),
                    spriteRenderer.Parent.Transform.Position.Y + (yAdd * spriteRenderer.Parent.Transform.Scale.Y),
                    0, 1);
                
                if (rotated) {
                    currentPos =  Input.Temp(transformMatrix, new Vector4(xAdd, yAdd, 0, 1));
                    _vertices[offset] = currentPos.X;
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
            }

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