using System;
using System.ComponentModel.DataAnnotations;

namespace TicTacToeCSharpPlayground.Core.Models
{
    public abstract class StandardEntity
    {
        [Key] public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
