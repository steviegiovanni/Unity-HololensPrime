/// author: Stevie Giovanni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace ModelViewer
{
    /// <summary>
    /// custom inspector for multiparts object
    /// </summary>
    [CustomEditor(typeof(MultiPartsObject))]
    public class MultiPartsObjectInspector : UnityEditor.Editor
    {
        /// <summary>
        /// the mpo associated with this editor
        /// </summary>
        MultiPartsObject obj;

        /// <summary>
        /// toggle for the lock/unlock all nodes button
        /// </summary>
        bool lockState;

        /// <summary>
        /// indent level for visualizing the tree
        /// </summary>
        int indentLevel;

        public override void OnInspectorGUI()
        {
            // draw default inspector gui
            base.OnInspectorGUI();

            // get the target
            obj = (MultiPartsObject)target;

            // to setup internal data structure after inserting the gameobject
            if (GUILayout.Button("Setup"))
                obj.Setup();

            // to fit the object into the cage size
            if (GUILayout.Button("Fit"))
                obj.FitToScale(obj.Root, obj.VirtualScale);

            // to find missing game objects
            if (GUILayout.Button("Find Missing Objects by Name"))
            {
                obj.FindMissingObjectsByName(obj.Root);
            }

            // to find missing game objects
            if (GUILayout.Button("Capture GameObject Names"))
            {
                obj.CaptureGameObjectName(obj.Root);
            }

            // to reset all transform position, rotation, and scale
            if (GUILayout.Button("Reset"))
            {
                obj.ResetAll(obj.Root);
                obj.transform.localScale = Vector3.one;
            }

            GUILayout.Space(10);

            // to unlock / lock all nodes with one click
            if(GUILayout.Button(lockState?"Unlock All Nodes":"Lock All Nodes"))
            {
                lockState = !lockState;
                obj.SetNodeLockRecursive(obj.Root, lockState);
            }

            // draw the tree structure
            GUILayout.Label("Tree");
            indentLevel = 0;
            DisplayNodeInfo(obj.Root);

            // if something is changed in the editor, make scene dirty
            if (GUI.changed)
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        /// <summary>
        /// displaying each node info
        /// </summary>
        public void DisplayNodeInfo(Node node)
        {
            if (node != null)
            {
				// node name and go, lock button, reset button, add task button
                GUILayout.BeginHorizontal();
                GUILayout.Space(indentLevel * 10);
				if (GUILayout.Button (node.Name + " ("+(node.GameObject==null?"Missing!": node.GameObject.name)+")" + (node.Selected?"*":""), "Button")) {
                    EditorGUIUtility.PingObject(node.GameObject);
                    obj.ToggleSelect(node);
				}

                // to lock or unlock node for interaction
                if (GUILayout.Button((node.Locked ? "X" : " "), "Button",GUILayout.Width(20)))
                {
                    if (node.Locked)
                        node.Locked = false;
                    else
                    {
                        obj.Release();
                        obj.Deselect(node);
                        node.Locked = true;
                    }    
                }

                // to reset this specific node
                if (GUILayout.Button("R", "Button", GUILayout.Width(20)))
                {
                    obj.Reset(node);
                }

                // to add a task related to the game object of this node
                if (GUILayout.Button("Add Task", "Button", GUILayout.Width(100)))
                {
                    GenericMenu genericMenu = new GenericMenu();
                    for (int i = 0; i < MultiPartsObjectEditorUtility.TaskTypes().Length; i++)
                    {
                        genericMenu.AddItem(new GUIContent(MultiPartsObjectEditorUtility.TaskTypes()[i]), false,
                            (param) =>
                            {
                                int index = (int)param;
                                switch (index)
                                {
                                    case 0:
                                        {
                                            TaskList tl = obj.TaskList;
                                            tl.Tasks.Add(new MovingTask(tl,node.GameObject, obj.transform.InverseTransformPoint(node.GameObject.transform.position), Quaternion.Inverse(obj.transform.rotation) * node.GameObject.transform.rotation));
                                        }
                                        break;
                                    case 1:
                                        {
                                            TaskList tl = obj.TaskList;
                                            tl.Tasks.Add(new ClickingTask(tl,node.GameObject));
                                        }
                                        break;
                                }
                            }
                        , i);
                    }
                    genericMenu.ShowAsContext();
                }
                GUILayout.EndHorizontal();


                // node details
                if (node.Selected)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(indentLevel * 10);
                    GUILayout.BeginVertical(EditorStyles.helpBox);

                    // allows to change the name of the node
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Name", GUILayout.Width(100));
                    node.Name = GUILayout.TextField(node.Name);
                    GUILayout.EndHorizontal();

                    // allows to change the game object of the node
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("GO", GUILayout.Width(100));
                    node.GameObject = (GameObject)EditorGUILayout.ObjectField(node.GameObject, typeof(GameObject), true);
                    GUILayout.EndHorizontal();

                    // go name
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("GOName", GUILayout.Width(100));
                    node.GOName = GUILayout.TextField(node.GOName);
                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }

                // node childs
                if (node.Childs.Count > 0)
                {
                    indentLevel++;
                    GUILayout.BeginVertical();
                    foreach (var child in node.Childs)
                    {
                        //GUILayout.BeginHorizontal();
                        //GUILayout.Space(20);
                        DisplayNodeInfo(child);
                        //GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();
                    indentLevel--;
                }
            }
        }
    }
}
