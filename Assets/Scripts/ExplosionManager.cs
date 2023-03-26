using System.Collections.Generic; // Required for "List<>"
using UnityEngine;
using UnityEditor;

namespace Softbody
{
    public class ExplosionManager : MonoBehaviour
    {
        [Header("Explosion settings")]
        [SerializeField][Range(0, 10)] private float radius;
        [SerializeField][Range(0, 1000)] private float force;

        [Header("Particles Parameters")]
        [SerializeField] private int quantity = 25;
        [SerializeField] private float speed = 0.75f;
        [SerializeField] private float mass = 0.5f;
        [SerializeField] private float lifespan = 1.0f;

        [Header("Particles Color")]
        [SerializeField] private Color startColor = Color.red;
        [SerializeField] private Color endColor = Color.yellow;

        [Header("Particles Prefab")]
        [SerializeField] private GameObject particlesPrefab;

        [HideInInspector][SerializeField] private SpriteRenderer[] bodyArray;

        // References
        private PhysicalWorld physicalWorld;

        // Particles Container
        private GameObject particlesParent;

        private float magnitude;
        private float timer;

        private Vector3 direction;
        private Vector3 velocity;
        private Vector3 screenPosition;
        private Vector3 worldPosition;

        private Color gizmoColor;

        private void OnDrawGizmos()
        {
            Handles.color = gizmoColor;
            Handles.DrawWireDisc(transform.position, Vector3.forward, radius, 1);

            if (timer <= 2)
            {
                if(timer >= 1 && timer < 2)
                {
                    gizmoColor = Color.Lerp(Color.yellow, Color.green, timer - 0.75f);
                }
                else
                {
                    gizmoColor = Color.Lerp(Color.red, Color.yellow, timer + 0.25f);
                }

                timer += Time.deltaTime;
            }
        }

        private void Awake()
        {
            gizmoColor = Color.green;
            timer = 3;
            
            physicalWorld = GameObject.Find("Physical World").GetComponent<PhysicalWorld>();
        }

        private void Update()
        {
            InitBodyArray();
            ComputePosition();
            DetermineAffectedBody();
            PlayerInput();
        }

        private void InitBodyArray()
        {
            if (bodyArray.Length < 1)
                {
                bodyArray = new SpriteRenderer[physicalWorld.PhysicalBodyList.Count];
                PhysicalBody[] body = physicalWorld.PhysicalBodyList.ToArray();
                
                for(int i = 0; i < bodyArray.Length; i++)
                {
                    bodyArray[i] = body[i].GetComponent<SpriteRenderer>();
                }
            }
        }

        private void ComputePosition()
        {
            screenPosition = Input.mousePosition;
            screenPosition.z = Camera.main.nearClipPlane + 1;

            worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

            transform.position = worldPosition;
        }

        private void PlayerInput()
        {
            if (Input.GetMouseButton(0))
            {
                ParticlesExplosion();
                SoftbodyExplosion();

                timer = 0;
            }
        }

        private void DetermineAffectedBody()
        {
            for(int i = 0; i < bodyArray.Length; i++)
            {
                if (!physicalWorld.PhysicalBodyList[i].IsAffected)
                {
                    if ((bodyArray[i].gameObject.transform.position - worldPosition).magnitude <= radius)
                    {
                        bodyArray[i].color = Color.green;
                    }
                    else
                    {
                        bodyArray[i].color = Color.white;
                    }
                }
            }
        }

        private void CalculateForce(PhysicalBody body)
        {
            direction = (body.transform.position - worldPosition).normalized;
            float forceRatio = 1 - (body.transform.position - worldPosition).magnitude / radius; 

            magnitude = force * forceRatio;
            velocity = direction * magnitude;
        }

        private void SoftbodyExplosion()
        {
            PhysicalBody[] body = physicalWorld.PhysicalBodyList.ToArray();

            for(int i = 0; i < body.Length; i++)
            {
                if ((body[i].transform.position - worldPosition).magnitude <= radius)
                {
                    body[i].IsAffected = true;
                    body[i].Timer = 0;
                    bodyArray[i].color = Color.red;
                    CalculateForce(body[i]);
                    body[i].AddForce(velocity);
                }
            }
        }

        private void ParticlesExplosion()
        {
            if (particlesParent == null)
            {
                particlesParent = new GameObject("Particles");
                particlesParent.transform.SetParent(physicalWorld.transform, false);
            }

            for (int i = 0; i < quantity; i++)
            {
                GameObject particle = Instantiate(particlesPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
                Particle thisParticle = particle.GetComponent<Particle>();

                thisParticle.name = "Particle_" + i;

                thisParticle.transform.SetParent(particlesParent.transform, false);

                thisParticle.mass = mass;
                thisParticle.lifespan = lifespan;

                thisParticle.startColor = startColor;
                thisParticle.endColor = endColor;

                thisParticle.velocity.x = Mathf.Cos(Mathf.Abs(i * 360 / quantity) * Mathf.Deg2Rad) * speed; 
                thisParticle.velocity.y = Mathf.Sin(Mathf.Abs(i * 360 / quantity) * Mathf.Deg2Rad) * speed; 

                thisParticle.applyForce = true;
            }
        }
    }
}