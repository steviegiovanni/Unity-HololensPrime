/// author: Stevie Giovanni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModelViewer
{
    /// <summary>
    /// serializable class to store task data
    /// </summary>
    [System.Serializable]
    public class SerializableTask
    {
        // base data
        public string TypeName;
        public GameObject GameObject;

        // move task data
        public Vector3 Position;
        public Quaternion Rotation;
        public float SnapThreshold;
        public string TaskName;
        public string Description;
        public MovingTaskType MoveType;
        public string GOName;

        // task event data
        public SerializableTaskEvent TaskEvent;
        
        /// <summary>
        /// constructor that takes a task and serialize it into its internal data structure
        /// </summary>
        public SerializableTask(Task t)
        {
            GOName = t.GOName;
            TypeName = t.GetType().Name;
            GameObject = t.GameObject;
            TaskName = t.TaskName;
            Description = t.Description;
            if(t.TaskEvent != null)
                TaskEvent = new SerializableTaskEvent(t.TaskEvent);
            switch (TypeName)
            {
                case "MovingTask":
                    {
                        MovingTask castedTask = t as MovingTask;
                        Position = castedTask.Position;
                        Rotation = castedTask.Rotation;
                        SnapThreshold = castedTask.SnapThreshold;
                        MoveType = castedTask.MoveType;
                    }break;
            }
        }
    }
}
