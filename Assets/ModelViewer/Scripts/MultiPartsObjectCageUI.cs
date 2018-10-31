// author: Stevie Giovanni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModelViewer
{
    /// <summary>
    /// UI class, adjust UI position and orientation to follow the multiparts object and rotate towards user
    /// </summary>
    public class MultiPartsObjectCageUI : MonoBehaviour
    {
        /// <summary>
        ///  the mpo associated with this ui
        /// </summary>
        [SerializeField]
        private MultiPartsObject _mpo;
        public MultiPartsObject MPO
        {
            get {
                if(_mpo == null)
                    _mpo = GameObject.FindObjectOfType<MultiPartsObject>();
                if (_mpo == null)
                    Debug.LogError("No MPO found");
                return _mpo;
            }
        }

        [SerializeField]
        private float _nearDistance = -0.5f;

        [SerializeField]
        private float _farDistance = 2.0f;

        [SerializeField]
        private GameObject _uiFrame;

        [SerializeField]
        private float _speed = 2.0f;

        private void Start()
        {
            _uiFrame.transform.position = new Vector3(0,0,_farDistance);
        }

        private void Update()
        {
            // position the UI so that it will always follow the mpo and at the user's head level
            if (Camera.main == null) return;
            Vector3 cameraPos = Camera.main.transform.position;
            Vector3 mpoPos = MPO.transform.position;
            this.transform.position = new Vector3(mpoPos.x,cameraPos.y,mpoPos.z);
            this.transform.LookAt(2*this.transform.position - cameraPos);
        }

        /// <summary>
        /// bring UI closer to the user
        /// </summary>
        public IEnumerator BringUICloserCoroutine()
        {
            while(_uiFrame.transform.localPosition.z > _nearDistance)
            {
                _uiFrame.transform.localPosition = new Vector3(_uiFrame.transform.localPosition.x, _uiFrame.transform.localPosition.y, _uiFrame.transform.localPosition.z - Time.deltaTime * _speed);
                yield return null;
            }
            yield return null;
        }

        /// <summary>
        /// push UI to the back of the user
        /// </summary>
        public IEnumerator BringUIFurtherCoroutine()
        {
            while (_uiFrame.transform.localPosition.z < _farDistance)
            {
                _uiFrame.transform.localPosition = new Vector3(_uiFrame.transform.localPosition.x, _uiFrame.transform.localPosition.y, _uiFrame.transform.localPosition.z + Time.deltaTime * _speed);
                yield return null;
            }
            yield return null;
        }

        public void BringUICloser()
        {
            this.StopAllCoroutines();
            StartCoroutine(BringUICloserCoroutine());
        }

        public void BringUIFurther()
        {
            this.StopAllCoroutines();
            StartCoroutine(BringUIFurtherCoroutine());
        }
    }
}
