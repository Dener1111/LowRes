using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.LWRP;
using UnityEngine.Experimental.Rendering.LWRP;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;

public class LowResPass : ScriptableRenderPass
{
    static readonly string k_RenderTag = "Render LowRes Effects";
    static readonly int MainTexId = Shader.PropertyToID("_MainTex");
    static readonly int TempTargetId = Shader.PropertyToID("LowRes");

    static readonly int HeightId = Shader.PropertyToID("_Height");
    static readonly int WidthId = Shader.PropertyToID("_Width");

    LowRes lowRes;
    Material lowResMaterial;
    RenderTargetIdentifier currentTarget;

    public LowResPass(RenderPassEvent evt)
    {
        renderPassEvent = evt;
        var shader = Shader.Find("PostEffect/LowRes");
        if (shader == null)
        {
            Debug.LogError("Shader not found.");
            return;
        }
        lowResMaterial = CoreUtils.CreateEngineMaterial(shader);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (lowResMaterial == null)
        {
            Debug.LogError("Material not created.");
            return;
        }

        if (!renderingData.cameraData.postProcessEnabled) return;

        var stack = VolumeManager.instance.stack;
        lowRes = stack.GetComponent<LowRes>();
        if (lowRes == null) { return; }
        if (!lowRes.IsActive()) { return; }

        var cmd = CommandBufferPool.Get(k_RenderTag);
        Render(cmd, ref renderingData);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public void Setup(in RenderTargetIdentifier currentTarget)
    {
        this.currentTarget = currentTarget;
    }

    void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        ref var cameraData = ref renderingData.cameraData;
        var source = currentTarget;
        int destination = TempTargetId;

        float ratio = ((float)cameraData.camera.scaledPixelWidth) / (float)cameraData.camera.scaledPixelHeight;

        var w = cameraData.camera.scaledPixelWidth;
        var h = cameraData.camera.scaledPixelHeight;
         
        lowResMaterial.SetInt(HeightId, (int)lowRes.height);
        lowResMaterial.SetInt(WidthId, Mathf.RoundToInt((int)lowRes.height * ratio));

        int shaderPass = 0;
        cmd.SetGlobalTexture(MainTexId, source);
        cmd.GetTemporaryRT(destination, w, h, 0, FilterMode.Point, RenderTextureFormat.Default);
        cmd.Blit(source, destination);
        cmd.Blit(destination, source, lowResMaterial, shaderPass);
    }
}
