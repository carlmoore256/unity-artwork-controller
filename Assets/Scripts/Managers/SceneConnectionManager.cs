using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Colyseus;
using Network;
using Network.StateSchema;
using UnityEngine;

public class SceneRoomStateOptions { }

// handles making the colyseus connection and establishing state of objects
public class SceneConnectionManager : MonoBehaviour
{
    public static SceneConnectionManager Instance { get; private set; }
    public ColyseusClient Client { get; private set; }
    public ColyseusRoom<SceneState> Room { get; private set; }
    public SceneState SceneState => Room?.State;
    public string RoomId { get; private set; }
    public string ServerAddress = "ws://localhost:2567";
    public bool IsConnected =>
        Client != null && Room.colyseusConnection != null && Room.colyseusConnection.IsOpen;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Client = new ColyseusClient(ServerAddress);
    }

    public void Disconnect()
    {
        if (Client == null)
        {
            Debug.LogWarning("Client is already null");
            return;
        }
        else if (Room == null)
        {
            Debug.LogWarning("Room is already null");
            return;
        }
        Room.Leave();
        Room = null;
    }

    public async Task JoinRoom(
        string roomId,
        SceneRoomStateOptions joinRoomOptions,
        Action<ColyseusRoom<SceneState>> onFirstStateChange
    )
    {
        var room = await Client.JoinById<SceneState>(
            roomId,
            new Dictionary<string, object>()
            {
                { "joinOptions", joinRoomOptions.ConvertToDictionary() }
            }
        );
        RegisterRoom(room, onFirstStateChange);
        // EventBus.Publish(new RoomJoinedEvent(room));
    }

    private void RegisterRoom(
        ColyseusRoom<SceneState> room,
        Action<ColyseusRoom<SceneState>> onFirstStateChange
    )
    {
        ColyseusRoom<SceneState>.RoomOnStateChangeEventHandler onFirstStateChangeWrapper = null;
        onFirstStateChangeWrapper = (SceneState state, bool isFirstState) =>
        {
            if (isFirstState)
            {
                // Invoke the user-provided callback then remove the handler
                onFirstStateChange.Invoke(room);
                room.OnStateChange -= onFirstStateChangeWrapper;
            }
        };
        room.OnStateChange += onFirstStateChangeWrapper;
    }

    public void LeaveRoom()
    {
        if (Room == null)
            return;

        Room.Leave();
        Room = null;
    }
}
