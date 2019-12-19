using System.Linq;

namespace RattusEngine.Fixtures
{
    public class GetRooms : QueryFixture<RoomView>
    {
        protected override RoomView[] Data => Common.App.RoomController.GetRooms();
        protected override string[] Fields => new string[] { nameof(RoomView.Name), nameof(RoomView.Action) };
    }
}