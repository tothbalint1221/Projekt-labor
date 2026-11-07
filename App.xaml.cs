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
				services.AddDbContext<AppDbContext>(ServiceLifetime.Scoped);

				services.AddScoped<CustomerRepository>();
				services.AddScoped<EquipmentRepository>();
				services.AddScoped<FaultRepository>();
				services.AddScoped<PartRepository>();
				services.AddScoped<ServiceTicketRepository>();
				services.AddScoped<TicketPartRepository>();
				services.AddScoped<UserRepository>();
				services.AddScoped<PrimaryWindow>();
				services.AddScoped<SignInWindow>();
				services.AddScoped<CustomerView>();
				services.AddScoped<EquipmentView>();
				services.AddScoped<DataSeeder>();
				services.AddScoped<MenuWindow>();
				services.AddScoped<ServiceTicketView>();
				services.AddScoped<UserView>();
				services.AddScoped<PartsView>();
				services.AddScoped<ProfileView>();
				services.AddSingleton<UserSession>();
            })
			.Build();

		await host.StartAsync();
		scope = host.Services.CreateScope();

		var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
		await seeder.SeedAsync();

		scope.ServiceProvider.GetRequiredService<PrimaryWindow>().Show();
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
