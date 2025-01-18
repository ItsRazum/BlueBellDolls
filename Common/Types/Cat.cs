using BlueBellDolls.Common.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace BlueBellDolls.Common.Types
{
    public abstract class Cat : IEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateOnly BirthDay { get; set; }
        public bool IsMale { get; set; }
        public string Description { get; set; }
        public List<string> Photos { get; set; }
    }
}
