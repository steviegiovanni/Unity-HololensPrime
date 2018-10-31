/// author: Stevie Giovanni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ModelViewer
{
    /// <summary>
    /// a node represents an element in a multipartsobject
    /// </summary>
    public class Node
    {
        /// <summary>
        /// store the name of the game object so that we can search for the go based on name if we switch the model
        /// </summary>
        private string _goName;
        public string GOName
        {
            get { return _goName; }
            set { _goName = value; }
        }

        /// <summary>
        /// the game object associated with this node
        /// </summary>
        private GameObject _gameObject;
        public GameObject GameObject
        {
            get { return _gameObject; }
            set { _gameObject = value; }
        }

        /// <summary>
        /// whether this node has a mesh or only a placeholder transform
        /// </summary>
        private bool _hasMesh = false;
        public bool HasMesh
        {
            get { return _hasMesh; }
            set { _hasMesh = value; }
        }

        /// <summary>
        /// the parent node of the node
        /// </summary>
        private Node _parent;
        public Node Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        /// <summary>
        /// childs of the node
        /// </summary>
        private List<Node> _childs;
        public List<Node> Childs
        {
            get
            {
                if (_childs == null)
                    _childs = new List<Node>();
                return _childs;
            }
            set { _childs = value; }
        }

        /// <summary>
        /// the original position of the node
        /// </summary>
        private Vector3 _p0;
        public Vector3 P0
        {
            get { return _p0; }
            set { _p0 = value; }
        }

        /// <summary>
        /// the original rotation of the node
        /// </summary>
        private Quaternion _r0;
        public Quaternion R0
        {
            get { return _r0; }
            set { _r0 = value; }
        }

        /// <summary>
        /// the original scale of the node
        /// </summary>
        private Vector3 _s0;
        public Vector3 S0
        {
            get { return _s0; }
            set { _s0 = value; }
        }

        /// <summary>
        /// the starting bounds of the mesh on the node
        /// </summary>
        private Bounds _bounds;
        public Bounds Bounds
        {
            get { return _bounds; }
            set { _bounds = value; }
        }

        /// <summary>
        /// whether the node is selected or not
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// original material of the geometry in the node
        /// </summary>
        private Material _material;
        public Material Material
        {
            get { return _material; }
            set { _material = value; }
        }

        /// <summary>
        /// whether the object is interactable or not
        /// </summary>
        private bool _locked;
        public bool Locked
        {
            get { return _locked; }
            set { _locked = value; }
        }

        /// <summary>
        /// given name of the node (default to gameobject's name)
        /// </summary>
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

		/// <summary>
		/// constructor that takes a go and a parent node
		/// </summary>
		public Node(GameObject go, Node parent, Transform cage)
		{
			// assign parent
			Parent = parent;

			// assign game object and game object name
			GameObject = go;
            GOName = GameObject.name;
            Name = go.name; // set default node name to the game object name

            // get the original transforms, position relative to the cage
            P0 = cage.InverseTransformPoint(go.transform.position);
            R0 = Quaternion.Inverse(cage.rotation) * go.transform.rotation;
            S0 = go.transform.localScale;

			// check whether game object has a mesh
			HasMesh = go.GetComponent<MeshFilter>() != null;

			// get starting bounds
			if (HasMesh)
				Bounds = go.GetComponent<Renderer>().bounds;
			else
				Bounds = new Bounds(go.transform.position, Vector3.zero);

			// get original material
			if (HasMesh)
				Material = go.GetComponent<Renderer>().sharedMaterial;
			else
				Material = null;

            // initialized locked to true (can't be interacted with)
            Locked = true;

			// add collider if doesn't exist
			if (HasMesh && go.GetComponent<Collider>() == null)
				go.AddComponent<MeshCollider>();

			// check childs and do recursive node reconstruction
			foreach (Transform child in go.transform){
				Childs.Add(new Node(child.gameObject, this,cage));
			}
		}

        /// <summary>
        /// constructor taking a serializable node
        /// </summary>
        public Node(SerializableNode sn)
        {
            GameObject = sn.GameObject;
            HasMesh = sn.HasMesh;
            P0 = sn.P0;
            R0 = sn.R0;
            S0 = sn.S0;
            Bounds = sn.Bounds;
            Material = sn.Material;
            Locked = sn.Locked;
            Name = sn.Name;
            GOName = sn.GOName;
        }

        /// <summary>
        /// return the cumulative bounds of a node and its childs
        /// </summary>
        public Bounds GetCumulativeBounds()
        {
            if (Childs.Count == 0)
                return Bounds;
            else
            {
                Bounds b = Bounds;
                foreach (var child in Childs)
                    b.Encapsulate(child.GetCumulativeBounds());
                return b;
            }
        }
    }

    /// <summary>
    /// structure used to store node data for serialization
    /// </summary>
    [System.Serializable]
    public struct SerializableNode
    {
        public int ChildCount;
        public int IndexOfFirstChild;
        public GameObject GameObject;
        public bool HasMesh;
        public int indexOfParent;
        public Vector3 P0;
        public Quaternion R0;
        public Vector3 S0;
        public Bounds Bounds;
        public Material Material;
        public bool Locked;
        public string Name;
        public string GOName;
    }

    /// <summary>
    /// a multi parts object is an object that consists of several different parts
    /// that can be detached and assembled back again
    /// </summary>
    public class MultiPartsObject : MonoBehaviour, ISerializationCallbackReceiver
    {
        /// <summary>
        /// the root node
        /// </summary>
        private Node _root;
        public Node Root
        {
            get { return _root; }
            set { _root = value; }
        }

        /// <summary>
        /// mapping between a game object and a node
        /// </summary>
        private Dictionary<GameObject, Node> _dict;
        public Dictionary<GameObject, Node> Dict
        {
            get
            {
                if (_dict == null)
                {
                    _dict = new Dictionary<GameObject, Node>();
                }
                return _dict;
            }
        }

        /// <summary>
        /// the desired virtual size of the object
        /// </summary>
        [Range(0.0f, 10.0f)]
        [SerializeField]
        private float _virtualScale = 1.0f;
        public float VirtualScale
        {
            get { return _virtualScale; }
            set { _virtualScale = value; }
        }

        /// <summary>
        /// output current scale of object to fit the virtual scale
        /// </summary>
        [SerializeField]
        private float _currentScale;
        public float CurrentScale
        {
            get { return _currentScale; }
            private set { _currentScale = value; }
        }

        /// <summary>
        /// list of currently selected nodes
        /// </summary>
        private List<Node> _selectedNodes;
        public List<Node> SelectedNodes
        {
            get
            {
                if (_selectedNodes == null)
                    _selectedNodes = new List<Node>();
                return _selectedNodes;
            }
        }

        /// <summary>
        /// highlight material for selected nodes
        /// </summary>
        [SerializeField]
        private Material _onSelectedMaterial;
        public Material OnSelectedMaterial
        {
            get { return _onSelectedMaterial; }
            set { _onSelectedMaterial = value; }
        }

		/// <summary>
		/// silhouette material (to be used when generating a silhouette of the original object)
		/// </summary>
		[SerializeField]
		private Material _silhouetteMaterial;
		public Material SilhouetteMaterial
		{
			get { return _silhouetteMaterial; }
			set { _silhouetteMaterial = value; }
		}

        /// <summary>
        /// whether we show the silhouette or not
        /// </summary>
        [SerializeField]
        private bool _showSilhouette = true;
        public bool ShowSilhouette
        {
            get {
                return _showSilhouette;
            }
            set {
                _showSilhouette = value;
                if (_showSilhouette == true)
                {
                    if (_silhouette == null)
                        SetupSilhouette();
                }
                if (_silhouette != null)
                    _silhouette.SetActive(_showSilhouette);
            }
        } 

        /// <summary>
        /// the frame attached to the controller frame (could be the head for gazing)
		/// this frame will be used to contain all the selected nodes when grabbed
        /// </summary>
        [SerializeField]
        private GameObject _movableFrame;
        public GameObject MovableFrame
        {
            get { return _movableFrame; }
        }

        /// <summary>
        /// whether we should snap or not after we release a node
        /// </summary>
        [SerializeField]
        private bool _snap = true;
        public bool SnapParts
        {
            get { return _snap; }
            set { _snap = value; }
        }
			
		/// <summary>
		/// The snap threshold
		/// </summary>
		[Range(0.0f, 1.0f)]
		[SerializeField]
		private float _snapThreshold = 0.1f;
		public float SnapThreshold{
			get{ return _snapThreshold;}
			set{ _snapThreshold = value;}
		}

		/// <summary>
		/// indicate whether to deselect selected node when they snap
		/// </summary>
		[SerializeField]
		private bool _deselectOnSnapped = true;
		public bool DeselectOnSnapped{
			get{ return _deselectOnSnapped;}
			set{ _deselectOnSnapped = value;}
		}

		/// <summary>
		/// container for temporary runtime instantiated silhouette object
		/// </summary>
		private GameObject _silhouette = null;

        /// <summary>
        /// the task list component
        /// </summary>
        [SerializeField]
        private TaskList _taskList;
        public TaskList TaskList
        {
            get {
                _taskList = GetComponent<TaskList>();
                if (_taskList == null)
                    _taskList = this.gameObject.AddComponent<TaskList>();
                return _taskList;
            }
        }

        /// <summary>
        /// events fired from MultiPartsObject when input such as select, grab, or released are received
        /// </summary>
        public class MPOEvent: UnityEvent<Node> { };

        /// <summary>
        /// event fired when release input is received
        /// </summary>
        private MPOEvent _onReleaseEvent;
        public MPOEvent OnReleaseEvent
        {
            get {
                if (_onReleaseEvent == null)
                    _onReleaseEvent = new MPOEvent();
                return _onReleaseEvent;
            }
        }

        /// <summary>
        /// event fired when select input is received
        /// </summary>
        private MPOEvent _onSelectEvent;
        public MPOEvent OnSelectEvent
        {
            get
            {
                if (_onSelectEvent == null)
                    _onSelectEvent = new MPOEvent();
                return _onSelectEvent;
            }
        }

        void Awake()
        {
            // check movable frame exists
            if (MovableFrame == null)
                Debug.LogWarning("no movable frame assigned. will not be able to move objects around.");

            // construct dictionary on awake as we're not serializing the dictionary
            ConstructDictionary();

            // setup default silhouette if we don't have a task list
            if(ShowSilhouette)
                SetupSilhouette();
        }

		/// <summary>
		/// Setups the silhouette (hints for where all the parts should be placed)
		/// </summary>
		public void SetupSilhouette(){
			// destry previous silhouette
			if (_silhouette != null)
				Destroy (_silhouette);

			// create temporary go
			_silhouette = new GameObject ("Silhouette");

			// sync transform with cage's transform
			_silhouette.transform.SetPositionAndRotation (this.transform.position, this.transform.rotation);
			_silhouette.transform.localScale = this.transform.localScale;

			// if setup (root is not null), call setup silhouette recursively
			if (Root != null) 
                SetupSilhouetteRecursive(Root, _silhouette);
		}

        /// <summary>
        /// setup silhouette recursively node by node
        /// </summary>
        public void SetupSilhouetteRecursive(Node node, GameObject parent)
        {
            GameObject copy = new GameObject(node.GameObject.name);
            copy.transform.SetPositionAndRotation(this.transform.TransformPoint(node.P0), this.transform.rotation * node.R0);
            copy.transform.localScale = node.GameObject.transform.lossyScale;
            if (node.GameObject.GetComponent<MeshFilter>() != null)
            {
                copy.AddComponent<MeshFilter>();
                copy.GetComponent<MeshFilter>().sharedMesh = node.GameObject.GetComponent<MeshFilter>().sharedMesh;
            }
            if (node.GameObject.GetComponent<Renderer>() != null)
            {
                copy.AddComponent<MeshRenderer>();
                copy.GetComponent<MeshRenderer>().material = SilhouetteMaterial;
            }
            copy.transform.SetParent(parent.transform);
            
            foreach(var child in node.Childs)
                SetupSilhouetteRecursive(child, copy);
        }

        // Update is called once per frame
        void Update()
        {
            // Update the transform of silhouette based on where the cage transform
            if (_silhouette != null)
                _silhouette.transform.SetPositionAndRotation(this.transform.position, this.transform.rotation);
            if (Input.GetKeyUp(KeyCode.Space))
                ShowSilhouette = !ShowSilhouette;
        }

        /// <summary>
        /// setup internal node structure, called from the editor
        /// </summary>
        public void Setup()
        {
			// clear selected nodes
            SelectedNodes.Clear();

            // if there's no child object, clear dictionary and null root
            if (this.gameObject.transform.childCount == 0)
            {
                Root = null;
                return;
            }
            else // construct internal data structure and dictionary if there's a loaded object
            {
                // initialize root
				Root = new Node(this.transform.GetChild(0).gameObject, null,this.transform);
            }
        }

        /// <summary>
        /// construct dictionary for easy mapping between a gameobject and a node
        /// </summary>
        public void ConstructDictionary()
        {
            Dict.Clear();
            if (Root == null) return;
            List<Node> curNodes = new List<Node>();
            curNodes.Add(Root);
            while (curNodes.Count > 0)
            {
                Node curNode = curNodes[0];
                Dict.Add(curNode.GameObject, curNode);
                foreach (var child in curNode.Childs)
                    curNodes.Add(child);
                curNodes.RemoveAt(0);
            }
        }

        /// <summary>
        /// resize the cage to fit the object to a scale
        /// </summary>
        public void FitToScale(Node node, float scale)
        {
            if (node != null)
            {
                Bounds b = node.GetCumulativeBounds();
                float scaleFactor = scale / b.size.magnitude;
                CurrentScale = scaleFactor;
				this.transform.localScale = Vector3.one * scaleFactor;
            }
        }

        /// <summary>
        /// use the node's game object name to find the game object associated to a node when it's missing
        /// </summary>
        public void FindMissingObjectsByName(Node node)
        {
            if (node == null) return;
            if (node.GameObject == null)
                node.GameObject = GameObject.Find(node.GOName);

            foreach (var child in node.Childs)
                FindMissingObjectsByName(child);
        }

        /// <summary>
        /// recapture the names of the game object associated to each node
        /// </summary>
        public void CaptureGameObjectName(Node node)
        {
            if (node == null) return;
            if (node.GameObject != null)
                node.GOName = node.GameObject.name;

            foreach (var child in node.Childs)
                CaptureGameObjectName(child);
        }

        /// <summary>
        /// reset transform of a node and its children recursively
        /// </summary>
        public void ResetAll(Node node)
        {
            CurrentScale = 1.0f;
            if (node != null)
            {
				node.GameObject.transform.SetPositionAndRotation(this.transform.TransformPoint(node.P0), this.transform.rotation * node.R0);
                node.GameObject.transform.localScale = node.S0;
                foreach (var child in node.Childs)
                    ResetAll(child);
            }
        }

        /// <summary>
        /// reset transform of specific node only
        /// </summary>
        public void Reset(Node node)
        {
           if(node != null)
            {
                // store all children position, rotations and scales
                List<Vector3> childrenPositions = new List<Vector3>();
                List<Quaternion> childrenRotations = new List<Quaternion>();
                List<Vector3> childrenScales = new List<Vector3>();
                foreach(var child in node.Childs)
                {
                    childrenPositions.Add(child.GameObject.transform.position);
                    childrenRotations.Add(child.GameObject.transform.rotation);
                    childrenScales.Add(child.GameObject.transform.lossyScale);
                }

                // reset current node transform
                node.GameObject.transform.SetPositionAndRotation(this.transform.TransformPoint(node.P0), this.transform.rotation * node.R0);
                node.GameObject.transform.localScale = node.S0;

                // restore all children positions, rotations, and scales
                int i = 0;
                foreach (var child in node.Childs)
                {
                    child.GameObject.transform.SetPositionAndRotation(childrenPositions[i], childrenRotations[i]);
                    child.GameObject.transform.localScale = childrenScales[i];
                    i++;
                }
            }
        }

        /// <summary>
        /// select a node that contains the game object pointed by the objectpointer
        /// </summary>
        public void Select()
        {
            if (ObjectPointer.Instance.HitInfo.collider != null)
                Select(ObjectPointer.Instance.HitInfo.collider.gameObject);
        }

        /// <summary>
        /// select a node that contains the specified game object
        /// </summary>
        public void Select(GameObject go)
        {
            Node selectedNode = null;
            if (Dict.TryGetValue(go, out selectedNode))
                Select(selectedNode);
        }

        /// <summary>
        /// select the specified node
        /// </summary>
        public void Select(Node node)
        {
            if (node.Locked) return;
            node.Selected = true;
            if (node.HasMesh)
                node.GameObject.GetComponent<Renderer>().material = new Material(OnSelectedMaterial);
            if (!SelectedNodes.Contains(node))
                SelectedNodes.Add(node);
            if (OnSelectEvent != null)
                OnSelectEvent.Invoke(node);
        }

        /// <summary>
        /// deselect a node that contains the game object pointed by the objectpointer
        /// </summary>
        public void Deselect()
        {
            if (ObjectPointer.Instance.HitInfo.collider != null)
                Deselect(ObjectPointer.Instance.HitInfo.collider.gameObject);
        }

        /// <summary>
        /// deselect a node that contains the specified game object
        /// </summary>
        public void Deselect(GameObject go)
        {
            Node deselectedNode = null;
            if (Dict.TryGetValue(go, out deselectedNode))
                Deselect(deselectedNode);
        }

        /// <summary>
        /// Deselect the specified node.
        /// </summary>
        public void Deselect(Node node)
        {
            //if (node.Locked) return;
            node.Selected = false;
            if (node.HasMesh)
            {
                node.GameObject.GetComponent<Renderer>().material = node.Material;
            }
            SelectedNodes.Remove(node);
        }

        /// <summary>
        /// toggle select a node that contains the game object pointed by the objectpointer
        /// </summary>
        public void ToggleSelect()
        {
            if (ObjectPointer.Instance.HitInfo.collider != null)
                ToggleSelect(ObjectPointer.Instance.HitInfo.collider.gameObject);
        }

        /// <summary>
        /// Toggle select a node that contains this game object
        /// </summary>
        public void ToggleSelect(GameObject go)
        {
            Node node = null;
            if (Dict.TryGetValue(go, out node))
                ToggleSelect(node);
        }

        /// <summary>
        /// Toggles select the specified node
        /// </summary>
        public void ToggleSelect(Node node)
        {
            if (node.Selected)
                Deselect(node);
            else
                Select(node);
        }

        /// <summary>
        /// grab all selected object and put them under movable frame
        /// </summary>
        public void Grab()
        {
            if (MovableFrame != null)
            {
                foreach (var obj in SelectedNodes)
                {
                    if (obj.Childs.Count > 0)
                    {
                        foreach (var child in obj.Childs)
                            if (child.GameObject.transform.parent.gameObject == child.Parent.GameObject)
                                child.GameObject.transform.SetParent(this.transform);
                    }
                    obj.GameObject.transform.SetParent(MovableFrame.transform);
                }
            }
        }

        /// <summary>
        /// call grab if pointer is pointing at a gameobject that is one of the selected node
        /// </summary>
        public void GrabIfPointingAt()
        {
            if (ObjectPointer.Instance.HitInfo.collider != null)
            {
                GameObject hitObject = ObjectPointer.Instance.HitInfo.collider.gameObject;
                Node hitNode = null;
                if (Dict.TryGetValue(hitObject, out hitNode))
                {
					if (SelectedNodes.Contains (hitNode)) {
						MovableFrame.transform.position = ObjectPointer.Instance.HitInfo.point;
						Grab ();
					}
                }
            }
        }

        /// <summary>
        /// release all selected object and put them back to their original structure
        /// </summary>
        public void Release()
        {
			Node[] selectedArray = SelectedNodes.ToArray ();
			for (int i = 0; i < selectedArray.Length; i++) {
                /*if (selectedArray[i].OnReleaseEvent != null)
                {
                    SelectedNodes[i].OnReleaseEvent.Invoke();
                }*/
                if (OnReleaseEvent != null)
                    OnReleaseEvent.Invoke(selectedArray[i]);

				if (selectedArray [i].Childs.Count > 0) {
					foreach (var child in selectedArray[i].Childs)
						child.GameObject.transform.SetParent (selectedArray [i].GameObject.transform);
				}

				if (selectedArray [i].Parent == null)
					selectedArray [i].GameObject.transform.SetParent (this.transform);
				else
					selectedArray [i].GameObject.transform.SetParent (selectedArray [i].Parent.GameObject.transform);

                if(SnapParts)
				    Snap (selectedArray [i]);
			}
        }

        /// <summary>
        /// grab the entire cage of the multi partsobject
        /// </summary>
        public void GrabCage()
        {
            if (MovableFrame != null)
            {
                this.transform.SetParent(MovableFrame.transform);
            }
        }

        /// <summary>
        /// release the entire cage of the multi parts object
        /// </summary>
        public void ReleaseCage()
        {
            this.transform.SetParent(null);
        }

		/// <summary>
		/// Snap the specified node to its original position in the cage if it's close enough
		/// </summary>
		public void Snap(Node node){
			//Vector3 oriPosRelativeToCage = node.P0 - CagePos;
			Vector3 oriPosAfterSetup = this.transform.TransformPoint(node.P0);
			if (Vector3.Distance (node.GameObject.transform.position,oriPosAfterSetup) < SnapThreshold) {

                List<Vector3> childPos = new List<Vector3>();
                List<Quaternion> childRot = new List<Quaternion>();
                foreach (Transform child in node.GameObject.transform)
                {
                    childPos.Add(child.position);
                    childRot.Add(child.rotation);
                }


                node.GameObject.transform.SetPositionAndRotation(oriPosAfterSetup, this.transform.rotation * node.R0);

                int i = 0;
                foreach (Transform child in node.GameObject.transform)
                {
                    child.SetPositionAndRotation(childPos[i], childRot[i]);
                    i++;
                }

                if (DeselectOnSnapped)
                {
                    Deselect(node);
                }
			}
		}

        /// <summary>
        /// change the lock of all the nodes
        /// </summary>
        public void SetNodeLockRecursive(Node root, bool value)
        {
            if(root != null)
            {
                root.Locked = value;
                foreach (var child in root.Childs)
                    SetNodeLockRecursive(child, value);
            }
        }


        /// ==================================================
        /// Gizmo stuff
        /// ==================================================
        public void OnDrawGizmos()
        {
            if (Root != null)
            {
                // draw gizmos for each node in the hierarchy
                DrawNode(Root);

                // draw root cumulative bounding box
                /*Bounds b = Root.GetCumulativeBounds();

                // draw bounding sphere
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(Vector3.zero,b.extents.magnitude * CurrentScale);*/
            }
        }

        public void DrawNode(Node node)
        {
            if (node != null)
            {
                if (node.GameObject != null)
                {
                    // draw origin
                    //Gizmos.color = Color.red;
                    //Gizmos.DrawSphere(node.GameObject.transform.position, 0.01f);

                    // draw bounding box if any
                    if (node.HasMesh && node.Selected)
                    {
                        Gizmos.color = Color.green;
                        Renderer rend = node.GameObject.GetComponent<Renderer>();
                        Mesh mesh = node.GameObject.GetComponent<MeshFilter>().sharedMesh;
                        Bounds bounds = rend.bounds;
                        //Bounds bounds = mesh.bounds;
                        Gizmos.DrawWireCube(bounds.center, bounds.extents * 2);
                        //Gizmos.DrawWireSphere(rend.bounds.center,rend.bounds.extents.magnitude);
                    }

                    if (node.HasMesh)
                    {
                        Color col = Color.cyan;
                        col.a = 0.25f;
                        Gizmos.color = col;
                        Gizmos.DrawMesh(node.GameObject.GetComponent<MeshFilter>().sharedMesh, this.transform.TransformPoint(node.P0), this.transform.rotation * node.R0 , node.GameObject.transform.lossyScale);
                    }
                }

                // call draw for each child
                foreach (var child in node.Childs)
                {
                    DrawNode(child);
                }
            }
        }

        /// ==================================================
        /// Serializing the node structure
        /// ==================================================

        /// <summary>
        /// the serialized node structure
        /// </summary>
        [HideInInspector]
        public List<SerializableNode> serializedNodes;

        /// <summary>
        /// add serialized nodes into the list of serialized nodes
        /// </summary>
        void AddNodesToSerializedNodesBFS()
        {
            if (Root == null) return;

            List<Node> toProcess = new List<Node>();
            toProcess.Add(Root);

            var serializedRoot = new SerializableNode()
            {
                ChildCount = Root.Childs.Count,
                IndexOfFirstChild = 1,
                GameObject = Root.GameObject,
                HasMesh = Root.HasMesh,
                indexOfParent = -1,
                P0 = Root.P0,
                R0 = Root.R0,
                S0 = Root.S0,
                Bounds = Root.Bounds,
                Material = Root.Material,
                Locked = Root.Locked,
                Name = Root.Name,
                GOName = Root.GOName
            };
            serializedNodes.Add(serializedRoot);

            int parentId = 0;
            while(toProcess.Count > 0)
            {
                Node n = toProcess[0];
                int nCousins = 0;
                int childid = 0;
                foreach(var child in n.Childs)
                {
                    var serializedNode = new SerializableNode()
                    {
                        ChildCount = child.Childs.Count,
                        IndexOfFirstChild = serializedNodes.Count + n.Childs.Count - childid + nCousins,
                        GameObject = child.GameObject,
                        HasMesh = child.HasMesh,
                        indexOfParent = parentId,
                        P0 = child.P0,
                        R0 = child.R0,
                        S0 = child.S0,
                        Bounds = child.Bounds,
                        Material = child.Material,
                        Locked = child.Locked,
                        Name = child.Name,
                        GOName = child.GOName
                    };
                    serializedNodes.Add(serializedNode);
                    nCousins += child.Childs.Count;
                    childid++;
                    toProcess.Add(child);
                }

                toProcess.RemoveAt(0);
                parentId++;
            }
        }

        /// <summary>
        /// serialization interface implementation
        /// </summary>
        public void OnBeforeSerialize()
        {
            if (serializedNodes == null) serializedNodes = new List<SerializableNode>();
            serializedNodes.Clear();
            //AddNodeToSerializedNodes(Root,-1);
            AddNodesToSerializedNodesBFS();
        }

        /// <summary>
        /// create a new node from a serialized node index
        /// </summary>
        Node ReadNodeFromSerializedNodes(int index, Node parent)
        {
            if (index < 0)
                return null;

            var serializedNode = serializedNodes[index];
            Node node = new Node(serializedNode);
            node.Parent = parent;
            var children = new List<Node>();
            for (int i = 0; i != serializedNode.ChildCount; i++)
                children.Add(ReadNodeFromSerializedNodes(serializedNode.IndexOfFirstChild + i,node));
            node.Childs = children;
            return node;
        }

        /// <summary>
        /// deserialization interface implementation
        /// </summary>
        public void OnAfterDeserialize()
        {
            if (serializedNodes.Count > 0)
            {
                Root = ReadNodeFromSerializedNodes(0, null);
            }
            else
                Root = null;
        }
    }
}