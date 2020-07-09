using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

/* Switches what hand looks like when needed and takes care of steering */
public class Hands : MonoBehaviour{
	private GameObject steeringWheel;
	private GameObject turnSignal;
	private GameObject leftHand;
	private GameObject rightHand;
	private GameObject leftPalm;
	private GameObject rightPalm;
	private GameObject leftFist;
	private GameObject rightFist;
	private GameObject rightPoint;
	private GameObject person;
	private float zVal;
	public bool leftSteer;
	public bool rightSteer;
	public bool pointing;
	public bool signalling;
	public DateTime startPoint;
	public DateTime startSignal;
	public Material skin;
	public Material transparent;
	
    /* Find objects  */
    void Start(){
        steeringWheel = GameObject.Find("steeringWheel");
		turnSignal = GameObject.Find("turnSignal");
		leftHand = GameObject.Find("leftHand");
		rightHand = GameObject.Find("rightHand");
		leftPalm = GameObject.Find("leftPalm");
		rightPalm = GameObject.Find("rightPalm");
		leftFist = GameObject.Find("leftFist");
		rightFist = GameObject.Find("rightFist");
		rightPoint = GameObject.Find("rightPoint");
		person = GameObject.Find("personRoot");
		leftSteer = false;
		rightSteer = false;
		zVal = 0.06F;
	}

	/* Runs through all ways hands should need to change */
    void FixedUpdate() {
        basicMovement(leftSteer, leftHand, GetComponent<XYAxis>().getLeftX(), GetComponent<XYAxis>().getLeftY(), GetComponent<XYAxis>().getLeftBlue(), GetComponent<XYAxis>().getLeftGreen(), false, false );
		basicMovement(rightSteer, rightHand, GetComponent<XYAxis>().getRightX(), GetComponent<XYAxis>().getRightY(), GetComponent<XYAxis>().getRightBlue(), GetComponent<XYAxis>().getRightGreen(), signalling, GetComponent<GearShift>().shifting);
		shiftingMovement();
		leftSteer = holdWheel(leftHand, leftPalm, leftFist, leftSteer);
		rightSteer = holdWheel(rightHand, rightPalm, rightFist, rightSteer);
		leftSteer = letGoWheel(leftHand, leftPalm, leftFist, leftSteer, GetComponent<XYAxis>().getLeftRed());
		rightSteer = letGoWheel(rightHand, rightPalm, rightFist, rightSteer, GetComponent<XYAxis>().getRightRed());
		stopPointing();
		initSignalling();
		signalDirection();
		stopSignalling();
    }
	
	/* Moves turn signal back and stops sound */
	private void stopSignalling() {
		if (startSignal.AddSeconds(2) < DateTime.Now && turnSignal.GetComponent<AudioSource>().isPlaying) {
			Vector3 temp = turnSignal.transform.rotation.eulerAngles;
			turnSignal.transform.rotation = Quaternion.Euler(temp[0], temp[1], 0);
			turnSignal.GetComponent<AudioSource>().Stop();			
		}
	}
	
	/* Move turn signal up/down, trigger sound, and let go */
	private void signalDirection() {
		float v = GetComponent<XYAxis>().getRightY();
		if (signalling && v != 0) {
			Vector3 temp = turnSignal.transform.rotation.eulerAngles;
			float dir = Math.Abs(v) * 1 / v * 25;
			turnSignal.transform.rotation = Quaternion.Euler(temp[0], temp[1], dir);
			
			temp = rightPalm.transform.localRotation.eulerAngles;
			rightPalm.transform.localRotation = Quaternion.Euler(temp[0] + 300.4F, temp[1] + 87F, temp[2] + 273F);
			rightHand.transform.Translate(1.5F,0,0);
	
			turnSignal.GetComponent<AudioSource>().Play();
			signalling = false;
			startSignal = DateTime.Now;
		}
	}
	
