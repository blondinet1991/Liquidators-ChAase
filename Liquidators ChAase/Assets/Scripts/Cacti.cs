using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cacti : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject PortalPrefab;

    public GameCore gameCore;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag.Equals("Player")) {
            GameObject tmpportal = transform.parent.GetComponent<Platforms>().SummonOnTop(PortalPrefab);
            if (tmpportal != null) {
                Debug.Log("portal game core set");
                tmpportal.GetComponent<Portals>().gameCore = gameCore;
            }
                
            //Instantiate(PortalPrefab, transform.position, transform.rotation);
            Destroy(transform.gameObject);
        }
    }

}
