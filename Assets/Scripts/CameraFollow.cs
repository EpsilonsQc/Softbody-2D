using UnityEngine;

namespace Softbody
{
    public class CameraFollow : MonoBehaviour
    {
        [Header("Main Camera")]
        [SerializeField] private Camera cam;

        private void Awake()
        {
            cam = Camera.main;
        }

        private void Start()
        {
            transform.position = new Vector3(cam.orthographicSize * cam.aspect, cam.orthographicSize, -1);
        }
    }
}