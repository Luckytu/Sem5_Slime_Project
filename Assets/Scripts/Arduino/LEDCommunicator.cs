using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Arduino
{
    public class LEDCommunicator : MonoBehaviour
    {
        private static List<string> serialPorts = new List<string>();

        public SerialPort serialPort;

        public string selectedPortName;

        public Vector3Int hsvValues;

        public int length;

        [SerializeField] private float animationDuration = 5f;
        [SerializeField] private Color[] exampleColors;
        [SerializeField] private bool queueExampleOnStart;

        private Queue<Vector3Int> HSVQueue;
        public bool isAnimationPlaying = false;

        // Start is called before the first frame update
        private void Awake()
        {
            serialPorts = SerialPort.GetPortNames().ToList();
            foreach(var port in serialPorts)
            {
                Debug.Log("Found Port:" + port);
            }

            HSVQueue = new Queue<Vector3Int>();

            if (queueExampleOnStart)
            {
                queueExample();
            }
            
            selectPort(selectedPortName);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F12))
            {
                selectPort(selectedPortName);
            }
            if (Input.GetKeyDown(KeyCode.F11))
            {
                addAnimationToQueue(hsvValues);
            }
            if (Input.GetKeyDown(KeyCode.F9))
            {
                closeComPort();
            }
            if (Input.GetKeyDown(KeyCode.F10))
            {
                queueExample();
            }

            playAnimation();
        }

        private void queueExample()
        {
            foreach(Color color in exampleColors)
            {
                float H, S, V;

                Color.RGBToHSV(color, out H, out S, out V);

                Vector3Int colorInt = new Vector3Int((int)(H * 360), (int)(S * 255), (int)(V * 255));

                addAnimationToQueue(colorInt);
            }
        }

        private void selectPort(string selectedPort)
        {
            try
            {
                serialPort = new SerialPort(selectedPort, 9600);
                serialPort.Open();
                Debug.Log("connected");
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        public void addAnimationToQueue(Vector3Int HSV)
        {
            HSVQueue.Enqueue(HSV);
            Debug.Log("Queue Count " + HSVQueue.Count);
        }

        public void addColorToQueue(Color color)
        {
            float H, S, V;

            Color.RGBToHSV(color, out H, out S, out V);

            Vector3Int colorInt = new Vector3Int((int)(H * 360), (int)(S * 255), (int)(V * 255));
            //Vector3Int colorInt = new Vector3Int(360, 0, 255);
            
            addAnimationToQueue(colorInt);
        }

        private void playAnimation()
        {
            if (!isAnimationPlaying && HSVQueue.Count > 0)
            {
                StartCoroutine(sendHSVToCom(HSVQueue.Dequeue(), length));
                Debug.Log("played animation. Animations remaining: " + HSVQueue.Count);
            }
        }

        private IEnumerator sendHSVToCom(Vector3Int hsvValues, int length)
        {
            isAnimationPlaying = true;

            string message = hsvValues.x.ToString("000") + "," + hsvValues.y.ToString("000") + "," + hsvValues.z.ToString("000") + "," + length.ToString("000") + "\n";
            
            if (serialPort != null)
            {
                serialPort.Write(message);

                yield return new WaitForSeconds(animationDuration);
            }
            
            yield return null;
            isAnimationPlaying = false;
        }

        private void closeComPort()
        {
            serialPort?.Close();
        }

        private void OnDestroy()
        {
            closeComPort();
        }
    }
}
