namespace DataAccess.DTOs
{
    public class UserFiles
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public byte[] BackgroundImageData { get; set; }

        public byte[] EzdImageData { get; set; }

        public byte[] ReadyMadeImageData { get; set; }
    }
}
