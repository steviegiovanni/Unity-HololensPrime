// author: Stevie Giovanni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModelViewer
{
    // a task event that lets user specifies an initial and goal transform of a part in a multipartsobject
    public class AnimationTaskEvent : TaskEvent
    {
        private string _animatorParam;
        public string AnimatorParam
        {
            get { return _animatorParam; }
            set { _animatorParam = value; }
        }

        private float _animatorParamValue;
        public float AnimatorParamValue
        {
            get { return _animatorParamValue; }
            set { _animatorParamValue = value; }
        }

        private bool _modifyAnimatorParam;
        public bool ModifyAnimatorParam
        {
            get { return _modifyAnimatorParam; }
            set { _modifyAnimatorParam = value; }
        }

        private string _animatorLayer;
        public string AnimatorLayer
        {
            get { return _animatorLayer; }
            set { _animatorLayer = value; }
        }

        private float _animatorLayerAmount;
        public float AnimatorLayerAmount
        {
            get { return _animatorLayerAmount; }
            set { _animatorLayerAmount = value; }
        }

        private bool _modifyAnimatorLayer;
        public bool ModifyAnimatorLayer
        {
            get { return _modifyAnimatorLayer; }
            set { _modifyAnimatorLayer = value; }
        }

        private GameObject _gameObject;
        public GameObject GameObject
        {
            get { return _gameObject; }
            set { _gameObject = value; }
        }

        /// <summary>
        /// constructor that deserialize a serializable task event
        /// </summary>
        public AnimationTaskEvent(SerializableTaskEvent ste) : base(ste) {
            GameObject = ste.GameObject;
            AnimatorParam = ste.AnimatorParam;
            AnimatorParamValue = ste.AnimatorParamValue;
            ModifyAnimatorParam = ste.ModifyAnimatorParam;
            AnimatorLayer = ste.AnimatorLayer;
            AnimatorLayerAmount = ste.AnimatorLayerAmount;
            ModifyAnimatorLayer = ste.ModifyAnimatorLayer;
        }

        /// <summary>
        /// constructor
        /// </summary>
        public AnimationTaskEvent() : base() { }

        /// <summary>
        /// coroutine that animates the object associated from start to finish
        /// </summary>
        public override IEnumerator TaskEventCoroutine()
        {
            if (ModifyAnimatorParam)
            {
                GameObject.GetComponent<ModelController>().SetParam(AnimatorParam,AnimatorParamValue);
            }

            if (_modifyAnimatorLayer)
            {
                GameObject.GetComponent<ModelController>().SetLayer(AnimatorLayer, AnimatorLayerAmount);
            }

            yield return null;
        }
    }
}
