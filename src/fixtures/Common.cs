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
    }
}