using System;
using RattusEngine.Models;
using RattusEngine.Tests;

namespace RattusEngine.Fixtures
{
    public class Common
    {
        public static LobbyEngine Engine;
        public static MemoryStorage Storage;
        public static ModifiableContext Context;
        public bool InitializeApplication()
        {
            try
            {
                Storage = new MemoryStorage();
                Context = new ModifiableContext()
                {
                    Storage = Storage
                };
                Engine = new LobbyEngine(Context, new GameStarter());
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while creating new application: ${ex.Message}");
                return false;
            }
        }

        public bool ClearRooms()
        {
            try
            {
                Engine.Context.Storage.DeleteAll<Room>();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while deleting all rooms: ${ex.Message}");
                return false;
            }
        }
    }
}