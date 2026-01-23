using System;
using UnityEngine;

namespace AudioPresetResearch
{
    [Serializable]
    public class AudioPresetData
    {
        public int priority;
        public string presetName;
        public float volume;
        public float pitch;
        public bool loop;
        public bool playOnAwake;
        public float spatialBlend;
        public float minDistance;
        public float maxDistance;
        public AudioRolloffMode rolloffMode;
        public float dopplerLevel;
        public bool bypassEffects;
        public bool bypassListenerEffects;
        public bool bypassReverbZones;
        
        public void CaptureFrom(AudioSource source)
        {
            priority = source.priority;
            volume = source.volume;
            pitch = source.pitch;
            loop = source.loop;
            playOnAwake = source.playOnAwake;
            spatialBlend = source.spatialBlend;
            minDistance = source.minDistance;
            maxDistance = source.maxDistance;
            rolloffMode = source.rolloffMode;
            dopplerLevel = source.dopplerLevel;
            bypassEffects = source.bypassEffects;
            bypassListenerEffects = source.bypassListenerEffects;
            bypassReverbZones = source.bypassReverbZones;
       
        }

        public void ApplyTo(AudioSource source)
        {
            source.priority = priority;
            source.volume = volume;
            source.pitch = pitch;
            source.loop = loop;
            source.playOnAwake = playOnAwake;
            source.spatialBlend = spatialBlend;
            source.minDistance = minDistance;
            source.maxDistance = maxDistance;
            source.rolloffMode = rolloffMode;
            source.dopplerLevel = dopplerLevel;
            source.bypassEffects = bypassEffects;
            source.bypassListenerEffects = bypassListenerEffects;
            source.bypassReverbZones = bypassReverbZones;
        }
    }
}