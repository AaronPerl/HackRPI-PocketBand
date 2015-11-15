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
	
	private float frequency = 440.0f;
	private float phase = 0;
	
	private List<float> volumes;
	private List<float> frequencies;
	private AudioClip playback;

	void Start()
	{
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
		if (volumes != null)
		{
			List<float> curVolumes;;
			List<float> curFreqs;
			lock(this)
			{
			curVolumes = volumes;
			curFreqs = frequencies;
			}
			for (int i = 0; i < data.Length; i++)
			{
				float total = 0.0f;
				for (int j = 0; j < curVolumes.Count; j++)
				{
					total +=
						Mathf.Min(
							Mathf.Max(
								1000 * curVolumes[j] * Mathf.Sin(1.5f * phase * curFreqs[j])
							, -1.0f)
						, 1.0f);
					total +=
						Mathf.Min(
							Mathf.Max(
								1000 * curVolumes[j] * Mathf.Sin(1.2f * phase * curFreqs[j])
							, -1.0f)
						, 1.0f);
				}
				data[i] = total;
				phase += 2 * Mathf.PI / 44100;
				phase %= 2 * Mathf.PI;
			}
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
		List<float> newVolumes = new List<float>();
		List<float> newFreqs = new List<float>();
		for (int i = 1; i < WINDOW_SIZE; i++) {
			if (i > WINDOW_SIZE/8 && spectrum[i] > threshold)
			{
				newVolumes.Add(spectrum[i]);
				newFreqs.Add(((float)i / 150) * 440);
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
		lock(this)
		{
		volumes = newVolumes;
		frequencies = newFreqs;
		}
		if (newVolumes.Count > 0)
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
}
