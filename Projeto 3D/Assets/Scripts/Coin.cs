using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int scoreValue;
  
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D col)
    {  
        if (col.gameObject.tag == "player")
        {
            
        }
    }

   
}
