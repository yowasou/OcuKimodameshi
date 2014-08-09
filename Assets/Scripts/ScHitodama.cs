using UnityEngine;
using System.Collections;

public class ScHitodama : MonoBehaviour {
	float speed = 1f;
	float radious = 0;
	CharacterController controller = null;

	void Start () {
		controller = (CharacterController)GetComponent("CharacterController");
		GameObject player = GameObject.Find("First Person Controller");

		Vector3 fo = player.transform.position;
		transform.position = fo;
		transform.rotation = Camera.main.transform.rotation;
		transform.Rotate(transform.TransformDirection(new Vector3 (0, 90f, 0)));
		//正面距離、高さ、右の距離
		controller.Move(transform.TransformDirection(new Vector3(-8f, 0.5f, 10f)));
	}

	// Update is called once per frame
	void Update () {
		Vector3 moveDirection = Vector3.zero;
		radious++;
		if (radious < 10) 
		{
			moveDirection = new Vector3(0f, 2f, -1f);
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= speed;
		} else 
		{
			moveDirection = new Vector3(0f, -2f, -1f);
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= speed;
		}
		if (radious > 20) 
		{
			radious = 0;
		}
		controller.Move(moveDirection * Time.deltaTime);
	}
}
