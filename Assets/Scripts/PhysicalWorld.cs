using System.Collections.Generic; // Required for "List<>"
using UnityEngine;
using System.Linq; // Required for "ToList()"

namespace Softbody
{
    public class PhysicalWorld : MonoBehaviour
    {
        [SerializeField] private List<Spring> springList = new List<Spring>();
        [SerializeField] private List<PhysicalBody> physicalBodyList = new List<PhysicalBody>();

        public List<PhysicalBody> PhysicalBodyList
        {
            get { return physicalBodyList; }
        }

        private void Awake()
        {
            springList = FindObjectsOfType<Spring>().ToList();
            physicalBodyList = FindObjectsOfType<PhysicalBody>().ToList();
        }

        private void Update()
        {
            foreach (var spring in springList)
            {
                spring.UpdateSpring();
            }

            foreach (var body in physicalBodyList) 
            {
                body.SetPosition(Time.deltaTime);
            }
        }

        public void AddBody(PhysicalBody newBody)
        {
            if (!physicalBodyList.Contains(newBody))
            {
                physicalBodyList.Add(newBody);
            }
        }
    }
}