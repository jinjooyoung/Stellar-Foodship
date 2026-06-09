using UnityEngine;

public class NetUIManager : MonoBehaviour
{
    public static NetUIManager Instance;

    public Transform WorldUIRoot;

    private void Awake()
    {
        Instance = this;
    }
}
