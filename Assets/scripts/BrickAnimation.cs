using UnityEngine;
using System.Collections;

public class BrickAnimation : MonoBehaviour {

    [SerializeField]
    int rotationSpeed = 30;

   
	// Use this for initialization
	void Start ()
    {
        transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
    }
	
	// Update is called once per frame
	void Update ()
    {           
        Vector3 rotation = new Vector3(1, rotationSpeed, 0);
        transform.Rotate((rotation) * Time.deltaTime);
    }

    public void stopAnimation()
    {
        transform.localScale = new Vector3(1, 1, 1);
        transform.localEulerAngles = new Vector3(0, 0, 0);
        enabled = false; 
    }

}
