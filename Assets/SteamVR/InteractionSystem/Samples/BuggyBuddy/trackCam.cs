using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
    public class trackCam : MonoBehaviour
    {
        public float speed;

        public bool negative;

        private void Update()
        {
            var look = Camera.main.transform.position - transform.position;
            if (negative)
            {
                look = -look;
            }
            if (speed == 0)
            {
                transform.rotation = Quaternion.LookRotation(look);
            }
            else
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(look), speed * Time.deltaTime);
            }
        }
    }
}