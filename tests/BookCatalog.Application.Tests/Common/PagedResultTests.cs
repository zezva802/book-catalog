using BookCatalog.Application.Common;

namespace BookCatalog.Application.Tests.Common;

public class PagedResultTests
{
    [Theory]
    [InlineData(0, 10, 0)]
    [InlineData(1, 10, 1)]
    [InlineData(10, 10, 1)]
    [InlineData(11, 10, 2)]
    public void TotalPages_IsTotalCountDividedByPageSize_RoundedUp(
        int totalCount, int pageSize, int expectedTotalPages)
    {
        var result = new PagedResult<int>([], totalCount, 1, pageSize);
        Assert.Equal(expectedTotalPages, result.TotalPages);
    }
}

