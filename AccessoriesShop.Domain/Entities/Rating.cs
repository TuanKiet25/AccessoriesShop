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
		public Guid Id { get; set; }
		public Guid ProductId { get; set; }
		public Guid AccountId { get; set; }
		public int Star { get; set; }
		public string? Comment { get; set; }
		public DateTime CreatedAt { get; set; }

		public Product Product { get; set; } = null!;
		public Account Account { get; set; } = null!;
	}
}
