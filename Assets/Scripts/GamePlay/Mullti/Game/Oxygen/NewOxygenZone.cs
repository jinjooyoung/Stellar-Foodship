using UnityEngine;

public class NewOxygenZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        NewPlayer player = other.GetComponent<NewPlayer>();

        if (player == null)
            return;

        if (!player.Object.HasStateAuthority)
            return;

        player.IsInOxygenZone = true;
    }

    private void OnTriggerExit(Collider other)
    {
        NewPlayer player = other.GetComponent<NewPlayer>();

        if (player == null)
            return;

        if (!player.Object.HasStateAuthority)
            return;

        player.IsInOxygenZone = false;
    }
}
