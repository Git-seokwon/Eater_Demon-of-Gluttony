using System;

public static class StaticEventHandler
{
    public static event Action<RoomChangedEventArgs> OnRoomEntered;

    public static void CallRoomEnterEvent(MainRoom mainRoom)
    {
        OnRoomEntered?.Invoke(new RoomChangedEventArgs() { mainRoom = mainRoom });
    }

    public static event Action<RoomChangedEventArgs> OnRoomExited;

    public static void CallRoomExitEvent(MainRoom mainRoom)
    {
        OnRoomExited?.Invoke(new RoomChangedEventArgs() { mainRoom = mainRoom });
    }
}

public class RoomChangedEventArgs : EventArgs
{
    public MainRoom mainRoom;
}