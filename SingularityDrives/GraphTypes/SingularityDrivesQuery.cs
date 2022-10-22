using GraphQL.Types;
using Libplanet.Action;
using Libplanet.Explorer.Queries;
using SingularityDrives.Action;

namespace SingularityDrives.GraphTypes;

public class SingularityDrivesQuery : ObjectGraphType
{
    public SingularityDrivesQuery()
    {
        Name = "SingularityDrivesQuery";
        Field<ExplorerQuery<PolymorphicAction<PlanetAction>>>(
            "explorer",
            resolve: context => new { }
        );

        // For compatibility with libplanet-explorer-frontend.
        Field<ExplorerQuery<PolymorphicAction<PlanetAction>>>(
            "chainQuery",
            resolve: context => new { }
        );
        Field<ApplicationQuery>(
            "application",
            resolve: context => new { }
        );
    }
}
