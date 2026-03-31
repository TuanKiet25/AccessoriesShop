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
		public Guid ProductId { get; set; }
		public Guid AccountId { get; set; }
		public int Star { get; set; }
		public string? Comment { get; set; }
	}
}
