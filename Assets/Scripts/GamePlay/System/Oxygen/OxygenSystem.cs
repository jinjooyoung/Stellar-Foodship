using UnityEngine;

public class OxygenSystem : MonoBehaviour
{
    public Player player;

    public float decreaseRate = 1f;     // 1초에 1 감소
    public float increaseRate = 24f;    // 1초에 24 증가

    void Update()
    {
        if (player.state == PlayerState.Uncontrollable) return;

        if (player.isInOxygenZone)
        {
            player.ChangeOxygen(increaseRate * Time.deltaTime);
        }
        else
        {
            player.ChangeOxygen(-decreaseRate * Time.deltaTime);
        }
    }
}