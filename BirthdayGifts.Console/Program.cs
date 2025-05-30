using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BirthdayGifts.Services.Interfaces.Employee;
using BirthdayGifts.Services.Interfaces.Gift;
using BirthdayGifts.Services.Interfaces.VotingSession;
using BirthdayGifts.Services.Interfaces.Vote;
using BirthdayGifts.Services.Interfaces.Authentication;
using BirthdayGifts.Services.DTOs.Employee;
using BirthdayGifts.Services.DTOs.Gift;
using BirthdayGifts.Services.DTOs.VotingSession;
using BirthdayGifts.Services.DTOs.Vote;
using BirthdayGifts.Services.DTOs.Authentication;
using BirthdayGifts.Services.Implementations;
using BirthdayGifts.Services.Implementations.Employee;
using BirthdayGifts.Services.Implementations.Gift;
using BirthdayGifts.Services.Implementations.VotingSession;
using BirthdayGifts.Services.Implementations.Vote;
using BirthdayGifts.Services.Implementations.Authentication;
using BirthdayGifts.Services.Helpers;
using BirthdayGifts.Repository.Interfaces;
using BirthdayGifts.Repository.Implementations;
using AutoMapper;

namespace BirthdayGifts.Console
{
    class Program
    {
        private static readonly Random _random = new Random();
        private static IServiceProvider _serviceProvider;
        private static string _uniqueIdentifier;

        static async Task Main(string[] args)
        {
            try
            {
                _uniqueIdentifier = DateTime.Now.ToString("yyyyMMddHHmmss");
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection");
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new Exception("Connection string not found in appsettings.json");
                }

                // Setup dependency injection
                var services = new ServiceCollection();
                ConfigureServices(services, connectionString);
                _serviceProvider = services.BuildServiceProvider();

                System.Console.WriteLine("Birthday Gifts Application");
                System.Console.WriteLine("-------------------------");

                // Clean database before starting
                await CleanDatabase();

                // Test Employee Service
                await TestEmployeeService();

                // Test Gift Service
                await TestGiftService();

                // Test Voting Session Service
                await TestVotingSessionService();

                // Test Vote Service
                await TestVoteService();

                // Test Authentication Service
                await TestAuthenticationService();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Console.WriteLine($"Inner Error: {ex.InnerException.Message}");
                }
            }

