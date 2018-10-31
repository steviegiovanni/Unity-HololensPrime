using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;
using UnityEngine.XR.WSA.Persistence;
using UnityEngine.XR.WSA.Sharing;

public class AnchorManager : Photon.PunBehaviour
{
    WorldAnchorStore store;
    bool initializingAnchorStore = false;
    bool anchorLoaded = false;
    public GameObject anchored;
    public TextMesh log;
    public GameObject SetAnchorBtn;
    public GameObject ShareAnchorBtn;
    public GameObject AnchorSharingUI;
    public GameObject AnchorGizmo;
    bool movingAnchor = false;

    private void Start()
    {
        AnchorSharingUI.SetActive(false);
    }


    // Update is called once per frame
    void Update () {
        if (!PhotonNetwork.connected) return;

        if (!initializingAnchorStore && store == null)
        {
            initializingAnchorStore = true;
            WorldAnchorStore.GetAsync(AnchorStoreLoaded);
        }

        if (store == null) return;

        AnchorSharingUI.SetActive(true);

        if (!anchorLoaded)
        {
            anchorLoaded = store.Load("anchor", anchored);
            if (!anchorLoaded)
            {
                //Debug.Log("No anchor has been set");
                log.text = "No anchor has been set";
            }
            else
            {
                //Debug.Log("There's an anchor set");
                log.text = "Anchor loaded";
            }
        }

        if (PhotonNetwork.isMasterClient)
        {
            SetAnchorBtn.SetActive(true);
            ShareAnchorBtn.SetActive(true);

            if (movingAnchor)
                AnchorSharingUI.SetActive(false);
            else
                AnchorSharingUI.SetActive(true);
        }
        else
        {
            SetAnchorBtn.SetActive(false);
            ShareAnchorBtn.SetActive(false);
        }

        progressBarFG.transform.localScale = new Vector3(0.1f * curLength / lengthOfData, 0.01f, 0.001f);
    }

    public void SetAnchorRoutine()
    {
        movingAnchor = true;
        AnchorGizmo.SetActive(true);
    }

    public void SetAnchor()
    {
        movingAnchor = false;
        AnchorGizmo.SetActive(false);
        WorldAnchor wa = anchored.GetComponent<WorldAnchor>();
        if (wa != null)
            Destroy(wa);
        anchored.transform.SetPositionAndRotation(AnchorGizmo.transform.position, AnchorGizmo.transform.rotation);
        anchored.AddComponent<WorldAnchor>();
        anchorLoaded = store.Save("anchor",anchored.GetComponent<WorldAnchor>());
    }

    private void AnchorStoreLoaded(WorldAnchorStore store)
    {
        this.store = store;
        initializingAnchorStore = false;
    }

    public byte[] dataToSend;
    public byte[] dataReceived;
    List<byte[]> bytesToSend;
    List<byte[]> bytesReceived;
    public GameObject progressBar;
    public GameObject progressBarFG;
    public int lengthOfData = 1;
    public int curLength = 0;
    bool sharingAnchor = false;

    public void ShareAnchorRoutine()
    {
        if (!anchorLoaded)
        {
            Debug.Log("Nothing to be shared as no anchor is loaded.");
            log.text = "Nothing to be shared as no anchor is loaded.";
            return;
        }

        if (sharingAnchor) return;

        sharingAnchor = true;

        if (bytesToSend == null)
            bytesToSend = new List<byte[]>();

        bytesToSend.Clear();

        WorldAnchorTransferBatch toSend = new WorldAnchorTransferBatch();
        toSend.AddWorldAnchor("anchor", anchored.GetComponent<WorldAnchor>());
        WorldAnchorTransferBatch.ExportAsync(toSend, OnExportDataAvailable, OnExportComplete);
    }

    private void OnExportDataAvailable(byte[] data)
    {
        bytesToSend.Add(data);
    }

    private void OnExportComplete(SerializationCompletionReason completionReason)
    {
        if (completionReason != SerializationCompletionReason.Succeeded)
        {
            Debug.Log("failed failed failed failed failed");
        }
        else
        {
            Debug.Log("success success success");
            int totallength = 0;
            foreach (var totransfer in bytesToSend)
                totallength += totransfer.Length;
            dataToSend = new byte[totallength];
            int offset = 0;
            foreach (var totransfer in bytesToSend)
            {
                System.Buffer.BlockCopy(totransfer, 0, dataToSend, offset, totransfer.Length);
                offset += totransfer.Length;
            }
            Debug.Log("data to send length: " + dataToSend.Length);
            StartCoroutine(SendDataCoroutine());
        }
    }

    IEnumerator SendDataCoroutine()
    {
        photonView.RPC("CommunicateDataLength", PhotonTargets.All, dataToSend.Length);
        int offset = 0;
        while (offset < dataToSend.Length)
        {
            int remaining = dataToSend.Length - offset;
            int tosendlength = remaining >= 1000 ? 1000 : remaining;
            byte[] tosend = new byte[tosendlength];
            System.Buffer.BlockCopy(dataToSend, offset, tosend, 0, tosendlength);
            photonView.RPC("CommunicatePartialData", PhotonTargets.All, tosend);
            offset += tosendlength;
            yield return new WaitForSeconds(0.05f);
        }
        photonView.RPC("CommunicateDataSent", PhotonTargets.All);

        sharingAnchor = false;
    }

    [PunRPC]
    void CommunicateDataLength(int length)
    {
        lengthOfData = length;
        curLength = 0;
        if (bytesReceived == null)
            bytesReceived = new List<byte[]>();
        bytesReceived.Clear();
    }

    [PunRPC]
    void CommunicatePartialData(byte[] data)
    {
        curLength += data.Length;
        bytesReceived.Add(data);
        Debug.Log("partial anchor received: " + bytesReceived.Count);
    }

    [PunRPC]
    void CommunicateDataSent()
    {
        int totallength = 0;
        foreach (var received in bytesReceived)
            totallength += received.Length;
        dataReceived = new byte[totallength];
        int offset = 0;
        foreach (var received in bytesReceived)
        {
            System.Buffer.BlockCopy(received, 0, dataReceived, offset, received.Length);
            offset += received.Length;
        }
        Debug.Log("received length: " + dataReceived.Length);
        WorldAnchorTransferBatch.ImportAsync(dataReceived, OnImportComplete);
    }

    private void OnImportComplete(SerializationCompletionReason completionReason, WorldAnchorTransferBatch deserializedTransferBatch)
    {
        if (completionReason != SerializationCompletionReason.Succeeded)
        {
            //this.GetComponent<Renderer>().material.color = new Color(1, 0, 0);
            Debug.Log("Failed to import: " + completionReason.ToString());
        }
        else
        {
            //this.GetComponent<Renderer>().material.color = new Color(1, 1, 1);
            string[] ids = deserializedTransferBatch.GetAllIds();
            Debug.Log("Length: " + ids.Length);
            foreach (var id in ids)
                Debug.Log("received id: " + id);
            if (!photonView.isMine)
            {
                foreach (string id in ids)
                {
                    Debug.Log("reached " + id);
                    //this.GetComponent<Renderer>().material.color = new Color(1, 0, 1);
                    deserializedTransferBatch.LockObject(id, this.anchored);
                }
                string[] allIds = this.store.GetAllIds();
                bool found = false;
                int l = 0;
                while (!found && l < allIds.Length)
                {
                    if (allIds[l] == "anchor")
                        found = true;
                    else
                        l++;
                }
                if (found)
                    this.store.Delete("anchor");
                this.store.Save("anchor", this.anchored.GetComponent<WorldAnchor>());
            }
        }
    }
}
