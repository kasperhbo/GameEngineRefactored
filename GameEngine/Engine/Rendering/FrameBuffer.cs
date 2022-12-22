using OpenTK.Graphics.OpenGL;

namespace GameEngine.Engine.Rendering;

public class FrameBuffer
{
    private int fboId = 0;
    public int FBOID => fboId;
        
    private Texture texture = null;
    public int TextureID => texture.TexId;

    public FrameBuffer(int width, int height)
    {
        //Generate FrameBuffer
        fboId = GL.GenFramebuffer();
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, fboId);

        //Create the texture to render the data to
        this.texture = new Texture(width, height);
            
        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, 
            TextureTarget.Texture2D, this.texture.TexId, 0);
            
        //Create render buffer, store depth info
        int rboId = GL.GenRenderbuffer();
        GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rboId);
        GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent32, width,
            height);
            
        GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, 
            FramebufferAttachment.DepthAttachment, 
            RenderbufferTarget.Renderbuffer, 
            rboId);
            
        if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
        {
            throw new Exception("Error: framebuffer is not complete");
        }
            
        //Bind frame buffer
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }

    public void Bind()
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, fboId);
    }

    public void UnBind()
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }
}