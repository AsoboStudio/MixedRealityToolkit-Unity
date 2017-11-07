// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine.Serialization;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Encapsulate a single Unity AudioClip with playback settings.
    /// </summary>
    [Serializable]
    public class UAudioClip
    {
        [FormerlySerializedAs("sound")]
        public UnityEngine.AudioClip Sound = null;
        [FormerlySerializedAs("looping")]
        public bool Looping = false;
        [FormerlySerializedAs("delayCenter")]
        public float DelayCenter = 0;
        [FormerlySerializedAs("delayRandomization")]
        public float DelayRandomization = 0;
    }
}