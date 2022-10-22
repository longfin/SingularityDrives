using Bencodex.Types;
using Libplanet.Action;

namespace SingularityDrives.Action;

public abstract class PlanetAction : IAction
{
    public abstract IValue PlainValue { get; }

    public abstract IAccountStateDelta Execute(IActionContext context);

    public abstract void LoadPlainValue(IValue plainValue);
}
