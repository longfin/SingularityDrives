using Cocona;
using GraphQL;
using GraphQL.MicrosoftDI;
using GraphQL.Server;
using GraphQL.Server.Transports.AspNetCore;
using GraphQL.SystemTextJson;
using Libplanet;
using Libplanet.Action;
using Libplanet.Assets;
using Libplanet.Explorer.Interfaces;
using Libplanet.Explorer.Queries;
using Libplanet.Extensions.Cocona.Commands;
using Libplanet.Headless;
using Libplanet.Headless.Hosting;
using LibTopless;
using LibTopless.Action;
using Serilog;
using SingularityDrives;
using SingularityDrives.GraphTypes;
using System.Collections.Immutable;
using System.Net;

var app = CoconaApp.Create();

app.AddCommand(() =>
{
    // Get configuration
    string configPath = Environment.GetEnvironmentVariable("PN_CONFIG_FILE") ?? "appsettings.json";

    var configurationBuilder = new ConfigurationBuilder()
        .AddJsonFile(configPath)
        .AddEnvironmentVariables("PN_");
    IConfiguration config = configurationBuilder.Build();

    var loggerConf = new LoggerConfiguration()
       .ReadFrom.Configuration(config);
    Log.Logger = loggerConf.CreateLogger();

    var headlessConfig = new Configuration();
    config.Bind(headlessConfig);

    var builder = WebApplication.CreateBuilder(args);
    builder.Services
        .AddLibplanet(
            headlessConfig,
            new PolymorphicAction<PlanetAction>[]
            {
                new InitializeStates(
                    new Dictionary<Address, FungibleAssetValue>
                    {
                        [new Address("0x25924579F8f1D6a0edE9aa86F9522e44EbC74C26")] = Currencies.SingularityDrivesGold * 1000,
                    }
                ),
            },
            ImmutableHashSet.Create(Currencies.SingularityDrivesGold),
            new DynamicTypeLoader("fraternity", "LibTopless.dll")
        )
        .AddGraphQL(builder =>
        {
            builder
                .AddSchema<SingularityDrivesSchema>()
                .AddGraphTypes(typeof(ExplorerQuery<PolymorphicAction<PlanetAction>>).Assembly)
                .AddGraphTypes(typeof(SingularityDrivesQuery).Assembly)
                .AddUserContextBuilder<ExplorerContextBuilder>()
                .AddSystemTextJson();
        })
        .AddCors()
        .AddSingleton<SingularityDrivesSchema>()
        .AddSingleton<SingularityDrivesQuery>()
        .AddSingleton<SingularityDrivesMutation>()
        .AddSingleton<GraphQLHttpMiddleware<SingularityDrivesSchema>>()
        .AddSingleton<IBlockChainContext<PolymorphicAction<PlanetAction>>, ExplorerContext>();

    if (
        headlessConfig.GraphQLHost is { } graphqlHost &&
        headlessConfig.GraphQLPort is { } graphqlPort
    )
    {
        builder.WebHost
            .ConfigureKestrel(options =>
            {
                options.Listen(IPAddress.Parse(graphqlHost), graphqlPort);
            });
    }

    using WebApplication app = builder.Build();
    app.UseCors(builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
    app.UseRouting();
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapGraphQLPlayground();
    });
    app.UseGraphQL<SingularityDrivesSchema>();

    app.Run();
});

app.AddSubCommand("key", x =>
{
    x.AddCommands<KeyCommand>();
});

app.Run();
