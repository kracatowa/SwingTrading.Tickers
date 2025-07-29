using Microsoft.EntityFrameworkCore;

namespace Tickers.Infrastructure.Repositories
{
    public interface IRepository
    {
        DbContext GetDbContext();
    }
}
