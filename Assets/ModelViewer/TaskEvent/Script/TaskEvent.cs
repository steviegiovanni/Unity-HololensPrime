// author: Stevie Giovanni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModelViewer
{
    /// <summary>
    /// represents an event associated with a task (e.g. rotate a part after user clicks it)
    /// </summary>
    public class TaskEvent 
    {
        /// <summary>
        /// the task that owns this taskevent
        /// </summary>
        private Task _task;
        public Task Task
        {
            get { return _task; }
            set { _task = value; }
        }

        /// <summary>
        /// constructor
        /// </summary>
        public TaskEvent() { }

        /// <summary>
        /// constructor that takes a serializable task event
        /// </summary>
        public TaskEvent(SerializableTaskEvent serializableTaskEvent) { }

        /// <summary>
        /// base coroutine. override to create different behaviors
        /// </summary>
        public virtual IEnumerator TaskEventCoroutine()
        {
            yield return null;
        }
    }
}
