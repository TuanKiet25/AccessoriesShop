using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Domain.Entities
{
	public class Rating : BaseEntity
	{
		public Guid ProductId { get; set; }
		public Guid AccountId { get; set; }

		[Range(1, 5)]
		public int Star { get; set; }

		[MaxLength(1000)]
		public string? Comment { get; set; }

		public bool IsVisible { get; set; } = true;
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime? UpdatedAt { get; set; }

		public virtual Product Product { get; set; } = null!;
		public virtual Account Account { get; set; } = null!;
	}
}
