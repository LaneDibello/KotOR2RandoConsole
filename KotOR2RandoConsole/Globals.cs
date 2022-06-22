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
}