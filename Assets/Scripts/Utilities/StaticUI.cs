using UnityEngine;

public class StaticUI : MonoBehaviour
{
    private static GameObject UIContainer;
    void Awake()
    {
        if (UIContainer)
        {
            Destroy(gameObject);
        }
        else
        {
            UIContainer = this.gameObject;
            DontDestroyOnLoad(gameObject);
        }
    }
}
