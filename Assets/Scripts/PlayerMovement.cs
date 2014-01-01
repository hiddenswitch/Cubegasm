using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	private CharacterController		controller;
	public	float					moveSpeed = 10;
	public	float					gravity = 50;
	public	float					jumpSpeed = 20;

	private Vector3					curMovement;

	private int						gravityDirection = -1;

	private RaycastHit				hit;

	public	LayerMask				gravityChangeMask;

	public	float					repositionDistance = 75;

	void Start () 
	{
		Application.targetFrameRate = 60;
		controller = GetComponent<CharacterController>();
	}

	void Update () {

		Movement();
		Gravity();
		Jump();

		controller.Move(new Vector3(curMovement.x*moveSpeed, curMovement.y, 0) * Time.deltaTime);

		Reposition();

		if (Input.GetKeyDown(KeyCode.W))
		{
			gravityDirection = gravityDirection == 1 ? -1 : 1;
		}

		//print ((controller.collisionFlags & CollisionFlags.Above) != 0);
	}

	void Movement()
	{

		if (Input.GetKey(KeyCode.RightArrow))
		{
			curMovement.x = 1;
		}else if (Input.GetKey(KeyCode.LeftArrow))
		{
			curMovement.x = -1;
		}else
		{
			curMovement.x = 0;
		}

		curMovement.x = 1;
	}

	void Jump()
	{


		if ((Input.GetKeyDown(KeyCode.Space) || (Input.touchCount>=1 && Input.GetTouch(0).phase ==TouchPhase.Began)))
		{
			float tempJumpSpeed = jumpSpeed;

			if (CheckPlatform() == 1){
				tempJumpSpeed *= 0.6f;
			}

			if (gravityDirection == -1 && controller.isGrounded)
				curMovement.y = tempJumpSpeed;
			else if (gravityDirection == 1 && (controller.collisionFlags & CollisionFlags.Above) != 0)
				curMovement.y = -tempJumpSpeed;

			
			if (CheckPlatform() == 1){
				gravityDirection *= -1;
			}
		}
	}

	void Gravity()
	{
		if (gravityDirection == -1)
		{
			if (!controller.isGrounded)
			{
				curMovement.y -= gravity * Time.deltaTime;
			}else
			{
				curMovement.y = -1;
			}
		}
		else if (gravityDirection == 1)
		{
			if ((controller.collisionFlags & CollisionFlags.Above) == 0)
			{
				curMovement.y += gravity * Time.deltaTime;
			}else
			{

				if (curMovement.y > 1)
				curMovement.y = 1;
			}
		}
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.CompareTag("Enemy"))
		{
			transform.position = new Vector3(0, transform.position.y, 0);
		}
	}

	void Reposition()
	{
		if (transform.position.x > repositionDistance)
			transform.position = new Vector3(0, transform.position.y, 0);
	}

	int CheckPlatform()
	{

		//if (Physics.Raycast(transform.position, -Vector3.up, 
		if (gravityDirection == -1){
			if (Physics.Raycast (transform.position, -Vector3.up, out hit, 2.0f, gravityChangeMask)) 
			{
				return 1;
			}
		}
		else if (gravityDirection == 1){
			if (Physics.Raycast (transform.position, Vector3.up, out hit, 2.0f, gravityChangeMask)) 
			{
				return 1;
			}
		}
		return 0;
	}
}
