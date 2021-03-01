using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Assets.PatternDesigner.Scripts.HapticHead;
using UnityEngine;

namespace Assets.PatternDesigner.Scripts.misc
{
    public class RaspberryCommandSender : Singleton<RaspberryCommandSender>, CommandSender
    {
        private static float[] intensities;
        private static int[] intensitiesScaled;
        private static Color[] colorsForIntensities;
        private static float[] motorActiveTimes;

        private static readonly Color deactivatedColor = new Color(1, 1, 1, 1);
        private static readonly Color activatedColor = new Color(1, 0, 0, 1);
        private static readonly int[] packetNumberArray = new int[1];
        private static readonly int[] checksumArray = new int[1];

        public static string
            HOST = "192.168.1.145"; // TODO: Change to your Raspberries configuration, look up the IP on the router and the port on the sticker below it

        public static int
            PORT = 55007; // TODO: Change to your Raspberries configuration, look up the IP on the router and the port on the sticker below it

        public static float
            UPDATE_RATE = 0.01f; // corresponds to update rate, is quite on point. 0.01 = 100Hz update rate

        private byte[] byteArrayToSend;

        public int connectionTimeout = 4;
        
        private Thread connectToServerThread;

        private int packetNumber;

        private Socket senderSocket;

        Thread senderThread;
        private bool senderThreadActive = false;

        private float[] zeroesFloat;

        // intensity must be 0...1
        public void updateActuator(int motorId, float intensity, float activeTime)
        {
            if (intensity > 1) print("Warning, trying to set intensity to > 1:  " + intensity.ToString("2F"));

            if (motorId < intensities.Length)
            {
                if (intensities[motorId] <= intensity || motorActiveTimes[motorId] <= 0)
                {
                    intensities[motorId] = intensity;
                    intensitiesScaled[motorId] = Math.Min((int) (intensity * 255f), 255);
                    motorActiveTimes[motorId] = intensity == 0 ? 0 : Mathf.Max(activeTime, motorActiveTimes[motorId]);
                }
                else if (activeTime == 0 && intensity == 0)
                {
                    intensities[motorId] = 0;
                    intensitiesScaled[motorId] = 0;
                    motorActiveTimes[motorId] = 0;
                }
            }
        }

        // intensities must be 0...1
        public void updateActuators(float[] newIntensities, float[] activeTimes)
        {
            for (var motorId = 0; motorId < intensities.Length; motorId++)
                if (motorId < newIntensities.Length)
                {
                    // uncomment the following debug message if needed to check whether actuaturs are correctly updated
                    // if (newIntensities[motorId] > 0) print("Set actuator " + motorId + " to " + newIntensities[motorId]);
                    if (newIntensities[motorId] > 1)
                        print("Warning, trying to set intensity to > 1:  " + newIntensities[motorId].ToString("2F"));
                    if (intensities[motorId] <= newIntensities[motorId] || motorActiveTimes[motorId] <= 0)
                        updateActuator(motorId, newIntensities[motorId], activeTimes[motorId]);
                    else if (activeTimes[motorId] == 0 && newIntensities[motorId] == 0)
                        updateActuator(motorId, 0, 0);
                }
        }

        public void deactivateFeedback()
        {
            updateActuators(zeroesFloat, zeroesFloat);
        }


        // Use this for initialization
        private void Start()
        {
            var motorCount = VibratorMesh.VIBRATOR_COUNT;
            intensities = new float[motorCount];
            intensitiesScaled = new int[motorCount];

            colorsForIntensities = new Color[motorCount];
            motorActiveTimes = new float[motorCount];

            zeroesFloat = new float[motorCount];

            byteArrayToSend = new byte[2 * sizeof(int) + 1 * motorCount * sizeof(int)];

            connectToServerThread = connectToServer(HOST, PORT);

            senderThread = startSenderThread();
        }

        private new void OnDestroy()
        {
            if (senderSocket != null && senderSocket.Connected)
            {
                deactivateFeedback();
                sendUpdates();
            }
            senderThreadActive = false;

            base.OnDestroy();
        }


