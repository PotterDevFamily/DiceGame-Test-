using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAlert : MonoBehaviour
{
    public bool playerInFlag = false;
    
    void OnTriggerEnter(Collider col){
        if(col.tag == "Player")
            playerInFlag = true;
    }
}