	/* If colliders cross, move hand to signal mode */
	private void initSignalling() {
		if (turnSignal.GetComponent<Collider>().bounds.Intersects(rightHand.GetComponent<Collider>().bounds) && !signalling) {
			signalling = true;
			Vector3 temp = rightPalm.transform.localRotation.eulerAngles;
			rightPalm.transform.localRotation = Quaternion.Euler(temp[0] - 300.4F, temp[1] - 87F, temp[2] - 273F);
		}
	}
	
	/* Stop pointing after certain amount of time */
	private void stopPointing() {
		if (pointing) {
			if (startPoint.AddSeconds(0.25) < DateTime.Now) {
				pointing = false;
				changeHands(rightPalm, rightPoint);
			}
		}
	}
	
	/* Change had after letting go of wheel */
	private bool letGoWheel(GameObject hand, GameObject palm, GameObject fist, bool steer, bool button) {
		if (button && steer) {
			hand.transform.Translate(0,0,-2);
			hand.transform.parent = person.transform;
			changeHands(palm, fist);
			return false;
        }
		return steer;
	}
	
	/* Changes hand for steering if intersect */
	private bool holdWheel(GameObject hand, GameObject palm, GameObject fist, bool steer) {
		float sign = 1;
		if (hand == GameObject.Find("leftHand")) { sign = -1; }
		if (steeringWheel.GetComponent<Collider>().bounds.Intersects(hand.GetComponent<Collider>().bounds) && !steer) {			
			hand.transform.position = steeringWheel.transform.position;
			hand.transform.Translate(sign * 1.2F,0.4F,0, Space.Self);
			hand.transform.parent = steeringWheel.transform;
			changeHands(fist, palm);
			pointing = false;
			return true;
		}
		return steer;
	}
	
	/* Can only move in shifting area */ 
	private void shiftingMovement() {
		float z = 0;
		if (GetComponent<XYAxis>().getRightBlue()) {
			z = zVal;
		}
		if (GetComponent<XYAxis>().getRightGreen()) {
			z = -zVal;
		}
		if (GetComponent<GearShift>().shifting) {
			GameObject baseplate = GameObject.Find("gearshift");
			float top = baseplate.transform.position[2] + 0.7F;
			float bottom = baseplate.transform.position[2] - 1F;
			if (rightHand.transform.position[2] < top || z < 0 ) {
				if (rightHand.transform.position[2] > bottom || z > 0) {
					rightHand.transform.Translate(0,0,z);
				}
			}
		}
	}
	
	/* Handles movement when not steering or signalling */
	private void basicMovement(bool steer, GameObject hand, float h, float v, bool zPlus, bool zMinus, bool signal, bool shift) {
		float z = 0;
		if (zPlus) {
			z = zVal;
		}
		if (zMinus) {
			z = -zVal;
		}
		
		if (!steer && !signal && !shift) {
			hand.transform.Translate(h,v, z);
			stayOnScreen(hand);
			Quaternion temp = hand.transform.rotation;
			temp[2] = 0;
			hand.transform.rotation = temp;
		}
	}
	
	/* Moves hands back within bounds */
	private void stayOnScreen(GameObject hand) {
		Camera cam = GameObject.Find("Main Camera").GetComponent<Camera>();
		Vector3 screenPos = cam.WorldToScreenPoint(hand.transform.position);
		float x = screenPos[0];
		float y = screenPos[1];
		float z = screenPos[2];
		
		
		if (x < 0) {
			screenPos[0] = 2;
		}
		if (x > cam.pixelWidth ) {
			screenPos[0] = cam.pixelWidth - 2;
		}
		if (y < 0) {
			screenPos[1] = 2;
		}
		if (y > cam.pixelHeight ) {
			screenPos[1] = cam.pixelHeight - 2;
		}
		if (z < 0) {
			screenPos[2] = 1;
		}
		hand.transform.position =  cam.ScreenToWorldPoint(screenPos);
		
	}
	
	/* Changes which model is visible */
	public void changeHands(GameObject inH, GameObject outH) {
		inH.transform.GetChild(0).GetComponent<MeshRenderer>().material = skin;
		outH.transform.GetChild(0).GetComponent<MeshRenderer>().material = transparent;
	}
	
	
}
