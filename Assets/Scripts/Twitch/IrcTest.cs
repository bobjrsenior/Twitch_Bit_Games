using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Threading;

public class IrcTest : MonoBehaviour {

    private Socket socket = null;

	// Use this for initialization
	void Start () {

        Thread sockThread = new Thread(new ThreadStart(StartSocket));
        sockThread.Start();

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void StartSocket()
    {
        print("GG");
        Socket socket = IRCHandler.ConnectToTwitch("irc.chat.twitch.tv", 6667, "botjrsenior", "oauth:");
        print("Connected to Socket");
        // IRCHandler.SendSocket(socket, "JOIN #botjrsenior\n");

        int counter = 0;
        do
        {
            counter++;

            string message = IRCHandler.ReadSocket(socket);
            if (message.Trim().Length > 1)
            {
                print(message);
            }
            Thread.Sleep(100);

        } while (counter < 10);


        socket.Close();
    }

    IEnumerator ContinuousRead(Socket socket)
    {
        
        do
        {
            string message = IRCHandler.ReadSocket(socket);
            if (message.Trim().Length > 1)
            {
                print(message);
            }
            yield return null;
        } while (true);
    }

    IEnumerator CheckDone(Thread thread)
    {

        do
        {
            
            yield return null;
        } while (thread.IsAlive);

        thread.Join();
    }
}
