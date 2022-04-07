using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class videoController : MonoBehaviour
{
    private Canvas mCanvas;
    VideoPlayer vHandler;
    RawImage vImage;

    public VideoClip videoClip;
    public bool Mute = true;
    bool SoundController = false;
    bool MuteCheck = false;
    void Start()
    {
        mCanvas = GetComponent<Canvas>();
        mCanvas.worldCamera = Camera.main;

        vHandler = mCanvas.GetComponentInChildren<VideoPlayer>();
        vImage = mCanvas.GetComponentInChildren<RawImage>();
        vImage.color = Color.black;

        StartCoroutine(PrepareVideo());
    }

    // Update is called once per frame
    void Update()
    {
        if (MuteCheck != Mute)
        {
            MuteCheck = Mute;

            if (MuteCheck)
            {
                vHandler.SetDirectAudioMute(0, false);
            }
            else
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
        }
    }

    protected IEnumerator PrepareVideo()
    {
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
    }
}
