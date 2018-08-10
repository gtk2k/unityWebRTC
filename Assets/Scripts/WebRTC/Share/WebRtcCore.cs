using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebRtcCore
{
    
    
    public Texture2D ReceivedTexture2D;
    public int width = 480;
    public int height = 640;

    private Texture2D inputTexture = null;
    private WebRtcMsgExchanger msgExchanger;

    public WebRtcCore(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    public WebRtcMsgExchanger MsgExchanger
    {
        set
        {
            msgExchanger = value;
            msgExchanger.RtcCore = this;
        }
        get
        {
            return this.msgExchanger;
        }
    }

    virtual public void Close()
    {
    }

    virtual public void CreateOffer()
    {
    }


    virtual public void Update()
    {
    }

    virtual public void FrameGate_Input(Texture2D tex)
    {
    }
    virtual public void FrameGate_Input(RenderTexture rtex)
    {
        if(inputTexture == null) inputTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = rtex;
        if (inputTexture.width != rtex.width || inputTexture.height != rtex.height) return;
        inputTexture.ReadPixels(new Rect(0, 0, rtex.width, rtex.height), 0, 0);
        inputTexture.Apply();
        RenderTexture.active = currentRT;
        FrameGate_Input(inputTexture);
    }

    virtual public void Destroy()
    {
    }

    virtual public void ReceivedMessage(string description, string message)
    {
    }
    








}




















