using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class BootSceneUI : MonoBehaviour
{
    UIDocument _document;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _document = GetComponent<UIDocument>();
        var root = _document.rootVisualElement;
        root.Q<Button>("Host").clicked += () => { Debug.Log("Hosted"); };
        root.Q<Button>("Join").clicked += () => { Debug.Log("Joined"); };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
