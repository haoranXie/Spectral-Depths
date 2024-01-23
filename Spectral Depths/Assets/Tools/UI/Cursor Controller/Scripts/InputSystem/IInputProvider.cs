using UnityEngine;

namespace SlimUI.CursorControllerPro.InputSystem{
    // Provides functions that allow CursorController to not directly reference Unity's Input system

    public enum InputType
    {
        None,
        MouseAndKeyboard,
        Gamepad
    }

    public enum GamepadPlayerNum
    {
        One,
        Two,
        Three,
        Four,
    }

    public interface IInputProvider
    {
        InputType GetActiveInputType();
        Vector2 GetAbsolutePosition();
        Vector2 GetRelativeMovement(GamepadPlayerNum player = GamepadPlayerNum.One);
        bool GetSubmitWasPressed();
        bool GetSubmitWasReleased();
    }
}