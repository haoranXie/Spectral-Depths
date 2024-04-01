using UnityEngine;
using System.Collections;

public class MouseOrbit_Katana : MonoBehaviour
{

    public Transform Target;
    public float distance = 0.5f;

    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;

    public float yMinLimit = -30.0f;
    public float yMaxLimit = 60.0f;

    private float x = 0.0f;
    private float y = 0.0f;
    public float CameraDist = 0.5f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.x-200;
        y = angles.y+140;
        
        if (this.GetComponent<Rigidbody>() == true)
            GetComponent<Rigidbody>().freezeRotation = true;
    }

    void LateUpdate()
    {
        if (Target)
        {
            x -= Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            y += Input.GetAxis("Mouse Y") * ySpeed * 0.05f;

            y = ClampAngle(y, yMinLimit, yMaxLimit);

            Quaternion rotation = Quaternion.Euler(y, x, 0);
            Vector3 position = rotation * new Vector3(0, 0, -distance) + Target.position;

            transform.rotation = rotation;
            transform.position = position;
            distance = CameraDist;

            if (Input.GetKey(KeyCode.Mouse0))
            {
                CameraDist -= Time.deltaTime * 2;
                CameraDist = Mathf.Clamp(CameraDist, 0.1f, 10);
            }
            if (Input.GetKey(KeyCode.Mouse1))
            {
                CameraDist += Time.deltaTime * 2;
                CameraDist = Mathf.Clamp(CameraDist, 0.1f, 10);
            }
        }
      
    }

    float ClampAngle(float ag, float min, float max)
    {
        if (ag < -360)
            ag += 360;
        if (ag > 360)
            ag -= 360;
        return Mathf.Clamp(ag, min, max);
    }

}
