using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanaonTranslation : MonoBehaviour
{
    PlayerInteraction playerInteraction;
    public GameObject canonObject;

    // Start is called before the first frame update
    void Start()
    {
        playerInteraction = GetComponent<PlayerInteraction>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 aimDirection = new Vector3(playerInteraction.shootDirection.x, playerInteraction.shootDirection.y, playerInteraction.shootDirection.z);

        canonObject.transform.rotation = Quaternion.FromToRotation(canonObject.transform.position, aimDirection);
    }
}
