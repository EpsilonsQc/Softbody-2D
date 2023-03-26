using UnityEngine;

namespace Softbody
{
    public class GridManager : MonoBehaviour
    {
        [Header("Grid Settings")]
        [SerializeField] private int XAxisBodyQty;
        [SerializeField] private int YAxisBodyQty;

        [Header("Body Settings")]
        [SerializeField] private float mass;

        [Header("Spring Settings")]
        [SerializeField] private float spring;
        [SerializeField] private float damping;

        [Header("Line Renderer Color")]
        public Color lineColor;

        [Header("Prefabs")]
        [SerializeField] private GameObject bodyPrefab;
        [SerializeField] private GameObject springPrefab;

        // Main Camera
        private Camera cam;
        private float camAspectRatio;
        private float camXAxis;
        private float camYAxis;

        // Container
        private GameObject physicalWorldParent;
        private GameObject bodyParent;
        private GameObject springParent;
        private GameObject springXAxisParent;
        private GameObject springYAxisParent;

        private void Awake()
        {
            int counter = 0;

            Vector2[] bodyPositions = new Vector2[XAxisBodyQty * YAxisBodyQty]; ;

            CameraManager();
            CreateContainer();

            ComputeBodyPositions(counter, bodyPositions);

            PhysicalBody[] physicalBody = new PhysicalBody[bodyPositions.Length];

            InstantiatePhysicalBody(counter, bodyPositions, physicalBody);
            AddSpring(physicalBody);
        }
        
        private void CameraManager()
        {
            cam = Camera.main;
            camAspectRatio = cam.aspect;
            camXAxis = 2 * cam.orthographicSize * camAspectRatio;
            camYAxis = 2 * cam.orthographicSize;
        }

        private void CreateContainer()
        {
            physicalWorldParent = GameObject.Find("Physical World");
            physicalWorldParent.transform.SetParent(physicalWorldParent.transform, false);

            bodyParent = new GameObject("Body");
            bodyParent.transform.SetParent(physicalWorldParent.transform, false);

            springParent = new GameObject("Spring");
            springParent.transform.SetParent(physicalWorldParent.transform, false);

            springXAxisParent = new GameObject("X-Axis");
            springXAxisParent.transform.SetParent(springParent.transform, false);

            springYAxisParent = new GameObject("Y-Axis");
            springYAxisParent.transform.SetParent(springParent.transform, false);
        }

        private void ComputeBodyPositions(int counter, Vector2[] bodyPositions)
        { 
            counter = 0;

            for(int y = 0; y < YAxisBodyQty; y++)
            {
                for(int x = 0; x < XAxisBodyQty; x++)
                {
                    bodyPositions[counter] = new Vector2(camXAxis / (XAxisBodyQty - 1) * x, camYAxis / (YAxisBodyQty - 1) * y);
                    counter++;
                }
            }
        }

        private void InstantiatePhysicalBody(int counter, Vector2[] bodyPositions, PhysicalBody[] physicalBody)
        {
            counter = 0;

            foreach(Vector2 position in bodyPositions)
            {
                GameObject gameObject = Instantiate(bodyPrefab);
                physicalBody[counter] = gameObject.AddComponent<PhysicalBody>();
                physicalBody[counter].Mass = mass;
                
                gameObject.name = "Body " + counter;
                gameObject.transform.position = position;
                gameObject.transform.parent = bodyParent.transform;
                
                DefineAnchors(physicalBody[counter]);

                counter++;
            }
        }

        private void AddSpring(PhysicalBody[] physicalBody)
        {
            for (int i = 0; i < physicalBody.Length; i++)
            {
                // X-Axis
                if (physicalBody[i].gameObject.transform.position.x < camXAxis)
                {
                    GameObject gameObject = Instantiate(springPrefab);
                    Spring spring = gameObject.GetComponent<Spring>();

                    spring.Springs = this.spring;
                    spring.Damping = damping;
                    spring.BodyA = physicalBody[i];
                    spring.BodyB = physicalBody[i + 1];

                    gameObject.name = "Spring X-Axis " + i;
                    gameObject.transform.parent = springXAxisParent.transform;
                    
                    spring.SetRestLength();
                    spring.LineRenderer();
                }

                // Y-Axis
                if (physicalBody[i].gameObject.transform.position.y < camYAxis)
                {
                    GameObject gameObject = Instantiate(springPrefab);
                    Spring spring = gameObject.GetComponent<Spring>();

                    spring.Springs = this.spring;
                    spring.Damping = damping;
                    spring.BodyA = physicalBody[i];
                    spring.BodyB = physicalBody[i + XAxisBodyQty];

                    gameObject.name = "Spring Y-Axis " + i;
                    gameObject.transform.parent = springYAxisParent.transform;
    
                    spring.SetRestLength();
                    spring.LineRenderer();
                }
            }
        }
        
        private void DefineAnchors(PhysicalBody body)
        {
            if(body.Position.x == 0 || body.Position.x == camXAxis || body.Position.y == 0 || body.Position.y == camYAxis)
            {
                body.IsKinematic = true;
            }
        }
    }
}