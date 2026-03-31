using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Application.ViewModels.Requests
{
	public class CreateRatingRequest
	{
		[Required]
		public Guid ProductId { get; set; }

		[Range(1, 5)]
		public int Star { get; set; }

		[MaxLength(1000)]
		public string? Comment { get; set; }
	}
}
