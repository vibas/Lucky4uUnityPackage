using UnityEngine;
using System.Collections.Generic;

namespace Lucky4u.Utility
{
    public class MultiRaycaster : MonoBehaviour
    {
        [SerializeField] Transform[] RaySourceTransforms;
        RaycastHit[] hitInfoArray;

        private void Start()
        {
            hitInfoArray = new RaycastHit[RaySourceTransforms.Length];
        }

        public List<GameObject> CastRay(string obstacleTag, float rayDistance)
        {
            List<GameObject> unitList = new List<GameObject>();
            for (int i = 0; i < RaySourceTransforms.Length; i++)
            {
                if (Physics.Raycast(RaySourceTransforms[i].position, RaySourceTransforms[i].forward * rayDistance, out hitInfoArray[i]))
                {
                    if (hitInfoArray[i].collider.CompareTag(obstacleTag))
                    {
                        Debug.DrawRay(RaySourceTransforms[i].position, RaySourceTransforms[i].forward * rayDistance, Color.red);
                        if (!unitList.Contains(hitInfoArray[i].collider.transform.parent.gameObject))
                        {
                            unitList.Add(hitInfoArray[i].collider.transform.parent.gameObject);
                        }
                    }
                }
                else
                {
                    Debug.DrawRay(RaySourceTransforms[i].position, RaySourceTransforms[i].forward * rayDistance, Color.green);
                }
            }
            return unitList;
        }
    }
}