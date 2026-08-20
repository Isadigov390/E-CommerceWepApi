namespace Shopping.Application.DTOs.ReviewDTOs.Responses
{
    public class ReviewResponseDTO
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
    }
}
