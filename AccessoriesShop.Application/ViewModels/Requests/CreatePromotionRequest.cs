using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Application.ViewModels.Requests
{
	public class CreatePromotionRequest
	{
		[Required]
		public Guid ProductId { get; set; }

		[Required]
		[MaxLength(200)]
		public string Name { get; set; } = string.Empty;

		[MaxLength(1000)]
		public string? Description { get; set; }

		[Required]
		public string DiscountType { get; set; } = "Percent";

		[Range(0, double.MaxValue)]
		public decimal DiscountValue { get; set; }

		public decimal? MaxDiscountAmount { get; set; }
		public decimal? MinOrderValue { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
	}
}
