using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class HotUpdate : MonoBehaviour
{
    private byte[] m_ReadPathFileListData;
    private byte[] m_ServerFileListData;

    internal class DownFileInfo
    {
        public string url;
        public string fileName;
        public DownloadHandler fileData;
    }
    
    int m_DownloadCount;
    /// <summary>
    /// 下载单个文件
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    IEnumerator DownLoadFile(DownFileInfo info,Action<DownFileInfo> Complete)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(info.url);
        yield return webRequest.SendWebRequest();
        if(webRequest.isHttpError||webRequest.isNetworkError)
        {
            Debug.LogError("下载文件出错:" + info.url);
            yield break;
            //重试
        }
        yield return new WaitForSeconds(0.2f);
        info.fileData = webRequest.downloadHandler;
        Complete?.Invoke(info);
        webRequest.Dispose();
    }

    /// 下载多个文件
    /// </summary>
    /// <param name="info"></param>
    /// <param name="Complete"></param>
    /// <returns></returns>
    IEnumerator DownLoadFile(List<DownFileInfo> infos, Action<DownFileInfo> Complete, Action DownLoadAllComplete)
    {
        foreach (DownFileInfo info in infos)
        {
            yield return DownLoadFile(info, Complete);
        }
        DownLoadAllComplete?.Invoke();
    }

    /// <summary>
    /// 获取文件信息
    /// </summary>
    /// <returns></returns>
    private List<DownFileInfo> GetFileList(string fileData, string path)
    {
        string content = fileData.Trim().Replace("\r", "");
        string[] files = content.Split('\n');
        List<DownFileInfo> downFileInfos = new List<DownFileInfo>(files.Length);
        for (int i = 0; i < files.Length; i++)
        {
            string[] info = files[i].Split('|');
            DownFileInfo fileInfo = new DownFileInfo();
            fileInfo.fileName = info[1];
            fileInfo.url = PathUtil.Combine(path, info[1]);
            downFileInfos.Add(fileInfo);
        }
        return downFileInfos;
    }

    private bool IsFirstInstall()
    {
        //判断只读目录是否存在版本文件
        bool isExistsReadPath = FileUtil.IsExists(PathUtil.Combine(PathUtil.ReadPath, AppConst.FileListName));

        //判断可读写目录是否存在版本文件
        bool isExistsReadWritePath = FileUtil.IsExists(PathUtil.Combine(PathUtil.ReadWritePath, AppConst.FileListName));

        return isExistsReadPath && !isExistsReadWritePath;
    }

    private void ReleaseResources()
    {
        m_DownloadCount = 0;
        string url = PathUtil.Combine(PathUtil.ReadPath, AppConst.FileListName);
        DownFileInfo info = new DownFileInfo();
        info.url = url;
        StartCoroutine(DownLoadFile(info, OnDownLoadReadPathFileListComplete));
    }

    private void OnDownLoadReadPathFileListComplete(DownFileInfo file)
    {
        m_ReadPathFileListData = file.fileData.data;
        List<DownFileInfo> fileInfos = GetFileList(file.fileData.text, PathUtil.ReadPath);
        StartCoroutine(DownLoadFile(fileInfos, OnReleaseFileComplete, OnReleaseAllFileComplete));
        loadingUI.InitProgress(fileInfos.Count, "正在释放资源，不消耗流量...");
    }

    private void OnReleaseAllFileComplete()
    {
        FileUtil.WriteFile(PathUtil.Combine(PathUtil.ReadWritePath, AppConst.FileListName), m_ReadPathFileListData);
        CheckUpdate();
    }

    private void OnReleaseFileComplete(DownFileInfo fileInfo)
    {
        Debug.Log("OnReleaseFileComplete:" + fileInfo.url);
        string writeFile = PathUtil.Combine(PathUtil.ReadWritePath, fileInfo.fileName);
        FileUtil.WriteFile(writeFile, fileInfo.fileData.data);
        m_DownloadCount++;
        loadingUI.UpdateProgress(m_DownloadCount);
    }

    private void CheckUpdate()
    {
        string url = PathUtil.Combine(AppConst.ResourcesUrl, AppConst.FileListName);
        DownFileInfo info = new DownFileInfo();
        info.url = url;
        StartCoroutine(DownLoadFile(info, OnDownLoadServerFileListComplete));
    }

    private void OnDownLoadServerFileListComplete(DownFileInfo file)
    {
        m_DownloadCount = 0;
        m_ServerFileListData = file.fileData.data;
        List<DownFileInfo> fileInfos = GetFileList(file.fileData.text, AppConst.ResourcesUrl);
        List<DownFileInfo> downListFiles = new List<DownFileInfo>();

        for (int i = 0; i < fileInfos.Count; i++)
        {
            string localFile = PathUtil.Combine(PathUtil.ReadWritePath, fileInfos[i].fileName);

            //通过文件的md5来进行校验
            if (!FileUtil.IsExists(localFile))
            {
                fileInfos[i].url = PathUtil.Combine(AppConst.ResourcesUrl, fileInfos[i].fileName);
                downListFiles.Add(fileInfos[i]);
            }
        }
        if (downListFiles.Count > 0)
        {
            StartCoroutine(DownLoadFile(fileInfos, OnUpdateFileComplete, OnUpdateAllFileComplete));
            loadingUI.InitProgress(downListFiles.Count, "正在更新...");
        }
        else
            EnterGame();
    }

    private void OnUpdateAllFileComplete()
    {
        FileUtil.WriteFile(PathUtil.Combine(PathUtil.ReadWritePath, AppConst.FileListName), m_ServerFileListData);
        EnterGame();
        loadingUI.InitProgress(0, "正在载入");
    }

    private void OnUpdateFileComplete(DownFileInfo file)
    {
        Debug.Log("OnUpdateFileComplete:" + file.url);
        string writeFile = PathUtil.Combine(PathUtil.ReadWritePath, file.fileName);
        FileUtil.WriteFile(writeFile, file.fileData.data);
        m_DownloadCount++;
        loadingUI.UpdateProgress(m_DownloadCount);
    }

    private void EnterGame()
    {
        Manager.Event.Fire((int)GameEvent.GameInit);
        Destroy(loadingObj);
    }

    private void OnComplete(UnityEngine.Object obj)
    {
        GameObject go = Instantiate(obj) as GameObject;
        go.transform.SetParent(this.transform);
        go.SetActive(true);
        go.transform.localPosition = Vector3.zero;
    }

    GameObject loadingObj;
    LoadingUI loadingUI;
    private void Start()
    {
        GameObject go = Resources.Load<GameObject>("LoadingUI");
        loadingObj = Instantiate(go);
        loadingObj.transform.SetParent(this.transform);
        loadingUI = loadingObj.GetComponent<LoadingUI>();
        if(IsFirstInstall())
        {
            ReleaseResources();
        }
        else
        {
            CheckUpdate();
        }
    }
}
