using System.Collections;
using UnityEngine;

public class Collide_disable : MonoBehaviour
{
    //script that attached to middle wall
    private BoxCollider wallCollider;
    private void Start()
    {
        wallCollider = GetComponent<BoxCollider>();
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Dice"))
           StartCoroutine(makeMeDis());
        
    }

    IEnumerator makeMeDis()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        wallCollider.enabled = false;
    }
   
}
