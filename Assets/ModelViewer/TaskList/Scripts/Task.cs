/// author: Stevie Giovanni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModelViewer
{
    /// <summary>
    /// represents a task
    /// </summary>
    public class Task
    {
        /// <summary>
        /// the task list that contains this task
        /// </summary>
        private TaskList _taskList;
        public TaskList TaskList
        {
            get { return _taskList; }
            set { _taskList = value; }
        }

        /// <summary>
        /// name of the task
        /// </summary>
        private string _taskName;
        public string TaskName
        {
            get { return _taskName; }
            set { _taskName = value; }
        }

        /// <summary>
        /// description of the task
        /// </summary>
        private string _description;
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// whether the task is finished of not
        /// </summary>
        private bool _finished = false;
        public bool Finished
        {
            get { return _finished; }
            set { _finished = value; }
        }

        /// <summary>
        /// the game object users need to interact with to complete the task
        /// </summary>
        private GameObject _go;
        public GameObject GameObject
        {
            get { return _go; }
            set { _go = value; }
        }

        /// <summary>
        /// the game object name to help find the object if the model is replaced
        /// </summary>
        private string _goName;
        public string GOName
        {
            get { return _goName; }
            set { _goName = value; }
        }

        /// <summary>
        /// whether this task is active or not
        /// </summary>
        private bool _active = false;
        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }

        private List<TaskEvent> _taskEvents;
        public List<TaskEvent> TaskEvents
        {
            get
            {
                if (_taskEvents == null)
                    _taskEvents = new List<TaskEvent>();
                return _taskEvents;
            }
        }

        /// <summary>
        /// whether the task is enabled or not
        /// </summary>
        private bool _enabled = true;
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        /// <summary>
        /// task event delay when the task is completed
        /// </summary>
        private float _taskDelay;
        public float TaskDelay
        {
            get { return _taskDelay; }
            set { _taskDelay = value; }
        }

        /// <summary>
        /// check task is finished, override for different task behavior
        /// </summary>
        public virtual bool CheckTask()
        {
            return true;
            //Finished = true;
            /*if (!IsCurrentTask()) return;
            Debug.Log("Base check task");
            if (Finished)
                TaskList.NextTask();*/
        }

        /// <summary>
        /// what kind of hint should be drawn? override for different task behaviour
        /// </summary>
        public virtual void DrawTaskHint()
        { }

        /// <summary>
        /// update task hint in case user is moving the cage around
        /// </summary>
        public virtual void UpdateTaskHint()
        { }

        /// <summary>
        /// draw hint gizmos on editor mode
        /// </summary>
        public virtual void DrawEditorTaskHint()
        { }

        /// <summary>
        /// base constructor taking a game object
        /// </summary>
        public Task(TaskList tl, GameObject go)
        {
            TaskDelay = 0;
            TaskList = tl;
            TaskName = "New Task";
            GameObject = go;
            GOName = go.name;
        }

        /// <summary>
        /// constructor taking a serializable task
        /// </summary>
        public Task(TaskList tl, SerializableTask task)
        {
            TaskDelay = task.TaskDelay;
            TaskList = tl;
            GameObject = task.GameObject;
            TaskName = task.TaskName;
            Description = task.Description;
            GOName = task.GOName;

            foreach (var taskEvent in task.TaskEvents)
            {
                switch (taskEvent.TypeName)
                {
                    case "TransformTaskEvent":
                        {
                            TaskEvent newTaskEvent = new TransformTaskEvent(taskEvent);
                            newTaskEvent.Task = this;
                            this.TaskEvents.Add(newTaskEvent);
                        }
                        break;
                    case "AnimationTaskEvent":
                        {

                            TaskEvent newTaskEvent = new AnimationTaskEvent(taskEvent);
                            newTaskEvent.Task = this;
                            this.TaskEvents.Add(newTaskEvent);
                        }
                        break;
                }
            }
        }
    }
}
