using Microsoft.VisualStudio.TestTools.UnitTesting;
using Data.Repository.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Models.Address;
using Microsoft.Data.SqlClient;
using Moq;

namespace Testing.Tests
{
    [TestClass()]
    public class AddressRepo
    {
        [TestMethod]
        public async Task AddAddressAsync_ValidAddress_ReturnsId()
        {
            // Arrange
            var address = new AddressModel
            {
                Street = "123 Main St",
                City = "Example City",
                PhoneNumber = "123-456-7890",
                State = "Example State",
                ZipCode = "12345"
            };
            string userId = "user123";

            // Mock SqlConnection and SqlCommand
            var mockConnection = new Mock<SqlConnection>();
            var mockCommand = new Mock<SqlCommand>();

            mockConnection.Setup(c => c.OpenAsync()).Returns(Task.CompletedTask);
            mockCommand.Setup(c => c.ExecuteScalarAsync()).ReturnsAsync(1); // Mock the return value of SCOPE_IDENTITY()

            mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);

            var service = new AddressRepository(mockConnection.Object);

            // Act
            var result = await service.AddAddressAsync(address, userId);

            // Assert
            Assert.AreEqual(1, result); // Assert that the returned id matches the expected value
        }

        [TestMethod]
        public async Task AddAddressAsync_ExceptionThrown_ReturnsMinusOne()
        {
            // Arrange
            var address = new AddressModel
            {
            };
            string userId = "user123";

            var mockConnection = new Mock<SqlConnection>();
            var mockCommand = new Mock<SqlCommand>();

            mockConnection.Setup(c => c.OpenAsync()).ThrowsAsync(new Exception());

            var service = new AddressRepository(mockConnection.Object);

            var result = await service.AddAddressAsync(address, userId);
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public async Task UpdateAddressAsync_ValidIdAndAddress_ReturnsTrue()
        {
            // Arrange
            int id = 1;
            var address = new AddressModel
            {
                Street = "123 Main St",
                City = "Example City",
                PhoneNumber = "123-456-7890",
                State = "Example State",
                ZipCode = "12345"
            };

            var expectedRowsAffected = 1;

            // Mock SqlConnection and SqlCommand
            var mockConnection = new Mock<SqlConnection>();
            var mockCommand = new Mock<SqlCommand>();

            mockConnection.Setup(c => c.OpenAsync()).Returns(Task.CompletedTask);
            mockCommand.Setup(c => c.ExecuteNonQueryAsync()).ReturnsAsync(expectedRowsAffected); // Mock the return value of ExecuteNonQueryAsync

            mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);

            var service = new AddressRepository(mockConnection.Object);

            // Act
            var result = await service.UpdateAddressAsync(id, address);

            // Assert
            Assert.IsTrue(result); // Assert that the method returns true
            mockCommand.Verify(); // Verify that SqlCommand was called as expected
        }

        [TestMethod]
        public async Task UpdateAddressAsync_ExceptionThrown_ReturnsFalse()
        {
            // Arrange
            int id = 1;
            var address = new AddressModel
            {
                Street = "123 Main St",
                City = "Example City",
                PhoneNumber = "123-456-7890",
                State = "Example State",
                ZipCode = "12345"
            };

            var mockConnection = new Mock<SqlConnection>();
            var mockCommand = new Mock<SqlCommand>();

            mockConnection.Setup(c => c.OpenAsync()).ThrowsAsync(new Exception());

            var service = new AddressRepository(mockConnection.Object);

            // Act
            var result = await service.UpdateAddressAsync(id, address);

            // Assert
            Assert.IsFalse(result); // Assert that the method returns false indicating failure
        }

        [TestMethod]
        public async Task GetAddressesByUserIdAsync_ReturnsAddresses()
        {
            // Arrange
            var userId = "testUserId";
            var expectedAddresses = new List<AddressModel>
    {
        new AddressModel { AddressId = 1, Street = "123 Main St", City = "Example City", PhoneNumber = "123-456-7890", State = "Example State", ZipCode = "12345" },
        new AddressModel { AddressId = 2, Street = "456 Elm St", City = "Test City", PhoneNumber = "987-654-3210", State = "Test State", ZipCode = "54321" }
    };

            var mockDataReader = new Mock<SqlDataReader>();
            mockDataReader.SetupSequence(r => r.ReadAsync())
                .ReturnsAsync(true)
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            mockDataReader.Setup(r => r["AddressId"]).Returns(1);
            mockDataReader.Setup(r => r["Street"]).Returns("123 Main St");
            mockDataReader.Setup(r => r["City"]).Returns("Example City");
            mockDataReader.Setup(r => r["PhoneNumber"]).Returns("123-456-7890");
            mockDataReader.Setup(r => r["States"]).Returns("Example State");
            mockDataReader.Setup(r => r["ZipCode"]).Returns("12345");

            mockDataReader.Setup(r => r["AddressId"]).Returns(2);
            mockDataReader.Setup(r => r["Street"]).Returns("456 Elm St");
            mockDataReader.Setup(r => r["City"]).Returns("Test City");
            mockDataReader.Setup(r => r["PhoneNumber"]).Returns("987-654-3210");
            mockDataReader.Setup(r => r["States"]).Returns("Test State");
            mockDataReader.Setup(r => r["ZipCode"]).Returns("54321");

            var mockCommand = new Mock<SqlCommand>();
            mockCommand.Setup(c => c.ExecuteReaderAsync()).ReturnsAsync(mockDataReader.Object);
            mockCommand.Setup(c => c.Parameters.AddWithValue("@userId", userId)).Verifiable();

            var mockConnection = new Mock<SqlConnection>();
            mockConnection.Setup(c => c.OpenAsync()).Returns(Task.CompletedTask);
            mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);

            var repository = new AddressRepository(mockConnection.Object);

            // Act
            var addresses = await repository.GetAddressesByUserIdAsync(userId);

            // Assert
            CollectionAssert.AreEqual(expectedAddresses, addresses, new AddressModelComparer());
            mockCommand.Verify(); // Verify that SqlCommand was called as expected
        }

        [TestMethod]
        public async Task GetAddressesByUserIdAsync_NoAddresses_ReturnsEmptyList()
        {
            // Arrange
            var userId = "testUserId";

            var mockDataReader = new Mock<SqlDataReader>();
            mockDataReader.SetupSequence(r => r.ReadAsync())
                .ReturnsAsync(false); // Simulate no rows returned

            var mockCommand = new Mock<SqlCommand>();
            mockCommand.Setup(c => c.ExecuteReaderAsync()).ReturnsAsync(mockDataReader.Object);
            mockCommand.Setup(c => c.Parameters.AddWithValue("@userId", userId)).Verifiable();

            var mockConnection = new Mock<SqlConnection>();
            mockConnection.Setup(c => c.OpenAsync()).Returns(Task.CompletedTask);
            mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);

            var repository = new AddressRepository(mockConnection.Object);

            // Act
            var addresses = await repository.GetAddressesByUserIdAsync(userId);

            // Assert
            Assert.IsNotNull(addresses); // Ensure the returned list is not null
            Assert.AreEqual(0, addresses.Count); // Ensure the returned list is empty
            mockCommand.Verify(); // Verify that SqlCommand was called as expected
        }



    }
}