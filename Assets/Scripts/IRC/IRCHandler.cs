using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System;

public class IRCHandler {

    private static string prevMessage = "";

    public static Socket ConnectSocket(string server, int port)
    {
        Socket s = null;
        IPHostEntry hostEntry = null;

        // Get host related information.
        hostEntry = Dns.GetHostEntry(server);

        Debug.Log("Cycling");

        // Loop through the AddressList to obtain the supported AddressFamily. This is to avoid
        // an exception that occurs when the host IP Address is not compatible with the address family
        // (typical in the IPv6 case).
        foreach (IPAddress address in hostEntry.AddressList)
        {
            IPEndPoint ipe = new IPEndPoint(address, port);
            Socket tempSocket =
                new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            tempSocket.Connect(ipe);

            if (tempSocket.Connected)
            {
                s = tempSocket;
                break;
            }
            else
            {
                continue;
            }
        }
        return s;
    }

    public static Socket ConnectToTwitch(string server, int port, string username, string auth)
    {
        Debug.Log("Start Connection");
        Socket socket = ConnectSocket(server, port);
        Debug.Log("Initial Connection");
        SendSocket(socket, "PASS " + auth + "\n");
        SendSocket(socket, "NICK " + username + "\n");

        return socket;

    }

    public static string ReadSocket(Socket socket)
    {
        string message = "";
        if (prevMessage.Contains("\n")){
            string[] prevSplit = prevMessage.Split('\n');
            message = prevSplit[0];

            prevMessage = "";
            for(int i = 1; i < prevSplit.Length; ++i)
            {
                prevMessage += prevSplit[i];
                if(i < prevSplit.Length - 1)
                {
                    prevMessage += "\n";
                }
            }

            return message;
        }
        
        Byte[] bytes = new Byte[512];

        int numBytes;
        
        do { 
            numBytes = socket.Receive(bytes, bytes.Length, 0);
           
            message += System.Text.Encoding.ASCII.GetString(bytes);
        } while (!message.Contains("\n"));

        string[] messageSplit = message.Split('\n');
        string returnMessage = messageSplit[0];
        for (int i = 1; i < messageSplit.Length; ++i)
        {
            prevMessage += messageSplit[i];
            if (i < messageSplit.Length - 1)
            {
                prevMessage += "\n";
            }
        }

        return returnMessage;
    }

    public static void SendSocket(Socket socket, string message)
    {
        socket.Send(System.Text.Encoding.ASCII.GetBytes(message));
    }
}
