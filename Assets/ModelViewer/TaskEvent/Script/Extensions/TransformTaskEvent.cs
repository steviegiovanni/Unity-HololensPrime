// author: Stevie Giovanni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModelViewer
{
    // a task event that lets user specifies an initial and goal transform of a part in a multipartsobject
    public class TransformTaskEvent : TaskEvent
    {
        /// <summary>
        /// starting position
        /// </summary>
        private Vector3 _startPos;
        public Vector3 StartPos
        {
            get { return _startPos; }
            set { _startPos = value; }
        }

        /// <summary>
        /// end position to animate to
        /// </summary>
        private Vector3 _endPos;
        public Vector3 EndPos
        {
            get { return _endPos; }
            set { _endPos = value; }
        }

        /// <summary>
        /// start rotation
        /// </summary>
        private Quaternion _startRotation;
        public Quaternion StartRotation
        {
            get { return _startRotation; }
            set { _startRotation = value; }
        }

        /// <summary>
        /// end rotation to animate to
        /// </summary>
        private Quaternion _endRotation;
        public Quaternion EndRotation
        {
            get { return _endRotation; }
            set { _endRotation = value; }
        }

        /// <summary>
        /// duration of the animation
        /// </summary>
        private float _duration = 3.0f;
        public float Duration
        {
            get { return _duration; }
            set { _duration = value; }
        }

        /// <summary>
        /// constructor that deserialize a serializable task event
        /// </summary>
        public TransformTaskEvent(SerializableTaskEvent ste) : base(ste) {
            StartPos = ste.StartPos;
            EndPos = ste.EndPos;
            StartRotation = ste.StartRotation;
            EndRotation = ste.EndRotation;
        }

        /// <summary>
        /// constructor
        /// </summary>
        public TransformTaskEvent() : base() { }

        /// <summary>
        /// coroutine that animates the object associated from start to finish
        /// </summary>
        public override IEnumerator TaskEventCoroutine()
        {
            // store all child start position and rotations because we only want to animate the associated part
            List<Vector3> childStartPositions = new List<Vector3>();
            List<Quaternion> childStartRotations = new List<Quaternion>();

            GameObject taskObj = Task.GameObject;
            foreach(Transform child in taskObj.transform)
            {
                childStartPositions.Add(child.position);
                childStartRotations.Add(child.rotation);
            }

            // interpolate slowly from start to end position and rotation, remember to reset all child transform to their original transforms
            float startTime = Time.time;
            float curTime = startTime;
            while(curTime - startTime < Duration)
            {
                curTime += Time.deltaTime;

                taskObj.transform.SetPositionAndRotation(taskObj.transform.position + (Task.TaskList.MPO.transform.TransformPoint(EndPos) - Task.TaskList.MPO.transform.TransformPoint(StartPos)) * Time.deltaTime / Duration, Quaternion.Lerp(Task.TaskList.MPO.transform.rotation * StartRotation, Task.TaskList.MPO.transform.rotation *EndRotation, (curTime - startTime) / Duration));

                for (int i = 0; i < taskObj.transform.childCount; i++)
                    taskObj.transform.GetChild(i).SetPositionAndRotation(childStartPositions[i],childStartRotations[i]);

                yield return null;
            }

            yield return null;
        }
    }
}
