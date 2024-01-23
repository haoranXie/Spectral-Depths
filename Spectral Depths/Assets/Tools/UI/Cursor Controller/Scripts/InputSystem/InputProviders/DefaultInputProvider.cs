using UnityEngine;

namespace SlimUI.CursorControllerPro.InputSystem
{
    // Provides default implementation of IInputProvider
    // The combination of IInputProvider plus DefaultInputProvider should be all you need
    // This is the only class that should access Unity's Input system
    // All public functions in this class should also be in the interface
    public class DefaultInputProvider : MonoBehaviour, IInputProvider {

        private string submitButtonName = "Submit";

        [Tooltip("The axis name in the Input Manager")]
        public string horizontalAxis = "Horizontal";
        public string horizontalAxis2 = "Horizontal2";
        public string horizontalAxis3 = "Horizontal3";
        public string horizontalAxis4 = "Horizontal4";
        [Tooltip("The axis name in the Input Manager")]
        public string verticalAxis = "Vertical";
        public string verticalAxis2 = "Vertical2";
        public string verticalAxis3 = "Vertical3";
        public string verticalAxis4 = "Vertical4";

        [Tooltip("The value of joystick movement that is needed until the joystick registers input detection.")]
        [Range(0, 1)]
        public float deadZone = 0.1f;

        public InputType GetActiveInputType() {
            if( Input.GetKey(KeyCode.Joystick1Button0)  ||
                Input.GetKey(KeyCode.Joystick1Button1)  ||
                Input.GetKey(KeyCode.Joystick1Button2)  ||
                Input.GetKey(KeyCode.Joystick1Button3)  ||
                Input.GetKey(KeyCode.Joystick1Button4)  ||
                Input.GetKey(KeyCode.Joystick1Button5)  ||
                Input.GetKey(KeyCode.Joystick1Button6)  ||
                Input.GetKey(KeyCode.Joystick1Button7)  ||
                Input.GetKey(KeyCode.Joystick1Button8)  ||
                Input.GetKey(KeyCode.Joystick1Button9)  ||
                Input.GetKey(KeyCode.Joystick1Button10) ||
                Input.GetKey(KeyCode.Joystick1Button11) ||
                Input.GetKey(KeyCode.Joystick1Button12) ||
                Input.GetKey(KeyCode.Joystick1Button13) ||
                Input.GetKey(KeyCode.Joystick1Button14) ||
                Input.GetKey(KeyCode.Joystick1Button15) ||
                Input.GetKey(KeyCode.Joystick1Button16) ||
                Input.GetKey(KeyCode.Joystick1Button17) ||
                Input.GetKey(KeyCode.Joystick1Button18) ||
                Input.GetKey(KeyCode.Joystick1Button19) ) {
                return InputType.Gamepad;
            }

            // joystick axis
            if( (Input.GetAxis(horizontalAxis) > deadZone || Input.GetAxis(horizontalAxis) < -deadZone) ||
                (Input.GetAxis(verticalAxis) > deadZone || Input.GetAxis(verticalAxis) < -deadZone) ) {
                return InputType.Gamepad;
            }

            if (Event.current.isMouse){
                return InputType.MouseAndKeyboard;
            }else if( Input.GetAxis("Mouse X") != 0.0f || Input.GetAxis("Mouse Y") != 0.0f ){
                return InputType.MouseAndKeyboard;
            }

            return InputType.None;
        }

        public Vector2 GetAbsolutePosition() {
            return (Vector2) Input.mousePosition;
        }

        public Vector2 GetRelativeMovement(GamepadPlayerNum player = GamepadPlayerNum.One) {
            switch(player) {
                case GamepadPlayerNum.One:
                    return new Vector2(Input.GetAxis(horizontalAxis), Input.GetAxis(verticalAxis));
                case GamepadPlayerNum.Two:
                    return new Vector2(Input.GetAxis(horizontalAxis2), Input.GetAxis(verticalAxis2));
                case GamepadPlayerNum.Three:
                    return new Vector2(Input.GetAxis(horizontalAxis3), Input.GetAxis(verticalAxis3));
                case GamepadPlayerNum.Four:
                    return new Vector2(Input.GetAxis(horizontalAxis4), Input.GetAxis(verticalAxis4));
                default:
                    return new Vector2(Input.GetAxis(horizontalAxis), Input.GetAxis(verticalAxis));
            }
        }
    
        public bool GetSubmitWasPressed() {
            return Input.GetButtonDown(submitButtonName);
        }
        public bool GetSubmitWasReleased() {
            return Input.GetButtonUp(submitButtonName);
        }
    }
}