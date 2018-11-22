/// author: Stevie Giovanni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ModelViewer
{
    /// <summary>
    /// a tasklist represents a series of tasks associated with the loaded object model
    /// </summary>
    public class TaskList : MonoBehaviour, ISerializationCallbackReceiver
    {
        [SerializeField]
        private GameObject _hintPrefab;
        public GameObject HintPrefab
        {
            get
            {
                return _hintPrefab;
            }
        }

        /// <summary>
        /// the multiparts object that this tasklist is associated with
        /// </summary>
        [SerializeField]
        private MultiPartsObject _mpo;
        public MultiPartsObject MPO
        {
            get
            {
                if (_mpo == null)
                {
                    _mpo = GameObject.FindObjectOfType<MultiPartsObject>();
                    if (_mpo == null)
                        Debug.LogError("No multiparts object found!");
                }
                return _mpo;
            }
        }

        /// <summary>
        /// the id of the currently active task
        /// </summary>
        private int _currentTaskId = 0;
        public int CurrentTaskId
        {
            get { return _currentTaskId; }
            set { _currentTaskId = value; }
        }

        /// <summary>
        /// the list of tasks
        /// </summary>
        private List<Task> _tasks;
        public List<Task> Tasks
        {
            get
            {
                if (_tasks == null)
                    _tasks = new List<Task>();
                return _tasks;
            }
        }

        /// <summary>
        /// the hint object, probably instantiated when each task is active
        /// </summary>
        private GameObject _hint;
        public GameObject Hint
        {
            get { return _hint; }
            set { _hint = value; }
        }

        /// <summary>
		/// silhouette material , in case needed
		/// </summary>
		[SerializeField]
        private Material _silhouetteMaterial;
        public Material SilhouetteMaterial
        {
            get { return _silhouetteMaterial; }
            set { _silhouetteMaterial = value; }
        }

        /// <summary>
        /// the serialized node structure
        /// </summary>
        [HideInInspector]
        public List<SerializableTask> serializedTasks;

        /// <summary>
        /// listeners notified when a new task starts, useful for UI
        /// </summary>
        public class TaskStartEvent : UnityEvent<Task> { }
        private TaskStartEvent _taskStartListeners;
        public TaskStartEvent TaskStartListeners
        {
            get
            {
                if (_taskStartListeners == null)
                    _taskStartListeners = new TaskStartEvent();
                return _taskStartListeners;
            }
        }

        public UnityEvent OnTaskFinished;

        /// <summary>
        /// find game object associated to a task based on its name (useful for when the multi parts object is replaced)
        /// </summary>
        public void FindMissingObjectsByName()
        {
            foreach (var task in Tasks)
            {
                if (task.GameObject == null)
                    task.GameObject = GameObject.Find(task.GOName);
            }
        }

        /// <summary>
        /// recapture the names of the game objects associated to all tasks
        /// </summary>
        public void CaptureGameObjectNames()
        {
            foreach (var task in Tasks)
            {
                if (task.GameObject != null)
                    task.GOName = task.GameObject.name;
            }
        }

        /// <summary>
        /// initial position and rotation of gameobjects when the scene is started
        /// </summary>
        private List<Vector3> _goInitialPos;
        private List<Quaternion> _goInitialRot;
        private Vector3 cagePos;
        private Quaternion cageRot;

        /// <summary>
        /// store initial position and rotation of game objects related to the task
        /// </summary>
        public void StoreInitialPosAndRot()
        {
            cagePos = MPO.transform.position;
            cageRot = MPO.transform.rotation;
            _goInitialPos = new List<Vector3>();
            _goInitialRot = new List<Quaternion>();
            foreach (var task in Tasks)
            {
                _goInitialPos.Add(task.GameObject.transform.position);
                _goInitialRot.Add(task.GameObject.transform.rotation);
            }
        }

        /// <summary>
        /// reset initial position and rotation of gameobjects related to the task 
        /// </summary>
        public void ResetInitialPosAndRot()
        {
            MPO.transform.SetPositionAndRotation(cagePos, cageRot);

            int i = 0;
            foreach (var task in Tasks)
            {
                // store all child pos and rot cos we only want to move this specific go
                List<Vector3> childInitPos = new List<Vector3>();
                List<Quaternion> childInitRot = new List<Quaternion>();
                foreach (Transform child in task.GameObject.transform)
                {
                    childInitPos.Add(child.position);
                    childInitRot.Add(child.rotation);
                }

                // reset go pos and rotation
                task.GameObject.transform.SetPositionAndRotation(_goInitialPos[i], _goInitialRot[i]);

                // reset all child pos and rot
                int j = 0;
                foreach (Transform child in task.GameObject.transform)
                {
                    child.SetPositionAndRotation(childInitPos[j], childInitRot[j]);
                    j++;
                }

                // increment iterator
                i++;
            }
        }

        /// <summary>
        /// reset the tasklist
        /// </summary>
        public void Reset()
        {
            this.StopAllCoroutines();
            ResetInitialPosAndRot();
            foreach (var task in Tasks)
                task.Finished = false;
            StartCoroutine(TaskListCoroutine());
        }

        /// <summary>
        /// serialization interface implementation
        /// </summary>
        public void OnBeforeSerialize()
        {
            if (serializedTasks == null)
                serializedTasks = new List<SerializableTask>();
            serializedTasks.Clear();
            foreach (var task in Tasks)
                serializedTasks.Add(new SerializableTask(task));
        }

        /// <summary>
        /// create a task from a serializable task
        /// </summary>
        public Task ReadTaskFromSerializedTask(SerializableTask st)
        {
            switch (st.TypeName)
            {
                case "MovingTask":
                    {
                        return new MovingTask(this, st);
                    }
                case "ClickingTask":
                    {
                        return new ClickingTask(this, st);
                    }
                default:
                    {
                        return new Task(this, st);
                    }
            }
        }

        /// <summary>
        /// serialization interface implementation
        /// </summary>
        public void OnAfterDeserialize()
        {
            Tasks.Clear();
            foreach (var serializedTask in serializedTasks)
                Tasks.Add(ReadTaskFromSerializedTask(serializedTask));
        }

        /// <summary>
        /// at start, after the multiparts object awake method, we setup the necessary things
        /// </summary>
        public void Start()
        {
            MPO.OnReleaseEvent.AddListener(CheckTaskOnRelease);
            MPO.OnSelectEvent.AddListener(CheckTaskOnSelect);
            //OnTaskFinished.AddListener(IncrementTaskId);

            StoreInitialPosAndRot();
            // register each task "CheckTask" function to onrelease of the appropriate node
            ChangeLockOfAllTaskNodes(true);
            CurrentTaskId = 0;
            StartCoroutine(TaskListCoroutine());
        }

        public IEnumerator TaskListCoroutine()
        {
            while (CurrentTaskId != Tasks.Count)
            {

                if (CurrentTaskId == -1)
                    yield return null;

                int tempTaskId = CurrentTaskId;

                // destroy previous hint if any
                if (Hint != null)
                    Destroy(Hint);

                // get the task
                Task task = Tasks[tempTaskId];

                // unlock node
                ChangeLockOfTaskNode(task, false);

                // fire task start event
                if (TaskStartListeners != null)
                    TaskStartListeners.Invoke(task);

                task.DrawTaskHint();
                while (tempTaskId == CurrentTaskId)
                {
                    task.UpdateTaskHint();
                    yield return null;
                }

                MPO.Release();
                MPO.Deselect(task.GameObject);

                ChangeLockOfTaskNode(task, true);

                // destroy hint if any
                if (Hint != null)
                    Destroy(Hint);

                // run task event coroutine if exists
                /*if (task.TaskEvent != null)
                {
                    //yield return StartCoroutine(task.TaskEvent.TaskEventCoroutine());
                    StartCoroutine(task.TaskEvent.TaskEventCoroutine());
                }*/
                foreach (var taskEvent in task.TaskEvents)
                    StartCoroutine(taskEvent.TaskEventCoroutine());

                yield return new WaitForSeconds(task.TaskDelay);

                //yield return null;
            }

            TaskStartListeners.Invoke(null);

            yield return null;
        }

        /*public IEnumerator TaskListCoroutine()
        {
            // set current task id to -1 as we're going to increment by 1 on NextTask() on start
            CurrentTaskId = -1;

            // lock all nodes
            ChangeLockOfAllTaskNodes(true);   

            foreach (var task in Tasks)
            {
                // destroy previous hint if any
                if (Hint != null)
                    Destroy(Hint);

                // increment task id
                CurrentTaskId++;

                // unlock node
                ChangeLockOfTaskNode(task, false);

                // fire task start event
                if (TaskStartListeners != null)
                    TaskStartListeners.Invoke(Tasks[CurrentTaskId]);

                // draw current task hint
                Tasks[CurrentTaskId].DrawTaskHint();
                while (!Tasks[CurrentTaskId].Finished)
                {
                    Tasks[CurrentTaskId].UpdateTaskHint();
                    yield return null;
                }
                
                MPO.Release();
                MPO.Deselect(Tasks[CurrentTaskId].GameObject);

                // lock node again
                ChangeLockOfTaskNode(task, true);

                // destroy hint if any
                if (Hint != null)
                    Destroy(Hint);

                // run task event coroutine if exists
                if (Tasks[CurrentTaskId].TaskEvent != null)
                    yield return StartCoroutine(Tasks[CurrentTaskId].TaskEvent.TaskEventCoroutine());
            }

            TaskStartListeners.Invoke(null);
            yield return null;
        }*/

        void ChangeLockOfTaskNode(Task task, bool value)
        {
            Node node = null;
            if (MPO.Dict.TryGetValue(task.GameObject, out node))
                node.Locked = value;
        }

        void ChangeLockOfAllTaskNodes(bool value)
        {
            foreach (var task in Tasks)
                ChangeLockOfTaskNode(task, value);
        }

        void OnDrawGizmosSelected()
        {
            if (Tasks.Count > CurrentTaskId && CurrentTaskId >= 0)
            {
                Tasks[CurrentTaskId].DrawEditorTaskHint();
            }
        }

        /// <summary>
        /// fires check task for moving task
        /// </summary>
        public void CheckTaskOnRelease(Node node)
        {
            if (CurrentTaskId < Tasks.Count && CurrentTaskId != -1)
            {
                Task task = Tasks[CurrentTaskId];
                if (!task.Enabled) return;
                if (task.GetType().Name != "MovingTask") return;
                if (task.GameObject == node.GameObject)
                {
                    // make sure we have a multiparts object
                    if (task.CheckTask() && OnTaskFinished != null) OnTaskFinished.Invoke();
                }
            }
        }

        /// <summary>
        /// fires check task for clicking task
        /// </summary>
        public void CheckTaskOnSelect(Node node)
        {
            if (CurrentTaskId < Tasks.Count && CurrentTaskId != -1)
            {
                Task task = Tasks[CurrentTaskId];
                //if (!task.Enabled) return;
                if (task.GetType().Name != "ClickingTask") return;
                if (task.GameObject == node.GameObject)
                {
                    // make sure we have a multiparts object
                    if (task.CheckTask() && OnTaskFinished != null) OnTaskFinished.Invoke();
                }
            }
        }

        public void IncrementTaskId()
        {
            CurrentTaskId++;
        }
    }
}
