// author: Stevie Giovanni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRUtilities
{
    /// <summary>
    /// Use this class to have an object track the transform of another object (head or hands of the player in VR)
    /// </summary>
    public class DeviceTracker : MonoBehaviour
    {
        /// <summary>
        /// the device being tracked
        /// </summary>
        private GameObject _device;

        /// <summary>
        /// name of the device to be tracked
        /// </summary>
        [SerializeField]
        private string _deviceName;
        public string DeviceName
        {
            get { return _deviceName; }
            set { _deviceName = value; }
        }

        /// <summary>
        /// call track device coroutine to start tracking device
        /// </summary>
        public void TrackDevice()
        {
            StartCoroutine(TrackDeviceCoroutine());
        }

        /// <summary>
        /// track the device position and orientation
        /// </summary>
        public IEnumerator TrackDeviceCoroutine()
        {
            while (true)
            {
                // try to find the device if null
                if (_device == null)
                {
                    _device = GameObject.Find(DeviceName);
                }

                // track the device if it exists
                if (_device != null)
                {
                    this.transform.SetPositionAndRotation(_device.transform.position, _device.transform.rotation);
                }

                yield return null;
            }
        }

        private void Start()
        {
            //TrackDevice();
        }
    }
}
