using BlueBellDolls.Common.Types;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueBellDolls.Common.Models
{
    [Table("telegram_photos")]
    public class TelegramPhoto : EntityBase
    {
        [Column("file_id")]
        public string FileId { get; set; }

        [Column("entity_photo_id")]
        [ForeignKey(nameof(EntityPhoto))]
        public int EntityPhotoId { get; set; }
        public EntityPhoto EntityPhoto { get; set; }
    }
}
