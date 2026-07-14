namespace ManageUsers.Application.DTOs
{
    public class GetZonesResponse : BaseResponse
    {
        public List<AreaDto> Areas { get; set; } = new();
    }

    public record AreaDto(int Id, string Name);
}
