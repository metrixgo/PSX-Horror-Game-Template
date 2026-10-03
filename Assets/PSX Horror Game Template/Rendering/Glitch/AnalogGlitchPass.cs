using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace Glitch
{

    sealed class AnalogGlitchPass : ScriptableRenderPass
    {
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var cameraData = frameData.Get<UniversalCameraData>();
            if (!cameraData.resolveFinalTarget) return;
            var controller = AnalogGlitchController.Instance;
            if (controller == null || !controller.enabled || !controller.IsActive) return;

            var resourceData = frameData.Get<UniversalResourceData>();
            if (resourceData.isActiveTargetBackBuffer) return;

            var source = resourceData.activeColorTexture;
            if (!source.IsValid()) return;

            var desc = renderGraph.GetTextureDesc(source);
            desc.name = "_AnalogGlitchColor";
            desc.clearBuffer = false;
            desc.depthBufferBits = 0;
            var dest = renderGraph.CreateTexture(desc);

            var mat = controller.UpdateMaterial();
            var param = new RenderGraphUtils.BlitMaterialParameters(source, dest, mat, 0);
            renderGraph.AddBlitPass(param, passName: "Glitch (Analog)");

            resourceData.cameraColor = dest;
        }
    }

}