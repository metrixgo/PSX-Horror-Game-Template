using UnityEngine;
using UnityEngine.Rendering;
using ShaderIDs = Glitch.ShaderPropertyIDs;

namespace Glitch
{

    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Glitch/Analog Glitch Controller")]
    public sealed class AnalogGlitchController : MonoBehaviour
    {
        public static AnalogGlitchController Instance { get; private set; }

        [field: SerializeField, Range(0, 1)]
        public float ScanLineJitter { get; set; }

        [field: SerializeField, Range(0, 1)]
        public float VerticalJump { get; set; }

        [field: SerializeField, Range(0, 1)]
        public float HorizontalShake { get; set; }

        [field: SerializeField, Range(0, 1)]
        public float ColorDrift { get; set; }

        [field: SerializeField, Range(0, 1)]
        public float HorizontalRipple { get; set; }

        [SerializeField, HideInInspector] Shader _shader = null;

        Material _material;
        float _jumpTime;

        void ReleaseResources()
        {
            CoreUtils.Destroy(_material);
            _material = null;
        }

        public bool IsActive
          => ScanLineJitter > 0 || VerticalJump > 0 ||
             HorizontalShake > 0 || ColorDrift > 0 ||
             HorizontalRipple > 0;

        public Material UpdateMaterial()
        {
            if (_material == null) _material = CoreUtils.CreateEngineMaterial(_shader);

            _material.SetFloat(ShaderIDs.ScanLineJitter, ScanLineJitter * 0.05f);

            var jump = new Vector2(VerticalJump, _jumpTime);
            _material.SetVector(ShaderIDs.VerticalJump, jump);

            var shake = (Random.value * 2 - 1) * HorizontalShake * 0.1f;
            _material.SetFloat(ShaderIDs.HorizontalShake, shake);

            _material.SetFloat(ShaderIDs.ColorDrift, ColorDrift);
            _material.SetFloat(ShaderIDs.HorizontalRipple, HorizontalRipple);

            return _material;
        }

        void OnEnable() => Instance = this;

        void OnDestroy() => ReleaseResources();

        void OnDisable() => ReleaseResources();

        void Update() => _jumpTime = (_jumpTime + Time.deltaTime * VerticalJump * 11.3f) % 600;
    }

}