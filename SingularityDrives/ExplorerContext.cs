using Libplanet.Action;
using Libplanet.Blockchain;
using Libplanet.Explorer.Interfaces;
using Libplanet.Net;
using Libplanet.Store;
using SingularityDrives.Action;

namespace SingularityDrives;

public class ExplorerContext : IBlockChainContext<PolymorphicAction<PlanetAction>>
{
    private readonly Swarm<PolymorphicAction<PlanetAction>> _swarm;

    public ExplorerContext(
        BlockChain<PolymorphicAction<PlanetAction>> blockChain,
        IStore store,
        Swarm<PolymorphicAction<PlanetAction>> swarm
    )
    {
        BlockChain = blockChain;
        Store = store;
        _swarm = swarm;
    }

    public bool Preloaded => _swarm.Running;

    public BlockChain<PolymorphicAction<PlanetAction>> BlockChain { get; private set; }

    public IStore Store { get; private set; }
}
