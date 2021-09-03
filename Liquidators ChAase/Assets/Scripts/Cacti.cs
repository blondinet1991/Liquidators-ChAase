using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cacti : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject PortalPrefab;

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag.Equals("Player")) {
            transform.parent.GetComponent<Platforms>().SummonOnTop(PortalPrefab);
            //Instantiate(PortalPrefab, transform.position, transform.rotation);
            Destroy(transform.gameObject);
        }
    }


}
