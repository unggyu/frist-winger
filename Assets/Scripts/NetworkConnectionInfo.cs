[System.Serializable]
public class NetworkConnectionInfo
{
    /// <summary>
    /// 호스트로 실행 여부
    /// </summary>
    public bool host;

    /// <summary>
    /// 클라이언트로 실행 시 접속할 호스트 IP 주소
    /// </summary>
    public string ipAddress;

    /// <summary>
    /// 클라이언트로 실행 시 접속할 호스트의 Port
    /// </summary>
    public int port;
}
