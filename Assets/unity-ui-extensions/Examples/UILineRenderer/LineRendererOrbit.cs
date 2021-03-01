using System.Collections.Generic;
using Scripts.Utilities;
using UnityEngine;

namespace Examples.UILineRenderer
{
    [RequireComponent(typeof(Scripts.Primitives.UILineRenderer))]
    public class LineRendererOrbit : MonoBehaviour
    {
        private Scripts.Primitives.UILineRenderer lr;
        private Circle circle;
        public GameObject OrbitGO;
        private RectTransform orbitGOrt;
        private float orbitTime;

        [SerializeField]
        private float _xAxis = 3;

        public float xAxis
        {
            get { return _xAxis; }
            set { _xAxis = value; GenerateOrbit(); }
        }

        [SerializeField]
        private float _yAxis = 3;

        public float yAxis
        {
            get { return _yAxis; }
            set { _yAxis = value; GenerateOrbit(); }
        }

        [SerializeField]
        private int _steps = 10;

        public int Steps
        {
            get { return _steps; }
            set { _steps = value; GenerateOrbit(); }
        }



        // Use this for initialization
        private void Awake()
        {
            lr = GetComponent<Scripts.Primitives.UILineRenderer>();
            orbitGOrt = OrbitGO.GetComponent<RectTransform>();
            GenerateOrbit();
        }

        // Update is called once per frame
        private void Update()
        {
            orbitTime = orbitTime > _steps ? orbitTime = 0 : orbitTime + Time.deltaTime;
            orbitGOrt.localPosition = circle.Evaluate(orbitTime);
        }

        private void GenerateOrbit()
        {
            circle = new Circle(xAxis: _xAxis, yAxis: _yAxis, steps: _steps);
            var Points = new List<Vector2>();
            for (var i = 0; i < _steps; i++)
            {
                Points.Add(circle.Evaluate(i));
            }
            Points.Add(circle.Evaluate(0));
            lr.Points = Points.ToArray();
        }

        private void OnValidate()
        {
            if (lr != null)
            {
                GenerateOrbit();
            }
        }
    }
}