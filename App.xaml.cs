using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServiceManagerApp.Frontend;
using ServiceManagerApp.Repositories;
using ServiceManagerApp.Services.Users;

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
				services.AddTransient<PrimaryWindow>();
				services.AddTransient<SignInWindow>();
				services.AddTransient<CustomerView>();
				services.AddTransient<EquipmentView>();
				services.AddTransient<DataSeeder>();
				services.AddTransient<MenuWindow>();
				services.AddTransient<ServiceTicketView>();
				services.AddTransient<UserView>();
				services.AddTransient<PartsView>();
				services.AddTransient<ProfileView>();
				services.AddSingleton<UserSession>();
            })
			.Build();

		await host.StartAsync();
		scope = host.Services.CreateScope();
		scope.ServiceProvider.GetRequiredService<SignInWindow>().Show();

		var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
		await seeder.SeedAsync();
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

