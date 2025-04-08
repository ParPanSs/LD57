using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class WebGlVideoLoader : MonoBehaviour
{
    public VideoPlayer videoPlayer;
  //  public  VideoClip videoClip;

    void Start()
    {
//#if UNITY_WEBGL
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = Application.streamingAssetsPath + "/Titles.mp4";
//#else
   //     videoPlayer.clip = videoClip;
//#endif

        videoPlayer.audioOutputMode = VideoAudioOutputMode.None; // временно убираем звук, если нужен
        videoPlayer.prepareCompleted += OnPrepared;
        videoPlayer.Prepare();
    }

    void OnPrepared(VideoPlayer vp)
    {
        vp.Play();
    }
}
