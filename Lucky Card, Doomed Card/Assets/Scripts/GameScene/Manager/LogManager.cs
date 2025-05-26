using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class LogManager : MonoBehaviourPunCallbacks
{
    public static LogManager Instance;

    public Transform logContent; // Scroll View > Content
    public GameObject logPrefab; // LogItem 프리팹
    private List<GameObject> logItems = new List<GameObject>(); // 생성된 로그 저장 리스트
    public int maxLogCount = 9; // 최대 로그 개수

    void Awake()
    {
        Instance = this;  
    }

    /// <summary>
    /// 로그 추가 함수 (모든 클라이언트에 동기화)
    /// </summary>
    public void AddRPCLog(string message)
    {
        photonView.RPC("RPC_AddLog", RpcTarget.All, message);
    }

    public void AddLog(string message)
    {
        GameObject newLog = Instantiate(logPrefab, logContent);
        newLog.GetComponent<Text>().text = message;

        logItems.Add(newLog);

        // 로그가 너무 많아지면 가장 오래된 로그 삭제
        if (logItems.Count > maxLogCount)
        {
            Destroy(logItems[0]);
            logItems.RemoveAt(0);
        }
    }

    [PunRPC]
    private void RPC_AddLog(string message)
    {
        GameObject newLog = Instantiate(logPrefab, logContent);
        newLog.GetComponent<Text>().text = message;

        logItems.Add(newLog);

        // 로그가 너무 많아지면 가장 오래된 로그 삭제
        if (logItems.Count > maxLogCount)
        {
            Destroy(logItems[0]);
            logItems.RemoveAt(0);
        }
    }
}
