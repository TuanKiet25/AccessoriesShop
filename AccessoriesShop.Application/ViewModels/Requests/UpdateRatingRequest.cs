using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Application.ViewModels.Requests
{
	public class UpdateRatingRequest
	{
		public int Star { get; set; }
		public string? Comment { get; set; }
	}
}
