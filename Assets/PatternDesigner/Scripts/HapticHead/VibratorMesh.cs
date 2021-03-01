using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.PatternDesigner.Scripts.HapticHead
{
    /// <summary>
    ///     Represents the mesh of vibrators around the head.
    /// </summary>
    [DisallowMultipleComponent]
    public class VibratorMesh : MonoBehaviour
    {

        public static int VIBRATOR_COUNT;

        public static bool isReady;

        public Mesh _mesh;

        private GameObject actuatorHolder;

        [SerializeField] private bool enableCollider;

        [SerializeField] private bool enableMesh;

        [SerializeField] private int[] triangles;

        public Vibrator[] vibrators { get; private set; }

        private void Awake()
        {
            //All the actuators used on the model must be in the ActuatorHolder.
            actuatorHolder = GameObject.Find("ActuatorHolder");
            if (actuatorHolder == null)
            {
                print("No ActuatorHolder GameObject found in scene, please create one (empty GameObject)");
                return;
            }


            vibrators = GetComponentsInChildren<Vibrator>();
            if (gameObject.transform.parent != null)
            {
                print("Found ActuatorHolder as child of " + gameObject.transform.parent.name);
                if (gameObject.transform.parent.name != "PaintMode")
                {
                    //snapVibratorsToMesh();
                }
            }
                       
            
            Array.Sort(vibrators, (x, y) => x.id - y.id);

            VIBRATOR_COUNT = vibrators.Length;
            
            if (enableMesh)
            {
                var vertices = new Vector3[vibrators.Length];

                for (var i = 0; i < vibrators.Length; i++)
                {
                    vibrators[i].gameObject.name = "Actuator " + vibrators[i].id;
                    vertices[i] = transform.InverseTransformPoint(vibrators[i].transform.position);
                }

                _mesh = new Mesh { vertices = vertices, triangles = triangles };
                _mesh.RecalculateNormals();
                AddRenderer();
            }

            if (enableCollider)
                AddCollider();

        }


        /// <summary>
        /// Snaps the vibrators to the mesh. Mesh must be in Interface/Model layer.
        /// </summary>
        public void snapVibratorsToMesh()
        {
            const int MAXRAYS = 10000;
            foreach (var t in vibrators)
            {
                var pos = new Vector3(0,0,0);
                var minDist = float.MaxValue;
                var vibratorPosition = t.transform.position;
                for (int i = 0; i < MAXRAYS; i++)
                {
                    Vector3 myRandomDirection = new Vector3(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value).normalized;

                    //Debug.DrawRay(vibratorPosition, myRandomDirection, Color.blue, 30, false);

                    Physics.queriesHitBackfaces = true;
                    var ray = new Ray(vibratorPosition, myRandomDirection);

                    if (Physics.Raycast(ray, out var hit, 10f, LayerMask.GetMask("Interface/Model")))
                    {
                        float currentDistance = Vector3.Distance(hit.point, vibratorPosition);
                        if (minDist > currentDistance)
                        {
                            minDist = currentDistance;
                            pos = hit.point;
                            //print("New record for vibrator " + t.id + " from pos " + vibratorPosition.ToString() + " to pos " + pos.ToString());
                            //Debug.DrawLine(vibratorPosition, pos, Color.red, 600, false);
                        }
                    }
                    //now reverse the ray to detect backfacing normals
                    ray.origin = ray.GetPoint(5);
                    ray.direction = -ray.direction;
                    if (Physics.Raycast(ray, out var hit2, 10f, LayerMask.GetMask("Interface/Model")))
                    {
                        float currentDistance = Vector3.Distance(hit2.point, vibratorPosition);
                        if (minDist > currentDistance)
                        {
                            minDist = currentDistance;
                            pos = hit2.point;
                            //print("New record (rev) for vibrator " + t.id + " from pos " + vibratorPosition.ToString() + " to pos " + pos.ToString());
                            //Debug.DrawLine(vibratorPosition, pos, Color.red, 600, false);
                        }
                    }
                }

                //Debug.DrawLine(vibratorPosition, pos, Color.green, 600, false);
                //print("Setting vibrator " + t.id + " from pos " + vibratorPosition.ToString() + " to pos " + pos.ToString());
                t.transform.position = pos;
            }

            isReady = true;
        }

        private void AddRenderer()
        {
            var filter = gameObject.AddComponent<MeshFilter>();
            filter.sharedMesh = _mesh;

            var renderer = gameObject.AddComponent<MeshRenderer>();
            renderer.material = new Material(Shader.Find("Legacy Shaders/Diffuse"));
        }

        private void AddCollider()
        {
            var collider = gameObject.AddComponent<MeshCollider>();
            collider.sharedMesh = _mesh;
        }

        //Triangles IDs to calculate which vibrators to activate. Only really needed when using the triangle evaluator. Default is ClosestEvaluator, therefore not needed by default.
        public int[] GetIdTriangle(int index)
        {
            var start = index * 3;

            return new[]
            {
                triangles[start],
                triangles[start + 1],
                triangles[start + 2]
            };
        }

        /// <summary>
        ///     Get count closest vibrators to position
        /// </summary>
        /// <param name="position">Position which should be stimulated by vibrators</param>
        /// <param name="count">Returns count nearast vibrators</param>
        /// <returns>Returns KeyValuePairs with distances from position to vibrator and the count nearest vibrator itself</returns>
        public List<KeyValuePair<float, Vibrator>> GetClosestVibrators(Vector3 position, int count)
        {
            var list = new SortedList<float, Vibrator>();

            foreach (var vibrator in vibrators)
            {
                var key = Vector3.Distance(position, vibrator.transform.position);
                //print("Distance from vibrator " + vibrator.id + " to position " + position + " is " + key);
                if (!list.ContainsKey(key))
                    list.Add(key, vibrator);
            }
            //print(list.Keys[0] + " " + list.Values[0].id + " " + list.Keys[1] + " " + list.Values[1].id + " " + list.Keys[2] + " " + list.Values[2].id);
            return list.Take(count).ToList();
        }
    }
}