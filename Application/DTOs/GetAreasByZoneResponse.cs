namespace ManageUsers.Application.DTOs
{
    public class GetAreasByZoneResponse : BaseResponse
    {
        public List<AreaDto> Areas { get; set; } = new();
    }
}
