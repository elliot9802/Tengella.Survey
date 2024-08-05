namespace Tengella.Survey.Data.Models
{
    public class DistributionList
    {
        public int DistributionListId { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Recipient> Recipients { get; set; } = [];
    }
}