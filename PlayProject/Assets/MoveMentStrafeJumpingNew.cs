using UnityEngine;
using System.Collections;
using UnityStandardAssets.Utility;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;

namespace QuakeMovement.Movement
{
    public class MoveMentStrafeJumpingNew : MonoBehaviour
    {

        private Transform m_cameraTransform;


        public float m_gravity      = 20.0f;
        public float m_friction     = 6.0f;

        public float m_moveSpeed    = 7.0f;
        public float m_groundAcc = 14.0f;
        public float m_groundDeAcc = 10.0f;
        public float m_airAcc = 2.0f;
        public float m_airDeAcc = 2.0f;
        public float m_airControl = 0.2f;
        public float m_airMoveSpeed = 20.0f;
        public float m_sideStrafeAcc = 50.0f;
        public float m_sideStrafeSpeed = 1.0f;
        public float m_jumpSpeed = 8.0f;
        public float m_moveScale = 1.0f;

        public float m_xMouseSens = 30.0f;
        public float m_yMouseSens = 30.0f;

        Text m_speedometer, m_camRotMeter, m_tranRotMeter, m_ctrlRotMeter, m_movVelMeter, m_buttonMeter, m_forwardMeter, m_rightMeter, m_verticalMeter, m_jumpTimer;
        Image m_groundedIndicator;
        float m_maxY = 0.0f;
        float m_maxYvel = 0.0f;

        private float m_fpsDisplayRate = 4.0f; // Shouldn't be necessary. Seems weird.

        private CharacterController m_characterController;
        private MouseController m_mouseLook;

        private float m_rotX = 0.0f;
        private float m_rotY = 0.0f;

        private Vector3 m_moveDirection = Vector3.zero;
        private Vector3 m_moveDirectionNormalized = Vector3.zero;
        private Vector3 m_playerVelocity = Vector3.zero;
        private Vector3 m_previousVelocity = Vector3.zero;

        private bool m_wishJump = false;
        public float m_doubleJumpThreshold = 0.4f;
        public float m_doubleJumpTimer = 0.0f;

        class Cmd
        {
            public float m_forwardMove;
            public float m_rightMove;
            public float m_upMove;
        }
        private Cmd cmd;
        

        // Use this for initialization
        void Start()
        {
            m_characterController = GetComponent<CharacterController>();

            m_cameraTransform = Camera.main.transform;
            m_cameraTransform.position = new Vector3(this.transform.position.x, this.transform.position.y+0.6f,this.transform.position.z);
            //m_cameraTransform.position.y = this.transform.position.y + 0.6f;
            Cursor.lockState = CursorLockMode.Locked;
           

            m_mouseLook = GetComponent<MouseController>();
            cmd = new Cmd();
            m_speedometer = GameObject.Find("Speedometer").GetComponent<Text>();
            m_camRotMeter = GameObject.Find("camRotationText").GetComponent<Text>();
            m_tranRotMeter = GameObject.Find("transRotationText").GetComponent<Text>();
            m_ctrlRotMeter = GameObject.Find("ctrlRotationText").GetComponent<Text>();
            m_movVelMeter = GameObject.Find("moveVelText").GetComponent<Text>();
            m_buttonMeter = GameObject.Find("buttonsText").GetComponent<Text>();
            m_forwardMeter = GameObject.Find("forwardInputText").GetComponent<Text>();
            m_rightMeter = GameObject.Find("rightInputText").GetComponent<Text>();
            m_verticalMeter = GameObject.Find("vertVelText").GetComponent<Text>();
            m_jumpTimer = GameObject.Find("jumpTimerText").GetComponent<Text>();
            m_groundedIndicator = GameObject.Find("groundedIndicator").GetComponent<Image>();
        }

        // Update is called once per frame
        void Update()
        {
            //Debug.Log(transform.rotation);
            m_previousVelocity = m_playerVelocity;
            m_rotX -= Input.GetAxisRaw("Mouse Y") * m_xMouseSens * 0.02f;
            m_rotY += Input.GetAxisRaw("Mouse X") * m_yMouseSens * 0.02f;
            
            //Clamp rot
            if (m_rotX < -90)
                m_rotX = -90;
            else if (m_rotX > 90)
                m_rotX = 90;

            this.transform.rotation = Quaternion.Euler(0, m_rotY, 0);
            m_cameraTransform.rotation = Quaternion.Euler(m_rotX, m_rotY, 0);
            m_cameraTransform.position = new Vector3(this.transform.position.x, this.transform.position.y + 1.0f, this.transform.position.z);

            QueueJump();

            //if((m_characterController.collisionFlags & CollisionFlags.Below) != 0)
            if(m_characterController.isGrounded)
            {
                m_groundedIndicator.color = Color.green;
                GroundMove();
            }
            //else if ((m_characterController.collisionFlags == CollisionFlags.None))
            else if (!m_characterController.isGrounded)
            {
                m_groundedIndicator.color = Color.red;
                AirMove();
            }

            m_characterController.Move(m_playerVelocity * Time.deltaTime);

            m_doubleJumpTimer += Time.deltaTime;
            UpdateGUI();
        }

