using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AbilityMachine : MonoBehaviour
{
    public Transform holdPoint;
    public GameObject weaponPrefab;
    private GameObject weaponInstance;
    public Transform player;
    private Vector3 lookPos;
    private bool isGunEquipped;


    private void makeGun()
    {
        if (!isGunEquipped)
        {
            weaponInstance = Instantiate(weaponPrefab, holdPoint.position, holdPoint.rotation, holdPoint);
            isGunEquipped = true;
        }
        else
        {
            DestroyGun();
        }
    }

    private void DestroyGun()
    {
        if (weaponInstance != null)
        {
            Destroy(weaponInstance);
            isGunEquipped = false;
        }
    }

    public void Start()
    {
       GameInput.Instance.OnEquipAction+=GameInput_OnEquipAction;
       isGunEquipped=false;
    }

    private void GameInput_OnEquipAction(object sender, EventArgs e)
    {
        makeGun();
    }

    public void Update()
    {
        AimAtMouse();
    }

private void AimAtMouse()
{
    Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
    RaycastHit hit;
    float range = 100f; // Sets Range

    if (Physics.Raycast(ray, out hit, range))
    {
        Vector3 lookPos = hit.point;
        Vector3 lookDir = lookPos - player.position;
        lookDir.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(lookDir, Vector3.up);

        // Smoothly rotate the holdPoint
        holdPoint.rotation = Quaternion.Lerp(holdPoint.rotation, targetRotation, Time.deltaTime * 10f);

        // Optionally, make the player look at the target too
        player.LookAt(player.position + lookDir, Vector3.up);
    }
}

}


