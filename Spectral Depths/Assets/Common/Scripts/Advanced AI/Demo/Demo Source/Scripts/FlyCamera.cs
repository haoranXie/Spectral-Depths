using UnityEngine;
using System.Collections;
#if (ENABLE_INPUT_SYSTEM)
using UnityEngine.InputSystem;
#endif

namespace EmeraldAI.Example
{
    public class FlyCamera : MonoBehaviour
    {
        public float mainSpeed = 10.0f;
        public float shiftAdd = 25.0f;
        public float maxShift = 25.0f;
        public float camSens = 0.25f;

        private Vector3 lastMouse = new Vector3(255, 255, 255);
        private float totalRun = 1.0f;

        void Update()
        {
#if (ENABLE_LEGACY_INPUT_MANAGER)
            LegacyInput();
#elif (ENABLE_INPUT_SYSTEM)
            NewInput();
#endif
        }

#if (ENABLE_LEGACY_INPUT_MANAGER)
        void LegacyInput()
        {
            if (Input.GetMouseButtonDown(1))
            {
                lastMouse = Input.mousePosition;
            }

            if (Input.GetMouseButton(1))
            {
                lastMouse = Input.mousePosition - lastMouse;
                lastMouse = new Vector3(-lastMouse.y * camSens, lastMouse.x * camSens, 0);
                lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x, transform.eulerAngles.y + lastMouse.y, 0);
                transform.eulerAngles = lastMouse;
                lastMouse = Input.mousePosition;
            }

            //Keyboard commands
            Vector3 p = GetBaseLegacyInput();
            if (Input.GetKey(KeyCode.LeftShift))
            {
                totalRun += Time.deltaTime;
                p = p * totalRun * shiftAdd;
                p.x = Mathf.Clamp(p.x, -maxShift, maxShift);
                p.y = Mathf.Clamp(p.y, -maxShift, maxShift);
                p.z = Mathf.Clamp(p.z, -maxShift, maxShift);
            }
            else
            {
                totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
                p = p * mainSpeed;
            }

            p = p * Time.deltaTime;
            transform.Translate(p);
        }
#endif

#if (ENABLE_INPUT_SYSTEM)
        void NewInput()
        {
            if (!Mouse.current.rightButton.isPressed)
            {
                lastMouse = Mouse.current.position.ReadValue();
            }

            if (Mouse.current.rightButton.isPressed)
            {
                lastMouse = (Vector3)Mouse.current.position.ReadValue() - lastMouse;
                lastMouse = new Vector3(-lastMouse.y * camSens, lastMouse.x * camSens, 0);
                lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x, transform.eulerAngles.y + lastMouse.y, 0);
                transform.eulerAngles = lastMouse;
                lastMouse = Mouse.current.position.ReadValue();
            }

            //Keyboard commands
            Vector3 p = GetBaseNewInput();
            if (Keyboard.current.leftShiftKey.isPressed)
            {
                totalRun += Time.deltaTime;
                p = p * totalRun * shiftAdd;
                p.x = Mathf.Clamp(p.x, -maxShift, maxShift);
                p.y = Mathf.Clamp(p.y, -maxShift, maxShift);
                p.z = Mathf.Clamp(p.z, -maxShift, maxShift);
            }
            else
            {
                totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
                p = p * mainSpeed;
            }

            p = p * Time.deltaTime;
            transform.Translate(p);
        }
#endif

#if (ENABLE_LEGACY_INPUT_MANAGER)
        private Vector3 GetBaseLegacyInput()
        {
            Vector3 p_Velocity = new Vector3();
            if (Input.GetKey(KeyCode.W))
            {
                p_Velocity += new Vector3(0, 0, 1);
            }
            if (Input.GetKey(KeyCode.S))
            {
                p_Velocity += new Vector3(0, 0, -1);
            }
            if (Input.GetKey(KeyCode.A))
            {
                p_Velocity += new Vector3(-1, 0, 0);
            }
            if (Input.GetKey(KeyCode.D))
            {
                p_Velocity += new Vector3(1, 0, 0);
            }
            if (Input.GetKey(KeyCode.Q))
            {
                p_Velocity += new Vector3(0, 1, 0);
            }
            if (Input.GetKey(KeyCode.E))
            {
                p_Velocity += new Vector3(0, -1, 0);
            }
            return p_Velocity;
        }
#endif

#if (ENABLE_INPUT_SYSTEM)
        private Vector3 GetBaseNewInput()
        {
            Vector3 p_Velocity = new Vector3();
            if (Keyboard.current.wKey.isPressed)
            {
                p_Velocity += new Vector3(0, 0, 1);
            }
            if (Keyboard.current.sKey.isPressed)
            {
                p_Velocity += new Vector3(0, 0, -1);
            }
            if (Keyboard.current.aKey.isPressed)
            {
                p_Velocity += new Vector3(-1, 0, 0);
            }
            if (Keyboard.current.dKey.isPressed)
            {
                p_Velocity += new Vector3(1, 0, 0);
            }
            if (Keyboard.current.qKey.isPressed)
            {
                p_Velocity += new Vector3(0, 1, 0);
            }
            if (Keyboard.current.eKey.isPressed)
            {
                p_Velocity += new Vector3(0, -1, 0);
            }
            return p_Velocity;
        }
#endif
    }
}