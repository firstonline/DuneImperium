using UnityEngine;
using UnityEngine.SceneManagement;

public class BootScript : MonoBehaviour
{
    [SerializeField] string _nextScene;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SceneManager.LoadSceneAsync(_nextScene);
    }
}
