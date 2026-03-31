using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Domain.Entities
{
	public class Promotion : BaseEntity
	{
		public Guid ProductId { get; set; }

		[MaxLength(200)]
		public string Name { get; set; } = string.Empty;

		[MaxLength(1000)]
		public string? Description { get; set; }

		[MaxLength(50)]
		public string DiscountType { get; set; } = "Percent";

		public decimal DiscountValue { get; set; }
		public decimal? MaxDiscountAmount { get; set; }
		public decimal? MinOrderValue { get; set; }

		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public bool IsActive { get; set; } = true;

		public virtual Product Product { get; set; } = null!;
	}
}
