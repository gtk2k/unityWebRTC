using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimplePeerConnectionM;
using System;
using System.Runtime.InteropServices;
using System.Threading;

public class WebRtcController : MonoBehaviour
{
    private WebRtcCore webRtcCore;
    public WebRtcMsgExchanger webRtcMsgExchanger;
    public GameObject[] RenderingTargets;
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
    public Texture2D desktopTexture;
#elif UNITY_IPHONE
    public RenderTexture SubCameraTexture;
#endif

    [Serializable]
    public class Size
    {
        public int width;
        public int height;
    }
    public Size StreamSize;

    // Use this for initialization
    void Start()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        webRtcCore = new WebRtcCoreWindows(StreamSize.width, StreamSize.height);

        desktopTexture = new Texture2D(StreamSize.width, StreamSize.height);
        Win32Api.BeginDesktopCapture(StreamSize.width, StreamSize.height);
#elif UNITY_IPHONE
		webRtcCore = new WebRtcCoreiOS(StreamSize.width, StreamSize.height);
#endif

        webRtcCore.MsgExchanger = webRtcMsgExchanger;

        foreach (GameObject tage in RenderingTargets)
        {
            tage.GetComponent<Renderer>().material.mainTexture = webRtcCore.ReceivedTexture2D;
        }
    }

    public void RequestCreateOffer()
    {
        webRtcCore.CreateOffer();
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        Win32Api.StretchDesktopCapture();
        desktopTexture.LoadRawTextureData(Win32Api.pPixelData, 4 * StreamSize.width * StreamSize.height);
        webRtcCore.FrameGate_Input(desktopTexture);
#elif UNITY_IPHONE
        webRtcCore.FrameGate_Input(SubCameraTexture);
#endif
        webRtcCore.Update();
    }

    private void OnDestroy()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        Win32Api.EndDesktopCapture();
#endif
        webRtcCore.Close();
    }
}



































