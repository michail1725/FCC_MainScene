using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class MovableCamera : MonoBehaviour
{
	private Camera mainCamera;
	private UnityServer unityServer;
	public float cameraSensitivity = 130;
	public float climbSpeed = 4;
	public float normalMoveSpeed = 10;
	public float slowMoveFactor = 0.25f;
	public float fastMoveFactor = 3;
	public Text typeOfUsingText;
	private bool IsFlyCamEnabled;
	private float rotationX = 0.0f;
	private float rotationY = 0.0f;
	public Canvas pause;
	public Canvas canvas;
	void Start()
	{
		typeOfUsingText.text = "????? ??????????????(space)";
		IsFlyCamEnabled = false;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		mainCamera = Camera.main;
		pause.gameObject.SetActive(false);
		unityServer = mainCamera.GetComponent<UnityServer>();
	}

	public void ReturnKeybordInput() {
		unityServer.SendMessageToClient("a");
		pause.gameObject.SetActive(false);
		canvas.gameObject.SetActive(true);
	}

	public void ChangeTypeOfUsing() {
		IsFlyCamEnabled = (IsFlyCamEnabled == false) ? true : false;
		if (IsFlyCamEnabled)
		{
			typeOfUsingText.text = "????? ?????????(space)";
		}
		else
		{
			typeOfUsingText.text = "????? ??????????????(space)";
		}
		Cursor.lockState = (Cursor.lockState == CursorLockMode.Locked) ? Cursor.lockState = CursorLockMode.None : Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = (Cursor.visible == true) ? Cursor.visible = false : Cursor.visible = true;
	}

	void Update()
	{
		if (Input.GetKey(KeyCode.Space)) {
			ChangeTypeOfUsing();
			Thread.Sleep(300);
		}
		if (IsFlyCamEnabled) {
			rotationX += Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
			rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;
			rotationY = Mathf.Clamp(rotationY, -90, 90);

			transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
			transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);

			if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
			{
				transform.position += transform.forward * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
				transform.position += transform.right * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
			}
			else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
			{
				transform.position += transform.forward * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
				transform.position += transform.right * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
			}
			else
			{

				transform.position += transform.forward * normalMoveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
				transform.position += transform.right * normalMoveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;

			}
			if (Input.GetKey(KeyCode.Q)) { transform.position += transform.up * climbSpeed * Time.deltaTime; }
			if (Input.GetKey(KeyCode.E)) { transform.position -= transform.up * climbSpeed * Time.deltaTime; }
			if (transform.position.x < -10)
			{
				transform.position = new Vector3(-10, transform.position.y, transform.position.z);
			}
			if (transform.position.x > 260)
			{
				transform.position = new Vector3(260, transform.position.y, transform.position.z);
			}
			if (transform.position.z < -10)
			{
				transform.position = new Vector3(transform.position.x, transform.position.y, -10);
			}
			if (transform.position.z > 260)
			{
				transform.position = new Vector3(transform.position.x, transform.position.y, 260);
			}
			if (transform.position.y < 1)
			{
				transform.position = new Vector3(transform.position.x, 1, transform.position.z);
			}
			if (transform.position.y > 300)
			{
				transform.position = new Vector3(transform.position.x, 300, transform.position.z);
			}
		}
	}
}