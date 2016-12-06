using UnityEngine;
using System.Collections;
using UnityStandardAssets.Utility;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;

namespace QuakeMovement.Movement
{
    public class MovementStrafeJumping : MonoBehaviour
    {


        // Quake physics specific variables
        public float m_friction = 8;
        public float m_maxGroundVelocity = 1000;
        public float m_maxAirVelocity = 2000;
        public float m_accelerationGround = 50;
        public float m_accelerationAir = 100;
        public float m_maxVelocity = 50;
        public float m_moveScale = 2;
        
        private Vector3 m_prevVelocity;


        public float m_gravity = 5;

        [SerializeField]private float m_moveSpeed = 10;
        private float m_currentSpeed;
        public float m_jumpSpeed;

        private CharacterController m_playerController;
        [SerializeField]
        private MouseController m_mouseLook;
        private Camera m_camera;
        private bool m_jumping, m_jump;
        private Vector3 m_cameraStartPosition;

        //Input
        private Vector2 m_input;
        private Vector3 m_moveDirection = Vector3.zero;
        private Vector3 m_oldMoveDirection = Vector3.zero;
        private CollisionFlags m_collisionFlags;

        private Text m_speedometer;

        // Use this for initialization
        void Start()
        {
            m_currentSpeed = 0;
            m_playerController = GetComponent<CharacterController>();
            m_camera = Camera.main;
            m_cameraStartPosition = m_camera.transform.localPosition;
            m_jumping = false;
            m_speedometer = GameObject.Find("Speedometer").GetComponent<Text>();
            m_speedometer.text = "fitta";
            //m_mouseLook.Init(m_playerController.transform, m_camera.transform);
        }

        // Update is called once per frame
        void Update()
        {
    
        }

        private void FixedUpdate()
        {
            //if (m_playerController.isGrounded)
            //{
            //    Vector3 moveForward = Input.GetAxis("Vertical") * m_camera.transform.forward;
            //    moveForward.y = 0;
            //    Vector3 moveSidewards = Input.GetAxis("Horizontal") * m_camera.transform.right;
            //    m_moveDirection = moveForward + moveSidewards;
            //    m_moveDirection *= m_moveSpeed;

            //    if (Input.GetKey(KeyCode.Mouse1))
            //    {
            //        Jump();
            //    }

            //}
            //else
            //{

            //}
            //m_moveDirection.y -= GetGravity(m_gravity);
            //m_playerController.Move(m_moveDirection * Time.deltaTime);
            //m_speedometer.text = m_playerController.velocity.magnitude.ToString();
            //m_prevVelocity = m_playerController.velocity;
            m_prevVelocity = m_playerController.velocity;
            Vector3 moveForward = Input.GetAxis("Vertical") * m_camera.transform.forward;
            moveForward.y = 0;
            Vector3 moveSidewards = Input.GetAxis("Horizontal") * m_camera.transform.right;
            moveForward.Normalize();
            moveSidewards.Normalize();
            m_moveDirection = moveForward + moveSidewards;
            m_moveDirection.y -= GetGravity(m_gravity);
            

            if (m_playerController.isGrounded)
            {
                if (Input.GetKey(KeyCode.Mouse1))
                {
                    Jump();
                }
                
                m_moveDirection.Normalize();
                m_playerController.Move(MoveGround(m_moveDirection * m_moveScale * Time.fixedDeltaTime));
            }
            else
            {
                m_moveDirection.Normalize();
                m_playerController.Move(MoveAir(m_moveDirection * m_moveScale * Time.fixedDeltaTime));
            }
            
            //m_playerController.Move(m_moveDirection * Time.deltaTime);
            m_speedometer.text = Mathf.Round(m_playerController.velocity.magnitude).ToString();
            //m_prevVelocity;
        }

        private void Jump()
        {
            m_moveDirection.y += m_jumpSpeed;
        }

        private void GetInput()
        {

        }

        private float GetGravity(float p_gravity)
        {
            return p_gravity;
        }

        private Vector3 Accelerate (Vector3 p_accelDirection, float p_maxVel, float p_acceleration)
        {
            float projVel = Vector3.Dot(m_prevVelocity, p_accelDirection);
            float accelVel = p_acceleration * Time.fixedDeltaTime;
            //Debug.Log(projVel + " " + accelVel);

            if(projVel + accelVel > p_maxVel)
            {
                accelVel = m_maxVelocity - projVel;
            }
            Vector3 result = m_prevVelocity + p_accelDirection * accelVel;
            //Debug.Log(result);
            return result;
        }

        private Vector3 MoveGround(Vector3 p_accelDirection)
        {
            float speed = m_prevVelocity.magnitude;
            if(speed != 0)
            {
                float drop = speed * m_friction * Time.fixedDeltaTime;
                m_prevVelocity *= Mathf.Max(speed - drop, 0) / speed;
            }
            return Accelerate(p_accelDirection, m_maxGroundVelocity, m_accelerationGround);
        }

        private Vector3 MoveAir(Vector3 p_accelDirection)
        {
            Vector3 result = Accelerate(p_accelDirection, m_maxAirVelocity, m_accelerationAir);
            //Debug.Log(p_accelDirection);
            return result;
        }


        Quaternion ClampRotation(Quaternion p_quarternion)
        {
            p_quarternion.x /= p_quarternion.w;
            p_quarternion.y /= p_quarternion.w;
            p_quarternion.z /= p_quarternion.w;
            p_quarternion.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(p_quarternion.x);
            angleX = Mathf.Clamp(angleX, -90, 90);
            p_quarternion.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return p_quarternion;
        }
    }
}

