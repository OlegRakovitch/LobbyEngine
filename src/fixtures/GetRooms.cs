using System.Linq;
using RattusEngine.Models;
using RattusEngine.Views;

namespace RattusEngine.Fixtures
{
    public class GetRooms : QueryFixture<RoomView>
    {
        public GetRooms(string username)
        {
            Common.Context.CurrentUser = Common.App.Context.Storage.Get<User>().Single(u => u.Username == username);
        }
        protected override RoomView[] Data => Common.App.RoomController.GetRooms();
        protected override string[] Fields => new string[] { nameof(RoomView.Name), nameof(RoomView.Status) };
    }
}