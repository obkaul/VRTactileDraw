using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Assets.PatternDesigner.Scripts.HapticHead;

public class MultiWaveNavigation : MonoBehaviour

{
    private static float[] intensities;
    
    private static bool[] motors;
    private static int[] initamplitudes;
    private static int[] initfrequencies;
    private static int[] initfunctions;
    private static int localPort;

    // prefs
    private static string HOST = "192.168.1.178";  // define in init
    public static int port = 8888;  // define in init

    // “connection” things
    IPEndPoint remoteEndPoint;
    Socket client;
    private int counter = 0;


    void Start()
    {
        var motorCount = VibratorMesh.VIBRATOR_COUNT; 
        motors = new bool[motorCount];

        intensities = new float[motorCount];
        initamplitudes = new int[motorCount];
        initfrequencies = new int[motorCount];
        initfunctions = new int[motorCount];

        for (int a = 0; a < initamplitudes.Length; a++)
        {
            initamplitudes[a] = 200;
        }

        for (int a = 0; a < initamplitudes.Length; a++)
        {
            initfrequencies[a] = 1;
        }

        for (int a = 0; a < initfunctions.Length; a++)
        {
            initfunctions[a] = 1;
        }

        for (int i = 0; i < motors.Length; i++)
        {
            motors[i] = false;
        }

        InvokeRepeating("StartCoroutineStarter", 0, 0.2f);
        // Send
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(HOST), port);
        client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        Activate(counter);
    }

    void StartCoroutineStarter()
    {
        StartCoroutine("UpdateActuators");
    }
    public void Activate(int number)
    {
            motors[number] = true;
    }

    public void DeActivate(int number)
    {
            motors[number] = false;       
    }

    public void DeActivateAll()
    {
        for (var i = 0; i < motors.Length; i++)
        {
            DeActivate(i);
        }
    }

    public void setFunction(int number)
    {
        var packageList = new List<int>();
        for (int i = 0; i < motors.Length; i++)
        {          
                packageList.Add(77);
                packageList.Add(i);
                packageList.Add(0);
                packageList.Add(number);                       
        }

        var package = packageList.ToArray();
        sendIntArray(package);
        sendIntArray(package);
        sendIntArray(package);
    }

    IEnumerator UpdateActuators()
    {   
        
        var packageList = new List<int>();
        for (int i = 0; i < motors.Length; i++)
        {
            if (motors[i] == true)
            {
                packageList.Add(77);
                packageList.Add(i);
                packageList.Add(2);
                packageList.Add((int)intensities[i]*100);              
            }
            else
            {
                packageList.Add(77);
                packageList.Add(i);
                packageList.Add(2);
                packageList.Add(0);                
            }
        }

        var package = packageList.ToArray();
        sendIntArray(package);
        
        // M .. number of Motor .. 1 Frequency 2 Amplitude 0 Func 3 Phase .. Value bei Freq == 2 byte
        yield return new WaitForSeconds(.2f);

    }

    private void sendIntArray(int[] message)
    {
        try
        {
            // Encode data using the UTF8 encoding to binary format.
            var result = new byte[message.Length * sizeof(int)];
            Buffer.BlockCopy(message, 0, result, 0, result.Length);
            var newresult = new byte[message.Length];
            var count = 0;
            for (var i = 0; i < result.Length; i = i + 4)
            {
                newresult[count] = result[i];
                count++;
            }
           
            // Send the message to the remote client.
            client.SendTo(newresult, remoteEndPoint);
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }


}
