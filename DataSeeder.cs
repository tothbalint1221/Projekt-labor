using ServiceManagerApp.Repositories;
using ServiceManagerApp.Entities;
using System.Diagnostics;

namespace ServiceManagerApp;

public class DataSeeder
{
    private readonly CustomerRepository _customerRepository;
    private readonly UserRepository _userRepository;
    private readonly EquipmentRepository _equipmentRepository;
    private readonly PartRepository _partRepository;
    private readonly ServiceTicketRepository _serviceTicketRepository;
    private readonly FaultRepository _faultRepository;
    private readonly TicketPartRepository _ticketPartRepository;

    public DataSeeder(
        CustomerRepository customerRepository,
        UserRepository userRepository,
        EquipmentRepository equipmentRepository,
        PartRepository partRepository,
        ServiceTicketRepository serviceTicketRepository,
        FaultRepository faultRepository,
        TicketPartRepository ticketPartRepository)
    {
        _customerRepository = customerRepository;
        _userRepository = userRepository;
        _equipmentRepository = equipmentRepository;
        _partRepository = partRepository;
        _serviceTicketRepository = serviceTicketRepository;
        _faultRepository = faultRepository;
        _ticketPartRepository = ticketPartRepository;
    }

    public async Task SeedAsync()
    {
        try
        {
            await SeedUsersAsync();
            await SeedCustomersAsync();
            await SeedPartsAsync();
            await SeedEquipmentAsync();
            await SeedFaultsAsync();
            await SeedServiceTicketsAsync();
            await SeedTicketPartsAsync();

            Debug.WriteLine("Database seeding completed");
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Error during database seeding: {e.Message}");
            throw;
        }
    }

    private async Task SeedUsersAsync()
    {
        var existingUsers = await _userRepository.GetAllAsync();
        if (existingUsers.Count > 0)
        {
            Debug.WriteLine("Users already exist, skipping seeding...");
            return;
        }

        var users = new List<User>
        {
            new User
            {
                Name = "Technician1",
                Password = "tech123",
                Phonenumber = "06301234567",
                Email = "techguy1@gmail.com",
                Address = "8200 Veszprem, Budapest ut 1"
            },
            new User
            {
                Name = "Clerk1",
                Password = "clerk123",
                Phonenumber = "06207654321",
                Email = "clerkguy1@gmail.com",
                Address = "8200 Veszprem, Budapest ut 3"
            },
            new User
            {
                Name = "Admin1",
                Password = "admin123",
                Phonenumber = "0610101010",
                Email = "admin1@gmail.com",
                Address = "8200 Veszprem, Budapest ut 5"
            }
            // szomszed mind es ez igy jo
        };

        foreach (var user in users)
        {
            await _userRepository.CreateAsync(user, save: false);
        }
        await _userRepository.SaveChangesAsync();
        Debug.WriteLine($"Seeded {users.Count} users");
    }

    private async Task SeedCustomersAsync()
    {
        var existingCustomers = await _customerRepository.GetAllAsync();
        if (existingCustomers.Count > 0)
        {
            Debug.WriteLine("Customers already exist, skipping seeding...");
            return;
        }

        var customers = new List<Customer>
        {
            new Customer
            {
                Name = "Kovacs Lajos",
                PhoneNumber = "0688123456",
                Email = "kovacslajos@gmail.com",
                Address = "8200 Veszprem, Kadartai ut 1",
                Created = DateTime.Now.AddDays(-30)
            },
            new Customer
            {
                Name = "Kemeny Szilard",
                PhoneNumber = "06704206769",
                Email = "kemenyszilard@gmail.com",
                Address = "8200 Veszprem, Kadartai ut 3",
                Created = DateTime.Now.AddDays(-20)
            },
            new Customer
            {
                Name = "Saul Goodman",
                PhoneNumber = "+1-505-503-4455",
                Email = "saulgoodman@gmail.com",
                Address = "8200 Veszprem, Kadartai ut 5",
                Created = DateTime.Now.AddDays(-10)
            }
        };

        foreach (var customer in customers)
        {
            await _customerRepository.CreateAsync(customer, save: false);
        }
        await _customerRepository.SaveChangesAsync();
        Debug.WriteLine($"Seeded {customers.Count} customers");
    }

    private async Task SeedPartsAsync()
    {
        var existingParts = await _partRepository.GetAllAsync();
        if (existingParts.Count > 0)
        {
            Debug.WriteLine("Parts already exist, skipping seeding...");
            return;
        }

        var parts = new List<Part>
        {
            new Part
            {
                PartName = "Tapegyseg 500W",
                Quantity = 5,
                PartCost = 5000.00m
            },
            new Part
            {
                PartName = "Univerzalis LCD kijelzo",
                Quantity = 3,
                PartCost = 17000.00m
            },
            new Part
            {
                PartName = "SSD 1TB",
                Quantity = 1,
                PartCost = 999999.99m
            },
            new Part
            {
                PartName = "RAM 8GB",
                Quantity = 0,
                PartCost = 99999.99m
            },
            new Part
            {
                PartName = "Huto ventilator",
                Quantity = 12,
                PartCost = 2500.00m
            }
        };

        foreach (var part in parts)
        {
            await _partRepository.CreateAsync(part, save: false);
        }
        await _partRepository.SaveChangesAsync();
        Debug.WriteLine($"Seeded {parts.Count} parts");
    }

