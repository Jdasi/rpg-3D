using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    private Coroutines _coroutines;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
        _coroutines = new Coroutines();

        DontDestroyOnLoad(this.gameObject);
    }

    private void OnDestroy()
    {
        if (_instance != this)
        {
            return;
        }

        // ..
    }

    private void Update()
    {
        _coroutines.Update();
    }
}
