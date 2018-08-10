using System;
using UnityEngine;
using WebSocketSharp;
using System.Threading;

public class WebSocketController : WebRtcMsgExchanger
{
    [Serializable]
    public class SignalingMessage
    {
        public string type;
        public string message;
    }

    public string WebSocketServerURL = "ws://localhost:8888";

    SynchronizationContext context;
    WebSocket wsClient;

    // Use this for initialization
    void Start()
    {
        context = SynchronizationContext.Current;
        wsClient = new WebSocket(WebSocketServerURL);
        wsClient.OnOpen += (s, e) => Debug.Log("WebSocket Open");
        wsClient.OnMessage += ReceivedMessage;
        wsClient.OnClose += (s, e) => Debug.Log("WebSocket Close");
        wsClient.Connect();
    }

    void ReceivedMessage(object sender, MessageEventArgs e)
    {
        context.Post((state) =>
        {
            var msg = JsonUtility.FromJson<SignalingMessage>((string)state);
            WebRtcCtr_ReceivedMessage(msg.type, msg.message);
        }, e.Data);
    }

    void SendMessage(string type, string message)
    {
        var msg = new SignalingMessage() { type = type, message = message };
        wsClient.Send(JsonUtility.ToJson(msg));
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        if (wsClient == null) return;
        wsClient.Close();
    }

    public override void RequiredSendingMessage(string type, string message)
    {
        if (wsClient == null)
        {
            Debug.Log("WebSocket err");
            return;
        }
        if (wsClient.ReadyState == WebSocketState.Open)
            SendMessage(type, message);
    }
}