    private async Task SeedEquipmentAsync()
    {
        var existingEquipment = await _equipmentRepository.GetAllAsync();
        if (existingEquipment.Count > 0)
        {
            Debug.WriteLine("Equipment already exist, skipping seeding...");
            return;
        }

        var customers = await _customerRepository.GetAllAsync();
        if (customers.Count == 0)
            return;

        var equipment = new List<Equipment>
        {
            new Equipment
            {
                CustomerId = customers[0].Id,
                Category = "Laptop",
                Brand = "Dell",
                Model = "XPS 15",
                SerialNumber = "DELL-XPS-001"
            },
            new Equipment
            {
                CustomerId = customers[0].Id,
                Category = "Desktop PC",
                Brand = "HP",
                Model = "ProDesk 400",
                SerialNumber = "HP-PRO-001"
            },
            new Equipment
            {
                CustomerId = customers[1].Id,
                Category = "Printer",
                Brand = "Brother",
                Model = "HL-L2350DW",
                SerialNumber = "BROTHER-PRN-001"
            },
            new Equipment
            {
                CustomerId = customers[2].Id,
                Category = "Server",
                Brand = "Lenovo",
                Model = "ThinkSystem SR650",
                SerialNumber = "LENOVO-SRV-001"
            }
        };

        foreach (var equip in equipment)
        {
            await _equipmentRepository.CreateAsync(equip, save: false);
        }
        await _equipmentRepository.SaveChangesAsync();
        Debug.WriteLine($"Seeded {equipment.Count} equipment items");
    }

    private async Task SeedFaultsAsync()
    {
        var existingFaults = await _faultRepository.GetAllAsync();
        if (existingFaults.Count > 0)
        {
            Debug.WriteLine("Faults already exist, skipping seeding...");
            return;
        }

        var equipment = await _equipmentRepository.GetAllAsync();
        if (equipment.Count == 0)
            return;

        var faults = new List<Fault>
        {
            new Fault
            {
                EquipmentId = equipment[0].Id,
                FaultName = "Villogo kijelzo",
                Diagnosis = "LCD panelen meghibasodott",
                Repairs = "LCD panel kicserelve",
                Status = Status.Finished
            },
            new Fault
            {
                EquipmentId = equipment[1].Id,
                FaultName = "Gep nem kapcsol be",
                Diagnosis = "Hibas tapegyseg",
                Repairs = "Tapegyseg kicserelve",
                Status = Status.Finished
            },
            new Fault
            {
                EquipmentId = equipment[2].Id,
                FaultName = "Beragadt papir",
                Diagnosis = "Papir beragadt az adagolo rendszerben",
                Repairs = "Papir eltavolitva, adagolo rendszert kitisztitva",
                Status = Status.Finished
            }
        };

        foreach (var fault in faults)
        {
            await _faultRepository.CreateAsync(fault, save: false);
        }
        await _faultRepository.SaveChangesAsync();
        Debug.WriteLine($"Seeded {faults.Count} faults");
    }

    private async Task SeedServiceTicketsAsync()
    {
        var existingTickets = await _serviceTicketRepository.GetAllAsync();
        if (existingTickets.Count > 0)
        {
            Debug.WriteLine("Service tickets already exist, skipping seeding...");
            return;
        }

        var users = await _userRepository.GetAllAsync();
        var equipment = await _equipmentRepository.GetAllAsync();

        if (equipment.Count == 0 || users.Count == 0)
            return;

        var tickets = new List<ServiceTicket>
        {
            new ServiceTicket
            {
                UserId = users[0].Id,
                EquipmentId = equipment[0].Id,
                IntakeDate = DateTime.Now.AddDays(-5),
                CompletionDate = DateTime.Now.AddDays(-2),
                Status = Status.Finished,
                LaborCost = 5000.00m,
                Price = 20000.00m // THESE ARE EXAMPLES VALUES, WILL BE AUTO CALCULATED
            },
            new ServiceTicket
            {
                UserId = users[1].Id,
                EquipmentId = equipment[1].Id,
                IntakeDate = DateTime.Now.AddDays(-3),
                CompletionDate = DateTime.Now,
                Status = Status.Finished,
                LaborCost = 5000.00m,
                Price = 20000.00m
            },
            new ServiceTicket
            {
                UserId = users[2].Id,
                EquipmentId = equipment[2].Id,
                IntakeDate = DateTime.Now.AddDays(-1),
                CompletionDate = null,
                Status = Status.Accepted,
                LaborCost = 5000.00m,
                Price = 20000.00m
            }
        };

        foreach (var ticket in tickets)
        {
            await _serviceTicketRepository.CreateAsync(ticket, save: false);
        }
        await _serviceTicketRepository.SaveChangesAsync();
        Debug.WriteLine($"Seeded {tickets.Count} service tickets");
    }

    private async Task SeedTicketPartsAsync()
    {
        var existingTicketParts = await _ticketPartRepository.GetAllAsync();
        if (existingTicketParts.Count > 0)
        {
            Debug.WriteLine("Ticket parts already exist, skipping seeding...");
            return;
        }

        var tickets = await _serviceTicketRepository.GetAllAsync();
        var parts = await _partRepository.GetAllAsync();

        if (tickets.Count == 0 || parts.Count == 0)
            return;

        var ticketParts = new List<TicketPart>
        {
            new TicketPart
            {
                ServiceTicketId = tickets[0].Id,
                PartId = parts[1].Id,
                Quantity = 1
            },
            new TicketPart
            {
                ServiceTicketId = tickets[1].Id,
                PartId = parts[0].Id,
                Quantity = 1
            }
        };

        foreach (var ticketPart in ticketParts)
        {
            await _ticketPartRepository.CreateAsync(ticketPart, save: false);
        }
        await _ticketPartRepository.SaveChangesAsync();
        Debug.WriteLine($"Seeded {ticketParts.Count} ticket parts");
    }
}
