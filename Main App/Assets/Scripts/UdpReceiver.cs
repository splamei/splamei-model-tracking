/*  Copyright 2026 Splamei
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UdpReceiver : MonoBehaviour
{
    [Header("Network config")]
    public int port = 58080;

    [Header("Smoothing config")]
    public float smoothing = 0.25f;

    private UdpClient udpClient;
    private Thread receiveThread;
    private volatile bool running;

    private readonly object dataLock = new object();

    private JointPacket latestPacket;
    private JointPacket smoothedPacket;
    private bool hadData;

    void Awake()
    {
        // probs don't need this but i'm adding it anyway
        Application.runInBackground = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        udpClient = new UdpClient(port);
        running = true;

        receiveThread = new Thread(receiveLoop);
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (!hadData) { return; }

        lock (dataLock)
        {
            applySmoothing(ref smoothedPacket, latestPacket);
        }
    }

    void OnDestroy()
    {
        disposeObjs();
    }

    void OnDisable()
    {
        disposeObjs();
    }

    void OnApplicationQuit()
    {
        disposeObjs();
    }

    private void disposeObjs()
    {
        running = false;

        try
        {
            if (udpClient != null)
            {
                udpClient.Close();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to close the UDP client! {e}");
        }

        try
        {
            if (receiveThread != null && receiveThread.IsAlive)
            {
                receiveThread.Join(2000);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to join the receive thread! {e}");
        }

        receiveThread = null;
        udpClient = null;
    }

    private void receiveLoop()
    {
        var endpoint = new IPEndPoint(IPAddress.Any, port);

        while (running)
        {
            try
            {
                byte[] data = udpClient.Receive(ref endpoint);
                string json = Encoding.UTF8.GetString(data);

                JointPacket packet = JsonUtility.FromJson<JointPacket>(json);

                lock (dataLock)
                {
                    latestPacket = packet;
                    hadData = true;
                }
            }
            catch (SocketException e)
            {
                Debug.LogError($"Error receiving UDP data! Did it shutdown or timeout? - {e}");
            }
            catch (ObjectDisposedException e)
            {
                Debug.LogError($"Error receiving UDP data! It was disposed. Now exiting - {e}");
                break;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error receiving UDP data: {e}");
            }
        }
    }

    private void applySmoothing(ref JointPacket target, JointPacket input)
    {
        if (input.version != 1.0f || input.type != "Full")
        {
            Debug.LogError($"[UdpReceiver] Unable to use a bridge because it's sending version '{input.version}' and type '{input.type}'!");
        }

        target.version = input.version;
        target.type = input.type;
        target.runtimeId = input.runtimeId;

        target.friendlyName = input.friendlyName;
        target.identifier = input.identifier;

        target.frame = input.frame;
        target.timestampL = input.timestampL;
        target.timestampG = input.timestampG;

        target.sendSpeed = input.sendSpeed;

        // ---

        float t = smoothing;

        target.head = Lerp(target.head, input.head, t);
        target.neck = Lerp(target.neck, input.neck, t);
        target.spineBase = Lerp(target.spineBase, input.spineBase, t);

        target.shoulderLeft = Lerp(target.shoulderLeft, input.shoulderLeft, t);
        target.shoulderRight = Lerp(target.shoulderRight, input.shoulderRight, t);

        target.elbowLeft = Lerp(target.elbowLeft, input.elbowLeft, t);
        target.elbowRight = Lerp(target.elbowRight, input.elbowRight, t);

        target.wristLeft = Lerp(target.wristLeft, input.wristLeft, t);
        target.wristRight = Lerp(target.wristRight, input.wristRight, t);

        target.handLeft = Lerp(target.handLeft, input.handLeft, t);
        target.handRight = Lerp(target.handRight, input.handRight, t);

        target.hipLeft = Lerp(target.hipLeft, input.hipLeft, t);
        target.hipRight = Lerp(target.hipRight, input.hipRight, t);

        target.kneeLeft = Lerp(target.kneeLeft, input.kneeLeft, t);
        target.kneeRight = Lerp(target.kneeRight, input.kneeRight, t);

        target.ankleLeft = Lerp(target.ankleLeft, input.ankleLeft, t);
        target.ankleRight = Lerp(target.ankleRight, input.ankleRight, t);

        target.footLeft = Lerp(target.footLeft, input.footLeft, t);
        target.footRight = Lerp(target.footRight, input.footRight, t);
    }

    private Vec3 Lerp(Vec3 a, Vec3 b, float t)
    {
        return new Vec3
        {
            x = a.x + (b.x - a.x) * t,
            y = a.y + (b.y - a.y) * t,
            z = a.z + (b.z - a.z) * t
        };
    }

    public JointPacket getLatest()
    {
        lock (dataLock)
        {
            return smoothedPacket;
        }
    }

    public bool hasData()
    {
        return hadData;
    }

    [Serializable]
    public struct JointPacket
    {
        // Metadata root
        public float version;
        public string type;
        public int runtimeId;

        public string friendlyName;
        public string identifier;

        public int frame;
        public long timestampL;
        public long timestampG;

        public int sendSpeed;

        // ---

        public Vec3 head;
        public Vec3 neck;
        public Vec3 spineBase;

        public Vec3 shoulderLeft;
        public Vec3 shoulderRight;

        public Vec3 elbowLeft;
        public Vec3 elbowRight;

        public Vec3 wristLeft;
        public Vec3 wristRight;

        public Vec3 handLeft;
        public Vec3 handRight;

        public Vec3 hipLeft;
        public Vec3 hipRight;

        public Vec3 kneeLeft;
        public Vec3 kneeRight;

        public Vec3 ankleLeft;
        public Vec3 ankleRight;

        public Vec3 footLeft;
        public Vec3 footRight;
    }

    [Serializable]
    public struct Vec3
    {
        public float x;
        public float y;
        public float z;
    }
}
