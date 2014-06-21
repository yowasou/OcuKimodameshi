using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using JDI.Common;
using JDI.Common.Logger;
using JDI.Pusher.Client;
using JDI.Common.Utils;
using System;


public class FPSBackProcess : MonoBehaviour {
	private FileLogger fileLogger;
	private string pusherAppKey = "fe4a15e1a9a1df8517e5";
	private PusherClient pusherClient;
	private PusherChannel pusherChannel;

	// Use this for initialization
	void Start () {
		// setup options
		PusherOptions options = new PusherOptions();
		options.EncryptionEnabled = true;
		options.MaskingEnabled = true;
		
		// init pusher client
		this.pusherClient = new PusherClient("Pusher", options);
		
		// attach event handlers
		this.pusherClient.ConnectionChanged += new PusherDelegates.ConnectionChangedEventHandler(pusherClient_ConnectionChanged);
		this.pusherClient.Connected += new PusherDelegates.ConnectedEventHandler(pusherClient_Connected);
		this.pusherClient.Disconnected += new PusherDelegates.DisconnectedEventHandler(pusherClient_Disconnected);
		this.pusherClient.Error += new PusherDelegates.ErrorEventHandler(pusherClient_Error);
		
		// monitor all events
		this.pusherClient.BindAll(this.pusherClient_BindAll);
		
		this.pusherChannel = null;
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

	}
	
	
	//接続処理
	private void Connect()
	{
		this.pusherClient.Connect(this.pusherAppKey);
	}
	
	
	private void pusherClient_BindAll(string eventName, string eventData)
	{
		Logger.WriteInfo("Main", "BindAll Event: " + eventName + ", Data: " + eventData);
	}
	
	private void pusherClient_ConnectionChanged(PusherState pusherState)
	{
		Logger.WriteInfo("Main", "Connection Status: " + pusherState.Name());
		//this.UpdateUI((int)pusherState, null, null);
	}
	
	private void pusherClient_Connected()
	{
		//this.UpdateUI((int)PusherState.Connected, null, null);
	}
	
	private void pusherClient_Disconnected()
	{
		//this.UpdateUI((int)PusherState.Disconnected, null, null);
	}
	
	private void pusherClient_Error(string message, string stackTrace = null)
	{
		Logger.WriteError("Main", message, stackTrace);
		//this.UpdateUI(null, null, "Error: " + message);
	}
	
	private void pusherChannel_StatusChanged(PusherChannelState channelState)
	{
		Logger.WriteInfo("Main", "Channel Status: " + channelState.Name());
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
	
	private void pusherChannel_BindAll(string eventName, string eventData)
	{
		Logger.WriteInfo("Main", "Channel Event: " + eventName + ", Data: " + eventData);
	}

	#region ILogger implementation
	
	private delegate void WriteLineCallback(string msgType, string date, string time, string source, string message, string stackTrace);
	
	private void WriteToLog(string msgType, string date, string time, string source, string message, string stackTrace)
	{
		Console.WriteLine (msgType + "/" + date + "/" + time + "/" + source + "/" + message + "/" + stackTrace);
	}
	
	public void WriteError(string source, string message, string stackTrace = "")
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
