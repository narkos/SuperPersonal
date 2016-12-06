using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace QuakeMovement.Movement
{
    public class MouseController : MonoBehaviour
    {

        public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
        public RotationAxes axes = RotationAxes.MouseXAndY;
        public float sensitivityX = 15F;
        public float sensitivityY = 15F;

        public float minimumX = -360F;
        public float maximumX = 360F;

        public float minimumY = -60F;
        public float maximumY = 60F;

        public bool smoothLookX;
        private float sensivityXmemory;
        private float maximumSmoothX;
        private float minimumSmoothX;

        private float rotationY = 0F;
        private float rotationX = 0F;

        public bool m_CursorLocked = true;

        Transform m_camTransform;
        Transform m_playerTransform;

        public float GetYRotation()
        {
            return rotationX;
        }

        void Update()
        {
            
            if (axes == RotationAxes.MouseXAndY)
            {
                rotationX = m_camTransform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
                //rotationX += Input.GetAxis("Mouse X") * sensitivityX;
                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
                rotationX = Mathf.Clamp(rotationX, minimumX, maximumX);

                m_camTransform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
                //m_playerTransform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
            }
            else if (axes == RotationAxes.MouseX)
            {
                m_camTransform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
            }
            else
            {
                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

                m_camTransform.localEulerAngles = new Vector3(-rotationY, m_camTransform.localEulerAngles.y, 0);
            }

            //NEBUCH
            if (smoothLookX)
            {
                if (rotationX > maximumSmoothX)
                {
                    sensitivityX = 1f;
                }

                if (rotationX < minimumSmoothX)
                {

                    sensitivityX = 1f;
                }

                if (rotationX > minimumSmoothX && rotationX < maximumSmoothX)
                {

                    sensitivityX = sensivityXmemory;

                }

            }

            if(Input.GetKeyDown(KeyCode.P))
            {
                SetCursorLock(!m_CursorLocked);
            }

        }

        void SetCursorLock(bool p_state)
        {
            m_CursorLocked = p_state;
            if(m_CursorLocked)
            {
                Cursor.lockState = UnityEngine.CursorLockMode.Locked;
            }
            else
            {
                Cursor.lockState = UnityEngine.CursorLockMode.None;
            }
        }

        void Start()
        {
            m_camTransform = Camera.main.transform;
            m_playerTransform = GetComponent<Transform>();
            // Make the rigid body not change rotation

            //NEBUCH
            sensivityXmemory = sensitivityX;
            maximumSmoothX = maximumX - maximumX / 5;
            minimumSmoothX = minimumX + minimumX / (-5);
            Cursor.lockState = UnityEngine.CursorLockMode.Locked;
            
        }

    }
}
