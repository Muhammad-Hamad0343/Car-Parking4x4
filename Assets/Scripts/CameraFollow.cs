using UnityEngine;
using System.Collections;
    public class CameraFollow : MonoBehaviour
    {
        public Transform target;
        [Range(0, 30f)]
        public float distance;
        [Range(0, 10f)]
        public float height;
        [Range(0, 10f)]
        public float targetUpOffset;
        [Range(-10f, 10f)]
        public float targetForwardOffset;
        [Range(0, 50f)]
        public float smoothing;
        private float angle;
        [Range(0, 5f)]
        public float angleFollowStrength;
        private Vector3 targetForward;
        
        public LayerMask wallLayer;
        public Vector3 offset;

        void LateUpdate()
        {
            //if (!target)
            //{
            //target = GameplayManager._instance.selectedObj.transform;
            //   // target = GameManager.instance.playerSettings.player.transform;
            //}
            Vector3 prevTargetForward = targetForward;
            targetForward = Vector3.Lerp(prevTargetForward, target.forward, Time.deltaTime);

            angle = AngleSigned(target.forward, (target.position - transform.position), Vector3.up);

            Vector3 targetCamPos = target.position + targetForward * -(distance) + Vector3.up * height;

            transform.position = targetCamPos;
            transform.LookAt(target.position + Vector3.up * targetUpOffset + target.forward * targetForwardOffset);
            transform.rotation = Quaternion.AngleAxis(-angle * angleFollowStrength, Vector3.up) * transform.rotation;
            CheckWall();
        }

        /// <summary>
        /// Determine the signed angle between two vectors, with normal 'n'
        /// as the rotation axis.
        /// </summary>
        public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
        {
            return Mathf.Atan2(
                Vector3.Dot(n, Vector3.Cross(v1, v2)),
                Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
        }

        void CheckWall()
        {


            //print("OutSide");
            RaycastHit hit;
            Vector3 start = target.position;
            Vector3 dir = transform.position - target.position;
            float dist = offset.z * -1;
            Debug.DrawRay(target.position, dir, Color.green);
            //if (Physics.Raycast(target.position, dir, out hit, dist, wallLayer))
            if (Physics.Linecast(target.position, transform.position, out hit, wallLayer))
            {
                float hitDist = hit.distance;
                Vector3 sphereCastCenter = target.position + (dir.normalized * hitDist);
                transform.position = sphereCastCenter;
                print("InSide");

            }
        }
    }

