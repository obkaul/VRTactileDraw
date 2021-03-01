using System.Linq;
using UnityEngine;

namespace Assets.PatternDesigner.Scripts.HapticHead
{
    /// <summary>
    ///     Applies intensities to the head using the Intensity shader. Basically a "heat map"
    /// </summary>
    [DisallowMultipleComponent]
    public class IntensityDisplay : MonoBehaviour
    {

        private Material _material;

        private MeshRenderer _meshRenderer;

        private bool _isInit;

        [SerializeField] private VibratorMesh vibratorMesh;



        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _material = new Material(Shader.Find("Interface/Intensity"));
        }

        private void Update()
        {
            if (!_isInit && VibratorMesh.isReady)
            {
                _meshRenderer.material = _material;

                //vibrator positions
                var positions = vibratorMesh.vibrators.Select(v => (Vector4)v.transform.position).ToArray();

                _material.SetInt("MOTOR_COUNT", VibratorMesh.VIBRATOR_COUNT);
                _material.SetVectorArray("_Positions", positions);
                               
                _isInit = true;
            }
        }

        public void Set(float[] intensities)
        {
            if (_isInit)
            {
                //intensities depending on the strokes
                _material.SetFloatArray("_Intensities", intensities);

                //var positions = vibratorMesh.vibrators.Select(v => (Vector4)v.transform.position).ToArray();
                //_material.SetVectorArray("_Positions", positions);
            }
        }
    }
}