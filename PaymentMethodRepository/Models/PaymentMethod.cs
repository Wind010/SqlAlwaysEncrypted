
using System;
using System.Data;

namespace SqlAlwaysEncrypted.PaymentMethod.Data.Sql.Models
{
    using Dapper;
    using Dapper.Contrib.Extensions;

    using static Dapper.SqlMapper;

    [Table("PaymentMethod")]
    public class PaymentMethod
    {
        [Key]
        public int Id { get; set; }

        [Computed]
        public Guid ExternalId { get; set; }

        public string AccountNumber { get; set; }
        public int ExpirationMonth { get; set; }
        public int ExpirationYear { get; set; }
        public string FullName { get; set; }
        public int PaymentType { get; set; }

        [Computed]
        public DateTime DateCreated { get; set; }

        public IDynamicParameters ToDynamicParameters()
        {
            var dp = new DynamicParameters();

            dp.Add(nameof(ExpirationMonth), dbType: DbType.Int32
                , direction: ParameterDirection.InputOutput, value: ExpirationMonth);

            dp.Add(nameof(ExpirationYear), dbType: DbType.Int32
                , direction: ParameterDirection.InputOutput, value: ExpirationYear);

            dp.Add(nameof(PaymentType), dbType: DbType.Int32
                , direction: ParameterDirection.InputOutput, value: PaymentType);

            // Encrypted fields
            dp.Add(nameof(AccountNumber), dbType: DbType.AnsiString
                , direction: ParameterDirection.InputOutput, size: 36, value: AccountNumber);

            dp.Add(nameof(FullName), dbType: DbType.String
                , direction: ParameterDirection.InputOutput, size: 255, value: FullName);

            return dp;
        }
    }
}