            System.Console.WriteLine("\nPress any key to exit...");
            System.Console.ReadKey();
        }

        private static void ConfigureServices(IServiceCollection services, string connectionString)
        {
            // Register AutoMapper
            services.AddAutoMapper(typeof(MappingProfile));

            // Register repositories with connection string
            services.AddScoped<IEmployeeRepository>(sp => new EmployeeRepository(connectionString));
            services.AddScoped<IGiftRepository>(sp => new GiftRepository(connectionString));
            services.AddScoped<IVotingSessionRepository>(sp => new VotingSessionRepository(connectionString));
            services.AddScoped<IVoteRepository>(sp => new VoteRepository(connectionString));

            // Register services
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IGiftService, GiftService>();
            services.AddScoped<IVotingSessionService, VotingSessionService>();
            services.AddScoped<IVoteService, VoteService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();

            // Register configuration
            services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build());
        }

        static async Task TestEmployeeService()
        {
            System.Console.WriteLine("\nTesting Employee Service:");
            var employeeService = _serviceProvider.GetRequiredService<IEmployeeService>();

            // Create employees with unique usernames and birthdays
            var today = DateTime.Today;
            var upcomingBirthdays = new[]
            {
                new CreateEmployeeDto
                {
                    Username = $"emp_{_uniqueIdentifier}_1",
                    Password = "Test123!",
                    FullName = $"Test Employee 1 {_uniqueIdentifier}",
                    DateOfBirth = new DateTime(1990, today.Month, today.AddDays(5).Day)
                },
                new CreateEmployeeDto
                {
                    Username = $"emp_{_uniqueIdentifier}_2",
                    Password = "Test123!",
                    FullName = $"Test Employee 2 {_uniqueIdentifier}",
                    DateOfBirth = new DateTime(1990, today.Month, today.AddDays(15).Day)
                }
            };

            foreach (var employee in upcomingBirthdays)
            {
                var createdEmployee = await employeeService.CreateEmployeeAsync(employee);
                System.Console.WriteLine($"Created employee: {createdEmployee.FullName} (Username: {createdEmployee.Username})");

                // Test updating employee
                var updateDto = new EmployeeUpdateDto
                {
                    FullName = $"Updated {createdEmployee.FullName}",
                    DateOfBirth = createdEmployee.DateOfBirth
                };
                var updatedEmployee = await employeeService.UpdateEmployeeAsync(createdEmployee.EmployeeId, updateDto);
                System.Console.WriteLine($"Updated employee: {updatedEmployee.FullName}");
            }

            // Test filtering employees
            var employeeFilter = new EmployeeFilterDto
            {
                Username = $"emp_{_uniqueIdentifier}",
                FullName = $"Test Employee {_uniqueIdentifier}"
            };
            var filteredEmployees = await employeeService.GetFilteredEmployeesAsync(employeeFilter);
            System.Console.WriteLine("\nFiltered Employees:");
            foreach (var employee in filteredEmployees)
            {
                System.Console.WriteLine($"- {employee.FullName} (Username: {employee.Username})");
            }

            // Test upcoming birthdays
            var upcomingBirthdayEmployees = await employeeService.GetEmployeesWithUpcomingBirthdaysAsync(30);
            System.Console.WriteLine("\nEmployees with Upcoming Birthdays (Next 30 days):");
            foreach (var employee in upcomingBirthdayEmployees)
            {
                System.Console.WriteLine($"- {employee.FullName} (Birthday: {employee.DateOfBirth:MM-dd})");
            }
        }

        static async Task TestGiftService()
        {
            System.Console.WriteLine("\nTesting Gift Service:");
            var giftService = _serviceProvider.GetRequiredService<IGiftService>();

            // Create just two gifts for testing
            var newGifts = new[]
            {
                new CreateGiftDto
                {
                    Name = $"Gift_{_uniqueIdentifier}_1",
                    Description = "Test gift 1",
                    Price = 99.99m
                },
                new CreateGiftDto
                {
                    Name = $"Gift_{_uniqueIdentifier}_2",
                    Description = "Test gift 2",
                    Price = 149.99m
                }
            };

            foreach (var gift in newGifts)
            {
                var createdGift = await giftService.CreateGiftAsync(gift);
                System.Console.WriteLine($"Created gift: {createdGift.Name} (Price: {createdGift.Price:C})");

                // Update the gift
                var updateDto = new GiftUpdateDto
                {
                    Name = $"Updated {createdGift.Name}",
                    Description = createdGift.Description,
                    Price = createdGift.Price + 10
                };
                var updatedGift = await giftService.UpdateGiftAsync(createdGift.GiftId, updateDto);
                System.Console.WriteLine($"Updated gift: {updatedGift.Name} (New Price: {updatedGift.Price:C})");
            }

            // Test filtering by name and price
            var giftFilter = new GiftFilterDto
            {
                Name = $"Gift_{_uniqueIdentifier}",
                MinPrice = 50,
                MaxPrice = 200
            };
            var filteredGifts = await giftService.GetFilteredGiftsAsync(giftFilter);
            System.Console.WriteLine("\nFiltered Gifts (by name and price range):");
            foreach (var gift in filteredGifts)
            {
                System.Console.WriteLine($"- {gift.Name} (Price: {gift.Price:C})");
            }

            // Test filtering by price range
            var priceFilter = new GiftFilterDto
            {
                MinPrice = 100,
                MaxPrice = 150
            };
            var priceFilteredGifts = await giftService.GetFilteredGiftsAsync(priceFilter);
            System.Console.WriteLine("\nGifts in price range $100-$150:");
            foreach (var gift in priceFilteredGifts)
            {
                System.Console.WriteLine($"- {gift.Name} (Price: {gift.Price:C})");
            }

            // Test filtering by name
            var nameFilter = new GiftFilterDto
            {
                Name = $"Updated Gift_{_uniqueIdentifier}"
            };
            var nameFilteredGifts = await giftService.GetFilteredGiftsAsync(nameFilter);
            System.Console.WriteLine("\nGifts with 'Updated' in name:");
            foreach (var gift in nameFilteredGifts)
            {
                System.Console.WriteLine($"- {gift.Name} (Price: {gift.Price:C})");
            }
        }

        static async Task TestVotingSessionService()
        {
            System.Console.WriteLine("\nTesting Voting Session Service:");
            var sessionService = _serviceProvider.GetRequiredService<IVotingSessionService>();
            var employeeService = _serviceProvider.GetRequiredService<IEmployeeService>();

            try
            {
                // Get existing employees
                var employees = await employeeService.GetAllEmployeesAsync();
                if (!employees.Any())
                {
                    throw new Exception("No employees found in the database. Please create employees first.");
                }

                // Create a new session with unique data
                var startDate = DateTime.Now.AddDays(1);
                var newSession = new CreateVotingSessionDto
                {
                    VoteSessionCreatorId = employees.First().EmployeeId,
                    BirthdayPersonId = employees.Skip(1).First().EmployeeId,
                    Year = DateTime.Now.Year + 1,
                    StartedAt = startDate,
                    EndedAt = startDate.AddDays(30)
                };

                var createdSession = await sessionService.CreateSessionAsync(newSession);
                System.Console.WriteLine($"Created new session with ID: {createdSession.VoteSessionId}");

                // Create an active session
                var activeSession = new CreateVotingSessionDto
                {
                    VoteSessionCreatorId = employees.First().EmployeeId,
                    BirthdayPersonId = employees.Skip(1).First().EmployeeId,
                    Year = DateTime.Now.Year + 1,
                    StartedAt = DateTime.Now.AddDays(-1),  // Started yesterday
                    EndedAt = DateTime.Now.AddDays(29),    // Ends in 29 days
                    IsActive = true                        // Set to active
                };

                var createdActiveSession = await sessionService.CreateSessionAsync(activeSession);
                System.Console.WriteLine($"Created active session with ID: {createdActiveSession.VoteSessionId}");

                // Test filtering sessions
                var filter = new VotingSessionFilterDto
                {
                    VoteSessionCreatorId = employees.First().EmployeeId,
                    Year = DateTime.Now.Year + 1
                };
                var filteredSessions = await sessionService.GetFilteredSessionsAsync(filter);
                System.Console.WriteLine("\nFiltered Sessions:");
                foreach (var session in filteredSessions)
                {
                    System.Console.WriteLine($"- Session {session.VoteSessionId} (Year: {session.Year})");
                }

                // Test active sessions
                var activeSessions = await sessionService.GetActiveSessionsAsync();
                System.Console.WriteLine("\nActive Sessions:");
                foreach (var session in activeSessions)
                {
                    System.Console.WriteLine($"- Session {session.VoteSessionId} (Year: {session.Year})");
                }

                // Test sessions by employee
                var employeeSessions = await sessionService.GetSessionsByEmployeeIdAsync(employees.First().EmployeeId);
                System.Console.WriteLine("\nSessions for Employee:");
                foreach (var session in employeeSessions)
                {
                    System.Console.WriteLine($"- Session {session.VoteSessionId} (Year: {session.Year})");
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Error in Voting Session Service test: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Console.WriteLine($"Inner error: {ex.InnerException.Message}");
                }
            }
        }

        static async Task TestVoteService()
        {
            System.Console.WriteLine("\nTesting Vote Service:");
            var voteService = _serviceProvider.GetRequiredService<IVoteService>();
            var sessionService = _serviceProvider.GetRequiredService<IVotingSessionService>();
            var employeeService = _serviceProvider.GetRequiredService<IEmployeeService>();
            var giftService = _serviceProvider.GetRequiredService<IGiftService>();

            try
            {
                // Get existing employees and gifts
                var employees = await employeeService.GetAllEmployeesAsync();
                var gifts = await giftService.GetAllGiftsAsync();
                if (!employees.Any() || !gifts.Any())
                {
                    throw new Exception("Required data not found. Please create employees and gifts first.");
                }

                // Create a new session with unique data
                var startDate = DateTime.Now.AddDays(1);
                var newSession = new CreateVotingSessionDto
                {
                    VoteSessionCreatorId = employees.First().EmployeeId,
                    BirthdayPersonId = employees.Skip(1).First().EmployeeId,
                    Year = DateTime.Now.Year + 2,  // Use year + 2 to avoid conflicts
                    StartedAt = startDate,
                    EndedAt = startDate.AddDays(30),
                    IsActive = true
                };

                var createdSession = await sessionService.CreateSessionAsync(newSession);
                System.Console.WriteLine($"Created new session with ID: {createdSession.VoteSessionId}");

                // Create unique votes
                var voters = employees.Take(2).ToList();
                var selectedGifts = gifts.Take(2).ToList();

                for (int i = 0; i < 2; i++)
                {
                    var newVote = new CreateVoteDto
                    {
                        VoteSessionId = createdSession.VoteSessionId,
                        GiftId = selectedGifts[i].GiftId,
                        VoterId = voters[i].EmployeeId
                    };

                    var createdVote = await voteService.CreateVoteAsync(newVote);
                    System.Console.WriteLine($"Created vote: {createdVote.VoteId}");

                    // Test updating vote
                    var updateDto = new VoteUpdateDto
                    {
                        VoteSessionId = createdSession.VoteSessionId,
                        GiftId = selectedGifts[(i + 1) % 2].GiftId,
                        VoterId = voters[i].EmployeeId
                    };
                    var updatedVote = await voteService.UpdateVoteAsync(createdVote.VoteId, updateDto);
                    System.Console.WriteLine($"Updated vote: {updatedVote.VoteId} to Gift {updatedVote.GiftId}");
                }

                // Test filtering votes
                var filter = new VoteFilterDto
                {
                    VoteSessionId = createdSession.VoteSessionId,
                    VoterId = voters[0].EmployeeId
                };
                var filteredVotes = await voteService.GetFilteredVotesAsync(filter);
                System.Console.WriteLine("\nFiltered Votes:");
                foreach (var vote in filteredVotes)
                {
                    System.Console.WriteLine($"- Vote {vote.VoteId} for Gift {vote.GiftId}");
                }

                // Test additional vote queries
                var votesBySession = await voteService.GetVotesBySessionIdAsync(createdSession.VoteSessionId);
                System.Console.WriteLine("\nVotes by Session:");
                foreach (var vote in votesBySession)
                {
                    System.Console.WriteLine($"- Vote {vote.VoteId} for Gift {vote.GiftId}");
                }

                var votesByVoter = await voteService.GetVotesByVoterIdAsync(voters[0].EmployeeId);
                System.Console.WriteLine("\nVotes by Voter:");
                foreach (var vote in votesByVoter)
                {
                    System.Console.WriteLine($"- Vote {vote.VoteId} for Gift {vote.GiftId}");
                }

                var votesByGift = await voteService.GetVotesByGiftIdAsync(selectedGifts[0].GiftId);
                System.Console.WriteLine("\nVotes by Gift:");
                foreach (var vote in votesByGift)
                {
                    System.Console.WriteLine($"- Vote {vote.VoteId} by Voter {vote.VoterId}");
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Error in Vote Service test: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Console.WriteLine($"Inner error: {ex.InnerException.Message}");
                }
            }
        }

        static async Task TestAuthenticationService()
        {
            System.Console.WriteLine("\nTesting Authentication Service:");
            var authService = _serviceProvider.GetRequiredService<IAuthenticationService>();
            var employeeService = _serviceProvider.GetRequiredService<IEmployeeService>();

            try
            {
                // Create a test employee with unique credentials
                var testEmployee = new CreateEmployeeDto
                {
                    Username = $"auth_test_{_uniqueIdentifier}",
                    Password = "Test123!",
                    FullName = $"Auth Test User {_uniqueIdentifier}",
                    DateOfBirth = new DateTime(1990, 1, 1)
                };

                var createdEmployee = await employeeService.CreateEmployeeAsync(testEmployee);
                System.Console.WriteLine($"Created test employee: {createdEmployee.Username}");

                // Test login
                var loginRequest = new LoginRequest
                {
                    Username = testEmployee.Username,
                    Password = testEmployee.Password
                };

                var loginResponse = await authService.LoginAsync(loginRequest);
                if (loginResponse.Success)
                {
                    System.Console.WriteLine("Login successful!");
                    System.Console.WriteLine($"Username: {loginResponse.Username}");
                    System.Console.WriteLine($"Full name: {loginResponse.FullName}");
                    System.Console.WriteLine($"Employee ID: {loginResponse.EmployeeId}");
                }
                else
                {
                    System.Console.WriteLine($"Login failed: {loginResponse.Message}");
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Error during authentication test: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Console.WriteLine($"Inner error: {ex.InnerException.Message}");
                }
            }
        }

        static async Task CleanDatabase()
        {
            System.Console.WriteLine("\nCleaning Database...");
            var voteService = _serviceProvider.GetRequiredService<IVoteService>();
            var sessionService = _serviceProvider.GetRequiredService<IVotingSessionService>();
            var giftService = _serviceProvider.GetRequiredService<IGiftService>();
            var employeeService = _serviceProvider.GetRequiredService<IEmployeeService>();

            try
            {
                // Delete in correct order to respect foreign key constraints
                // 1. Delete votes first (depends on sessions and gifts)
                var votes = await voteService.GetAllVotesAsync();
                foreach (var vote in votes)
                {
                    await voteService.DeleteVoteAsync(vote.VoteId);
                }
                System.Console.WriteLine("Deleted all votes");

                // 2. Delete voting sessions (depends on employees)
                var sessions = await sessionService.GetAllSessionsAsync();
                foreach (var session in sessions)
                {
                    await sessionService.DeleteSessionAsync(session.VoteSessionId);
                }
                System.Console.WriteLine("Deleted all voting sessions");

                // 3. Delete gifts
                var gifts = await giftService.GetAllGiftsAsync();
                foreach (var gift in gifts)
                {
                    await giftService.DeleteGiftAsync(gift.GiftId);
                }
                System.Console.WriteLine("Deleted all gifts");

                // 4. Delete employees last (other tables depend on it)
                var employees = await employeeService.GetAllEmployeesAsync();
                foreach (var employee in employees)
                {
                    await employeeService.DeleteEmployeeAsync(employee.EmployeeId);
                }
                System.Console.WriteLine("Deleted all employees");

                System.Console.WriteLine("Database cleaned successfully!");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Error cleaning database: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Console.WriteLine($"Inner error: {ex.InnerException.Message}");
                }
            }
        }
    }
} 