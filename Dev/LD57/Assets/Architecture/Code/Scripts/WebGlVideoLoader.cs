using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

public class WebGlVideoLoader : MonoBehaviour
{
    public VideoPlayer videoPlayer;
  //  public  VideoClip videoClip;

    private void Start()
    {
//#if UNITY_WEBGL
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = Application.streamingAssetsPath + "/Titles.mp4"; 
 //#endif
      //  videoPlayer.Play();
        //
        videoPlayer.audioOutputMode = VideoAudioOutputMode.None; // �������� ������� ����, ���� �����
         videoPlayer.prepareCompleted += OnPrepared;
         videoPlayer.Prepare();
    }

    private void OnPrepared(VideoPlayer vp)
    {
       // vp.Play();
    }
}
