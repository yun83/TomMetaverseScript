using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TransparencyCapture : MonoBehaviour
{
    public delegate void Callback();
    public Camera _camera;

    public int ItemCount = 0;
    public Transform[] Item;

    public GameObject BaseObject;

    public int AllItemSize = 0;
    public int AniIndex = 0;
    public int MaxAniInd = 10;

    public Animator ShowAni;
    bool ChangeCheck = false;
    int screenShotCnt = 0;

    List<Transform> itemTrans = new List<Transform>();

    void Awake()
    {
        AllItemSize = 0;
        for (int i = 0; i < Item.Length; i++)
        {
            AllItemSize += Item[i].childCount;
        }


        for (int i = 0; i < Item.Length; i++)
        {
            for (int j = 0; j < Item[i].childCount; j++)
            {
                itemTrans.Add(Item[i].GetChild(j));
            }
        }
        screenShotCnt = 0;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ItemCount = 0;
            AllCapture();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            AniIndex++;
            if (AniIndex > MaxAniInd)
                AniIndex = 1;

            ChangeCheck = true;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            AniIndex--;
            if (AniIndex < 0)
                AniIndex = MaxAniInd;

            ChangeCheck = true;
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            CaptureSub();
        }
    }

    private void AllCapture()
    {
        string timeStr = System.DateTime.Now.Year.ToString() +
                  System.DateTime.Now.Month.ToString() +
                  System.DateTime.Now.Day.ToString() +
                  System.DateTime.Now.Hour.ToString() + 
                  System.DateTime.Now.Minute.ToString() + "_";
        string path = Application.dataPath + "/capture_" + timeStr + ItemCount.ToString() + ".png";

        for (int i = 0; i < itemTrans.Count; i++)
        {
            itemTrans[i].gameObject.SetActive(false);
        }

        if (ItemCount < Item[0].childCount)
            BaseObject.SetActive(true);
        else
            BaseObject.SetActive(false);

        itemTrans[ItemCount].gameObject.SetActive(true);

        StartCoroutine(CoCapture(path));
    }

    private IEnumerator CoCapture(string path)
    {
        if (path == null)
        {
            yield break;
        }

        // ReadPixels을 하기 위해서 쉬어줌
        yield return new WaitForEndOfFrame();

        Rect rect = new Rect(0f, 0f, Screen.width, Screen.height);
        Texture2D texture = Capture(Camera.main, rect);

        byte[] bytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(path, bytes);

        yield return new WaitForEndOfFrame();

        ItemCount++;
        if(ItemCount < itemTrans.Count)
        {
            Invoke("AllCapture", 0.1f);
        }
        else
        {
            ItemCount = 0;
            Debug.Log("<color=blue> 스크린샷 저장 완료</color>");
        }
    }

    void CaptureSub()
    {
        string timeStr = System.DateTime.Now.Year.ToString() +
                  System.DateTime.Now.Month.ToString() +
                  System.DateTime.Now.Day.ToString() +
                  System.DateTime.Now.Hour.ToString() +
                  System.DateTime.Now.Minute.ToString()  +
                  System.DateTime.Now.Second.ToString() + "_";
        string path = Application.dataPath + "/capture_" + timeStr + screenShotCnt.ToString() + ".png";

        StartCoroutine(CoCaptureSub(path));
    }

    private IEnumerator CoCaptureSub(string path)
    {
        if (path == null)
        {
            yield break;
        }

        // ReadPixels을 하기 위해서 쉬어줌
        yield return new WaitForEndOfFrame();

        Rect rect = new Rect(0f, 0f, Screen.width, Screen.height);
        Texture2D texture = Capture(Camera.main, rect);

        byte[] bytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(path, bytes);

        yield return new WaitForEndOfFrame();
        screenShotCnt++;
        Debug.Log("<color=blue> 스크린샷 저장 완료</color>");
    }

    private Texture2D Capture(Camera camera, Rect pRect)
    {
        Texture2D capture;
        CameraClearFlags preClearFlags = camera.clearFlags;
        Color preBackgroundColor = camera.backgroundColor;
        {
            camera.clearFlags = CameraClearFlags.SolidColor;

            camera.backgroundColor = Color.black;
            camera.Render();
            Texture2D blackBackgroundCapture = CaptureView(pRect);

            camera.backgroundColor = Color.white;
            camera.Render();
            Texture2D whiteBackgroundCapture = CaptureView(pRect);

            for (int x = 0; x < whiteBackgroundCapture.width; ++x)
            {
                for (int y = 0; y < whiteBackgroundCapture.height; ++y)
                {
                    Color black = blackBackgroundCapture.GetPixel(x, y);
                    Color white = whiteBackgroundCapture.GetPixel(x, y);
                    if (black != Color.clear)
                    {
                        whiteBackgroundCapture.SetPixel(x, y, GetColor(black, white));
                    }
                }
            }

            whiteBackgroundCapture.Apply();
            capture = whiteBackgroundCapture;
            Object.DestroyImmediate(blackBackgroundCapture);
        }
        camera.backgroundColor = preBackgroundColor;
        camera.clearFlags = preClearFlags;
        return capture;
    }

    private Color GetColor(Color black, Color white)
    {
        float alpha = GetAlpha(black.r, white.r);
        return new Color(
            black.r / alpha,
            black.g / alpha,
            black.b / alpha,
            alpha);
    }

    private float GetAlpha(float black, float white)
    {
        return 1 + black - white;
    }

    private Texture2D CaptureView(Rect rect)
    {
        Texture2D captureView = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.ARGB32, false);
        captureView.ReadPixels(rect, 0, 0, false);
        return captureView;
    }
}