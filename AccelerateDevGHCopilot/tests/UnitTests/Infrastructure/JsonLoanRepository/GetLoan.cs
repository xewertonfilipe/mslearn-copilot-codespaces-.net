using System.Collections.Generic;
using System.IO;
using NSubstitute;
using Xunit;
using Library.ApplicationCore;
using Library.ApplicationCore.Entities;
using Library.Infrastructure.Data;
using Microsoft.Extensions.Configuration;

namespace UnitTests.Infrastructure.JsonLoanRepositoryTests;

public class GetLoanTest
{
    private readonly ILoanRepository _mockLoanRepository;
    private readonly JsonLoanRepository _jsonLoanRepository;
    private readonly IConfiguration _configuration;
    private readonly JsonData _jsonData;

    public GetLoanTest()
    {
        _mockLoanRepository = Substitute.For<ILoanRepository>();

        string basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "src", "Library.Console", "Json"));

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "JsonPaths:Authors", Path.Combine(basePath, "Authors.json") },
                { "JsonPaths:Books", Path.Combine(basePath, "Books.json") },
                { "JsonPaths:BookItems", Path.Combine(basePath, "BookItems.json") },
                { "JsonPaths:Patrons", Path.Combine(basePath, "Patrons.json") },
                { "JsonPaths:Loans", Path.Combine(basePath, "Loans.json") }
            })
            .Build();

        _jsonData = new JsonData(_configuration);
        _jsonLoanRepository = new JsonLoanRepository(_jsonData);
    }

    [Fact(DisplayName = "JsonLoanRepository.GetLoan: Returns loan when loan id exists in data")]
    public async Task GetLoan_ReturnsLoan_WhenLoanIdExists()
    {
        // Arrange
        int expectedLoanId = 1;
        var expectedLoan = new Loan {
             Id = expectedLoanId,
             BookItemId = 17,
             PatronId = 22,
             LoanDate = DateTime.Parse("2023-12-08T00:40:43.1808862"),
             DueDate = DateTime.Parse("2023-12-22T00:40:43.1808862"),
             ReturnDate = null
         };
        _mockLoanRepository.GetLoan(expectedLoanId).Returns(expectedLoan);

        // Act
        Loan? actualLoan = await _jsonLoanRepository.GetLoan(expectedLoanId);

        // Assert
        Assert.NotNull(actualLoan);
        Assert.Equal(expectedLoan.Id, actualLoan.Id);
    }

    [Fact(DisplayName = "JsonLoanRepository.GetLoan: Returns null when loan id does not exist in data")]
    public async Task GetLoan_ReturnsNull_WhenLoanIdDoesNotExist()
    {
        // Arrange
        int nonExistentLoanId = 999; // Assuming this ID does not exist in the data

        _mockLoanRepository.GetLoan(nonExistentLoanId).Returns((Loan?)null);

        // Act
         var actualLoan = await _jsonLoanRepository.GetLoan(nonExistentLoanId);

        // Assert
        Assert.Null(actualLoan);
    }
}
