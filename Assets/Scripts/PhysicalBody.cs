using UnityEngine;

namespace Softbody
{
    public class PhysicalBody : MonoBehaviour
    {
        [Header("Body Mass")]
        [SerializeField] private float mass;

        [Header("Body Position")]
        [SerializeField] private Vector2 position;

        [Header("Body Parameters")]
        [SerializeField] private Vector2 velocity;
        [SerializeField] private Vector2 acceleration;
        [SerializeField] private Vector2 force;

        [Header("Body State")]
        private bool isKinematic;
        private bool isAffected;

        [Header("Sprite")]
        private SpriteRenderer spriteRenderer;

        private float inverseMass;
        private float timer;

        public bool IsKinematic
        {
            get { return isKinematic; }
            set { isKinematic = value; }
        }

        public bool IsAffected
        {
            get { return isAffected; }
            set { isAffected = value; }
        }

        public float Timer
        {
            get { return timer; }
            set { timer = value; }
        }

        public float Mass
        {
            get { return mass; }
            set { mass = value; }
        }

        public Vector2 Position 
        { 
            get { return position = transform.position; } 
        }

        public Vector2 Velocity 
        { 
            get { return velocity; } 
        }

        float InverseMass 
        {
            get
            {
                if (mass < Mathf.Epsilon) 
                {
                    inverseMass = 0f;
                }
                else
                {
                    inverseMass = 1f / mass;
                }

                return inverseMass;
            }
        }

        private void Awake()
        {
            timer = 0;

            velocity = Vector2.zero;
            acceleration = Vector2.zero;
            force = Vector2.zero;

            spriteRenderer = GetComponent<SpriteRenderer>();

            PhysicalWorld physicalWorld = GameObject.Find("Physical World").GetComponent<PhysicalWorld>();
            physicalWorld.AddBody(this);
        }

        private void Update()
        {
            if (isAffected)
            {
                ManageAffectedState();
            }
        }

        private void ManageAffectedState()
        {
            if(spriteRenderer.color != Color.white)
            {
                spriteRenderer.color = Color.Lerp(Color.red, Color.white, (timer += Time.deltaTime) / 2);
            }
            else if(spriteRenderer.color == Color.white)
            {
                isAffected = false;
                timer = 0;
            }
        }

        private void ResetForces()
        {
            force = Vector2.zero;
        }

        public void AddForce(Vector2 newValue)
        {
            force += newValue;
        }

        public void SetPosition(float deltaTime)
        {
            if (!isKinematic)
            {
                acceleration = force * InverseMass;
                velocity += acceleration * deltaTime;

                position += velocity * deltaTime;
                transform.position = position;

                ResetForces();
            }
        }
    }
}