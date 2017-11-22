// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// The different rules for how audio should be played back.
    /// </summary>
    public enum AudioContainerType
    {
        Random,
        Sequence,
        Simultaneous,
        ContinuousSequence,
        ContinuousRandom
    }

    /// <summary>
    /// Defines the behavior for when the instance limit is reached for a particular event.
    /// </summary>
    public enum AudioEventInstanceBehavior
    {
        KillOldest,
        KillNewest
    }

    /// <summary>
    /// The different types of spatial positioning.
    /// </summary>
    public enum SpatialPositioningType
    {
        /// <summary>
        /// Stereo
        /// </summary>
        TwoD,
        /// <summary>
        /// 3D Audio
        /// </summary>
        ThreeD,
        /// <summary>
        /// Microsoft Spatial Sound
        /// </summary>
        SpatialSound
    }

    /// <summary>
    /// The AudioEvent class is the main component of UAudioManager and contains settings and a container for playing audio clips.
    /// </summary>
    [Serializable]
    public class AudioEvent : IComparable, IComparable<AudioEvent>
    {
        [FormerlySerializedAs("name")]
        [Tooltip("The name of this AudioEvent.")]
        public string Name = "_NewAudioEvent";

        [Tooltip("How this sound is to be positioned.")]
        [FormerlySerializedAs("spatialization")]
        public SpatialPositioningType Spatialization = SpatialPositioningType.TwoD;

        [Tooltip("The size of the Microsoft Spatial Sound room.  Only used when positioning is set to SpatialSound.")]
        [FormerlySerializedAs("roomSize")]
        public SpatialSoundRoomSizes RoomSize = SpatialSoundSettings.DefaultSpatialSoundRoom;

        [FormerlySerializedAs("minGain")]
        [Tooltip("The minimum gain, in decibels.  Only used when positioning is set to SpatialSound.")]
        [Range(SpatialSoundSettings.MinimumGainDecibels, SpatialSoundSettings.MaximumGainDecibels)]
        public float MinGain = SpatialSoundSettings.DefaultMinGain;

        [FormerlySerializedAs("maxGain")]
        [Tooltip("The maximum gain, in decibels.  Only used when positioning is set to SpatialSound.")]
        [Range(SpatialSoundSettings.MinimumGainDecibels, SpatialSoundSettings.MaximumGainDecibels)]
        public float MaxGain = SpatialSoundSettings.DefaultMaxGain;

        [FormerlySerializedAs("attenuationCurve")]  
        [Tooltip("The volume attenuation curve for simple 3D sounds. Used when positioning is set to 3D or Spatial")]    
        public AnimationCurve AttenuationCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f); // By default simple attenuation

        [FormerlySerializedAs("spatialCurve")]
        [Tooltip("The spatial attenuation curve for simple 3D sounds. Only used when positioning is set to 3D")]
        public AnimationCurve SpatialCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 1f); // by default Full 3D sound

        [FormerlySerializedAs("spreadCurve")]
        [Tooltip("The spread attenuation curve for simple 3D sounds. Only used when positioning is set to 3D")]
        public AnimationCurve SpreadCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 0f); // by default no spread

        [FormerlySerializedAs("lowPassCurve")]
        [Tooltip("The lowpass attenuation curve for simple 3D sounds. Only used when positioning is set to 3D")]
        public AnimationCurve LowPassCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 0f); // by default no lowpass

        [FormerlySerializedAs("reverbCurve")] 
        [Tooltip("The reverb attenuation curve for simple 3D sounds. Only used when positioning is set to 3D")]
        public AnimationCurve ReverbCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 0f); // by default no reverb

        [FormerlySerializedAs("maxDistanceAttenuation3D")]
        [Tooltip("The maximum attenuation distance for simple 3D sounds. Only used when positioning is set to 3D")]
        [Range(1f, 500f)]
        public float MaxDistanceAttenuation3D = 100f;

        [FormerlySerializedAs("unityGainDistance")]
        [Tooltip("The distance, in meters at which the gain is 0 decibels.  Only used when positioning is set to SpatialSound.")]
        [Range(SpatialSoundSettings.MinimumUnityGainDistanceMeters, SpatialSoundSettings.MaximumUnityGainDistanceMeters)]
        public float UnityGainDistance = SpatialSoundSettings.DefaultUnityGainDistance;

        [FormerlySerializedAs("bus")]   
        [Tooltip("The AudioMixerGroup to use when playing.")]
        public AudioMixerGroup AudioBus;

        [FormerlySerializedAs("pitchCenter")]
        [Tooltip("The default or center pitch around which randomization can be done.")]
        [Range(-3.0f, 3.0f)]
        public float PitchCenter = 1.0f;

        /// <summary>
        /// The amount in either direction from Pitch Center that the pitch can randomly vary upon playing the event.
        /// </summary>
        /// <remarks>The supported range is 0.0f - 2.0f.</remarks>

        [FormerlySerializedAs("pitchRandomization")]
        [HideInInspector]
        public float PitchRandomization;

        [FormerlySerializedAs("volumeCenter")]
        [Tooltip("The default or center volume level around which randomization can be done.")]
        [Range(0.0f, 1.0f)]
        public float VolumeCenter = 1.0f;

        /// <summary>
        /// The amount in either direction from Volume Center that the volume can randomly vary upon playing the event.
        /// </summary>
        /// <remarks>The supported range is 0.0f - 0.5f.</remarks>
        [FormerlySerializedAs("volumeRandomization")]
        [HideInInspector]
        public float VolumeRandomization;

        [FormerlySerializedAs("panCenter")]
        [Tooltip("The default or center panning. Only used when positioning is set to 2D.")]
        [Range(-1.0f, 1.0f)]
        public float PanCenter;

        /// <summary>
        /// The amount in either direction from Pan Center that panning can randomly vary upon playing the event.
        /// </summary>
        /// <remarks>The supported range is 0.0f - 0.5f.</remarks>
        [FormerlySerializedAs("panRandomization")]
        [HideInInspector]
        public float PanRandomization;

        [FormerlySerializedAs("spatialCurve")]
        [Tooltip("Time, in seconds, for the audio to fade from 0 to the selected volume.  Does not apply to continuous containers in which the Crossfade time property is used.")]
        [Range(0f, 20f)]
        public float FadeInTime;

        [FormerlySerializedAs("fadeOutTime")]
        [Tooltip("Time, in seconds, for the audio to fade out from the selected volume to 0.  Does not apply to continuous containers in which the Crossfade time property is used.")]
        [Range(0f, 20f)]
        public float FadeOutTime;

        [FormerlySerializedAs("instanceLimit")]
        [Tooltip("The maximum number of instances that should be allowed at a time for this event. Any new instances will be suppressed.")]
        public int InstanceLimit;

        [FormerlySerializedAs("instanceTimeBuffer")]
        [Tooltip("The amount of time in seconds that an event will remain active past when the sound ends. Useful for limiting the instances of an event beyond the clip play time.")]
        public float InstanceTimeBuffer;

        [FormerlySerializedAs("instanceBehavior")]
        [Tooltip("The behavior when the instance limit is reached.")]
        public AudioEventInstanceBehavior AudioEventInstanceBehavior = AudioEventInstanceBehavior.KillOldest;

        /// <summary>
        /// Contains the sounds associated with this AudioEvent.
        /// </summary>
        [FormerlySerializedAs("container")]
        public AudioContainer Container = new AudioContainer();

        /// <summary>
        /// Is this AudioEvent's container a continuous container?
        /// </summary>
        /// <returns>True if this AudioEvent's container is one of the continuous types (random or sequential), otherwise false.</returns>
        public bool IsContinuous()
        {
            return Container.ContainerType == AudioContainerType.ContinuousRandom ||
                   Container.ContainerType == AudioContainerType.ContinuousSequence;
        }

        /// <summary>
        /// Compares this AudioEvent with another object.
        /// </summary>
        /// <param name="obj">The object to compare against.</param>
        /// <returns>An integer that indicates whether this AudioEvent precedes (-1), follows (1),
        /// or appears in the same position (0) in the sort order as the AudioEvent being compared.</returns>
        /// <remarks>If the specified object is not an AudioEvent, the return value is 1.</remarks>
        public int CompareTo(object obj)
        {
            if (obj == null) { return 1; }

            var tempEvent = obj as AudioEvent;

            if (tempEvent != null)
            {
                return CompareTo(tempEvent);
            }

            throw new ArgumentException("Object is not an AudioEvent");
        }

        /// <summary>
        /// Compares this AudioEvent with another AudioEvent.
        /// </summary>
        /// <param name="other">The AudioEvent to compare against.</param>
        /// <returns>An integer that indicates whether this AudioEvent precedes (-1), follows (1),
        /// or appears in the same position (0) in the sort order as the AudioEvent being compared.</returns>
        public int CompareTo(AudioEvent other)
        {
            return other == null ? 1 : string.CompareOrdinal(Name, other.Name);
        }
    }
}