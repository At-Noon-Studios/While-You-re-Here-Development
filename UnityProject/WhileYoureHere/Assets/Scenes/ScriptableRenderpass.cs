using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace Scenes
{
    public class EdgeDetectionRendererFeature : ScriptableRendererFeature
    {
        private EdgeDetectionPass _renderPass;
        public Shader edgeDetectionShader;
        
        public Color colorOutlineColor = Color.black;
        public Color depthOutlineColor = Color.black;
        public Color normalOutlineColor = Color.black;
        [Range(0.0f, 1.0f)]
        public float colorThreshold = 0.5f;
        [Range(0.0f, 10.0f)]
        public float depthThreshold = 0.5f;
        [Range(0.0f, 10.0f)]
        public float normalThreshold = 0.5f;
        
        public override void Create()
        {
            if (edgeDetectionShader == null) return;

            _renderPass = new EdgeDetectionPass(edgeDetectionShader)
            {
                renderPassEvent = RenderPassEvent.AfterRenderingOpaques
            };
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (_renderPass == null) return;
            
            _renderPass.Setup(colorOutlineColor, depthOutlineColor, normalOutlineColor, colorThreshold, depthThreshold,  normalThreshold);
            renderer.EnqueuePass(_renderPass);
        }
    }
    
    public class EdgeDetectionPass : ScriptableRenderPass
    {
        private static readonly int DepthThreshold = Shader.PropertyToID("_DepthThreshold");
        private static readonly int ColorThreshold = Shader.PropertyToID("_ColorThreshold");
        private static readonly int NormalThreshold = Shader.PropertyToID("_NormalThreshold");
        private static readonly int DepthOutlineColor = Shader.PropertyToID("_DepthOutlineColor");
        private static readonly int ColorOutlineColor = Shader.PropertyToID("_ColorOutlineColor");
        private static readonly int NormalOutlineColor = Shader.PropertyToID("_NormalOutlineColor");
        private readonly Material _material;

        public EdgeDetectionPass(Shader shader)
        {
            _material = CoreUtils.CreateEngineMaterial(shader);
        }

        public void Setup(Color colorOutlineColor, Color depthOutlineColor, Color normalOutlineColor, float colorThreshold, float depthThreshold, float normalThreshold)
        {
            if (_material == null) return;
            _material.SetColor(ColorOutlineColor, colorOutlineColor);
            _material.SetColor(DepthOutlineColor, depthOutlineColor);
            _material.SetColor(NormalOutlineColor, normalOutlineColor);
            _material.SetFloat(ColorThreshold, colorThreshold);
            _material.SetFloat(DepthThreshold, depthThreshold);
            _material.SetFloat(NormalThreshold, normalThreshold);
        }
        
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (_material == null) return;

            const string renderPassName = "EdgeDetectionPass";
            
            var resourceData = frameData.Get<UniversalResourceData>();
            var sourceTexture = resourceData.activeColorTexture;
            
            var destinationDescriptor = renderGraph.GetTextureDesc(sourceTexture); 
            destinationDescriptor.name = $"CameraColor-{renderPassName}-Destination";
            destinationDescriptor.clearBuffer = false;
            var destinationTexture = renderGraph.CreateTexture(destinationDescriptor);
            
            RenderGraphUtils.BlitMaterialParameters parameters = new(sourceTexture, destinationTexture, _material, 0);
            renderGraph.AddBlitPass(parameters, renderPassName);
            
            resourceData.cameraColor = destinationTexture;
        }
    }
}
