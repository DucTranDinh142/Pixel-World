using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Load : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI textMeshProUGUI;
    public void LoadMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && textMeshProUGUI.gameObject.activeSelf)
        {
            textMeshProUGUI.gameObject.SetActive(false);
        } else if(Input.GetKeyDown(KeyCode.I) && !textMeshProUGUI.gameObject.activeSelf)
        {
            textMeshProUGUI.gameObject.SetActive(true);
        }
    }
}
