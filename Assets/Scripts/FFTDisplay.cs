using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FFTDisplay : MonoBehaviour {

	public AudioSource audioSource;
	public AudioSource playbackSource;
	public float threshold;

	public int freq = 24000;
	public float delay = 0.033f;
	const int WINDOW_SIZE = 1024;
	private float phase = 0;
	
	private int prevDominant = 0;
	public List<int> pitches;
	public float volume = 0.0f;
	//private Dictionary<int, float> pitchesToVolumes;
	private AudioClip playback;
	
	private Harmonizer harmonizer;

	void Start()
	{
		harmonizer = GetComponent<Harmonizer>();
		audioSource.clip = Microphone.Start("", true, 5, freq); 
		playback = AudioClip.Create(
		"PlaybackSine",
		44100,
		1,
		44100,
		true,
		OnAudioRead);
		playbackSource.clip = playback;
	}
	
	void OnAudioRead(float[] data)
	{
		List<int> curPitches;
		float curVolume;
		lock(this)
		{
		curPitches = pitches;
		curVolume = volume;
		}
		
		for (int i = 0; i < data.Length; i++)
		{
			float total = 0.0f;
			foreach (int curTone in curPitches)
			{
				total += 1000 * curVolume * Mathf.Sin(phase * Harmonizer.getFrequency(curTone));
			}
			data[i] = total;
			phase += 2 * Mathf.PI / 44100;
			phase %= 2 * Mathf.PI;
		}
	}


	public float[] spectrum = new float[WINDOW_SIZE];

	void Update() {
		int microphoneSamples = Microphone.GetPosition("");
		if (microphoneSamples / freq > delay)
		{
			if (!audioSource.isPlaying)
			{
				audioSource.timeSamples = (int) (microphoneSamples - (delay * freq));
				audioSource.Play ();
			}
		}
		audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Hanning);
		Dictionary<int, float> newPitchesToVolumes = new Dictionary<int, float>();
		for (int i = 1; i < WINDOW_SIZE; i++) {
			if (spectrum[i] > threshold)
			{
				float curFreq = ((float) i) / 150 * 440;
				int pitch = Harmonizer.getPitch(curFreq);
				if (newPitchesToVolumes.ContainsKey(pitch))
				{
					float prevMax = newPitchesToVolumes[pitch];
					if (spectrum[i] > prevMax)
						newPitchesToVolumes[pitch] = spectrum[i];
				}
				else
				{
					newPitchesToVolumes.Add(pitch, spectrum[i]);
				}
			}
			Debug.DrawLine( new Vector3(i - 1, 50000f * spectrum[i - 1] + 10, 0), 
				new Vector3(i, 50000f * spectrum[i] + 10, 0), 
				Color.red);
			Debug.DrawLine( new Vector3(i - 1, Mathf.Log(spectrum[i - 1]) + 10, 2),
				new Vector3(i, Mathf.Log(spectrum[i]) + 10, 2),
				Color.cyan);
			Debug.DrawLine( new Vector3(Mathf.Log(i - 1), spectrum[i - 1] - 10, 1), 
				new Vector3(Mathf.Log(i), spectrum[i] - 10, 1), 
				Color.green);
			Debug.DrawLine( new Vector3(Mathf.Log(i - 1), Mathf.Log(spectrum[i - 1]), 3), 
				new Vector3(Mathf.Log(i), Mathf.Log(spectrum[i]), 3), 
				Color.yellow);
		}
		
		int dominantPitch = 0;
		float maxVolume = 0.0f;
		foreach (KeyValuePair<int, float> pair in newPitchesToVolumes)
		{
			if (pair.Value > maxVolume)
			{
				dominantPitch = pair.Key;
				harmonizer.key = dominantPitch;
				maxVolume = pair.Value;
			}
		}
		
		if (prevDominant != dominantPitch)
		{
			if (maxVolume != 0.0f)
			{
				List<int> newPitches = harmonizer.updateHarmony(dominantPitch);
				
				lock(this)
				{
					pitches = newPitches;
					volume = maxVolume;
					//pitchesToVolumes = newPitchesToVolumes;
				}
			
				if (newPitches.Count > 0)
				{
					if (!playbackSource.isPlaying)
					{
						playbackSource.Play();
						Debug.Log("starting audio");
					}
				}
				else
				{
					if (playbackSource.isPlaying)
					{
						playbackSource.Stop();
						Debug.Log("stopping audio");
					}
				}
			}
			else
			{
				pitches = new List<int>();
				volume = 0.0f;
			}
		}
		
		prevDominant = dominantPitch;
	}
}
