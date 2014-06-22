using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using JDI.Common;
using JDI.Common.Logger;
using JDI.Pusher.Client;
using JDI.Common.Utils;
using System;

public class FPSBackProcess : MonoBehaviour {
	public enum EventType
	{
		none,
		light_off,
		light_tenmetu,
		light_rev
	}
	public EventType et = EventType.none;
	public FileLogger fileLogger;
	public string pusherAppKey = "fe4a15e1a9a1df8517e5";
	public string channelName = "channel";
	public PusherClient pusherClient;
	public PusherChannel pusherChannel;
	public Light HandLight = null;
	public float HandLightMax = 0.8f;

	private int lightFrame = 0;

	// Use this for initialization
	void Start () {
		// setup options
		PusherOptions options = new PusherOptions();
		options.EncryptionEnabled = false;
		options.MaskingEnabled = false;
		
		// init pusher client
		this.pusherClient = new PusherClient("Pusher", options);
		
		// attach event handlers
		this.pusherClient.ConnectionChanged += new PusherDelegates.ConnectionChangedEventHandler(pusherClient_ConnectionChanged);
		this.pusherClient.Connected += new PusherDelegates.ConnectedEventHandler(pusherClient_Connected);
		this.pusherClient.Disconnected += new PusherDelegates.DisconnectedEventHandler(pusherClient_Disconnected);
		this.pusherClient.Error += new PusherDelegates.ErrorEventHandler(pusherClient_Error);
		
		// monitor all events
		this.pusherClient.BindAll(this.pusherClient_BindAll);

		HandLight = (Light)GameObject.Find("Spotlight").GetComponent("Light");

		Connect();
	}
	void OnApplicationQuit()
	{
		if (this.pusherClient != null)
		{
			this.pusherClient.UnBindAll(this.pusherClient_BindAll);
			this.pusherClient.Disconnect();
			this.pusherClient.Dispose();
		}
		this.pusherClient = null;
		
		if (this.fileLogger != null)
		{
			this.fileLogger.Close();
			this.fileLogger.Dispose();
		}
		this.fileLogger = null;
	}
	void Update () {
		if (et == EventType.light_off) 
		{
			HandLight.intensity = 0f;
			et = EventType.light_rev;
		}
		if (et == EventType.light_rev) 
		{
			HandLight.intensity = HandLight.intensity + 0.005f;
			if (HandLight.intensity >= HandLightMax)
			{
				et = EventType.none;
				HandLight.intensity = HandLightMax;
			}
		}
		if (et == EventType.light_tenmetu) 
		{
			lightFrame++;
			if (lightFrame % 20 == 0)
			{
				HandLight.intensity = HandLightMax;
			}
			else if (lightFrame % 10 == 0)
			{
				HandLight.intensity = 0;
			}
			else if (lightFrame >= 120)
			{
				lightFrame = 0;
				HandLight.intensity = 0f;
				et = EventType.light_rev;
			}
		}
	}
	
	
	//接続処理
	private void Connect()
	{
		this.pusherClient.Connect(this.pusherAppKey);
	}

	private void pusherChannell_BindAll(string eventName, string eventData)
	{
		//Logger.WriteInfo("Main", "BindAll Event: " + eventName + ", Data: " + eventData);
	}

	
	private void pusherClient_BindAll(string eventName, string eventData)
	{
		//Logger.WriteInfo("Main", "BindAll Event: " + eventName + ", Data: " + eventData);
	}
	
	private void pusherClient_ConnectionChanged(PusherState pusherState)
	{
		//Logger.WriteInfo("Main", "Connection Status: " + pusherState.Name());
		//this.UpdateUI((int)pusherState, null, null);
	}
	
	private void pusherClient_Connected()
	{
		this.pusherChannel = pusherClient.Subscribe(channelName, new PusherDelegates.ChannelStatusCallback(pusherChannel_StatusChanged));
		//this.UpdateUI((int)PusherState.Connected, null, null);
	}
	
	private void pusherClient_Disconnected()
	{
		//this.UpdateUI((int)PusherState.Disconnected, null, null);
	}
	
	private void pusherClient_Error(string message, string stackTrace)
	{
		//Logger.WriteError("Main", message, stackTrace);
		//this.UpdateUI(null, null, "Error: " + message);
	}
	
	private void pusherChannel_StatusChanged(PusherChannelState channelState)
	{
		//Logger.WriteInfo("Main", "Channel Status: " + channelState.Name());
		//this.UpdateUI(null, (int)channelState, null);
		if (channelState == PusherChannelState.Subscribed)
		{
			this.pusherChannel.BindAll(this.pusherChannel_BindAll);
		}
		else if (this.pusherChannel != null)
		{
			this.pusherChannel.UnBindAll(this.pusherChannel_BindAll);
		}
	}

	/// <summary>
	/// Pushers the channel_ bind all.
	/// チャンネルで何か起きたらここに
	/// </summary>
	/// <param name="eventName">Event name.</param>
	/// <param name="eventData">Event data.</param>
	private void pusherChannel_BindAll(string eventName, string eventData)
	{
		//Logger.WriteInfo("Main", "Channel Event: " + eventName + ", Data: " + eventData);
		Console.WriteLine (eventName + "/" + eventData);
		if (eventName == "light_off") 
		{
			et = EventType.light_off;
		}
		if (eventName == "light_tenmetu") 
		{
			et = EventType.light_tenmetu;
		}
	}

	#region ILogger implementation
	
	private delegate void WriteLineCallback(string msgType, string date, string time, string source, string message, string stackTrace);
	
	private void WriteToLog(string msgType, string date, string time, string source, string message, string stackTrace)
	{
		Console.WriteLine (msgType + "/" + date + "/" + time + "/" + source + "/" + message + "/" + stackTrace);
	}
	
	public void WriteError(string source, string message, string stackTrace)
	{
		this.WriteToLog("Error", DateTime.Now.ToString("MM/dd/yyyy"), DateTime.Now.ToString("HH:mm:ss.fff"), source, message, stackTrace);
	}
	
	public void WriteInfo(string source, string message)
	{
		this.WriteToLog("Info", DateTime.Now.ToString("MM/dd/yyyy"), DateTime.Now.ToString("HH:mm:ss.fff"), source, message, "");
	}
	
	public void WriteDebug(string source, string message)
	{
		this.WriteToLog("Debug", DateTime.Now.ToString("MM/dd/yyyy"), DateTime.Now.ToString("HH:mm:ss.fff"), source, message, "");
	}
	
	#endregion
}
