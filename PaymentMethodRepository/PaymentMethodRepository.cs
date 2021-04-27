
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SqlAlwaysEncrypted.PaymentMethod.Data.Sql
{
    using Dapper;
    using Dapper.Contrib.Extensions;
    using Models;

    public class PaymentMethodRepository : IPaymentMethodRepository
    {
        string _connectionString;

        public PaymentMethodRepository(string connectionString)
        {
            _connectionString = string.IsNullOrWhiteSpace(connectionString)
                ? throw new ArgumentNullException(connectionString) : connectionString;
        }

        public async Task<PaymentMethod> AddAsync(PaymentMethod paymentMethod)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // If we didn't have encrypted columns, just use Dapper.Contrib:
            //return await connection.InsertAsync(paymentMethod);

            // If we just wanted to return the Id
            //const string query = @"INSERT INTO PaymentMethod (
            //    AccountNumber, ExpirationMonth, ExpirationYear, FullName, PaymentType)
            //    OUTPUT Inserted.Id
            //    VALUES(@AccountNumber, @ExpirationMonth, @ExpirationYear, @FullName, @PaymentType)";

            //return Convert.ToInt32(await connection.ExecuteScalarAsync(query
            //  , paymentMethod.ToDynamicParameters()));

            const string query = @"INSERT INTO PaymentMethod (
                AccountNumber, ExpirationMonth, ExpirationYear, FullName, PaymentType)
                OUTPUT Inserted.Id, Inserted.ExternalId
                VALUES(@AccountNumber, @ExpirationMonth, @ExpirationYear, @FullName, @PaymentType)";

            var insertedPm = (await connection.QueryAsync<PaymentMethod>(query, paymentMethod.ToDynamicParameters()))
                .FirstOrDefault();

            paymentMethod.Id = insertedPm.Id;
            paymentMethod.ExternalId = insertedPm.ExternalId;

            return paymentMethod;
        }

        public async Task<bool> DeleteAllAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return await connection.DeleteAllAsync<PaymentMethod>();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return await connection.DeleteAsync(new PaymentMethod { Id = id });
        }

        public async Task<List<PaymentMethod>> GetAllAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return (await connection.GetAllAsync<PaymentMethod>()).ToList();
        }

        public async Task<PaymentMethod> GetAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return await connection.GetAsync<PaymentMethod>(id);
        }

        public async Task<bool> UpdateAsync(PaymentMethod paymentMethod)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return await connection.UpdateAsync(paymentMethod);
        }
    }
}
