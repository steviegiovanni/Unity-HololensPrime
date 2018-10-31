/// author: Stevie Giovanni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModelViewer
{
    /// <summary>
    /// simple UI, listens to on task start event and shows the task name in front of the user for each task 
    /// </summary>
    public class TaskListUI : MonoBehaviour
    {
        /// <summary>
        /// the task list to listen to
        /// </summary>
        [SerializeField]
        private TaskList _taskList;
        public TaskList TaskList
        {
            get { return _taskList; }
            set { _taskList = value; }
        }

        /// <summary>
        /// the task name will only be shown for this short duration of time
        /// </summary>
        [SerializeField]
        private float _hintDuration = 3.0f;
        public float HintDuration
        {
            get { return _hintDuration; }
            set { _hintDuration = value; }
        }

        [SerializeField]
        private GameObject hintText;

        private void Awake()
        {
            // listen to task list start task event
            TaskList.TaskStartListeners.AddListener(ShowNextTask);
        }

        /// <summary>
        /// the function to be called when this object receives the start task event
        /// </summary>
        public void ShowNextTask(Task task)
        {
            if(task == null)
                hintText.GetComponent<TextMesh>().text = "Well Done!";
            else
                hintText.GetComponent<TextMesh>().text = task.TaskName;
        }
    }
}
