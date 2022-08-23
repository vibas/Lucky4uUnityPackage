using System.Collections.Generic;
using UnityEngine;

namespace Lucky4u.Utility
{
    public class BillboardHelper
    {
        Transform camTransform;
        List<GameObject> billboardObjectsList;

        public BillboardHelper()
        {
            camTransform = Camera.main.transform;
        }

        public void RegisterBillbardObject(GameObject obj)
        {
            if (billboardObjectsList == null)
                billboardObjectsList = new List<GameObject>();

            billboardObjectsList.Add(obj);
        }

        public void UnregisterBillboardObject(GameObject obj)
        {
            if (billboardObjectsList.Contains(obj))
            {
                billboardObjectsList.Remove(obj);
            }
        }

        public void UpdateBillboard(GameObject obj)
        {
            if (billboardObjectsList.Contains(obj))
            {
                obj.transform.LookAt(camTransform);
            }
        }
    }
}

