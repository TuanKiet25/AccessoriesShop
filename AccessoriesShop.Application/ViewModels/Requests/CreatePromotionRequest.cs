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
		public Guid ProductId { get; set; }
		public string Name { get; set; } = null!;
		public decimal DiscountValue { get; set; }
		public bool IsPercentage { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public bool IsActive { get; set; }
	}
}
