using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace Scenes
{
    public class OutlineRendererFeature : ScriptableRendererFeature
    {
        private OutlineRenderPass _renderPass;
        public Shader shader;
        
        public override void Create()
        {
            _renderPass = new OutlineRenderPass(shader)
            {
                renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing
            };
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(_renderPass);
        }
    }

    public class OutlineRenderPass : ScriptableRenderPass
    {
  private readonly Material _material;
    
    public OutlineRenderPass(Shader shader)
    {
        _material = new Material(shader);
        requiresIntermediateTexture = true;
    }
    
    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        var stack = VolumeManager.instance.stack;
        // var customPostProcessingEffectComponent = stack.GetComponent<CustomPostProcessingEffect>();
        // if (!customPostProcessingEffectComponent || !customPostProcessingEffectComponent.IsActive()) return;
        // _material.SetFloat("_Strength", customPostProcessingEffectComponent.strength.value);
        // _material.SetFloat("_FogAmount", customPostProcessingEffectComponent.fogAmount.value);
        // _material.SetColor("_Color", customPostProcessingEffectComponent.color.value);
        // _material.SetFloat("_FogDensity", customPostProcessingEffectComponent.fogDensity.value);
        // _material.SetFloat("_FogStart", customPostProcessingEffectComponent.fogStart.value);
        // _material.SetFloat("_FogEnd", customPostProcessingEffectComponent.fogEnd.value);
        // _material.SetFloat("_FogSpeed", customPostProcessingEffectComponent.fogSpeed.value);
        // _material.SetFloat("_FogCloudDensity", customPostProcessingEffectComponent.fogCloudDensity.value);
        
        const string renderPassName = "nameof(CustomPostProcessingEffectPass)";
        var resourceData = frameData.Get<UniversalResourceData>();
        if (resourceData.isActiveTargetBackBuffer)
        {
            Debug.LogError("Skipping render pass. CustomPostProcessingEffectPass requires an intermediate render texture.");
            return;
        }
        var intermediateTexture = resourceData.activeColorTexture;
        
        var destinationDescriptor = renderGraph.GetTextureDesc(intermediateTexture); //Create copy of source descriptor to turn into destination texture
        destinationDescriptor.name = $"CameraColor-{renderPassName}";
        destinationDescriptor.clearBuffer = false;
        var destination = renderGraph.CreateTexture(destinationDescriptor);

        RenderGraphUtils.BlitMaterialParameters parameters = new(intermediateTexture, destination, _material, 0);
        renderGraph.AddBlitPass(parameters, renderPassName);

        resourceData.cameraColor = destination;
    }
    }
}