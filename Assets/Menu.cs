using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Menu : MonoBehaviour
{
    [SerializeField] GameObject LoadingBar;
    [SerializeField] GameObject MenuPanel;


    public void Play()
    {
        StartCoroutine(LoadScene(1));
    }

    IEnumerator LoadScene(int index)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(index);

        LoadingBar.SetActive(true);

        yield return null;

        while (!operation.isDone)
        {
            Debug.Log(operation.progress);
            yield return null;
        }
    }

    public void Exit()
    {
        Application.Quit();
    }
}
