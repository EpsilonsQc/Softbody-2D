using UnityEngine;

namespace Softbody
{
    public class Spring : MonoBehaviour
    {
        [Header("Line Renderer")]
        [SerializeField] private LineRenderer lineRenderer;

        [Header("Physical Body")]
        [SerializeField] private PhysicalBody bodyA;
        [SerializeField] private PhysicalBody bodyB;

        [Header("Spring Settings")]
        [SerializeField] private float spring;
        [SerializeField] private float damping;
        [SerializeField] private float restLength;

        [Header("Game Object")]
        private GridManager gridManager;

        private bool lineRendererSetup;

        public PhysicalBody BodyA
        {
            get { return bodyA; }
            set { bodyA = value; }
        }

        public PhysicalBody BodyB
        {
            get { return bodyA; }
            set { bodyB = value; }
        }

        public float Springs
        { 
            get { return spring; }
            set { spring = value; }
        }

        public float Damping
        { 
            get { return damping; }
            set { damping = value; }
        }

        private void Awake()
        {
            gridManager = GameObject.Find("Grid Manager").GetComponent<GridManager>();
            lineRenderer = GetComponent<LineRenderer>();
            
            restLength = 0f;
        }

        public void UpdateSpring()
        {
            SpringForceCalculation();
            DampingForceCalculation();

            LineRenderer();
        }

        private void SpringForceCalculation()
        {
            Vector2 direction = bodyB.Position - bodyA.Position ;
            float finalLength = direction.magnitude;

            float forceMagnitude = - spring * (restLength - finalLength);
            Vector2 forceDirection = direction.normalized;

            Vector2 springForce = forceMagnitude * forceDirection;

            ApplyForce(springForce);
        }

        private void DampingForceCalculation()
        {
            Vector2 direction = bodyA.Velocity;
            Vector2 velocityRelative = ( bodyB.Velocity - bodyA.Velocity);

            float velocityRelativeMagnitude = velocityRelative.magnitude;

            float forceMagnitude =  - damping * velocityRelativeMagnitude;
            Vector2 forceDirection = direction.normalized;

            Vector2 springDampingForce = forceMagnitude * forceDirection;

            ApplyForce(springDampingForce);
        }

        private void ApplyForce(Vector2 force)
        {
            if (!bodyA.IsKinematic)
            {
                bodyA.AddForce(force);
            }
            
            if (!bodyB.IsKinematic)
            {
                bodyB.AddForce(-force);
            }
        }

        public void SetRestLength()
        {
            restLength = (bodyA.Position - bodyB.Position).magnitude;
        }

        public void LineRenderer()
        {
            if (!lineRendererSetup)
            {
                lineRenderer.positionCount = 2;
                lineRenderer.widthMultiplier = 0.025f;

                lineRenderer.startColor = gridManager.lineColor;
                lineRenderer.endColor = gridManager.lineColor;

                lineRendererSetup = true;
            }

            lineRenderer.SetPosition(0, bodyA.Position);
            lineRenderer.SetPosition(1, bodyB.Position);
        }
    }
}