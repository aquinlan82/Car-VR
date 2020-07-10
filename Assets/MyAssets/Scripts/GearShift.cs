using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class GearShift : MonoBehaviour {
	
	public bool shifting;
	public GameObject gearShift;
	private GameObject rightHand;
	private GameObject baseplate;
	private GameObject label;
	public String pos;
	public Material parkSkin;
	public Material reverseSkin;
	public Material neutralSkin;
	public Material driveSkin;
	private DateTime startTime;
	
	
    /* Find objects */
    void Start() {
        gearShift = GameObject.Find("gearshift").transform.GetChild(0).gameObject;;
		baseplate  = GameObject.Find("gearshift");
		rightHand = GameObject.Find("rightHand");
		label = GameObject.Find("label");
		pos = "P";
		startTime = DateTime.Now;
    }

    /*  Handles grabbing shifter and changing modes */
    void FixedUpdate() {
		grabOrLetGo();

		if (!shifting) {	
			float[] increments = getIncrements();
			float cur = gearShift.transform.localPosition[2];
			setMode(increments, cur);
			keepInBounds(increments[0], increments[increments.Length - 1], cur);
		}
    }
	
	/* Sets gear and skin */
	private void setMode(float[] increments, float cur) {
		string[] modes = new string[] {"P", "R", "N", "D" };
		Material[] skins = new Material[] {parkSkin, reverseSkin, neutralSkin, driveSkin };
		
		for (int i = 0; i < modes.Length; i++) {	
			if (cur < increments[i] && cur > increments[i+1]) {
				pos = modes[i];
				label.GetComponent<MeshRenderer>().material = skins[i];
			}
		}
	}
	
	/* Move shifter back if out of bounds */
	private void keepInBounds(float top, float bottom, float cur) {
		if (cur > top) {
				Vector3 temp = gearShift.transform.localPosition;
				temp[2] = top;
				gearShift.transform.localPosition = temp;
			}
			
			if (cur < bottom) {
				Vector3 temp = gearShift.transform.localPosition;
				temp[2] = bottom;
				gearShift.transform.localPosition = temp;
			}
	}
	
	/* Get transitions between shift modes */
	private float[] getIncrements() {
		float width = (label.transform.localScale[2] / 4F);
		//Debug.Log(width + " " + GameObject.Find("label").transform.lossyScale[2]);
		float centerWidth = label.transform.localScale[2] / 2;
		float top = label.transform.localPosition[2] + centerWidth;
		float underP = top - width;
		float underR = top - (width * 2);
		float underN = top - (width * 3);
		float bottom = top - (width * 4); 
		
		
		return new float[] {top, underP, underR, underN, bottom };
	}
	
	/* To let go, press enter
	   To grab, press gas and enter while touching shifter */
	private void grabOrLetGo() {
		if (shifting && GetComponent<XYAxis>().getRightRed() && sleepOver()) {
			gearShift.transform.parent = GameObject.Find("gearshift").transform;
			shifting = false;
			startTime = DateTime.Now;
		} else if (gearShift.GetComponent<Collider>().bounds.Intersects(rightHand.GetComponent<Collider>().bounds)
		&& GetComponent<XYAxis>().getRightRed()
		&& sleepOver()) {
			gearShift.transform.parent = rightHand.transform;
			shifting = true;
			startTime = DateTime.Now;
		}
	}
	
	/* True 1 second after start time set */
	private bool sleepOver() {
		return DateTime.Now > startTime.AddSeconds(1);
	}
	
}
