using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

public class IrcTest : MonoBehaviour {

    private Socket socket = null;

    private bool continueSock = true;

    private Thread sockThread;

    private List<string> messages = new List<string>();

    private bool lockList = false;

    // Use this for initialization
    void Start () {

        sockThread = new Thread(new ThreadStart(StartSocket));
        sockThread.Start();
        //StartCoroutine(StartSocketCoRoutine());
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            continueSock = false;
            sockThread.Abort();
        }

        if (!lockList)
        {
            lockList = true;
            if(messages.Count > 0)
            {
                print(messages[0]);
                messages.RemoveAt(0);
            }
            lockList = false;
        }

    }

    /// <summary>
    /// Threaded Version
    /// </summary>
    private void StartSocket()
    {
        socket = IRCHandler.ConnectToTwitch("irc.chat.twitch.tv", 6667, "botjrsenior", "oauth:");
        IRCHandler.SendSocket(socket, "JOIN #botjrsenior\n");

        int counter = 0;
        do
        {
            counter++;

            string message = IRCHandler.ReadSocket(socket);
            if (message.Trim().Length > 1)
            {
                while (lockList) { Thread.Sleep(10); }
                lockList = true;
                messages.Add(message);
                lockList = false;
            }
            Thread.Sleep(100);

        } while (continueSock && socket.Connected);


        socket.Close();
    }

    /// <summary>
    /// Coroutine Version
    /// </summary>
    /// <returns></returns>
    IEnumerator StartSocketCoRoutine()
    {
        socket = IRCHandler.ConnectToTwitch("irc.chat.twitch.tv", 6667, "botjrsenior", "oauth:");
        yield return null;
        IRCHandler.SendSocket(socket, "JOIN #botjrsenior\n");
        yield return null;

        int counter = 0;

        do
        {
            counter++;
            string message = IRCHandler.ReadSocket(socket);
            if (message.Trim().Length > 1)
            {
                while (lockList) { yield return new WaitForSeconds(0.01f); }
                lockList = true;
                messages.Add(message);
                lockList = false;
            }
            yield return new WaitForSeconds(.1f);
        } while (continueSock && socket.Connected);


        socket.Close();
    }

}
