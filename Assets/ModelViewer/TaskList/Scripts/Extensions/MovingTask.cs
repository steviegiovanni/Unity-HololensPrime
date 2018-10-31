/// author: Stevie Giovanni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModelViewer
{
    public enum MovingTaskType
    {
        MoveTo,
        AwayFrom
    }


    /// <summary>
    /// represents a task that requires the user to bring an object to a specific position
    /// </summary>
    public class MovingTask : Task
    {
        /// <summary>
        /// the end position to which the object must be brought to
        /// </summary>
        private Vector3 _position;
        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        /// <summary>
        /// the end rotation of the object
        /// </summary>
        private Quaternion _rotation;
        public Quaternion Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        /// <summary>
        /// the threshold in which the object will snap to the position
        /// </summary>
        private float _snapThreshold = 0.1f;
        public float SnapThreshold
        {
            get { return _snapThreshold; }
            set { _snapThreshold = value; }
        }

        /// <summary>
        /// whether we have to move to or away from a certain location
        /// </summary>
        private MovingTaskType _moveType;
        public MovingTaskType MoveType
        {
            get { return _moveType; }
            set { _moveType = value; }
        }

        /// <summary>
        /// constructor taking a go and position
        /// </summary>
        public MovingTask(TaskList tl, GameObject go, Vector3 position, Quaternion rotation) :base(tl,go)
        {
            Position = position;
            Rotation = rotation;
        }

        /// <summary>
        /// constructor taking a serializable task
        /// </summary>
        public MovingTask(TaskList tl, SerializableTask task) : base(tl,task)
        {
            Position = task.Position;
            Rotation = task.Rotation;
            SnapThreshold = task.SnapThreshold;
            MoveType = task.MoveType;
        }

        /// <summary>
        /// reimplementation of checktask. simply checks whether the position of the go is close to the goal position
        /// </summary>
        public override bool CheckTask()
        {
            bool isFinished = (MoveType==MovingTaskType.MoveTo)?(Vector3.Distance(GameObject.transform.position, TaskList.MPO.transform.TransformPoint(Position)) <= SnapThreshold): (Vector3.Distance(GameObject.transform.position, TaskList.MPO.transform.TransformPoint(Position)) > SnapThreshold);
            if (isFinished)
            {
                // snap to position if movetype is  moveto
                if (MoveType == MovingTaskType.MoveTo)
                {
                    GameObject.transform.position = TaskList.MPO.transform.TransformPoint(Position);
                    GameObject.transform.rotation = TaskList.MPO.transform.rotation * Rotation;
                }
            }
            return isFinished;
        }

        /// <summary>
        /// reimplemntation of draw task hint. draw the silhouette of the game object at the goal pos
        /// </summary>
        public override void DrawTaskHint()
        {
            TaskList.Hint = new GameObject();
            TaskList.Hint.transform.SetPositionAndRotation(TaskList.MPO.transform.TransformPoint(Position), TaskList.MPO.transform.rotation * Rotation);
            TaskList.Hint.transform.localScale = GameObject.transform.lossyScale;
            //TaskList.Hint = GameObject.Instantiate(TaskList.HintPrefab,TaskList.MPO.transform.TransformPoint(Position),TaskList.MPO.transform.rotation * Rotation);
            TaskList.Hint.AddComponent<MeshFilter>();
            TaskList.Hint.GetComponent<MeshFilter>().sharedMesh = GameObject.GetComponent<MeshFilter>().sharedMesh;
            TaskList.Hint.AddComponent<MeshRenderer>();
            TaskList.Hint.GetComponent<Renderer>().material = TaskList.SilhouetteMaterial;

            /*if (TaskList.Hint.GetComponent<Collider>() != null)
                GameObject.Destroy(TaskList.Hint.GetComponent<Collider>());
            if (TaskList.Hint.GetComponent<Renderer>() != null)
                TaskList.Hint.GetComponent<Renderer>().material = TaskList.SilhouetteMaterial;*/
        }

        /// <summary>
        /// update task hint in case user is moving the cage around
        /// </summary>
        public override void UpdateTaskHint()
        {
            if (TaskList.Hint != null)
                TaskList.Hint.transform.SetPositionAndRotation(TaskList.MPO.transform.TransformPoint(Position), TaskList.MPO.transform.rotation * Rotation);
        }

        /// <summary>
        /// draw hint gizmos on editor mode
        /// </summary>
        public override void DrawEditorTaskHint()
        {
            if(GameObject != null && GameObject.GetComponent<MeshFilter>() != null)
            {
                Color shadowColor = Color.magenta;
                Gizmos.color = shadowColor;
                Gizmos.DrawMesh(GameObject.GetComponent<MeshFilter>().sharedMesh,TaskList.MPO.transform.TransformPoint(Position),TaskList.MPO.transform.rotation * Rotation,GameObject.transform.lossyScale);
                Color selectedColor = Color.yellow;
                Gizmos.color = selectedColor;
                Gizmos.DrawMesh(GameObject.GetComponent<MeshFilter>().sharedMesh, GameObject.transform.position, GameObject.transform.rotation, GameObject.transform.lossyScale);
            }
        }
    }
}
