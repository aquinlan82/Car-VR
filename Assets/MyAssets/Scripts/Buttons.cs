﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Buttons : MonoBehaviour
{
	public bool[] buttons;
	private GameObject button1;
	private GameObject button2;
	private GameObject button3;
	private GameObject button4;
	private GameObject leftHand;
	private GameObject rightHand;
	private GameObject rightPalm;
	private GameObject rightPoint;
	private GameObject leftWiper;
	private GameObject rightWiper;
	private bool wipeUp;
	private bool wiping;
	
	
    /* Find objects */
    void Start() {
        button1 = GameObject.Find("button1").transform.GetChild(0).gameObject;
		button2 = GameObject.Find("button2").transform.GetChild(0).gameObject;;
		button3 = GameObject.Find("button3").transform.GetChild(0).gameObject;;
		button4 = GameObject.Find("button4").transform.GetChild(0).gameObject;;
		leftHand = GameObject.Find("leftHand");
		rightHand = GameObject.Find("rightHand");
		rightPalm = GameObject.Find("rightPalm");
		rightPoint= GameObject.Find("rightPoint");
		leftWiper = GameObject.Find("leftWiperRoot");
		rightWiper = GameObject.Find("rightWiperRoot");
		buttons = new bool[4];
		wipeUp = true;
		wiping = false;
    }
	
	/* Check buttons pressed */
    void FixedUpdate() {
		checkButtons(button1);
		checkButtons(button2);
		checkButtons(button3);
		checkButtons(button4);	
	
		if (wiping) {
			wipe();
		}
    }
	
	/* If colliding, button pressed, and button blue, turn on
		If colliding, button pressed, and button red, turn off */
	private void checkButtons(GameObject button) {
		if (button.GetComponent<Collider>().bounds.Intersects(rightHand.GetComponent<Collider>().bounds) 
		&& GetComponent<XYAxis>().getRightRed()
		&& button.GetComponent<Renderer>().material.GetColor("_Color") != Color.red
		&& !GetComponent<Hands>().pointing) {
			button.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
			GetComponent<Hands>().startPoint = DateTime.Now;
			GetComponent<Hands>().pointing = true;
			GetComponent<Hands>().changeHands(rightPoint, rightPalm);
			turnOn(button);
		} else if (button.GetComponent<Collider>().bounds.Intersects(rightHand.GetComponent<Collider>().bounds) 
		&& GetComponent<XYAxis>().getRightRed()
		&& button.GetComponent<Renderer>().material.GetColor("_Color") == Color.red
		&& !GetComponent<Hands>().pointing) {
			GetComponent<Hands>().startPoint = DateTime.Now;
			GetComponent<Hands>().pointing = true;
			GetComponent<Hands>().changeHands(rightPoint, rightPalm);
			button.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
			turnOff(button);
		}
	}
	
	/* Roll down window */
	private void rolldown(GameObject window) {
		Camera cam = GameObject.Find("Main Camera").GetComponent<Camera>();
		float y = cam.WorldToScreenPoint(window.transform.position)[1];
		window.transform.Translate(0,-10F,0);
	}
	
	/* Roll up window */
	private void rollup(GameObject window) {
		Camera cam = GameObject.Find("Main Camera").GetComponent<Camera>();
		float y = cam.WorldToScreenPoint(window.transform.position)[1];
		window.transform.Translate(0,10F,0);
	}
	
	/* Activate button when first pressed */
	private void turnOn(GameObject button) {
		if (button == button1) {
			//music on
			button.GetComponent<AudioSource>().Play();
		}
		if (button == button2) {
			//left window down
			GameObject window = GameObject.Find("leftWindow");
			rolldown(window);
		}
		if (button == button3) {
			//right window down
			GameObject window = GameObject.Find("rightWindow");
			rolldown(window);
		}
		if (button == button4) {
			//windshied wipers
			wiping = true;
		}
	}
	
	
	
	/* Initiates turning off button */
	private void turnOff(GameObject button) {
		if (button == button1) {
			//music off
			button.GetComponent<AudioSource>().Stop();
		}
		if (button == button2) {
			//left window up
			GameObject window = GameObject.Find("leftWindow");
			rollup(window);
		}
		if (button == button3) {
			//right window up
			GameObject window = GameObject.Find("rightWindow");
			rollup(window);
		}
	}
	
	/* Moves windshield wiper */
	private void wipe() {
		float cur = rightWiper.transform.rotation.eulerAngles[2];
		float size = 5;
		if (wipeUp) {
			rightWiper.transform.Rotate(0,0,size);
			leftWiper.transform.Rotate(0,0,-size);
		}
		if (!wipeUp) {
			rightWiper.transform.Rotate(0,0,-size);
			leftWiper.transform.Rotate(0,0,size);
		}
		if (cur > 90) {
			wipeUp = false;
		}
		if (cur < 0 || cur > 180) {
			wipeUp = true;
			wiping = false;
			button4.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
			Vector3 temp = rightWiper.transform.rotation.eulerAngles;
			rightWiper.transform.rotation = Quaternion.Euler(temp[0], temp[1], 0);
			leftWiper.transform.rotation = Quaternion.Euler(temp[0], temp[1], 0);
		}
	}
	
}
