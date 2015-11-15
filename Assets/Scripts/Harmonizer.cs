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
		//Debug.Log (getPitch(getFrequency(pitch)));
	}
	public List<int> updateHarmony(int newPitch)
	{
		int degree = (newPitch-key)%12;
		List<int> possibleFunctions = getPossibleFunctions(degree);
		function = getNewFunction(possibleFunctions);
		List<int> pitches = getFunctionPitches(function);
		for(int i = 0; i < pitches.Count; i++)
		{
			pitches[i] += key;
		}
		return pitches;
	}
	private List<int> getPossibleFunctions(int degree)
	{
		switch(degree)
		{
			case 0:	return new List<int>(){0,1,6,7,10,11};
			case 1:	return new List<int>();
			case 2:	return new List<int>(){2,3,8,9,12,13};
			case 3:	return new List<int>(){0,1,4,5,10,11};
			case 4:	return new List<int>();
			case 5:	return new List<int>(){2,3,6,7,12,13};
			case 6:	return new List<int>();
			case 7:	return new List<int>(){0,1,4,5,8,9};
			case 8:	return new List<int>(){2,6,10};
			case 9:	return new List<int>(){3,7,11};
			case 10:return new List<int>(){4,8,12};
			case 11:return new List<int>(){5,9,13};
		}
		return new List<int> ();
	}
	private List<int> getFunctionPitches(int function)
	{
		switch(function)
		{
			case 0:	return new List<int>(){0,3,7};
			case 1:	return new List<int>(){0,4,7};
			case 2:	return new List<int>(){2,5,7};
			case 3:	return new List<int>(){2,5,8};
			case 4:	return new List<int>(){3,7,9};
			case 5:	return new List<int>(){3,7,10};
			case 6:	return new List<int>(){0,5,7};
			case 7:	return new List<int>(){0,5,8};
			case 8:	return new List<int>(){2,7,9};
			case 9:	return new List<int>(){2,7,10};
			case 10:return new List<int>(){0,3,8};
			case 11:return new List<int>(){0,3,9};
			case 12:return new List<int>(){2,5,10};
			case 14:return new List<int>(){2,5,11};
		}
		return new List<int>();
	}
	private int getNewFunction(List<int> possibleFunctions)
	{
		if (possibleFunctions.Count > 0)
			return possibleFunctions[Random.Range(0,possibleFunctions.Count-1)];
		else
			return 0;
	}
	public static int getPitch(float frequency)
	{
		return Mathf.RoundToInt(12f*Mathf.Log(frequency/C0)/Mathf.Log(2f));
	}
	public static float getFrequency(int pitch)
	{
		return C0*Mathf.Pow(root,pitch);
	}
	public static string getName(int pitch)
	{
		switch(pitch%12)
		{
		case 0:		return "C ";
		case 1:		return "Db";
		case 2:		return "D ";
		case 3:		return "Eb";
		case 4:		return "E ";
		case 5:		return "F ";
		case 6:		return "Gb";
		case 7:		return "G ";
		case 8:		return "Ab";
		case 9:		return "A ";
		case 10:	return "Bb";
		case 11:	return "B ";
		}
		return "You dun goofed.";
	}
}
