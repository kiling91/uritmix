using DataAccess.Auth;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Model.Auth;
using Model.Person;
using Service.Security.PasswordHasher;

namespace Service.HostedService.InitAdmin;

public class InitAdminService : IInitAdminService
{
    private readonly ILogger<InitAdminService> _logger;
    private readonly IOptions<InitAdminOptions> _options;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IPersonRepository _personRepository;

    public InitAdminService(ILogger<InitAdminService> logger, IPersonRepository personRepository,
        IPasswordHasher passwordHasher, IOptions<InitAdminOptions> options)
    {
        _logger = logger;
        _personRepository = personRepository;
        _passwordHasher = passwordHasher;
        _options = options;
    }

    public async Task DoWork(CancellationToken stoppingToken)
    {
        var adminCount = await _personRepository.RoleCount(AuthRole.Admin);
        if (adminCount > 0)
            return;


        var opt = _options.Value;
        var salt = Guid.NewGuid().ToByteArray();
        var person = await _personRepository.Create(new PersonModel
        {
            FirstName = opt.FirstName,
            LastName = opt.LastName,
            HaveAuth = true,
            Auth = new AuthModel
            {
                Role = AuthRole.Admin,
                Status = AuthStatus.Activated,
                Email = opt.Email,
                Hash = _passwordHasher.Hash(opt.Password, salt),
                Salt = salt
            }
        });

        _logger.LogInformation("Create admin: {Email}", person.Auth!.Email);
    }
}