using GraphQL.Types;

namespace SingularityDrives.GraphTypes;

public class SingularityDrivesSchema : Schema
{
    public SingularityDrivesSchema(IServiceProvider services)
        : base(services)
    {
        Query = services.GetRequiredService<SingularityDrivesQuery>();
        Mutation = services.GetRequiredService<SingularityDrivesMutation>();
    }
}
