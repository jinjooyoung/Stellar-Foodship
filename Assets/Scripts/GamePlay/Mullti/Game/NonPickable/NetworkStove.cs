using UnityEngine;

public class NetworkStove : NewNonPickable
{
    protected override void OnItemPlaced(NewPickable item)
    {
        NetworkCookware cookware =
            item.GetComponent<NetworkCookware>();

        if (cookware != null)
        {
            //cookware.StartCooking();
        }
    }

    protected override void OnItemTaken(NewPickable item)
    {
        NetworkCookware cookware =
            item.GetComponent<NetworkCookware>();

        if (cookware != null)
        {
            //cookware.StopCooking();
        }
    }
}