        private void sendUpdates()
        {
            packetNumberArray[0] = packetNumber;

            var sum = 0;
            Array.ForEach(intensitiesScaled, delegate(int i) { sum += i; });

            var checksum = packetNumber + sum;

            checksumArray[0] = checksum;

            Buffer.BlockCopy(checksumArray, 0, byteArrayToSend, 0, sizeof(int));
            Buffer.BlockCopy(packetNumberArray, 0, byteArrayToSend, sizeof(int), sizeof(int));
            Buffer.BlockCopy(intensitiesScaled, 0, byteArrayToSend, 2 * sizeof(int),
                VibratorMesh.VIBRATOR_COUNT * sizeof(int));

            if (senderSocket != null && senderSocket.Connected)
            {
                senderSocket.Send(byteArrayToSend);
            }
            else if (connectToServerThread == null) connectToServerThread = connectToServer(HOST, PORT);

            ++packetNumber;

            for (var i = 0; i < VibratorMesh.VIBRATOR_COUNT; i++)
                if (intensities[i] == 0)
                    colorsForIntensities[i] = deactivatedColor;
                else if (intensities[i] >= 1)
                    colorsForIntensities[i] = activatedColor;
                else
                    colorsForIntensities[i] = new Color(1, 1 - intensities[i], 1 - intensities[i]);
        }

        private Thread startSenderThread()
        {
            print("Initializing Sender Thread...");
            senderThreadActive = true;
            var t = new Thread(() => SenderThreadMethod());
            t.IsBackground = false;
            t.Start();
            return t;
        }


        void SenderThreadMethod()
        {
            double lastTimestamp = getTimeMillis();
            double currentTimestamp, timeSinceLastSend;

            while (senderThreadActive)
            {
                //yield return new WaitForEndOfFrame();
                sendUpdates();

                currentTimestamp = getTimeMillis();
                timeSinceLastSend = currentTimestamp - lastTimestamp;
                lastTimestamp = currentTimestamp;

                updateMotorTimes(timeSinceLastSend);
                Thread.Sleep((int)(UPDATE_RATE * 1000));
            }
        }

        private void updateMotorTimes(double subtractTimeMillis)
        {
            for (int motorId = 0; motorId < intensities.Length; motorId++)
            {
                motorActiveTimes[motorId] -= (float)(subtractTimeMillis / 1000.0);
                if (motorActiveTimes[motorId] <= 0)
                {
                    intensities[motorId] = 0;
                    intensitiesScaled[motorId] = 0;
                }
            }
        }

        private void OnApplicationQuit()
        {
            if (senderSocket != null && senderSocket.Connected)
            {
                // Release the socket.
                senderSocket.Shutdown(SocketShutdown.Both);
                senderSocket.Close();
            }

            senderThreadActive = false;

            base.OnDestroy();
        }

        public static double getTimeMillis()
        {
            return DateTime.Now.Ticks / (double) TimeSpan.TicksPerMillisecond;
        }

        private Thread connectToServer(string host, int port)
        {
            var t = new Thread(() => connectToServerThreadMethodWiFi(host, port));
            t.IsBackground = false;
            t.Start();
            return t;
        }

        private void connectToServerThreadMethodWiFi(string host, int port)
        {
            Thread.Sleep(1000);

            // Connect to a remote device.
            try
            {
                var remoteEP = new IPEndPoint(IPAddress.Parse(host), port);

                var socketTarget = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                // Connect the socket to the remote endpoint. Catch any errors.
                try
                {
                    var result = socketTarget.BeginConnect(remoteEP, null, null);

                    var success = result.AsyncWaitHandle.WaitOne(connectionTimeout * 1000, true);

                    if (!success)
                    {
                        socketTarget.Close();
                        throw new ApplicationException("Failed to connect server. Timeout after " + connectionTimeout +
                                                       "s.");
                    }

                    print("Socket connected " + socketTarget.RemoteEndPoint);
                    if (host.Equals(HOST))
                    {
                        senderSocket = socketTarget;
                        connectToServerThread = null;
                    }

                    return;
                }
                catch (ArgumentNullException ane)
                {
                    print("ArgumentNullException : " + ane);
                }
                catch (SocketException se)
                {
                    print("SocketException : " + se);
                }
                catch (Exception e)
                {
                    print("Unexpected exception : " + e);
                }
            }
            catch (Exception e)
            {
                print(e.ToString());
            }

            if (host.Equals(HOST))
                connectToServerThread = null;
        }
    }
}