using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DistortionRendererFeatureDemo : ScriptableRendererFeature
{
    class DistortionRenderPass : ScriptableRenderPass
    {
        private int tempRTID, targetTextureID;
        private bool isRenderingObject;
        private ShaderTagId shaderTagID;

        public DistortionRenderPass(DistortionSettings settings)
        {
            renderPassEvent = settings.renderPassEvent;
            targetTextureID = Shader.PropertyToID(settings.textureID);
            tempRTID = Shader.PropertyToID("_TempRT");
            isRenderingObject = settings.isRenderingObject;
            shaderTagID = new ShaderTagId(settings.shaderTagID);
        }

        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in an performance manner.
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            cmd.GetTemporaryRT(tempRTID, cameraTextureDescriptor);
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get();
            cmd.Blit(BuiltinRenderTextureType.CameraTarget, tempRTID);
            cmd.SetGlobalTexture(targetTextureID, tempRTID);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

            if(isRenderingObject)
            {
                DrawingSettings drawingSettings = CreateDrawingSettings(shaderTagID, ref renderingData, SortingCriteria.CommonTransparent);
                FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.transparent);
                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
            }
        }

        /// Cleanup any allocated resources that were created during the execution of this render pass.
        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(tempRTID);
        }
    }

    private DistortionRenderPass distortionRenderPass;
    [SerializeField] private DistortionSettings settings;

    [Serializable]
    private class DistortionSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        public string textureID = "_DistortionTexture";
        public bool isRenderingObject = false;
        public string shaderTagID = "DistortionPass";
    }


    public override void Create()
    {
        distortionRenderPass = new DistortionRenderPass(settings);

        // Configures where the render pass should be injected.
        //distortionRenderPass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(distortionRenderPass);
    }
}


