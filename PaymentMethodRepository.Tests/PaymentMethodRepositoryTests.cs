
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.IO;
using System.Threading.Tasks;

namespace SqlAlwaysEncrypted.PaymentMethod.Data.Sql.Tests
{
    using AutoFixture;
    using FluentAssertions;

    using Models;


    [TestClass]
    public class PaymentMethodRepositoryTests
    {
        static string _connectionString;
        static IFixture _fixture;
        static IPaymentMethodRepository _paymentMethodRepo;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            IConfiguration config = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appSettings.json", optional: false, reloadOnChange: false)
               .AddUserSecrets<PaymentMethodRepositoryTests>()
               .Build();

            _connectionString = config.GetSection("ConnectionStrings:PaymentMethodDb").Value;
            _fixture = new Fixture();
            _paymentMethodRepo = new PaymentMethodRepository(_connectionString);
            CleanUp();
        }

        public static void CleanUp()
        {
            try
            {
                _paymentMethodRepo.DeleteAllAsync();
            }
            catch
            {
                //
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
        }

        // Usually separate unit-tests from integration tests.
        [TestMethod]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow(null)]
        [DataRow("\t")]
        public void PaymentMethodRepository_NullConnectionString_ThrowArgumentNullException(string connectionString)
        {
            Action act = () => new PaymentMethodRepository(connectionString);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be(nameof(connectionString));
        }


        [TestMethod]
        [TestCategory("Integration")]
        public async Task AddAsync_ValidPaymentMethod_Inserted()
        {
            // Arrange
            PaymentMethod pm = GeneratePaymentMethod();

            // Act
            PaymentMethod insertedPm = await _paymentMethodRepo.AddAsync(pm);

            // Assert
            insertedPm.Should().NotBeNull();
            insertedPm.Id.Should().BeGreaterThan(0);
            insertedPm.ExternalId.Should().NotBeEmpty();
        }

        [TestMethod]
        [TestCategory("Integration")]
        public async Task GetAsync_ValidPaymentMethod_Inserted()
        {
            // Arrange
            PaymentMethod pm = GeneratePaymentMethod();

            PaymentMethod insertedPm = await _paymentMethodRepo.AddAsync(pm);

            // Act
            var returnedPm = await _paymentMethodRepo.GetAsync(insertedPm.Id);

            // Assert
            returnedPm.Should().BeEquivalentTo(pm, opt => 
                opt.Excluding(x => x.ExternalId)
                    .Excluding(x => x.Id)
                    .Excluding(x => x.DateCreated));
        }


        private PaymentMethod GeneratePaymentMethod()
        {
            return _fixture.Build<PaymentMethod>()
                .Without(c => c.Id)
                .Without(c => c.ExternalId)
                .With(c => c.AccountNumber, "4444333322221111")
                .With(c => c.FullName, "Testy McTestFace")
                .Create();
        }

    }
}
