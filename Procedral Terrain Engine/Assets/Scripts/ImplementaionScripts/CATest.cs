using UnityEngine;
using System.Collections;
using cellularAutomataLib;

public class CATest : MonoBehaviour 
{

	void Start()
	{
		Vector2[,] cells = new Vector2[3,3];

		for(int x=0; x < 3; x ++)
		{
			for(int y=0; y < 3; y ++)
			{
				cells[x,y] = new Vector2(x,y);
			}
		}
		Vector2[] output =neighborAcces.getNeigborsNESW<Vector2>(ref cells,new Vector2(2,2));
		foreach(Vector2 v in output)
		{
			print(v);
		}
	}
}
