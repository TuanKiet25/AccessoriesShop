using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Application.ViewModels.Responses
{
	public class RatingResponse
	{
		public Guid Id { get; set; }
		public Guid ProductId { get; set; }
		public Guid AccountId { get; set; }
		public int Star { get; set; }
		public string? Comment { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}