        private void FixedUpdate()
        {
            
        }

        private void UpdateGUI()
        {
            m_speedometer.text = Mathf.Round(m_playerVelocity.magnitude).ToString();
            m_camRotMeter.text = "Cam Rotation: " + m_cameraTransform.rotation.ToString();
            m_tranRotMeter.text = "Trans Rotation: " + transform.rotation.ToString();
            m_ctrlRotMeter.text = "Ctrl Rotation: " + m_characterController.transform.rotation.ToString();
            m_movVelMeter.text = "Velocity: " + m_playerVelocity.ToString();

            string buttonString = "Keys: ";
            if (Input.GetKey(KeyCode.W))
                buttonString += "W, ";
            if (Input.GetKey(KeyCode.A))
                buttonString += "A, ";
            if (Input.GetKey(KeyCode.S))
                buttonString += "S, ";
            if (Input.GetKey(KeyCode.D))
                buttonString += "D, ";
            if (Input.GetKey(KeyCode.Mouse1))
                buttonString += "JUMP, ";
            m_buttonMeter.text = buttonString;

            m_forwardMeter.text = "Forward: " + Input.GetAxisRaw("Vertical").ToString();
            m_rightMeter.text = "Right: " + Input.GetAxisRaw("Horizontal").ToString();

            float yPos = transform.position.y;
            string verticalString = "Vertical: " + yPos.ToString("F2");

            if (yPos > m_maxY)
                m_maxY = yPos;
            verticalString += " (" + m_maxY.ToString("F2") + ")    ";
            float yVel = m_playerVelocity.y;
            if (yVel > m_maxYvel)
                m_maxYvel = yVel;
            verticalString += yVel.ToString("F2") + " ( " + m_maxYvel.ToString("F2") + ")";
            m_verticalMeter.text = verticalString;

            m_jumpTimer.text = "Jump Timer: " + m_doubleJumpTimer;

    }

        private void GroundMove()
        {
            Vector3 wishDirection = Vector3.zero;

            if(!m_wishJump)
            {
                ApplyFriction(1.0f);
            }
            else
            {
                ApplyFriction(0.0f);
            }
            float scale = CmdScale();
            SetMovementDir();

            //wishDirection = new Vector3(cmd.m_rightMove, 0, cmd.m_forwardMove);
            //wishDirection += transform.TransformDirection(wishDirection); //Maybe issues. Consult old code

            Vector3 forwardVector = m_cameraTransform.forward;
            Vector3 rightVector = m_cameraTransform.right;
            wishDirection.x = forwardVector.x * cmd.m_forwardMove + rightVector.x * cmd.m_rightMove;
            wishDirection.z = forwardVector.z * cmd.m_forwardMove + rightVector.z * cmd.m_rightMove;

            wishDirection.Normalize();
            m_moveDirectionNormalized = wishDirection;
            //Debug.Log(m_characterController.transform.rotation);

            float wishSpeed = wishDirection.magnitude;
            wishSpeed *= m_moveSpeed;

            Accelerate(wishDirection, wishSpeed, m_groundAcc);

            float vel = m_playerVelocity.magnitude;
            m_playerVelocity.Normalize();
            m_playerVelocity *= vel;
            //m_playerVelocity.y = 0.0f; // ska inte vara där för då FETEpajar isGrounded
            if(m_wishJump)
            {
                m_playerVelocity.y = m_jumpSpeed;
                //if (m_previousVelocity.y > 0.0f)
                if (m_doubleJumpTimer <= m_doubleJumpThreshold)
                {
                    //Debug.Log(m_playerVelocity.y);
                    m_playerVelocity.y = m_playerVelocity.y * 2;
                }
                else
                {
                    
                }

                m_wishJump = false;
                m_doubleJumpTimer = 0.0f;
            }

        }

