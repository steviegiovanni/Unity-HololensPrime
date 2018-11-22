/// author: Stevie Giovanni 

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ModelViewer
{
    /// <summary>
    /// custom inspector for TaskList showing all the tasks
    /// </summary>
    [CustomEditor(typeof(TaskList))]
    public class TaskListInspector : UnityEditor.Editor
    {
        TaskList obj;

        private int _selectedTaskId = -1;
        public int SelectedTaskId
        {
            get { return _selectedTaskId; }
            set
            {
                if (_selectedTaskId != value)
                {
                    _selectedTaskId = value;
                    EditorGUI.FocusTextInControl(string.Empty);
                }
            }
        }

        private int _selectedTaskEventId = -1;
        public int SelectedTaskEventId
        {
            get { return _selectedTaskEventId; }
            set
            {
                if (_selectedTaskEventId != value)
                {
                    _selectedTaskEventId = value;
                    EditorGUI.FocusTextInControl(string.Empty);
                }
            }
        }

        /// <summary>
        /// display this if the task is of type moving task
        /// </summary>
        /// <param name="t"></param>
        public void DisplayMovingTaskDetail(Task t)
        {
            MovingTask castedt = t as MovingTask;

            // to modify snap threshold
            GUILayout.Label("Distance");
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            castedt.SnapThreshold = GUILayout.HorizontalSlider(castedt.SnapThreshold, 0.1f, 1f);
            GUILayout.Label(castedt.SnapThreshold.ToString(), GUILayout.Width(30));
            GUILayout.EndHorizontal();

            // move type
            GUILayout.BeginHorizontal();
            GUILayout.Label("Type", GUILayout.Width(30));
            castedt.MoveType = (MovingTaskType)EditorGUILayout.EnumPopup(castedt.MoveType);
            GUILayout.EndHorizontal();

            // to modify goal position
            GUILayout.Label("Goal");
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("x: ");
            float x = EditorGUILayout.FloatField(castedt.Position.x);
            GUILayout.Label("y: ");
            float y = EditorGUILayout.FloatField(castedt.Position.y);
            GUILayout.Label("z: ");
            float z = EditorGUILayout.FloatField(castedt.Position.z);
            castedt.Position = new Vector3(x, y, z);
            GUILayout.Space(20);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("i: ");
            float i = EditorGUILayout.FloatField(castedt.Rotation.x);
            GUILayout.Label("j: ");
            float j = EditorGUILayout.FloatField(castedt.Rotation.y);
            GUILayout.Label("k: ");
            float k = EditorGUILayout.FloatField(castedt.Rotation.z);
            GUILayout.Label("w: ");
            float w = EditorGUILayout.FloatField(castedt.Rotation.w);
            castedt.Rotation = new Quaternion(i, j, k, w);
            GUILayout.Space(20);
            GUILayout.EndHorizontal();

            // easily get the go current position as the goal pos
            if (GUILayout.Button("Use Current GO Position and rotation"))
            {
                if (castedt.GameObject != null)
                {
                    castedt.Position = obj.transform.InverseTransformPoint(t.GameObject.transform.position);
                    castedt.Rotation = Quaternion.Inverse(obj.transform.rotation) * t.GameObject.transform.rotation;
                }
            }
        }

        public void DisplayAnimationTaskEventIOfTask(Task t, int index)
        {
            AnimationTaskEvent tte = t.TaskEvents[index] as AnimationTaskEvent;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Model Controller", GUILayout.Width(100));
            tte.GameObject = (GameObject)EditorGUILayout.ObjectField(tte.GameObject, typeof(GameObject), true);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Parameter", GUILayout.Width(100));
            tte.AnimatorParam = EditorGUILayout.TextField(tte.AnimatorParam);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Value", GUILayout.Width(100));
            tte.AnimatorParamValue = EditorGUILayout.FloatField(tte.AnimatorParamValue);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Modify Parameter", GUILayout.Width(100));
            tte.ModifyAnimatorParam = EditorGUILayout.Toggle(tte.ModifyAnimatorParam);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Layer", GUILayout.Width(100));
            tte.AnimatorLayer = EditorGUILayout.TextField(tte.AnimatorLayer);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Amount", GUILayout.Width(100));
            tte.AnimatorLayerAmount = EditorGUILayout.FloatField(tte.AnimatorLayerAmount);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Modify Layer", GUILayout.Width(100));
            tte.ModifyAnimatorLayer = EditorGUILayout.Toggle(tte.ModifyAnimatorLayer);
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// display task event of type transformtaskevent
        /// </summary>
        public void DisplayTransformTaskEventIOfTask(Task t, int index)
        {
            TransformTaskEvent tte = t.TaskEvents[index] as TransformTaskEvent;
            // to modify start and end position
            GUILayout.Label("Start");
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("x: ");
            float x = EditorGUILayout.FloatField(tte.StartPos.x);
            GUILayout.Label("y: ");
            float y = EditorGUILayout.FloatField(tte.StartPos.y);
            GUILayout.Label("z: ");
            float z = EditorGUILayout.FloatField(tte.StartPos.z);
            tte.StartPos = new Vector3(x, y, z);
            GUILayout.Space(20);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("i: ");
            float i = EditorGUILayout.FloatField(tte.StartRotation.x);
            GUILayout.Label("j: ");
            float j = EditorGUILayout.FloatField(tte.StartRotation.y);
            GUILayout.Label("k: ");
            float k = EditorGUILayout.FloatField(tte.StartRotation.z);
            GUILayout.Label("w: ");
            float w = EditorGUILayout.FloatField(tte.StartRotation.w);
            tte.StartRotation = new Quaternion(i, j, k, w);
            GUILayout.Space(20);
            GUILayout.EndHorizontal();

            // easily get the go current position as the goal pos
            if (GUILayout.Button("Use Current GO Position and rotation"))
            {
                if (t.GameObject != null)
                {
                    tte.StartPos = obj.transform.InverseTransformPoint(t.GameObject.transform.position);
                    tte.StartRotation = Quaternion.Inverse(obj.transform.rotation) * t.GameObject.transform.rotation;
                }
            }

            GUILayout.Label("End");
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("x: ");
            float xend = EditorGUILayout.FloatField(tte.EndPos.x);
            GUILayout.Label("y: ");
            float yend = EditorGUILayout.FloatField(tte.EndPos.y);
            GUILayout.Label("z: ");
            float zend = EditorGUILayout.FloatField(tte.EndPos.z);
            tte.EndPos = new Vector3(xend, yend, zend);
            GUILayout.Space(20);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("i: ");
            float endi = EditorGUILayout.FloatField(tte.EndRotation.x);
            GUILayout.Label("j: ");
            float endj = EditorGUILayout.FloatField(tte.EndRotation.y);
            GUILayout.Label("k: ");
            float endk = EditorGUILayout.FloatField(tte.EndRotation.z);
            GUILayout.Label("w: ");
            float endw = EditorGUILayout.FloatField(tte.EndRotation.w);
            tte.EndRotation = new Quaternion(endi, endj, endk, endw);
            GUILayout.Space(20);
            GUILayout.EndHorizontal();

            // easily get the go current position as the goal pos
            if (GUILayout.Button("Use Current GO Position and rotation"))
            {
                if (t.GameObject != null)
                {
                    tte.EndPos = obj.transform.InverseTransformPoint(t.GameObject.transform.position);
                    tte.EndRotation = Quaternion.Inverse(obj.transform.rotation) * t.GameObject.transform.rotation;
                }
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Duration ");
            tte.Duration = EditorGUILayout.FloatField(tte.Duration);
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// display a particular task detail. case by case depending on the task type
        /// </summary>
        public void DisplayTaskDetail(Task t)
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);

            // task type
            GUILayout.Label(t.GetType().Name, EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            GUILayout.Label("GO", GUILayout.Width(50));
            t.GameObject = (GameObject)EditorGUILayout.ObjectField(t.GameObject, typeof(GameObject), true);
            GUILayout.EndHorizontal();

            // go name
            GUILayout.BeginHorizontal();
            GUILayout.Label("GOName", GUILayout.Width(50));
            t.GOName = GUILayout.TextField(t.GOName);
            GUILayout.EndHorizontal();

            // task name
            GUILayout.BeginHorizontal();
            GUILayout.Label("Name", GUILayout.Width(50));
            t.TaskName = GUILayout.TextField(t.TaskName);
            GUILayout.EndHorizontal();

            // task delay
            GUILayout.BeginHorizontal();
            GUILayout.Label("Delay", GUILayout.Width(50));
            t.TaskDelay = EditorGUILayout.FloatField(t.TaskDelay);
            GUILayout.EndHorizontal();

            // task description
            GUILayout.BeginHorizontal();
            GUILayout.Label("Description");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            t.Description = GUILayout.TextArea(t.Description, GUILayout.Height(100));
            GUILayout.EndHorizontal();

            switch (t.GetType().Name)
            {
                case "MovingTask":
                    {
                        DisplayMovingTaskDetail(t);
                    }
                    break;
            }
            GUILayout.EndVertical();

            // for task events
            GUILayout.BeginVertical(EditorStyles.helpBox);
            for (int i = 0; i < t.TaskEvents.Count; i++)
            {
                GUILayout.BeginHorizontal();

                var tevent = t.TaskEvents[i];

                GUIStyle style = new GUIStyle(EditorStyles.toolbarButton);
                style.alignment = TextAnchor.MiddleLeft;
                bool clicked = GUILayout.Toggle(i == SelectedTaskEventId, i + 1 + ". " + tevent.GetType().Name, style);
                if (clicked != (i == SelectedTaskEventId))
                {
                    if (clicked)
                    {
                        SelectedTaskEventId = i;
                        GUI.FocusControl(null);
                    }
                    else
                    {
                        SelectedTaskEventId = -1;
                    }
                }

                // remove task from task event list
                if (GUILayout.Button("-", EditorStyles.toolbarButton, GUILayout.Width(30)))
                {
                    if (SelectedTaskEventId != -1)
                        SelectedTaskEventId = -1;
                    t.TaskEvents.RemoveAt(i);
                }

                GUILayout.EndHorizontal();

                // display task detail if task is selected
                if (i == SelectedTaskEventId)
                {
                    switch (tevent.GetType().Name)
                    {
                        case "TransformTaskEvent":
                            {
                                DisplayTransformTaskEventIOfTask(t, i);
                            }
                            break;
                        case "AnimationTaskEvent":
                            {
                                DisplayAnimationTaskEventIOfTask(t, i);
                            }
                            break;
                    }
                }

                /*if (GUILayout.Button("Remove Task Event"))
                {
                    t.TaskEvents.Remove(tevent);
                }*/
            }

            if (GUILayout.Button("Add Task Event"))
            {
                GenericMenu genericMenu = new GenericMenu();
                for (int eventTypeId = 0; eventTypeId < TaskListEditorUtility.EventTypes().Length; eventTypeId++)
                {
                    genericMenu.AddItem(new GUIContent(TaskListEditorUtility.EventTypes()[eventTypeId]), false,
                        (param) =>
                        {
                            int index = (int)param;
                            switch (index)
                            {
                                case 0:
                                    {
                                        t.TaskEvents.Add(new TransformTaskEvent());
                                    }
                                    break;
                                case 1:
                                    {
                                        t.TaskEvents.Add(new AnimationTaskEvent());
                                    }
                                    break;
                            }
                        }
                    , eventTypeId);
                }
                genericMenu.ShowAsContext();
            }
            GUILayout.EndVertical();
        }

        public override void OnInspectorGUI()
        {
            // draw base inspector gui
            base.OnInspectorGUI();

            obj = (TaskList)target;

            // to find missing game objects
            if (GUILayout.Button("Find Missing Objects by Name"))
            {
                obj.FindMissingObjectsByName();
            }

            // to recapture associated game object names
            if (GUILayout.Button("Capture Game Objects Names"))
            {
                obj.CaptureGameObjectNames();
            }

            GUILayout.BeginVertical();
            for (int i = 0; i < obj.Tasks.Count; i++)
            {
                GUILayout.BeginHorizontal();

                // toggle show task detail on and off
                string taskName = "Missing!";
                if (obj.Tasks[i].GameObject != null)
                {
                    taskName = obj.Tasks[i].TaskName + " (" + obj.Tasks[i].GameObject.name + ")";
                }

                GUIStyle style = new GUIStyle(EditorStyles.toolbarButton);
                style.alignment = TextAnchor.MiddleLeft;
                bool clicked = GUILayout.Toggle(i == SelectedTaskId, i + 1 + ". " + taskName, style);
                if (clicked != (i == SelectedTaskId))
                {
                    if (clicked)
                    {
                        EditorGUIUtility.PingObject(obj.Tasks[i].GameObject);
                        obj.CurrentTaskId = i;
                        SelectedTaskId = i;
                        GUI.FocusControl(null);
                    }
                    else
                    {
                        obj.CurrentTaskId = -1;
                        SelectedTaskId = -1;
                    }
                }

                // to easily reorder tasks up and down
                if (i <= 0)
                    GUI.enabled = false;
                if (GUILayout.Button(i > 0 ? "^" : " ", EditorStyles.toolbarButton, GUILayout.Width(30)))
                {
                    Task temp = obj.Tasks[i];
                    obj.Tasks[i] = obj.Tasks[i - 1];
                    obj.Tasks[i - 1] = temp;
                }
                GUI.enabled = true;

                // to easily reorder tasks up and down
                if (i >= obj.Tasks.Count - 1)
                    GUI.enabled = false;
                if (GUILayout.Button(i < obj.Tasks.Count - 1 ? "v" : " ", EditorStyles.toolbarButton, GUILayout.Width(30)))
                {
                    Task temp = obj.Tasks[i];
                    obj.Tasks[i] = obj.Tasks[i + 1];
                    obj.Tasks[i + 1] = temp;
                }
                GUI.enabled = true;

                // remove task from task list
                if (GUILayout.Button("-", EditorStyles.toolbarButton, GUILayout.Width(30)))
                {
                    if (SelectedTaskId != -1)
                        SelectedTaskId = -1;
                    obj.Tasks.RemoveAt(i);
                }

                GUILayout.EndHorizontal();

                // display task detail if task is selected
                if (i == SelectedTaskId)
                {
                    DisplayTaskDetail(obj.Tasks[i]);
                }
            }
            GUILayout.EndVertical();

            if (GUI.changed)
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
    }
}
