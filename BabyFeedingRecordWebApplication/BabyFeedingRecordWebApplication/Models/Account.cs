namespace BabyFeedingRecordWebApplication.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string LineID { get; set; }

        public string? status;

        public string? currCtrl;
        public string? currActn;
    }

    public class Group
    {
        public int Id { get; set; }
        public string? LineId { get; set; }
    }

}
