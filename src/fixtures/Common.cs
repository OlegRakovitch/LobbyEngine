using System;
using System.Linq;

namespace RattusEngine.Fixtures
{
    public class Common
    {
        public static Application App;
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
                App = new Application(Context);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while creating new application: ${ex.Message}");
                return false;
            }
        }
        public bool SetLoggedInUser(string username)
        {
            try
            {
                Context.CurrentUser = App.Context.Storage.Get<User>().Where(user => user.Username == username).Single();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while settings logged in user: ${ex.Message}");
                return false;
            }
            
        }

        public bool ClearRooms()
        {
            try
            {
                App.Context.Storage.DeleteAll<Room>();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while deleting all rooms: ${ex.Message}");
                return false;
            }
        }

        public bool StartGameInRoom(string roomName)
        {
            try
            {
                return App.RoomController.StartGame(roomName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while starting game in room {roomName}: ${ex.Message}");
                return false;
            }
        }
    }
}