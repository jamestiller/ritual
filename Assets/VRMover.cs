using UnityEngine;
using System.Collections;

public class VRMover : MonoBehaviour {

	public Camera cameraVR;
	public Rigidbody character;
	public BlurEffect blur;
	public Light light;

	float startDrag = 0;
	// Use this for initialization
	void Start () {
		//startDrag = character.drag;
	}

	float walkForce = 1;
	float rotationMax = .5f;

	float targetBlindness = 0;
	float targetIterations = 10;
	float targetDownsamples = 10;

	public void SetDarkness(float normal)
	{
		light.intensity = 1.0f * normal;
	}

	public void SetSuddenBlindness(float normal)
	{
		// Sudden blindness is 50;
		SetBlur(true);
		blur.Iterations = (int)(targetIterations);
		blur.Downsample = (int)Mathf.Max(1,targetDownsamples*normal);
		targetBlindness = 50 * normal;
		blur.Blur = targetBlindness;
		//StartCoroutine("SuddenBlindness");
	}

	private void SetBlur(bool turnOn)
	{
		if (turnOn)
		{
			blur.Blur = 0;
			blur.Downsample = 1;
			blur.Iterations = 0;
			blur.enabled = turnOn;

		}
	}

	IEnumerator SuddenBlindness()
	{
		float iterations = 0;
		float totalIterations = 10;

		while(iterations < totalIterations)
		{
			yield return null;
			blur.Blur = targetBlindness*iterations/totalIterations;
			blur.Downsample = (int)(targetDownsamples*iterations/totalIterations);
			if (blur.Downsample < 1)
			{
				blur.Downsample = 1;
			}

			blur.Iterations = (int)(targetIterations*iterations/totalIterations);
		}
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.Alpha0))
		{
			SetSuddenBlindness(.01f);
			SetDarkness(.9f);
		}

		if (Input.GetKey(KeyCode.Alpha1))
		{
			SetSuddenBlindness(.03f);
			SetDarkness(.8f);
		}

		if (Input.GetKey(KeyCode.Alpha3))
		{
			SetSuddenBlindness(.055f);
			SetDarkness(.7f);
		}


		if (Input.GetKey(KeyCode.Alpha4))
		{
			SetSuddenBlindness(.08f);
			SetDarkness(.6f);
		}


		if (Input.GetKey(KeyCode.Alpha5))
		{
			SetSuddenBlindness(.1f);
			SetDarkness(.5f);
		}


		if (Input.GetKey(KeyCode.Alpha6))
		{
			SetSuddenBlindness(.15f);
			SetDarkness(.4f);
		}

		if (Input.GetKey(KeyCode.Alpha7))
		{
			SetSuddenBlindness(.27f);
			SetDarkness(.3f);
		}


		if (Input.GetKey(KeyCode.Alpha8))
		{
			SetSuddenBlindness(.5f);
			SetDarkness(.2f);
		}

		if (Input.GetKey(KeyCode.Alpha9))
		{
			SetSuddenBlindness(.8f);
			SetDarkness(.1f);
		}

		if (Input.GetKey(KeyCode.Alpha0))
		{
			SetSuddenBlindness(1.0f);
			SetDarkness(.05f);
		}

		if (Input.GetKey(KeyCode.UpArrow))
		{
			//character.drag = startDrag;
			//character.AddForce(cameraVR.gameObject.transform.forward * walkForce);
			character.velocity = cameraVR.transform.forward * walkForce;
			//character.MovePosition(new Vector3(Random.Range(-100,100),Random.Range(-100,100),Random.Range(-100,100)));
		}
		else
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			//character.drag = startDrag;
			//character.AddForce(-cameraVR.gameObject.transform.right * walkForce);
				//character.velocity = -cameraVR.transform.forward * walkForce;
				gameObject.transform.Rotate(0, -rotationMax, 0);
		}
		else
		if (Input.GetKey(KeyCode.RightArrow))
		{
					//character.drag = startDrag;
			//character.AddForce(cameraVR.gameObject.transform.right * walkForce);
					//character.velocity = cameraVR.transform.right * walkForce;

					gameObject.transform.Rotate(0, rotationMax, 0);
		}
		else
		if (Input.GetKey(KeyCode.DownArrow))
		{
			//character.drag = startDrag;
					character.velocity = -cameraVR.transform.forward * walkForce;
		}
		else
		{
			//character.drag = 10;
		}

		// Figure out the joystick controls!
		float horizontal = XboxCtrlrInput.XCI.GetAxis(XboxCtrlrInput.XboxAxis.LeftStickX);
		float vertical = XboxCtrlrInput.XCI.GetAxis(XboxCtrlrInput.XboxAxis.LeftStickY);
		//Debug.Log("vert:" + horizontal + ":" + vertical);

		if (vertical > .2f || vertical < -.2)
		{
			//Debug.Log("wjat");
			//character.drag = 0;
			//cameraVR.transform.forward * walkForce * vertical;
			//character.velocity = character.transform.forward * walkForce*vertical;
			character.velocity = cameraVR.transform.forward * walkForce * vertical;
		}
		else
		{
			//character.drag = 10;
		}
		//gameObject.transform.forward.normalized * walkForce;
		//AddForce(gameObject.transform.forward * vertical * walkForce);
		//gameObject.transform.Rotate(0, horizontal * rotationMax, 0);
		//camera.gameObject.transform.forward * vertical * walkForce);
		//character.AddForce(camera.gameObject.transform.right * horizontal * 10);
	}
	int test = 0;
}
