using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlanetCollisions : MonoBehaviour
{
    public UnityEvent OnTentacleCollision;
    public UnityEvent OnBlackHoleCollision;
    public void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.CompareTag("Tentacle"))
        {
            SetParentOfPlanet(coll.gameObject);
            OnTentacleCollision?.Invoke();
        }
        else if (coll.gameObject.CompareTag("BlackHole"))
        {
            OnBlackHoleCollision?.Invoke();
            Debug.Log("BLACK HOLE HIT");
            Destroy(gameObject);
        }

    }

    public void SetParentOfPlanet(GameObject parent)
    {
        transform.SetParent(parent.transform);
    }
}
