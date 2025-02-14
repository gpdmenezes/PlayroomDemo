using UnityEngine;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviour
{
    public static InterfaceManager Instance;

    [SerializeField] private Text dogPlayerName = null;
    [SerializeField] private Text jaguarPlayerName = null;

    private void Awake()
    {
        Instance = this;
    }

    public void SetJaguarPlayerName (string name)
    {
        jaguarPlayerName.text = name;
    }

    public void SetDogPlayerName (string name)
    {
        dogPlayerName.text = name;
    }
}
