using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServiceManagerApp.Repositories;

namespace ServiceManagerApp;

public partial class App : Application
{
	private IHost? host;
	private IServiceScope? scope;

	protected override async void OnStartup(StartupEventArgs e)
	{
		base.OnStartup(e);

		host = Host.CreateDefaultBuilder()
			.ConfigureServices(services =>
			{
				services.AddDbContext<AppDbContext>(ServiceLifetime.Transient);

				services.AddTransient<CustomerRepository>();
				services.AddTransient<EquipmentRepository>();
				services.AddTransient<FaultRepository>();
				services.AddTransient<PartRepository>();
				services.AddTransient<ServiceTicketRepository>();
				services.AddTransient<TicketPartRepository>();
				services.AddTransient<UserRepository>();
				services.AddTransient<MainWindow>();
				services.AddTransient<CustomerWindow>();
				services.AddTransient<EquipmentWindow>();
				services.AddTransient<DataSeeder>();
			})
			.Build();

		await host.StartAsync();
		scope = host.Services.CreateScope();

		var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
		await seeder.SeedAsync();

		scope.ServiceProvider.GetRequiredService<MainWindow>().Show();
	}

	protected override async void OnExit(ExitEventArgs e)
	{
		if (host != null)
		{
			await host.StopAsync();
			host.Dispose();
		}

		scope?.Dispose();
		base.OnExit(e);
	}
}

