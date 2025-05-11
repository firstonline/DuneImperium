using Unity.Multiplayer.Widgets;
using Unity.Netcode;
using UnityEngine;

public class ConnectionScreen : MonoBehaviour
{
    [SerializeField] WidgetConfiguration _widgetConfiguration;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void LoadScene()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("GamePlayScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }
}
