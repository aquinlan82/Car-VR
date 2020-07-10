using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using System.IO.Ports;

public class XYAxis : MonoBehaviour {
	private Bluetooth blue;
	private Vector3 initPos;
	private GameObject leftHand;
	private GameObject rightHand;
	private float[] rightOffset;
	private float[] leftOffset;
	
	public float[] rightTranslate;
	public float[] leftTranslate;
	public float[] buttons;
	public Image indicator;
	
    void Start() {
		blue = GameObject.Find("carRoot").GetComponent<Bluetooth>();
		leftHand = GameObject.Find("leftHand");
		rightHand = GameObject.Find("rightHand");
		initPos = (rightHand.transform.position + leftHand.transform.position)/2;
		rightOffset = new float[] {82, 70, 0};
		leftOffset = new float[] {70, 355, 0};
		rightTranslate = new float[] {0,0};
		leftTranslate = new float[] {0,0};
		buttons = new float[] {0,0,0,0,0,0};
		indicator = GameObject.Find("bar").GetComponent<Image>();
    }

    /* Get data from controller and translate for unity */
    void FixedUpdate() {
		string data = blue.getMsg();
		
		if (data != "") {
			string[] coords = data.Split('/');
			
			if (coords.Length > 6) {
				readButtons(coords);
				setAxes(coords);
				setIndicator(Color.green);
			} else {
				setIndicator(Color.blue);
			}
		} else {
			setIndicator(Color.red);
		}
	}
	
	/* Adjusts for fact that gyros aren't lined up in controller */
	private float[] applyOffset(float[] values, float[] offset) {
		float[] output = new float[values.Length];
		for (int i = 0; i < values.Length; i++) {
			float temp = values[i] - offset[i];
			if (temp < 0) {
				temp = temp + 360;
			}
			output[i] = temp;
		}
		return output;
	}
	
	/* Converts array of strings to floats */
	private float[] stringToNum(string[] coords) {
		float[] output = new float[3];
		output[0] = float.Parse(coords[0]);
		output[1] = float.Parse(coords[1]);
		output[2] = float.Parse(coords[2]);
		return output;
	}
	
	/* Converts 0-360 angles to translation values */
	private float[] changeScale(float[] coords) {
		float maxAngle = 80F;
		float maxSpeed = 0.1F;
		float factor = maxAngle / maxSpeed;
		float[] output = new float[3] {0,0,0};
		
		//valid inputs are from [0,maxAngle] and [360-maxAngle,360]
		
		//x
		if (coords[0] < maxAngle) {
			output[0] = coords[0] / factor;
		}
		if (coords[0] > 360 - maxAngle) {
			output[0] = (360 - coords[0]) / -factor;
		}
		
		//y
		maxSpeed = 0.1F;
		factor = maxAngle / maxSpeed;
		if (coords[1] < maxAngle) {
			output[1] = coords[1] / factor;
		}
		if (coords[1] > 360 - maxAngle) {
			output[1] = (360 - coords[1]) / -factor;
		}
		
		return output;
	}
	
	/* Sets buttons list based on inputs */
	private void readButtons(string[] coords) {
		if (coords.Length == 12) {
			for (int i = 6; i < 12; i++) {
				buttons[i-6] = float.Parse(coords[i]);
			}
		}
	}
	
	/*  Sets values from controller to unity translation */
	private void setAxes(string[] coords) {
		float[] right = stringToNum(new string[]{coords[0], coords[1], coords[2]});
		float[] left = stringToNum(new string[]{coords[3], coords[4], coords[5]});
		right = applyOffset(right, rightOffset);
		left = applyOffset(left, leftOffset);
		right = changeScale(right);
		left = changeScale(left);
		rightTranslate[0] = right[0];
		leftTranslate[0] = left[0];
		rightTranslate[1] = right[1];
		leftTranslate[1] = left[1];
	}
	
	/* UI settings */
	private void setIndicator(Color color) {
		color.a = 0.6F;
		indicator.color = color;
	}
	
	
	/* Getters from contoller tilt */
	public float getLeftX() {
		return leftTranslate[0];
	}
	public float getLeftY() {
		return leftTranslate[1];
	}
	public float getRightX() {
		return rightTranslate[0];
	}
	public float getRightY() {
		return rightTranslate[1];
	}
	
	
	/* Getters from controller buttons */
	public bool getLeftRed() {
		return buttons[3] == 1;
	}
	public bool getRightRed() {
		return buttons[0] == 1;
	}
	public bool getLeftBlue() {
		return buttons[4] == 1;
	}
	public bool getRightBlue() {
		return buttons[1] == 1;
	}
	public bool getLeftGreen() {
		return buttons[5] == 1;
	}
	public bool getRightGreen() {
		return buttons[2] == 1;
	}
}