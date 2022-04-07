using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TvController : MonoBehaviour
{
    private Canvas mCanvas;
    VideoPlayer vHandler;
    RawImage vImage;

    public VideoClip videoClip;
    public GameObject IconButton;
    public GameObject RemoteController;

    private bool PlayVideoInit = false;
    private int State = 0;
    private bool videoInitCheck = false;
    bool SoundController = false;
    // Start is called before the first frame update
    void Start()
    {
        mCanvas = GetComponent<Canvas>();
        mCanvas.worldCamera = Camera.main;

        vHandler = mCanvas.GetComponentInChildren<VideoPlayer>();
        vImage = mCanvas.GetComponentInChildren<RawImage>();
        vImage.color = Color.black;

        State = 0;
        videoInitCheck = false;
        IconButton.SetActive(true);
        RemoteController.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (SoundController != DataInfo.ins.OptionInfo.EffectSound)
        {
            SoundController = DataInfo.ins.OptionInfo.EffectSound;

            if (SoundController)
                vHandler.SetDirectAudioMute(0, false);
            else
                vHandler.SetDirectAudioMute(0, true);
        }
    }

    public void OnClick_TV()
    {
        if (State == 1)
        {
            vImage.color = Color.black;
            vHandler.Stop();
            State = 0;
            IconButton.SetActive(true);
            RemoteController.SetActive(false);
        }
        else
        {
            if (!videoInitCheck)
            {
                StartCoroutine(PrepareVideo());
            }
        }
    }

    protected IEnumerator PrepareVideo()
    {
        //�ڷ�ƾ ������ �ߺ� ����
        videoInitCheck = true;

        IconButton.SetActive(false);
        RemoteController.SetActive(true);
        vHandler.clip = videoClip;

        yield return null;

        // ���� �غ�
        vHandler.Prepare();

        // ������ �غ�Ǵ� ���� ��ٸ�
        while (!vHandler.isPrepared)
        {
            yield return new WaitForSeconds(0.1f);
        }

        vImage.color = Color.white;
        yield return null;


        vHandler.Play();

        yield return null;

        videoInitCheck = false;
        State = 1;
    }
}