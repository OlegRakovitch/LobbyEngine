namespace RattusEngine.Fixtures
{
    public class GivenRooms
    {
        public string Name;

        public void Execute()
        {
            Common.App.Context.Storage.Save(new Room() { Name = Name });
        }
    }
}