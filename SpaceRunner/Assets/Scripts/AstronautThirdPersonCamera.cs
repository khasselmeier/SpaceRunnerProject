using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstronautPlayer
{
    public class AstronautThirdPersonCamera : MonoBehaviour
    {
        public Vector3 camOffset = new Vector3(10f, 50f, -13f);

        private Transform target;

        void Start()
        {
            target = GameObject.Find("player").transform;
        }

        void LateUpdate()
        {
            this.transform.position = target.TransformPoint(camOffset);

            this.transform.LookAt(target);
        }
    }
}