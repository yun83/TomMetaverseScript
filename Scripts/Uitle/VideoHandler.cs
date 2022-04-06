using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoHandler : MonoBehaviour
{
    public RawImage mScreen = null;
    public VideoClip videoClip;
    public VideoPlayer mVideoPlayer = null;
    bool playCheck = false;

    public void InitVideo(string src)
    {
        if (videoClip == null)
        {
            print(src + " 파일이 존재하지 않습니다");
            return;
        }

        playCheck = false;

        if (mScreen != null && mVideoPlayer != null)
        {
            mVideoPlayer.clip = videoClip;
            // 비디오 준비 코루틴 호출
            StartCoroutine("PrepareVideo");
        }
    }

    private void FixedUpdate()
    {
        if (!playCheck)
        {
            if (mVideoPlayer.isPrepared)
            {
                PlayVideo();
                playCheck = true;
            }
        }
    }

    protected IEnumerator PrepareVideo()
    {
        // 비디오 준비
        mVideoPlayer.Prepare();

        // 비디오가 준비되는 것을 기다림
        while (!mVideoPlayer.isPrepared)
        {
            yield return new WaitForSeconds(0.5f);
        }

        // VideoPlayer의 출력 texture를 RawImage의 texture로 설정한다
        mScreen.texture = mVideoPlayer.texture;
    }

    public void PlayVideo()
    {
        if (mVideoPlayer != null)
        {
            // 비디오 재생
            mVideoPlayer.Play();
        }
    }

    public void StopVideo()
    {
        if (mVideoPlayer != null)
        {
            // 비디오 멈춤
            mVideoPlayer.Stop();
        }
    }

    public void PauseVideo()
    {
        if (mVideoPlayer != null)
        {
            // 비디오 멈춤
            mVideoPlayer.Pause();
        }
    }
    public void OnClick_StopPlay()
    {
        if (!mVideoPlayer.isPaused)
        {
            PauseVideo();
        }
        else
        {
            PlayVideo();
        }
    }
}
