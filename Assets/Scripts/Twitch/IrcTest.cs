using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

public class IrcTest : MonoBehaviour {

    private Socket socket = null;

    private bool continueSock = true;

    //private Thread sockThread;

    private List<string> messages = new List<string>();

    private bool lockMessageList = false;

    private List<string> bitMessages = new List<string>();

    private bool lockBitMessageList = false;

    // Use this for initialization
    void Start () {

        //sockThread = new Thread(new ThreadStart(StartSocket));
        //sockThread.Start();
        //StartCoroutine(StartSocketCoRoutine());
        StartCoroutine(ParseMessageList());
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            continueSock = false;
            //sockThread.Abort();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!lockMessageList)
            {
                lockMessageList = true;
                if (System.DateTime.Now.Ticks % 2 == 0)
                {
                    messages.Add("@badges=staff/1,bits/1000;bits=100;color=;display-name=TWITCH_UserNaME;emotes=;id=b34ccfc7-4977-403a-8a94-33c6bac34fb8;mod=0;room-id=1337;subscriber=0;turbo=1;user-id=1337;user-type=staff :twitch_username!twitch_username@twitch_username.tmi.twitch.tv PRIVMSG #channel :cheer100");
                }
                else
                {
                    messages.Add("@badges=global_mod/1,turbo/1;color=#0D4200;display-name=TWITCH_UserNaME;emotes=25:0-4,12-16/1902:6-10;mod=0;room-id=1337;subscriber=0;turbo=1;user-id=1337;user-type=global_mod :twitch_username!twitch_username@twitch_username.tmi.twitch.tv PRIVMSG #channel :Kappa Keepo Kappa");
                }
                lockMessageList = false;
            }
        }

        /*
        if (!lockMessageList)
        {
            lockMessageList = true;
            if(messages.Count > 0)
            {
                print(messages[0]);
                messages.RemoveAt(0);
            }
            lockMessageList = false;
        }
        */

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
                while (lockMessageList) { Thread.Sleep(10); }
                lockMessageList = true;
                messages.Add(message);
                lockMessageList = false;
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
                while (lockMessageList) { yield return new WaitForSeconds(0.01f); }
                lockMessageList = true;
                messages.Add(message);
                lockMessageList = false;
            }
            yield return new WaitForSeconds(.1f);
        } while (continueSock && socket.Connected);
        continueSock = false;

        socket.Close();
    }

    IEnumerator ParseMessageList()
    {
        do
        {
            if (!lockMessageList && !lockBitMessageList)
            {
                string message = "";
                lockMessageList = true;
                lockBitMessageList = true;
                if(messages.Count > 0)
                {
                    message = messages[0];
                    messages.RemoveAt(0);
                }
                lockMessageList = false;
                if (!message.Equals("")){
                    string[] messageSplit = message.Split(':');
                    if(messageSplit.Length >= 2 && messageSplit[0].Contains(";bits="))
                    {
                        print(messageSplit[0]);

                        string[] stats = messageSplit[0].Split(';');
                    }
                }
                lockBitMessageList = false;
            }
            yield return new WaitForSeconds(.1f);
        } while (continueSock);


        socket.Close();
    }
}
