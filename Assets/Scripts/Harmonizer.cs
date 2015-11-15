using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Harmonizer : MonoBehaviour
{
	private static float C0 = 16.35f;
	private static float root = 1.05946309436f;

	public int key = 0;
	public int pitch = 0;
	public int function = 1;

	void Start ()
	{
		
	}

	void Update ()
	{
		Debug.Log (getPitch(getFrequency(pitch)));
	}
	List<int> changeHarmony(int newPitch)
	{
		return null;
	}
	/*

	public List<int> getPossibleFunctions(int newNote)
	{
		int relativeNote = (newNote-key)%7;

		switch(relativeNote+1)
		{
			case 1:		break;
			case 2:		break;
			case 3:		break;
			case 4:		break;
			case 5:		break;
			case 6:		break;
			case 7:		break;
		}
	}

*/
	public static int getPitch(float frequency)
	{
		return Mathf.RoundToInt(12f*Mathf.Log(frequency/C0)/Mathf.Log(2f));
	}
	public static float getFrequency(int pitch)
	{
		return C0*Mathf.Pow(root,pitch);
	}
}
