using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingPage : MonoBehaviour
{
    public static string nextScene;
    
    [SerializeField]
    Image progressBar = null;
    
    private void Start()
    {
        progressBar.fillAmount = 0;
        if (progressBar == null)
            progressBar = GameObject.Find("ProgressBar").GetComponent<Image>();
        StartCoroutine(LoadScene());
    }

    public static void LoadScene(string sceneName) 
    {
        nextScene = sceneName;
        SceneManager.LoadScene(sceneName);

        if (DataInfo.ins.Now_QID == 0 && sceneName.Equals("Room_A"))
        {
            DataInfo.ins.QuestData[0].State = 1;
        }
        if (DataInfo.ins.Now_QID == 1 && sceneName.Equals("World_A"))
        {
            DataInfo.ins.QuestData[1].State = 1;
        }
    }

    IEnumerator LoadScene()
    {
        yield return null;

        AsyncOperation op;
        op = SceneManager.LoadSceneAsync(nextScene); 
        op.allowSceneActivation = false; 

        float timer = 0.0f; 
        
        while (!op.isDone)
        {
            yield return null;
            timer += Time.deltaTime;
            if (op.progress < 0.9f) 
            { 
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, op.progress, timer);
                if (progressBar.fillAmount >= op.progress) { 
                    timer = 0f; 
                }
            }
            else
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer); 
                
                if (progressBar.fillAmount >= 0.99f)
                {
                    op.allowSceneActivation = true; 
                    yield break;
                }
            }
        }
    }
}
