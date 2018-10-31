// author: Stevie Giovanni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModelViewer
{
    /// <summary>
    /// serializable class to store taskevents data
    /// </summary>
    [System.Serializable]
    public class SerializableTaskEvent
    {
        public string TypeName;
        public Vector3 StartPos;
        public Quaternion StartRotation;
        public Vector3 EndPos;
        public Quaternion EndRotation;

        /// <summary>
        /// constructor, takes an actual taskevent and serialize it into its internal structure
        /// called during serialization
        /// </summary>
        public SerializableTaskEvent(TaskEvent taskEvent) {
            if (taskEvent != null) {
                TypeName = taskEvent.GetType().Name;
                switch (TypeName)
                {
                    case "TransformTaskEvent":
                        {
                            TransformTaskEvent castedEvent = taskEvent as TransformTaskEvent;
                            StartPos = castedEvent.StartPos;
                            EndPos = castedEvent.EndPos;
                            StartRotation = castedEvent.StartRotation;
                            EndRotation = castedEvent.EndRotation;
                        }break;
                }
            }
        }
    }
}
