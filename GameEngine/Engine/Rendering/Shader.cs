using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace GameEngine.Engine.Rendering
{
    public class Shader
    {
        private readonly string fragmentSource;

        private readonly string vertexSource;
        private string filepath;
        private int shaderProgramId;

        public Shader(string vertexFilePath, string fragmentFilePath)
        {
            vertexSource = File.ReadAllText(vertexFilePath);
            fragmentSource = File.ReadAllText(fragmentFilePath);
        }

        public void Compile()
        {
            int vertexId, fragmentId;

            vertexId = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexId, vertexSource);
            GL.CompileShader(vertexId);
            GL.GetShader(vertexId, ShaderParameter.CompileStatus, out var succes);
            if (succes != (int)All.True)
            {
                // We can use `GL.GetShaderInfoLog(shader)` to get information about the error.
                var infoLog = GL.GetShaderInfoLog(vertexId);
                throw new Exception($"Error occurred whilst compiling Shader({vertexId}).\n\n{infoLog}");
            }

            fragmentId = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentId, fragmentSource);
            GL.CompileShader(fragmentId);

            GL.GetShader(fragmentId, ShaderParameter.CompileStatus, out succes);
            if (succes != (int)All.True)
            {
                // We can use `GL.GetShaderInfoLog(shader)` to get information about the error.
                var infoLog = GL.GetShaderInfoLog(fragmentId);
                throw new Exception($"Error occurred whilst compiling Shader({fragmentId}).\n\n{infoLog}");
            }

            shaderProgramId = GL.CreateProgram();
            GL.AttachShader(shaderProgramId, vertexId);
            GL.AttachShader(shaderProgramId, fragmentId);
            GL.LinkProgram(shaderProgramId);

#pragma warning disable CS0618 // Type or member is obsolete
            GL.GetProgram(shaderProgramId, ProgramParameter.LinkStatus, out succes);
#pragma warning restore CS0618 // Type or member is obsolete

            if (succes != (int)All.True)
                // We can use `GL.GetProgramInfoLog(program)` to get information about the error.
                throw new Exception($"Error occurred whilst linking Program({shaderProgramId})");
        }

        public void Use()
        {
            GL.UseProgram(shaderProgramId);
        }

        public void Detach()
        {
            GL.UseProgram(0);
        }

        public void UploadMat4F(string varName, Matrix4 mat4)
        {
            Use();
            var varLocation = GL.GetUniformLocation(shaderProgramId, varName);
            GL.UniformMatrix4(varLocation, false, ref mat4);
        }

        public void UploadMat3F(string varName, Matrix3 mat3)
        {
            Use();
            var varLocation = GL.GetUniformLocation(shaderProgramId, varName);

            GL.UniformMatrix3(varLocation, false, ref mat3);
        }

        public void UploadVec4F(string varName, Vector4 vec)
        {
            Use();
            var varLocation = GL.GetUniformLocation(shaderProgramId, varName);

            GL.Uniform4(varLocation, vec);
        }

        public void UploadVec3F(string varName, Vector3 vec)
        {
            Use();
            var varLocation = GL.GetUniformLocation(shaderProgramId, varName);

            GL.Uniform3(varLocation, vec);
        }

        public void UploadVec2F(string varName, Vector2 vec)
        {
            Use();
            var varLocation = GL.GetUniformLocation(shaderProgramId, varName);

            GL.Uniform2(varLocation, vec);
        }

        public void UploadFloat(string varName, float val)
        {
            Use();
            var varLocation = GL.GetUniformLocation(shaderProgramId, varName);

            GL.Uniform1(varLocation, val);
        }

        public void UploadInt(string varName, int val)
        {
            Use();
            var varLocation = GL.GetUniformLocation(shaderProgramId, varName);

            GL.Uniform1(varLocation, val);
        }

        public void UploadIntArray(string varName, int[] data)
        {
            Use();
            var varLocation = GL.GetUniformLocation(shaderProgramId, varName);
            GL.Uniform1(varLocation, data.Length, data);
        }

        public void UploadTexture(string varName, int slot)
        {
            Use();
            var varLocation = GL.GetUniformLocation(shaderProgramId, varName);

            GL.Uniform1(varLocation, slot);
        }
    }
}