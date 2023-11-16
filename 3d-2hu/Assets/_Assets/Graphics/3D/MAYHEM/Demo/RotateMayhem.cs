using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateMayhem : MonoBehaviour
{  
    [SerializeField] private Camera _camera;
    [SerializeField] private Text ui;
    
    private string text;

    void Start() {
        text = ui.text;
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(new Vector3(2.57f, 0.19f, 0), Vector3.up, 36*Time.deltaTime);
        ui.color = new Color(1, 1, 1, Random.Range(0, 6) == 0 ? 0 : 1);
        ui.text = text.Substring(0, (int)(Time.time / 0.2f) % text.Length);
        _camera.fieldOfView = 38 - 28 * Mathf.Cos(Time.time * Mathf.PI / 2.5f);
    }
}
