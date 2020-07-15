namespace lasterMark.DataAccess.DTOs
{
    public class UserFiles
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        public byte[] BackgroundImageData { get; set; }

        public byte[] EzdImageData { get; set; }

        public byte[] ReadyMadeImageData { get; set; }
    }
}
