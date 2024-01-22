using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class LEDCommunicator : MonoBehaviour
{
    static List<string> _serialPorts = new List<string>();

    public SerialPort _serialPort;

    public string _selectedPortName;

    public Vector3Int _hsvValues;

    public int length;

    [SerializeField] private float animationDuration = 5f;
    [SerializeField] private Color[] exampleColors;
    [SerializeField] private bool queueExampleOnStart;

    private Queue<Vector3Int> HSVQueue;
    public bool isAnimationPlaying = false;

    // Start is called before the first frame update
    private void Start()
    {
        _serialPorts = SerialPort.GetPortNames().ToList();
        foreach(var port in _serialPorts)
        {
            Debug.Log("Found Port:" + port);
        }

        HSVQueue = new Queue<Vector3Int>();

        if (queueExampleOnStart)
        {
            queueExample();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            selectPort(_selectedPortName);
        }
        if (Input.GetKeyDown(KeyCode.F11))
        {
            addAnimationToQueue(_hsvValues);
        }
        if (Input.GetKeyDown(KeyCode.F10))
        {
            closeComPort();
        }
        if (Input.GetKeyDown(KeyCode.F9))
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
            _serialPort = new SerialPort(selectedPort, 9600);
            _serialPort.Open();
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
        Debug.Log(message);
        if (_serialPort != null)
        {
            _serialPort.Write(message);

            Debug.Log("serialPort != null");

            /*
            while (_serialPort.ReadTo("\n") != "finished")
            {
                Debug.Log("Waiting for finished-Signal");
                yield return new WaitForSeconds(5f);
                Debug.Log("another loop-di-loop");
            }
            */

            yield return new WaitForSeconds(animationDuration);
        }

        Debug.Log("finished Animation");
        yield return null;
        isAnimationPlaying = false;
    }

    private void closeComPort()
    {
        if(_serialPort != null)
            _serialPort.Close();
    }
}
