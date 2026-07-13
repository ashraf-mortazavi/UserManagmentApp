namespace ManageUsers.Application.DTOs
{
    public class GetRegionsByAreaResponse : BaseResponse
    {
        public List<RegionDto> Regions { get; set; } = new();
    }

    public record RegionDto(int Id, string Name);
}
