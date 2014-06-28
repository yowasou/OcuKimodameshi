using UnityEngine;
using System.Collections;

public class BatFly : MonoBehaviour {
	GameObject player = null;
	float radious = 0;
	float speed = 10f;
	Vector3 moveDirection = Vector3.zero;
	CharacterController controller = null;

	// Use this for initialization
	void Start () {
		player = GameObject.Find("OVRPlayerController");
		controller = (CharacterController)GetComponent("CharacterController");
		//Vector3 fo = player.transform.TransformDirection(Vector3.forward);
		Vector3 fo = player.transform.position;
		transform.position = fo;
		transform.rotation = Camera.main.transform.rotation;
		transform.Rotate(transform.TransformDirection(new Vector3 (0, 90f, 0)));
		//正面距離、高さ、右の距離
		controller.Move(transform.TransformDirection(new Vector3(-8f, 0, 10f)));
		//コウモリは10秒で死ぬ
		Destroy(this, 10.0f);
	}
	
	// Update is called once per frame
	void Update () {
		radious++;
		if (radious < 10) 
		{
			moveDirection = new Vector3(0f, 1f, -1f);
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= speed;
		} else 
		{
			moveDirection = new Vector3(0f, -1f, -1f);
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
