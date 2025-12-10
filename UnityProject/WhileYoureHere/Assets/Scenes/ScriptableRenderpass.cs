using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

// Ensure this namespace matches your file structure
namespace CustomEffects
{
    public class EdgeDetectionRendererFeature : ScriptableRendererFeature
    {
        private EdgeDetectionPass _renderPass;
        public Shader edgeDetectionShader;
        public Color edgeColor = Color.black;
        [Range(0.0f, 1.0f)]
        public float threshold = 0.1f;

        public override void Create()
        {
            if (edgeDetectionShader == null) return;

            _renderPass = new EdgeDetectionPass(edgeDetectionShader)
            {
                // This event injects the pass after the opaque objects are drawn
                renderPassEvent = RenderPassEvent.AfterRenderingOpaques
            };
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (_renderPass == null) return;
            
            // Pass the current settings from the feature GUI to the pass material
            _renderPass.Setup(edgeColor, threshold);
            renderer.EnqueuePass(_renderPass);
        }
    }

    public class EdgeDetectionPass : ScriptableRenderPass
    {
        private readonly Material _material;

        public EdgeDetectionPass(Shader shader)
        {
            _material = CoreUtils.CreateEngineMaterial(shader);
        }

        public void Setup(Color color, float threshold)
        {
            if (_material != null)
            {
                _material.SetColor("_EdgeColor", color);
                _material.SetFloat("_Threshold", threshold);
            }
        }
        
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (_material == null) return;

            const string renderPassName = "EdgeDetectionPass";
            
            var resourceData = frameData.Get<UniversalResourceData>();
            // This is the source texture we are reading from
            var sourceTexture = resourceData.activeColorTexture;
            
            // Define a new temporary texture for our output (the destination)
            var destinationDescriptor = renderGraph.GetTextureDesc(sourceTexture); 
            destinationDescriptor.name = $"CameraColor-{renderPassName}-Destination";
            destinationDescriptor.clearBuffer = false;
            var destinationTexture = renderGraph.CreateTexture(destinationDescriptor);
            
            // Blit from source to destination using our material
            RenderGraphUtils.BlitMaterialParameters parameters = new(sourceTexture, destinationTexture, _material, 0);
            renderGraph.AddBlitPass(parameters, renderPassName);
            
            // CRITICAL STEP: Reassign the result as the main color buffer
            resourceData.cameraColor = destinationTexture;
        }
    }
}
