using System.Collections.Generic;
using System.Threading.Tasks;

namespace SqlAlwaysEncrypted.PaymentMethod.Data.Sql
{
    using Models;

    public interface IPaymentMethodRepository
    {
        /// <summary>
        /// Add a payment method.
        /// </summary>
        /// <param name="paymentMethod"></param>
        /// <returns><see cref="PaymentMethod"/>Populated with Id and ExternalId</returns>
        Task<PaymentMethod> AddAsync(PaymentMethod paymentMethod);

        Task<bool> UpdateAsync(PaymentMethod paymentMethod);

        Task<PaymentMethod> GetAsync(int id);

        Task<List<PaymentMethod>> GetAllAsync();

        Task<bool> DeleteAsync(int id);

        Task<bool> DeleteAllAsync();

    }
}
