using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platforms : MonoBehaviour
{

    public float platformWidth;
    public float platformY;
    private void Awake() {
        SpriteRenderer SR = GetComponent<SpriteRenderer>();

        platformWidth = SR.sprite.bounds.size.x;
        platformY = transform.position.y;
    }

    public void SummonOnTop(GameObject gameObject){
        Renderer sr = null;
        if (!gameObject.transform.TryGetComponent<Renderer>(out sr))
           gameObject.transform.GetChild(0).TryGetComponent<Renderer>(out sr);

        if (sr != null) {
           Instantiate(gameObject, new Vector2(transform.position.x + (Random.Range(0f, platformWidth/2)), platformY+(sr.bounds.size.y)/2), transform.rotation, transform);
        //    Debug.Log(getObjectSize(gameObject));
        }
     }

    public Vector2 getObjectSize(GameObject gameObject) {
         
         var p1 = gameObject.transform.TransformPoint(0, 0, 0);
         var p2 = gameObject.transform.TransformPoint(1, 1, 0);
         var s1 = Camera.main.WorldToScreenPoint(p1);
         var s2 = Camera.main.WorldToScreenPoint(p2);
         var w = p2.x - p1.x;
         var h = p2.y - p1.y;
         var sw = s2.x - s1.x;
         var sh = s2.y - s1.y;
       return new Vector2(w, h);
        // return new Vector2(sw, sh);
    }

}