        private void AirMove()
        {
            Vector3 wishDirection = Vector3.zero;
            float wishVelocity, accel;

            wishVelocity = m_airAcc;

            float scale = CmdScale();
            SetMovementDir();

            //wishDirection = new Vector3(cmd.m_rightMove, 0, cmd.m_forwardMove);
            Vector3 forwardVector = m_cameraTransform.forward;
            Vector3 rightVector = m_cameraTransform.right;
            wishDirection.x = forwardVector.x * cmd.m_forwardMove + rightVector.x * cmd.m_rightMove;
            wishDirection.z = forwardVector.z * cmd.m_forwardMove + rightVector.z * cmd.m_rightMove;
            wishDirection.y = forwardVector.y * cmd.m_forwardMove + rightVector.y * cmd.m_rightMove;

            //wishDirection = new Vector3(cmd.m_rightMove, 0, cmd.m_forwardMove);
            //wishDirection = m_cameraTransform.TransformDirection(wishDirection);

            float wishSpeed = wishDirection.magnitude;
            wishSpeed *= m_airMoveSpeed;
            wishSpeed *= scale;

            wishDirection.Normalize();
            m_moveDirectionNormalized = wishDirection;
            

            //AirControl
            float wishSpeed2 = wishSpeed;
            if(Vector3.Dot(m_playerVelocity, wishDirection) < 0)
            {
                accel = m_airDeAcc;
            }
            else
            {
                accel = m_airAcc;
            }
            if(cmd.m_rightMove == 0.0f && cmd.m_forwardMove != 0.0f)
            {
                if(wishSpeed > m_sideStrafeSpeed)
                {
                    wishSpeed = m_sideStrafeSpeed;
                }
                accel = m_sideStrafeAcc;
            }

            Accelerate(wishDirection, wishSpeed, accel);

            if(m_airControl > 0.0f)
            {
                AirControl(wishDirection, wishSpeed2);
            }

            m_playerVelocity.y -= m_gravity * Time.deltaTime;
        }

        private void AirControl(Vector3 p_wishDir, float p_wishSpeed)
        {
            float zSpeed, speed, dot, k;

            zSpeed = m_playerVelocity.y;
            m_playerVelocity.y = 0;
            speed = m_playerVelocity.magnitude;
            m_playerVelocity.Normalize();

            //if (cmd.m_forwardMove == 0 || p_wishSpeed == 0)
            //    return;

            dot = Vector3.Dot(m_playerVelocity, p_wishDir);
            k = 32.0f;
            k *= m_airControl * dot * dot * Time.deltaTime;
            
            if(dot > 0)
            {
                m_playerVelocity.x = m_playerVelocity.x * speed + p_wishDir.x * k;
                //m_playerVelocity.y = m_playerVelocity.y * speed + p_wishDir.y * k;
                m_playerVelocity.z = m_playerVelocity.z * speed + p_wishDir.z * k;

                m_playerVelocity.Normalize();
                m_moveDirectionNormalized = m_playerVelocity;
            }

            m_playerVelocity.x *= speed;
            m_playerVelocity.y = zSpeed;
            m_playerVelocity.z *= speed;
        }

        private void Accelerate(Vector3 p_wishDir, float p_wishSpeed, float p_acceleration)
        {
            float addSpeed, accelSpeed, currentSpeed;

            currentSpeed = Vector3.Dot(m_playerVelocity, p_wishDir);
            addSpeed = p_wishSpeed - currentSpeed;

            if(addSpeed <= 0)
            {
                return;
            }
            accelSpeed = p_acceleration * Time.deltaTime * p_wishSpeed;
            if(accelSpeed > addSpeed)
            {
                accelSpeed = addSpeed;
            }
            m_playerVelocity.x += accelSpeed * p_wishDir.x;
            m_playerVelocity.y += accelSpeed * p_wishDir.y;
            m_playerVelocity.z += accelSpeed * p_wishDir.z;
        }

        private void SetMovementDir()
        {
            cmd.m_forwardMove = Input.GetAxisRaw("Vertical");
            cmd.m_rightMove = Input.GetAxisRaw("Horizontal");
        }

        private void ApplyFriction(float p_frictionMultiplier)
        {
            Vector3 vec = m_playerVelocity;
            float speed, newSpeed, control, drop;
            vec.y = 0.0f;
            speed = vec.magnitude;
            drop = 0.0f;

            if(m_characterController.isGrounded)
            {
                control = speed < m_groundAcc ? m_groundDeAcc : speed;
                drop = control * m_friction * Time.deltaTime * p_frictionMultiplier;
            }

            newSpeed = speed - drop;
            if(newSpeed < 0.0f)
            {
                newSpeed = 0;
            }
            if(speed > 0.0f )
            {
                newSpeed /= speed;
            }
            m_playerVelocity.x *= newSpeed;
            m_playerVelocity.z *= newSpeed;
        }

        private void QueueJump()
        {
            if(Input.GetKeyDown(KeyCode.Mouse1) && !m_wishJump)
            {
                m_wishJump = true;
            }
            if(Input.GetKeyUp(KeyCode.Mouse1))
            {
                m_wishJump = false;
            }
        }

        private float CmdScale()
        {
            float max;
            float total, scale;

            max = Mathf.Abs(cmd.m_forwardMove);
            if(Mathf.Abs(cmd.m_rightMove) > max)
            {
                max = Mathf.Abs(cmd.m_rightMove);
            }
            if (max >= 0) //UNCERTAIN
            {
                return 0;
            }

            total = Mathf.Sqrt(cmd.m_forwardMove * cmd.m_forwardMove + cmd.m_rightMove * cmd.m_rightMove);
            scale = m_moveSpeed * max / (m_moveScale * total);
            return scale;
        }
    }
}