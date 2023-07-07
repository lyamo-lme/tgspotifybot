using Microsoft.Extensions.Logging;

namespace SpotifyTelegramBot.Data;

public class UowRepository : IDisposable
{
    private readonly ILogger<UowRepository>? _logger;
    private readonly AppDbContext _appDbContext;
    private readonly RepositoryFactory _repositoryFactory;
    private Dictionary<string, object>? _repositories;

    public UowRepository(ILogger<UowRepository> logger, RepositoryFactory repositoryFactory,
        AppDbContext appDbContext)
    {
        _logger = logger;
        _repositoryFactory = new RepositoryFactory();
        _appDbContext = appDbContext;
    }

    public UowRepository(RepositoryFactory repositoryFactory,
        AppDbContext appDbContext)
    {
        _repositoryFactory = new RepositoryFactory();
        _appDbContext = appDbContext;
    }

    public void Dispose()
    {
        _appDbContext.Dispose();
    }

    public GenericRepository<T> GenericRepository<T>() where T : class
    {
        if (_repositories == null)
        {
            _repositories = new Dictionary<string, object>();
        }

        var type = typeof(T).Name;
        if (!_repositories.ContainsKey(type))
        {
            var repository = _repositoryFactory.Instance<T>(_appDbContext);
            _repositories.Add(type, repository);
        }

        return (GenericRepository<T>)_repositories[type];
    }

    public Task SaveAsync()
    {
        try
        {
            return _appDbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.Log(LogLevel.Critical, e, e.Message);
            throw;
        }
    }
}