using UnityEngine;

namespace FSMExample
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class AreaAwareness : MonoBehaviour
    {
        private Transform target;

        public bool HasTarget() => target != null;

        public int GetTargetDirection()
        {
            if (!HasTarget()) return int.MaxValue;
            return target.position.x > transform.position.x ? 1 : -1;
        }

        public float GetTargetDistance() => Vector2.Distance(transform.position, target.position);

        public float GetTargetHorizontalDistance() 
            => Mathf.Abs(transform.position.x - target.position.x);

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.CompareTag("Player"))
            {
                target = collision.transform;
            }
        }

        public void ResetTarget() => target = null;
    }
}
