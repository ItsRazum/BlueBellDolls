using BlueBellDolls.Common.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlueBellDolls.Common.Types
{
    public abstract class Cat : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateOnly BirthDay { get; set; }
        public bool IsMale { get; set; }
        public string Description { get; set; }
        public List<string> Photos { get; set; }
    }
}
