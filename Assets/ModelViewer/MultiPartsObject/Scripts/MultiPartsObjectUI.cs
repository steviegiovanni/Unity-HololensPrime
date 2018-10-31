/// author: Stevie Giovanni

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModelViewer
{
    /// <summary>
    /// simple UI, listens to on hover node event and shows the part name in front of the user 
    /// </summary>
    public class MultiPartsObjectUI : MonoBehaviour
    {
        /// <summary>
        /// the text mesh that will show the part's name
        /// </summary>
        [SerializeField]
        private TextMesh _partName;
        public TextMesh PartName
        {
            get { return _partName; }
            set { _partName = value; }
        }

        /// <summary>
        /// the multipartsobject we're going to query this object to get the node info once we get a hover event
        /// </summary>
        [SerializeField]
        private MultiPartsObject _mpo;

        // Use this for initialization
        void Start()
        {
            // listens to hover event of the object pointer
            //ObjectPointer.Instance.OnHoverEvent.AddListener(OnHoverEvent);
            StartCoroutine(ListensToObjectPointerHover());
        }

        public IEnumerator ListensToObjectPointerHover()
        {
            while (ObjectPointer.Instance == null)
                yield return null;
            ObjectPointer.Instance.OnHoverEvent.AddListener(OnHoverEvent);
            yield return null;
        }

        /// <summary>
        /// when we received the on hover event we query the multiparts object for the node info
        /// </summary>
        void OnHoverEvent(GameObject hovered)
        {
            if (hovered == null)
            {
                if (PartName != null)
                    PartName.gameObject.SetActive(false);
            }
            else
            {
                if(PartName != null &&  _mpo != null)
                {
                    Node nodeInfo = null;
                    if(_mpo.Dict.TryGetValue(hovered,out nodeInfo)) {
                        PartName.text = nodeInfo.Name;
                        PartName.gameObject.SetActive(true);
                    }
                }
            }
        }
    }
}
