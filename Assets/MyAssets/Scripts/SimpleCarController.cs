using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/**
Car VR
Created by Allison Quinlan
Started June 2020
**/

[System.Serializable]
public class AxleInfo {
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
    public bool steering;
}
 
/* Makes Car Go */
public class SimpleCarController : MonoBehaviour {
    public List<AxleInfo> axleInfos; 
	public GameObject steeringWheel;
	public GameObject cam;
	public GameObject body;
	private GameObject root;
	private GameObject brake;
	private GameObject gas;
    public float maxMotorTorque;
    public float maxSteeringAngle;
	private float motor;
	private float braker;
	
	/* Find all objects */
	public void Start() {
			steeringWheel = GameObject.Find("steeringWheel");
			cam = GameObject.Find("personRoot");
			body = GameObject.Find("Cube");
			root = GameObject.Find("carRoot");
			brake = GameObject.Find("brake");
			gas = GameObject.Find("gas");
	}
	
     
    /* Finds the corresponding visual wheel correctly applies the transform */
    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0) {
            return;
        }
     
        Transform visualWheel = collider.transform.GetChild(0);
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);
     
		//rotate visual wheels to look right
		rotation = rotation * Quaternion.Euler(new Vector3(0,90,0));
	 
		//move and spin visual wheels
        visualWheel.transform.position = position;
		visualWheel.transform.rotation = rotation;
    }
     
	/* On each iteration, rotate physical parts and apply motor/brake */
    public void FixedUpdate() {
		
		motor = 0;
		braker = 0;
		setMotor();
		pushPedals(KeyCode.LeftShift, gas);
		pushPedals(KeyCode.LeftControl, brake);
		
		//rotate steering wheel
		float steering = maxSteeringAngle * Input.GetAxis("Horizontal");
		float y = body.transform.eulerAngles[1] + 90;
		steeringWheel.transform.rotation = Quaternion.Euler(steering,y,90);
		
		spinWheels(steering);
	 
	}
	
	/* Sets motor and braker variables */
	private void setMotor() {
		if (GetComponent<GearShift>().pos == "D") {
			setDriveMotor(1);
		}
		if (GetComponent<GearShift>().pos == "R") {
			setDriveMotor(-1);
		}
		// in park don't move
		if (GetComponent<GearShift>().pos == "P") {
			braker = Mathf.Infinity;
			motor = 0;
		}
		// in neutral don't stop movement, just motor
		if (GetComponent<GearShift>().pos == "N") {
			motor = 0;
		}
		
	}
	
	/* Spin car wheels */
	private void spinWheels(float steering) {
		foreach (AxleInfo axleInfo in axleInfos) {
            if (axleInfo.steering) {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor) {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
				axleInfo.leftWheel.brakeTorque = braker;
                axleInfo.rightWheel.brakeTorque = braker;
            }
            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }
	}
	
	/* Called every iteration, pushes pedals reguardless of gear */
	private void pushPedals(KeyCode code, GameObject pedal) {
		if (Input.GetKey(code)) {
			//push pedal
			Vector3 loc = pedal.transform.eulerAngles;
			loc[0] = -20;
			pedal.transform.eulerAngles = loc;
		} else {
			//don't push 
			Vector3 loc = pedal.transform.eulerAngles;
			loc[0] = -50;
			pedal.transform.eulerAngles = loc;
		}
	}
	
	/* Only called when in D or R */
	private void setDriveMotor(float sign) {
		//gas
		if (Input.GetKey(KeyCode.LeftShift)) {
			root.GetComponent<Rigidbody>().isKinematic = false;
			motor = maxMotorTorque * 10 * sign;
		} 
		//brake
		if (Input.GetKey(KeyCode.LeftControl)) {
			braker = maxMotorTorque * 15;
		}
	}
}