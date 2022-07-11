using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace KotOR2RandoConsole
{
    [Serializable]
    public enum RandomizationLevel // Thank you Glasnonck
    {
        /// <summary> No randomization. </summary>
        None = 0,
        /// <summary> Randomize similar types within the same category. </summary>
        Subtype = 1,
        /// <summary> Randomize within the same category. </summary>
        Type = 2,
        /// <summary> Randomize with everything else set to Max. </summary>
        Max = 3,
    }

    [Serializable]
    public enum TexturePack
    {
        /// <summary> High Quality </summary>
        [Description("High Quality")]
        HighQuality = 0,
        /// <summary> Medium Quality </summary>
        [Description("Medium Quality")]
        MedQuality = 1,
        /// <summary> Low Quality </summary>
        [Description("Low Quality")]
        LowQuality = 2,
    }

    public static class Globals
    {
        public static readonly Dictionary<string, Tuple<float, float, float>> FIXED_COORDINATES = new Dictionary<string, Tuple<float, float, float>>()
        {
            { "202TEL", new Tuple<float, float, float>(
                (-13.5f),
                (-63.4f),
                (11.51f)) },
        };
    }

    
}