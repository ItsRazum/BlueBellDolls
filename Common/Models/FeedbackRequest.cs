using BlueBellDolls.Common.Types;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueBellDolls.Common.Models
{
    [Table("feedback_requests")]
    public class FeedbackRequest : EntityBase
    {
        [Column("name")]
        public string Name { get; set; }

        [Column("phone")]
        public string Phone { get; set; }

        [Column("is_processed")]
        public bool IsProcessed { get; set; }
    }
}
