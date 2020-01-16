using System.Linq;
using LobbyEngine.Models;
using LobbyEngine.Views;

namespace LobbyEngine.Fixtures
{
    public class GetRooms : QueryFixture<RoomView>
    {
        public GetRooms(string username)
        {
            Common.Context.CurrentUser = Common.Engine.Context.Storage.Get<User>(u => u.Username == username).GetAwaiter().GetResult().Single();
        }
        protected override RoomView[] Data => Common.Engine.RoomController.GetRooms().GetAwaiter().GetResult();
        protected override string[] Fields => new string[] { nameof(RoomView.Name), nameof(RoomView.Status), nameof(RoomView.GameType) };
    }
}