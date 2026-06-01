using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public GameObject OxygenPlace;
    public GameObject targetPosition;

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            OxygenPlace.transform.position = targetPosition.transform.position;
        }
    }

}
