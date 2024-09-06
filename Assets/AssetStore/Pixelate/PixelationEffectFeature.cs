using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PixelationEffectFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class PixelationEffectSettings
    {
        public RenderPassEvent Event = RenderPassEvent.AfterRenderingTransparents;
        public PixelationEffect pixelationEffect;
        public Material material;
    }

    public PixelationEffectSettings settings = new PixelationEffectSettings();
    PixelationEffectPass _pixelationEffectPass;

    public override void Create()
    {
        _pixelationEffectPass = new PixelationEffectPass();
        _pixelationEffectPass.renderPassEvent = settings.Event;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.pixelationEffect == null) return;

        Camera camera = renderingData.cameraData.camera;
        

        _pixelationEffectPass.Setup(renderer.cameraColorTarget, RenderTargetHandle.CameraTarget, settings.material, settings.pixelationEffect);
        renderer.EnqueuePass(_pixelationEffectPass);
    }
}

class PixelationEffectPass : ScriptableRenderPass
{
    private RenderTargetIdentifier source { get; set; }
    private RenderTargetHandle destination { get; set; }

    private Material _material;
    private PixelationEffect _settings;

    public PixelationEffectPass()
    {
        renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    }

    public void Setup(RenderTargetIdentifier source, RenderTargetHandle destination, Material material, PixelationEffect settings)
    {
        this.source = source;
        this.destination = destination;
        this._material = material;
        this._settings = settings;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (_material == null) return;

        int pixelSize = _settings.pixelSize.value;
        float widthMod = 1.0f / (float)renderingData.cameraData.camera.pixelWidth;
        float heightMod = 1.0f / (float)renderingData.cameraData.camera.pixelHeight;

        _material.SetInt("_PixelSize", pixelSize);
        _material.SetFloat("_WidthMod", widthMod);
        _material.SetFloat("_HeightMod", heightMod);

        CommandBuffer cmd = CommandBufferPool.Get("PixelationEffect");
        cmd.Blit(source, destination.Identifier(), _material, 0);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
}
