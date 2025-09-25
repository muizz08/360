using System;
using System.Collections.Generic;

[Serializable]
public class RoomResponse
{
    public List<RoomData> data;
}

[Serializable]
public class RoomData
{
    public int id;
    public string Name;
    public List<ButtonData> Button;
    public List<ImageData> Image;
}

    [Serializable]
public class ButtonData
{
    public int id;
    public string Label;
    public string ActionType;
    public string ActionData;

    // ✅ sekarang Position jadi object
    public PositionData Position;
    // kalau ada target room
    public TargetRoomData Target_Room;
}

[Serializable]
public class PositionData
{
    public float PositionX;
    public float PositionY;
    public float Distance;
}

[Serializable]
public class TargetRoomData
{
    public int id;
    public string Name;
}

[Serializable]
public class ImageData
{
    public int id;
    public string name;
    public string url;

}
