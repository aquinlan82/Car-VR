using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArduinoBluetoothAPI;
using System;

public class Bluetooth : MonoBehaviour
{
    private BluetoothHelper helper;
    private string deviceName;
	private string msg;
	
	
    // Start is called before the first frame update
    void Start()
    {
		msg = "";
		deviceName = "BabyDozer";
        try
        {
            helper = BluetoothHelper.GetInstance(deviceName);
            helper.OnConnected += OnConnected;
            helper.OnConnectionFailed += OnConnectionFailed;
            helper.setTerminatorBasedStream("\n"); //every messages ends with new line character
        
			//Debug.Log(helper.isDevicePaired());
			if(helper.isDevicePaired()){helper.Connect();}
			
		}
        catch (Exception){}

    }

    void OnConnected()
    {
        helper.StartListening();
    }

    void OnConnectionFailed()
    {
    }
	
	void FixedUpdate() {
		if(helper.Available) {
			msg = helper.Read();
		} 
	}
	
	public string getMsg() {
		return this.msg;
	}
}
