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

    private bool PlayVideo = false;
    private bool PlayVideoIconShow = false;
    private int State = 0;
    private bool videoInitCheck = false;
    bool SoundController = false;

    float PlayerDis;
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
        PlayVideo = false;
        PlayVideoIconShow = false;
        IconButton.SetActive(false);
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

        if (!PlayVideo)
        {
            PlayerDis = Vector3.Distance(DataInfo.ins.infoController.PlayerObject.position, transform.position);
            if (2 > PlayerDis)
            {
                if (!PlayVideoIconShow)
                {
                    PlayVideoIconShow = true;
                    IconButton.SetActive(true);
                }
            }
            else
            {
                if (PlayVideoIconShow)
                {
                    PlayVideoIconShow = false;
                    IconButton.SetActive(false);
                }
            }
        }
    }

    public void OnClick_TV()
    {
        if (State == 1)
        {
            vImage.color = Color.black;
            vHandler.Stop();
            PlayVideo = false;
            State = 0;
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
        //코루틴 실행중 중복 방지
        videoInitCheck = true;

        RemoteController.SetActive(true);
        vHandler.clip = videoClip;

        yield return null;

        // 비디오 준비
        vHandler.Prepare();

        // 비디오가 준비되는 것을 기다림
        while (!vHandler.isPrepared)
        {
            yield return new WaitForSeconds(0.1f);
        }

        vImage.color = Color.white;
        yield return null;


        vHandler.Play();

        yield return null;

        PlayVideo = true;
        videoInitCheck = false;
        PlayVideoIconShow = false;
        IconButton.SetActive(false);

        State = 1;
    }
}
