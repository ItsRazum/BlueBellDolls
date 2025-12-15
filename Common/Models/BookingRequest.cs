using BlueBellDolls.Common.Types;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueBellDolls.Common.Models
{
    [Table("booking_requests")]
    public class BookingRequest : EntityBase
    {
        [Column("customer_name")]
        public string CustomerName { get; set; }

        [Column("customer_phone")]
        public string CustomerPhone { get; set; }

        [Column("kitten_id")]
        [ForeignKey(nameof(Kitten))]
        public int KittenId { get; set; }
        public Kitten Kitten { get; set; }

        [Column("curator_telegram_id")]
        public long? CuratorTelegramId { get; set; }

        [Column("is_processed")]
        public bool IsProcessed { get; set; }
    }
}
