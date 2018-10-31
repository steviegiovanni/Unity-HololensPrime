/// author: Stevie Giovanni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ModelViewer
{

    /// <summary>
    /// interface for everything that uses a ray
    /// implements this interface for different devices (e.g. hololens, vive)
    /// </summary>
    public interface IHasRay
    {
        /// <summary>
        /// get the ray component that will used as the pointer
        /// </summary>
        Ray GetRay();
    }

    /// <summary>
    /// Object pointer returns the hitinfo of a specified ray in the simulation
    /// </summary>
    public class ObjectPointer : MonoBehaviour
    {
        /// <summary>
        /// the instance of an object pointer
        /// </summary>
        static private ObjectPointer _instance = null;
        static public ObjectPointer Instance
        {
            get
            {
                if (_instance == null)
                    _instance = GameObject.FindObjectOfType<ObjectPointer>();

                if (_instance == null)
                    Debug.LogError("No ObjectPointer found. Don't forget to put an ObjectPointer into the scene");

                return _instance;
            }
        }

        /// <summary>
        /// the ray used to point to object
        /// need to be assigned accordingly whether u're using the ray from hololens' headset, vive's controller etc...
        /// </summary>
        [SerializeField]
        private GameObject _ray;
        private IHasRay _iray;
        public IHasRay Ray
        {
            get { return _iray; }
            set { _iray = value; }
        }

        /// <summary>
        /// line renderer to show ray
        /// </summary>
        private LineRenderer _lr;
        public LineRenderer LR
        {
            get
            {
                if (_lr == null)
                    _lr = GetComponent<LineRenderer>();
                if (_lr == null)
                    Debug.LogError("No line renderer attached. Don't forget to attach a line renderer to the object.");
                return _lr;
            }
        }

        /// <summary>
        /// show the ray or not
        /// </summary>
        [SerializeField]
        private bool _rayVisible;
        public bool RayVisible
        {
            get { return _rayVisible; }
            set { _rayVisible = value; }
        }

        /// <summary>
        /// check ray whenever we modify the ray field
        /// </summary>
        void OnValidate()
        {
            CheckRay();
        }

        /// <summary>
        /// check whether we have a valid ray object 
        /// </summary>
        void CheckRay()
        {
            if (_ray != null)
            {
                _iray = _ray.GetComponent<IHasRay>();
                if (_iray == null) _ray = null;
            }
        }

        /// <summary>
        /// the hit info
        /// </summary>
        private RaycastHit _hitInfo;
        public RaycastHit HitInfo
        {
            get { return _hitInfo; }
        }

        /// <summary>
        /// events fired when object pointer is gazing 
        /// </summary>
        public class GazeEvent : UnityEvent<GameObject> { }
        private GazeEvent _onHoverEvent;
        public GazeEvent OnHoverEvent
        {
            get
            {
                if (_onHoverEvent == null)
                    _onHoverEvent = new GazeEvent();
                return _onHoverEvent;
            }
        }

        private void Start()
        {
            CheckRay();
        }

        // Update is called once per frame
        void Update()
        {
            // use default ray with no dir if there's no HasRay object
            Ray ray;
            if (Ray == null)
            {
                ray = new Ray(Vector3.zero, Vector3.zero);
            }
            else
                ray = Ray.GetRay();

            // raycast to get hitInfo
            if (Physics.Raycast(ray, out _hitInfo, Mathf.Infinity, Physics.DefaultRaycastLayers))
            {
                if (OnHoverEvent != null)
                    OnHoverEvent.Invoke(_hitInfo.collider.gameObject);
            }
            else
            {
                if (OnHoverEvent != null)
                    OnHoverEvent.Invoke(null);
            }

            // draw ray if there's a line renderer and ray is visible
            if (LR != null)
            {
                if (RayVisible)
                {
                    LR.SetPosition(0, ray.origin);
                    if (HitInfo.collider != null)
                        LR.SetPosition(1, HitInfo.point);
                    else
                        LR.SetPosition(1, ray.origin + ray.direction * 100f);
                }
                else
                {
                    LR.enabled = false;
                }
            }
        }
    }

}