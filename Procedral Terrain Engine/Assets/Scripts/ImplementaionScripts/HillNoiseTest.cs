using UnityEngine;
using System.Collections;


public class HillNoiseTest : MonoBehaviour {
	public int size;
	public float maxRadius;
	public float minRadius;
	public float hillStrength;
	public int hillCount;
	// Use this for initialization
	void Start () {
		gameObject.GetComponent<MeshRenderer>().material.mainTexture = NoiseExtention.hillNoise.testText(hillCount,maxRadius,minRadius,size,hillStrength);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
