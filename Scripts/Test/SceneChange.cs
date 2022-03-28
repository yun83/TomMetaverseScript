using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneChange : MonoBehaviour
{
    public string SceneName;

    void Awake()
    {
        Button temp = GetComponent<Button>();

        temp.onClick.RemoveAllListeners();
        temp.onClick.AddListener(() =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName);
        });
    }
}
