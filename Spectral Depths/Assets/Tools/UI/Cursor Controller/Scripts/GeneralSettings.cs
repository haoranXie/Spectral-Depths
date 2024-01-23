

using UnityEngine;

[CreateAssetMenu(fileName = "GeneralSettings", menuName = "GeneralSettings")]
public class GeneralSettings : ScriptableObject{
    [Tooltip("The cursor x-Axis movement speed according to Input Manager sensitivity")]
    [Range(1, 50)]
    public float horizontalSpeed;
    [Tooltip("The cursor y-Axis movement speed according to Input Manager sensitivity")]
    [Range(1, 50)]
    public float verticalSpeed;

    [Space]

    [Tooltip("The speed of the cursor when hovering over a button")]
    [Range(0.1f,1.0f)]
    public float hoverMultiplier = 0.33f;
    [Tooltip("The size of the cursor")]
    [Range(0.5f, 1.5f)]
    public float cursorScale = 1f;
    [Tooltip("The value of joystick movement that is needed until the joystick registers input detection.")]
    [Range(0, 1)]
    public float deadZone = 0.1f;
    [Range(1, 250)]
    [Tooltip("The farthest distance that the cursor will interact with a Canvas (designed for World Space UI)")]
    public float maxDetectionDistance = 50.0f;
    [HideInInspector]
    public float tempHspeed = 0.0f;
    [HideInInspector]
    public float tempVspeed = 0.0f;
    [Tooltip("Similar to Depth Layer of a Camera. Setting the Z offset will allow you to control how close to the rendering camera the cursor is. This is helpful in stopping the cursor from rendering behind 3D objects in a scene.")]
    public float cursorZOffset = 500f;
}
