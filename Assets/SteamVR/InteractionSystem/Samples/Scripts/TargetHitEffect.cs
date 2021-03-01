//======= Copyright (c) Valve Corporation, All rights reserved. ===============

using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
    public class TargetHitEffect : MonoBehaviour
    {
        public Collider targetCollider;

        public GameObject spawnObjectOnCollision;

        public bool colorSpawnedObject = true;

        public bool destroyOnTargetCollision = true;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider == targetCollider)
            {
                var contact = collision.contacts[0];
                RaycastHit hit;

                var backTrackLength = 1f;
                var ray = new Ray(contact.point - (-contact.normal * backTrackLength), -contact.normal);
                if (collision.collider.Raycast(ray, out hit, 2))
                {
                    if (colorSpawnedObject)
                    {
                        var renderer = collision.gameObject.GetComponent<Renderer>();
                        var tex = (Texture2D)renderer.material.mainTexture;
                        var color = tex.GetPixelBilinear(hit.textureCoord.x, hit.textureCoord.y);

                        if (color.r > 0.7f && color.g > 0.7f && color.b < 0.7f)
                            color = Color.yellow;
                        else if (Mathf.Max(color.r, color.g, color.b) == color.r)
                            color = Color.red;
                        else if (Mathf.Max(color.r, color.g, color.b) == color.g)
                            color = Color.green;
                        else
                            color = Color.yellow;

                        color *= 15f;

                        var spawned = GameObject.Instantiate(spawnObjectOnCollision);
                        spawned.transform.position = contact.point;
                        spawned.transform.forward = ray.direction;

                        var spawnedRenderers = spawned.GetComponentsInChildren<Renderer>();
                        for (var rendererIndex = 0; rendererIndex < spawnedRenderers.Length; rendererIndex++)
                        {
                            var spawnedRenderer = spawnedRenderers[rendererIndex];
                            spawnedRenderer.material.color = color;
                            if (spawnedRenderer.material.HasProperty("_EmissionColor"))
                            {
                                spawnedRenderer.material.SetColor("_EmissionColor", color);
                            }
                        }
                    }
                }
                Debug.DrawRay(ray.origin, ray.direction, Color.cyan, 5, true);

                if (destroyOnTargetCollision)
                    Destroy(this.gameObject);
            }
        }
    }
}