using GameEngine.Engine.Components;

namespace GameEngine.Engine.Rendering;

public class Renderer
{
    private List<RenderBatch> renderBatches = new();

    public void AddSprite(SpriteRenderer spriteRenderer)
    {
        Console.WriteLine("Adding sprite to renderer");
        var added = false;
        foreach (var batch in renderBatches)
            if (batch.HasRoom && batch.ZIndex == spriteRenderer.ZIndex)
            {
                var tex = spriteRenderer.Texture;
                if (tex == null || batch.HasTexture(tex) || batch.TextureRoom)
                {
                    batch.AddSprite(spriteRenderer);
                    added = true;
                    break;
                }
            }
        Console.WriteLine("Added sprite to renderer");

        if (!added)
        {
            Console.WriteLine("Creating new batch");
            var newBatch = new RenderBatch(spriteRenderer.ZIndex);
            newBatch.Start();
            
            renderBatches.Add(newBatch);
            
            newBatch.AddSprite(spriteRenderer);
            
            renderBatches.Sort();
        }
    }

    public void Render()
    {
        foreach (var batch in renderBatches) batch.Render();
    }

}