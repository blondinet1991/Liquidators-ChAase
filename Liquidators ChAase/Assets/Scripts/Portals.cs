using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portals : MonoBehaviour
{
    public GameObject LikidatorPrefab;

    private float counter=0f;
    private bool spawned = false;
    public float spawnTimer=3f;

    public GameCore gameCore;
    private void Update() {
        counter += Time.deltaTime;

            if (counter >= spawnTimer && !spawned) {
                spawned = true;
                GameObject TMPLikidator = transform.parent.GetComponent<Platforms>().SummonOnTopNonParent(LikidatorPrefab);
                if (TMPLikidator != null)
                    TMPLikidator.GetComponent<Likidator>().gameCore = gameCore;
                //Instantiate(PortalPrefab, transform.position, transform.rotation);
                Destroy(transform.gameObject, 3f);
            }

    }
}
