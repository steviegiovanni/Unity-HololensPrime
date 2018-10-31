// author: Stevie Giovanni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModelViewer
{
    /// <summary>
    /// utility class, list all task types
    /// </summary>
    static public class MultiPartsObjectEditorUtility
    {
        static public string [] TaskTypes()
        {
            return new string[]
            {
                "Moving Task",
                "Clicking Task"
            };
        }
    }
}
