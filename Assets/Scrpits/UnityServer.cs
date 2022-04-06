using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UnityServer : MonoBehaviour
{
	#region private members 	
	/// <summary> 	
	/// TCPListener to listen for incomming TCP connection 	
	/// requests. 	
	/// </summary> 	
	
	private TcpListener tcpListener;
	/// <summary> 
	/// Background thread for TcpServer workload. 	
	/// </summary> 	
	private Thread tcpListenerThread;
	/// <summary> 	
	/// Create handle to connected tcp client. 	
	/// </summary> 	
	private TcpClient connectedTcpClient;
	#endregion
	private GameObject gm;
	private PlacingProcedure sc;
	private MovableCamera mc;
	TemporaryProvision temporary;
	string clientMessage = "";
	public Canvas pause;
	public Canvas canvas;
	// Use this for initialization

	void Awake()
	{
        // Start TcpServer background thread 		
        tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests));
        tcpListenerThread.IsBackground = true;
        tcpListenerThread.Start();
        gm = GameObject.Find("Plane");
        sc = gm.GetComponent<PlacingProcedure>();
    }

	// Update is called once per frame
	void Update()
	{
		if (clientMessage != "")
		{
			if (temporary != null) {
				temporary.SetNormal();
				temporary = null;
			}
			if (clientMessage[0] == 'p')
			{
				sc.LoadNewObjOnScene(clientMessage.Substring(2));
				canvas.gameObject.SetActive(true);
				pause.gameObject.SetActive(false);
			}
			else if (clientMessage[0] == 's')
			{
				canvas.gameObject.SetActive(true);
				pause.gameObject.SetActive(false);
				GameObject go = GameObject.Find(Scene.basic_name + clientMessage.Substring(2));
				temporary = new TemporaryProvision();
				temporary.gameObject = go;
				temporary.rend = temporary.gameObject.GetComponentsInChildren<Renderer>();
				temporary.SetSelected();
			}
			else if (clientMessage[0] == 'd')
			{
				canvas.gameObject.SetActive(true);
				pause.gameObject.SetActive(false);
				GameObject go = GameObject.Find(Scene.basic_name + clientMessage.Substring(2));
				Destroy(go);
				Scene.DeleteFromSceneList(Convert.ToInt32(clientMessage.Substring(2)));
				SendMessageToClient("s " + Math.Round(Scene.ReturnArea(),2));
				Thread.Sleep(50);
				SendMessageToClient("h " + Math.Round(Scene.highest_point,2));
			}
			else if (clientMessage[0] == 'r')
			{
				canvas.gameObject.SetActive(true);
				pause.gameObject.SetActive(false);
				Scene.RelocateObject(Convert.ToInt32(clientMessage.Substring(2)));
				GameObject go = GameObject.Find(Scene.basic_name + clientMessage.Substring(2));
				temporary = new TemporaryProvision();
				temporary.gameObject = go.gameObject;
				temporary.IsMounted = true;
				temporary.mountedIndex = Convert.ToInt32(clientMessage.Substring(2));
				temporary.rend = temporary.gameObject.GetComponentsInChildren<Renderer>();
				sc.RelocateObject(temporary);
			}
			else if (clientMessage[0] == 'w') {
				canvas.gameObject.SetActive(false);
				pause.gameObject.SetActive(true);
			}
			clientMessage = "";
		}
	}

    void OnApplicationQuit()
    {
        tcpListener?.Stop();
        connectedTcpClient?.Close();
        tcpListenerThread?.Abort();
    }

    /// <summary> 	
    /// Runs in background TcpServerThread; Handles incomming TcpClient requests 	
    /// </summary> 	
    private void ListenForIncommingRequests()
	{
		try
		{ 			
			tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8050);
			tcpListener.Start();
			Debug.Log("Server is listening");
			byte[] bytes = new byte[1024];
			while (true)
			{
				using (connectedTcpClient = tcpListener.AcceptTcpClient())
				{
					// Get a stream object for reading 					
					using (NetworkStream stream = connectedTcpClient.GetStream())
					{
						int length;
						// Read incomming stream into byte arrary. 						
						while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
						{
							var incommingData = new byte[length];
							Array.Copy(bytes, 0, incommingData, 0, length);							
							clientMessage = Encoding.ASCII.GetString(incommingData);
							Debug.Log("client message received as: " + clientMessage);
						}
						bytes = new byte[1024];
					}
				}
			}
		}
		catch (SocketException socketException)
		{
			Debug.Log("SocketException " + socketException.ToString());
		}
	}

	
	/// <summary> 	
	/// Send message to client using socket connection. 	
	/// </summary> 	
	public void SendMessageToClient(string serverMessage)
	{
        if (connectedTcpClient == null)
        {
            return;
        }

        try
        {
            // Get a stream object for writing. 			
            NetworkStream stream = connectedTcpClient.GetStream();
            if (stream.CanWrite)
            {
                // Convert string message to byte array.                 
                byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(serverMessage);
                // Write byte array to socketConnection stream.               
                stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);
                Debug.Log("Server sent his message - should be received by client");
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }
}
