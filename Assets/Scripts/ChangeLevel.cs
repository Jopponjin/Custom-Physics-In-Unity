using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeLevel : MonoBehaviour
{
    public string sceneName = "";
    public Scene scene;

    private void Start()
    {
        scene = SceneManager.GetActiveScene();
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.GetComponentInParent<Body>())
        {
            Debug.Log("ChangeLevel.cs: " + sceneName);

            if (sceneName != "") ChangeScene();
        }
    }

    public void ChangeScene()
    {
        if (sceneName == "Level 3" && scene.name != "Level 2") { Debug.Log("Thats the game, good work"); return; }
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
