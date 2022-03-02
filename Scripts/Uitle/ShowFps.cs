using UnityEngine;
using System.Collections;


public class ShowFps : MonoBehaviour
{
    [Range(1, 100)]
    public int FontSizeOf = 20;
    [Range(0, 1)]
    public float Red, Green, Blue;

    float mDeltaTime = 0.0f;
    GUIStyle mStyle = new GUIStyle();
    void Awake()
    {
    }

    void Update()
    {
        mDeltaTime += (Time.unscaledDeltaTime - mDeltaTime) * 0.1f;
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        Rect rect = new Rect(0, 0, w, h * 2/100);
        float msce = mDeltaTime * 1000.0f;
        float fps = 1.0f / mDeltaTime;
        string fpsString = string.Format("{0:0,0} ms ({1:0.} fps)", msce, fps);

        mStyle.alignment = TextAnchor.UpperLeft;
        mStyle.fontSize = h * 2 / FontSizeOf;
        mStyle.normal.textColor = new Color(Red, Green, Blue, 1.0f);

        GUI.Label(rect, fpsString, mStyle);
    }
}