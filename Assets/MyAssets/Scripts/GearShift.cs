using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
	
	
    /* Find objects */
    void Start() {
        gearShift = GameObject.Find("gearshift").transform.GetChild(0).gameObject;;
		baseplate  = GameObject.Find("gearshift");
		rightHand = GameObject.Find("rightHand");
		label = GameObject.Find("label");
		pos = "P";
    }

    /*  Handles grabbing shifter and changing modes */
    void Update() {
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
		
		for (int i = 0; i < modes.Length - 1; i++) {	
			if (cur < increments[i] && cur > increments[i+1]) {
				pos = modes[i+1];
				label.GetComponent<MeshRenderer>().material = skins[i+1];
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
		float width = (0.9F / 4F) - 0.01F;
		float top = 0.4F;
		float underP = top - width;
		float underR = top - (width * 2);
		float underN = top - (width * 3);
		float bottom = top - (width * 4); 
		return new float[] {top, underP, underR, underN, bottom };
	}
	
	/* To let go, press enter
	   To grab, press gas and enter while touching shifter */
	private void grabOrLetGo() {
		if (shifting && Input.GetKeyDown("return")) {
			gearShift.transform.parent = GameObject.Find("gearshift").transform;
			shifting = false;
		} else if (gearShift.GetComponent<Collider>().bounds.Intersects(rightHand.GetComponent<Collider>().bounds)
		&& Input.GetKeyDown("return")
		&& Input.GetKey(KeyCode.LeftControl) ) {
			gearShift.transform.parent = rightHand.transform;
			shifting = true;
		}
	}
}
