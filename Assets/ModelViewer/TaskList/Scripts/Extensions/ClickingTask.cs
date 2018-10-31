using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModelViewer
{
    /// <summary>
    /// represents a generic task. can be used when user selects a component and stuff need to happen
    /// </summary>
    public class ClickingTask : Task
    {
        /// <summary>
        /// constructor taking a go
        /// </summary>
        public ClickingTask(TaskList tl, GameObject go) : base(tl,go){}

        /// <summary>
        /// constructor taking a serializable task
        /// </summary>
        public ClickingTask (TaskList tl, SerializableTask task):base(tl, task){}

        /// <summary>
        /// reimplemntation of draw task hint. draw the silhouette of the game object at the goal pos
        /// </summary>
        public override void DrawTaskHint()
        {
            TaskList.Hint = GameObject.Instantiate(TaskList.HintPrefab, GameObject.transform.position, GameObject.transform.rotation);
            TaskList.Hint.GetComponent<MeshFilter>().sharedMesh = GameObject.GetComponent<MeshFilter>().sharedMesh;
            TaskList.Hint.transform.localScale = GameObject.transform.lossyScale;
            if (TaskList.Hint.GetComponent<Collider>() != null)
                GameObject.Destroy(TaskList.Hint.GetComponent<Collider>());
            if (TaskList.Hint.GetComponent<Renderer>() != null)
                TaskList.Hint.GetComponent<Renderer>().material = TaskList.SilhouetteMaterial;
        }

        /// <summary>
        /// update task hint in case user is moving the cage around
        /// </summary>
        public override void UpdateTaskHint()
        {
            if (TaskList.Hint != null)
                TaskList.Hint.transform.SetPositionAndRotation(GameObject.transform.position, GameObject.transform.rotation);
        }

        /// <summary>
        /// draw hint gizmos on editor mode
        /// </summary>
        public override void DrawEditorTaskHint()
        {
            if (GameObject != null && GameObject.GetComponent<MeshFilter>() != null)
            {
                Color selectedColor = Color.yellow;
                Gizmos.color = selectedColor;
                Gizmos.DrawMesh(GameObject.GetComponent<MeshFilter>().sharedMesh, GameObject.transform.position, GameObject.transform.rotation, GameObject.transform.lossyScale);
            }
        }
    }
}
