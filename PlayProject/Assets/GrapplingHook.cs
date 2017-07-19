using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{

    public Transform hitObject;
    private Transform currentHitObject;
    private Camera playerCamera;
	private Transform playerTransform;

	public float hookLatchLength = 100.0f;
	public float hookLength = 10.0f;
    [Range(0.0f, 1.0f)]
    public float cooldown = 0.5f;
    private float currentTime = 0.0f;
    private bool canShoot = true;
    private bool isLatched = false;
    private int hits = 0;

	private LayerMask playerMask;

    // Use this for initialization
    void Start()
    {
		playerTransform = GetComponent<Transform>();
	    playerCamera = GetComponentInChildren<Camera>();
		playerMask = ~(1<<8);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            hits++;
            //print("PRESS" + hits);
            if (canShoot)
            {
                Shoot();

            }
        }
		if(Input.GetKey(KeyCode.Space) || Input.GetMouseButton(1)) 
		{
			Delatch();
		}
		if(!canShoot) 
		{
			currentTime += Time.deltaTime;
			if (currentTime >= cooldown)
			{
				currentTime = 0.0f;
				canShoot = true;
			}
		}
    }

    void Shoot()
    {
        if (hitObject != null)
        {
            if (currentHitObject != null)
            {
                Destroy(currentHitObject.gameObject);
            }
			RaycastHit hit;
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, hookLatchLength))
            {
           		currentHitObject = (Transform) Instantiate(hitObject, hit.point, playerCamera.transform.rotation);
                canShoot = false;
				isLatched = true;
            } else {
				isLatched = false;
			}
        }
    }

	public Vector3 ManageGrapplingHook (Vector3 currentVelocity) 
	{
		Vector3 newVelocity;
		if(isLatched) 
		{
			Vector3 wantedPos = playerTransform.position + currentVelocity * Time.deltaTime;
			if((wantedPos - currentHitObject.position).magnitude > hookLength) {
				wantedPos = (currentHitObject.position - wantedPos).normalized;
				return currentVelocity + wantedPos;
			}
			return currentVelocity;
		} else {
			return currentVelocity;
		}
	}

	private void Delatch () 
	{
		if(currentHitObject != null) 
		{
			Destroy(currentHitObject.gameObject);
			isLatched = false;
		}
	}
}
