using AccessoriesShop.Application.ViewModels.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Application.IServices
{
    /// <summary>
    /// Service for managing stock reservations during order lifecycle
    /// Handles: reserve on order creation, revert on payment failure/cancellation
    /// </summary>
    public interface IStockReservationService
    {
        /// <summary>
        /// Reserve stock for an order (reduce stock quantities)
        /// Called when order is created
        /// </summary>
        Task<ServiceResult<string>> ReserveStockAsync(Guid orderId);

        /// <summary>
        /// Confirm stock reservation (no action needed - stock already reserved)
        /// Called when payment succeeds
        /// </summary>
        Task<ServiceResult<string>> ConfirmStockReservationAsync(Guid orderId);

        /// <summary>
        /// Revert stock reservation (restore stock quantities)
        /// Called when payment fails, is cancelled, or expires
        /// </summary>
        Task<ServiceResult<string>> RevertStockReservationAsync(Guid orderId);
    }
}
