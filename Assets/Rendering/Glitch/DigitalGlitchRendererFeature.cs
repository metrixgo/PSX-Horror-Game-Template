using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Glitch
{
    sealed class DigitalGlitchRendererFeature : ScriptableRendererFeature
    {
        [SerializeField]
        RenderPassEvent _passEvent =
          RenderPassEvent.AfterRenderingPostProcessing;

        DigitalGlitchPass _pass;

        public override void Create()
          => _pass = new DigitalGlitchPass { renderPassEvent = _passEvent };

        public override void AddRenderPasses
          (ScriptableRenderer renderer, ref RenderingData renderingData)
          => renderer.EnqueuePass(_pass);
    }

}