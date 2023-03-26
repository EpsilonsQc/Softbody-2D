using UnityEngine;

namespace Softbody
{
    public class Particle : MonoBehaviour
    {
        // Parameters
        private float timer;
        [HideInInspector] public float mass;
        [HideInInspector] public float lifespan;
        [HideInInspector] public bool applyForce;

        // Color
        [HideInInspector] public Color startColor;
        [HideInInspector] public Color endColor; 

        // Movement vector
        [HideInInspector] public Vector3 velocity;

        private void Start()
        {
            InvokeRepeating("DestroyParticles", lifespan, 1.0f); 
        }

        private void Update()
        {
            ChangeColor();

            if (applyForce)
            {
                velocity += velocity / mass;
                applyForce = false;
            }

            transform.position += velocity * Time.deltaTime;
        }

        private void ChangeColor()
        {
            timer += Time.deltaTime;
            GetComponent<SpriteRenderer>().color = Color.Lerp(startColor, endColor, timer / lifespan);
        }

        private void DestroyParticles()
        {
            Destroy(gameObject);
        }
    }
}