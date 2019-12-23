namespace RattusEngine
{
    public enum RoomCreateStatus
    {
        OK,
        AlreadyInRoom,
        DuplicateName
    }

    public enum RoomJoinStatus
    {
        AlreadyInRoom,
        OK,
        RoomNotFound,
        RoomIsFull
    }

    public enum RoomLeaveStatus
    {
        OK,
        NotInRoom,
        GameInProgress
    }

    public enum GameStartStatus
    {
        OK,
        NotAnOwner,
        NotEnoughPlayers,
        GameInProgress,
        NotInRoom
    }

    public enum RoomViewStatus
    {
        Joinable,
        Full,
        InRoom,
        InGame
        
    }
}