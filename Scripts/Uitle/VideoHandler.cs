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
            print(src + " ������ �������� �ʽ��ϴ�");
            return;
        }

        playCheck = false;

        if (mScreen != null && mVideoPlayer != null)
        {
            mVideoPlayer.clip = videoClip;
            // ���� �غ� �ڷ�ƾ ȣ��
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
        // ���� �غ�
        mVideoPlayer.Prepare();

        // ������ �غ�Ǵ� ���� ��ٸ�
        while (!mVideoPlayer.isPrepared)
        {
            yield return new WaitForSeconds(0.5f);
        }

        // VideoPlayer�� ��� texture�� RawImage�� texture�� �����Ѵ�
        mScreen.texture = mVideoPlayer.texture;
    }

    public void PlayVideo()
    {
        if (mVideoPlayer != null)
        {
            // ���� ���
            mVideoPlayer.Play();
        }
    }

    public void StopVideo()
    {
        if (mVideoPlayer != null)
        {
            // ���� ����
            mVideoPlayer.Stop();
        }
    }

    public void PauseVideo()
    {
        if (mVideoPlayer != null)
        {
            // ���� ����
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
