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
            await SeedServiceTicketsAsync();
            await SeedFaultsAsync();
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
                Name = "Farkas Dániel",
                Password = "tech123",
                Phonenumber = "+36 30 555 0101",
                Email = "daniel.farkas@example.com",
                Address = "8200 Veszprém, Minta utca 12.",
                Role = Role.Technician
            },
            new User
            {
                Name = "Varga Eszter",
                Password = "tech456",
                Phonenumber = "+36 20 555 0102",
                Email = "eszter.varga@example.com",
                Address = "8200 Veszprém, Próba tér 4.",
                Role = Role.Technician
            },
            new User
            {
                Name = "Tóth Réka",
                Password = "clerk123",
                Phonenumber = "+36 70 555 0103",
                Email = "reka.toth@example.com",
                Address = "8200 Veszprém, Minta utca 8.",
                Role = Role.Clerk
            },
            new User
            {
                Name = "Szerviz Admin",
                Password = "admin123",
                Phonenumber = "+36 30 555 0104",
                Email = "admin@example.com",
                Address = "8200 Veszprém, Próba tér 1.",
                Role = Role.Admin
            }

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
                Name = "Kovács Péter",
                PhoneNumber = "+36 30 555 0201",
                Email = "peter.kovacs@example.com",
                Address = "8200 Veszprém, Minta utca 15.",
                Created = DateTime.Now.AddDays(-30)
            },
            new Customer
            {
                Name = "Szabó Anna",
                PhoneNumber = "+36 20 555 0202",
                Email = "anna.szabo@example.com",
                Address = "8200 Veszprém, Próba tér 7.",
                Created = DateTime.Now.AddDays(-20)
            },
            new Customer
            {
                Name = "Nagy Gábor",
                PhoneNumber = "+36 70 555 0203",
                Email = "gabor.nagy@example.com",
                Address = "8400 Ajka, Példa utca 9.",
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
                PartName = "ATX tápegység 550 W",
                Quantity = 5,
                PartCost = 22900m
            },
            new Part
            {
                PartName = "SSD 1 TB",
                Quantity = 4,
                PartCost = 28900m
            },
            new Part
            {
                PartName = "DDR4 memória 16 GB",
                Quantity = 8,
                PartCost = 15900m
            },
            new Part
            {
                PartName = "Laptop LCD panel 15,6 hüvelyk",
                Quantity = 2,
                PartCost = 49900m
            },
            new Part
            {
                PartName = "Hűtőventilátor 120 mm",
                Quantity = 0,
                PartCost = 6900m
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

        var customers = (await _customerRepository.GetAllAsync())
            .OrderBy(customer => customer.Id)
            .ToList();
        if (customers.Count == 0)
            return;

        var equipment = new List<Equipment>
        {
            new Equipment
            {
                CustomerId = customers[0].Id,
                Category = "Laptop",
                Brand = "Lenovo",
                Model = "ThinkPad E14",
                SerialNumber = "HU-VM-0001"
            },
            new Equipment
            {
                CustomerId = customers[0].Id,
                Category = "Desktop PC",
                Brand = "HP",
                Model = "ProDesk 400 G6",
                SerialNumber = "HU-VM-0002"
            }
        };

        if (customers.Count > 1)
        {
            equipment.Add(new Equipment
            {
                CustomerId = customers[1].Id,
                Category = "Laptop",
                Brand = "ASUS",
                Model = "VivoBook 15",
                SerialNumber = "HU-VM-0003"
            });
        }

        if (customers.Count > 2)
        {
            equipment.Add(new Equipment
            {
                CustomerId = customers[2].Id,
                Category = "Nyomtató",
                Brand = "Brother",
                Model = "HL-L2350DW",
                SerialNumber = "HU-AJ-0001"
            });
        }

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

        var tickets = (await _serviceTicketRepository.GetAllAsync())
            .OrderBy(ticket => ticket.Id)
            .ToList();
        if (tickets.Count == 0)
            return;

        var faults = new List<Fault>();

        if (tickets.Count > 0)
        {
            faults.Add(new Fault
            {
                ServiceTicketId = tickets[0].Id,
                FaultName = "A laptop kijelzője nem ad képet",
                Diagnosis = "A kijelző háttérvilágítása meghibásodott.",
                Repairs = "Az LCD panel cseréje befejeződött."
            });
        }

        if (tickets.Count > 1)
        {
            faults.Add(new Fault
            {
                ServiceTicketId = tickets[1].Id,
                FaultName = "A számítógép nem indul",
                Diagnosis = "A tápegység instabil feszültséget ad le.",
                Repairs = "A tápegység cseréje folyamatban van."
            });
        }

        if (tickets.Count > 2)
        {
            faults.Add(new Fault
            {
                ServiceTicketId = tickets[2].Id,
                FaultName = "A laptop időnként újraindul",
                Diagnosis = "A készülék bevizsgálása folyamatban van.",
                Repairs = string.Empty
            });
        }

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
            var ticketsWithoutChangeLog = existingTickets
                .Where(ticket => ticket.ChangeLog.Count == 0)
                .ToList();

            foreach (var ticket in ticketsWithoutChangeLog)
            {
                ticket.ChangeLog.Add($"{ticket.IntakeDate} - Ticket Created");
            }

            if (ticketsWithoutChangeLog.Count > 0)
            {
                await _serviceTicketRepository.SaveChangesAsync();
                Debug.WriteLine($"Added initial change-log entries to {ticketsWithoutChangeLog.Count} service tickets");
            }

            Debug.WriteLine("Service tickets already exist, skipping seeding...");
            return;
        }

        var users = await _userRepository.GetAllAsync();
        var technicians = users
            .Where(user => user.Role == Role.Technician)
            .OrderBy(user => user.Id)
            .ToList();
        var equipment = (await _equipmentRepository.GetAllAsync())
            .OrderBy(item => item.Id)
            .ToList();

        if (equipment.Count == 0 || technicians.Count == 0)
            return;

        var firstIntakeDate = DateTime.Now.AddDays(-10);
        var firstCompletionDate = DateTime.Now.AddDays(-7);
        var secondIntakeDate = DateTime.Now.AddDays(-4);
        var thirdIntakeDate = DateTime.Now.AddDays(-2);
        var tickets = new List<ServiceTicket>();

        if (equipment.Count > 0)
        {
            tickets.Add(new ServiceTicket
            {
                UserId = technicians[0].Id,
                EquipmentId = equipment[0].Id,
                IntakeDate = firstIntakeDate,
                CompletionDate = firstCompletionDate,
                Status = Status.Finished,
                LaborCost = 18000m,
                Price = 67900m,
                ChangeLog =
                [
                    $"{firstIntakeDate} - Ticket Created",
                    $"{firstIntakeDate} - Assigned to: {technicians[0].Name}",
                    $"{firstCompletionDate} - Status changed to: {Status.Finished}"
                ]
            });
        }

        if (equipment.Count > 1)
        {
            var technician = technicians.Count > 1 ? technicians[1] : technicians[0];
            tickets.Add(new ServiceTicket
            {
                UserId = technician.Id,
                EquipmentId = equipment[1].Id,
                IntakeDate = secondIntakeDate,
                CompletionDate = null,
                Status = Status.InRepair,
                LaborCost = 12000m,
                Price = 34900m,
                ChangeLog =
                [
                    $"{secondIntakeDate} - Ticket Created",
                    $"{secondIntakeDate} - Assigned to: {technician.Name}",
                    $"{DateTime.Now.AddDays(-1)} - Status changed to: {Status.InRepair}"
                ]
            });
        }

        if (equipment.Count > 2)
        {
            tickets.Add(new ServiceTicket
            {
                UserId = technicians[0].Id,
                EquipmentId = equipment[2].Id,
                IntakeDate = thirdIntakeDate,
                CompletionDate = null,
                Status = Status.Diagnosing,
                LaborCost = 0m,
                Price = 0m,
                ChangeLog =
                [
                    $"{thirdIntakeDate} - Ticket Created",
                    $"{thirdIntakeDate} - Assigned to: {technicians[0].Name}",
                    $"{DateTime.Now.AddDays(-1)} - Status changed to: {Status.Diagnosing}"
                ]
            });
        }

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

        var tickets = (await _serviceTicketRepository.GetAllAsync())
            .OrderBy(ticket => ticket.Id)
            .ToList();
        var parts = (await _partRepository.GetAllAsync())
            .OrderBy(part => part.Id)
            .ToList();

        var ticketParts = new List<TicketPart>();

        if (tickets.Count > 0 && parts.Count > 3)
        {
            ticketParts.Add(new TicketPart
            {
                ServiceTicketId = tickets[0].Id,
                PartId = parts[3].Id,
                Quantity = 1
            });
        }

        if (tickets.Count > 1 && parts.Count > 0)
        {
            ticketParts.Add(new TicketPart
            {
                ServiceTicketId = tickets[1].Id,
                PartId = parts[0].Id,
                Quantity = 1
            });
        }

        foreach (var ticketPart in ticketParts)
        {
            await _ticketPartRepository.CreateAsync(ticketPart, save: false);
        }
        await _ticketPartRepository.SaveChangesAsync();
        Debug.WriteLine($"Seeded {ticketParts.Count} ticket parts");
    }
}
