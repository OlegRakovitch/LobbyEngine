namespace RattusEngine.Fixtures
{
    public class GivenPlayers
    {
        public string Username;

        public void Execute()
        {
            Common.App.UserController.CreateUser(Username);
        }
    }
}