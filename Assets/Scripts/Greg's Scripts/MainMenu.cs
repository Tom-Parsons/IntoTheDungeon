using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public ChildPartileSystemsController portal;

    void Update()
    {

    }

    public void LoadGame ()
    {
        StartCoroutine(LoadLevel());
    }

    public void ExitGame ()
    {
        Application.Quit();
    }

    IEnumerator LoadLevel()
    {
        portal.PlayChildSystems();
        yield return new WaitForSeconds(3);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainScene");


        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
