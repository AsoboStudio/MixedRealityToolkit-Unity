// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// The AudioContainer class is sound container for an AudioEvent. It also specifies the rules of how to
    /// play back the contained AudioClips.
    /// </summary>
    [Serializable]
    public class AudioContainer
    {
        [FormerlySerializedAs("containerType")]
        [Tooltip("The type of the audio container.")]
        public AudioContainerType ContainerType = AudioContainerType.Random;

        [FormerlySerializedAs("looping")]
        public bool Looping = false;
        [FormerlySerializedAs("loopTime")]
        public float LoopTime = 0;
        [FormerlySerializedAs("sounds")]
        public UAudioClip[] Sounds = null;
        [FormerlySerializedAs("crossfadeTime")]
        public float CrossfadeTime = 0f;
        [FormerlySerializedAs("currentClip")]
        public int CurrentClip = 0;
    }
}