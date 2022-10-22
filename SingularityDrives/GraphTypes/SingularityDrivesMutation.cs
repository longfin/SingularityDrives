using GraphQL;
using GraphQL.Types;
using Libplanet;
using Libplanet.Action;
using Libplanet.Action.Sys;
using Libplanet.Assets;
using Libplanet.Blockchain;
using Libplanet.Crypto;
using Libplanet.Explorer.GraphTypes;
using Libplanet.Explorer.Mutations;
using SingularityDrives.Action;
using System.Collections.Immutable;

namespace SingularityDrives.GraphTypes;

public class SingularityDrivesMutation : ObjectGraphType
{
    public SingularityDrivesMutation(BlockChain<PolymorphicAction<PlanetAction>> blockChain)
    {
        Field<TransactionMutation<PolymorphicAction<PlanetAction>>>(
            "transaction",
            description: "Adds a transaction to the pending list so that a next block to be " +
                "mined may contain the given transaction.",
            resolve: context => new { }
        );

        // TODO: This mutation should be upstreamed to Libplanet.Explorer so that any native tokens
        // can work together with this mutation:
        Field<TransactionType<PolymorphicAction<PlanetAction>>>(
            "transferAsset",
            description: "Transfers the given amount of PLT from the account of the specified " +
                "privateKeyHex to the specified recipient.  A made transaction is signed using " +
                "the privateKeyHex and added to the pending list (and eventually included in " +
                "one of the next blocks).",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<StringGraphType>>
                {
                	    Name = "recipient",
                	    Description = "The recipient's 40-hex address.",
                	},
                new QueryArgument<NonNullGraphType<StringGraphType>>
                {
                	    Name = "amount",
                	    Description = "The amount to transfer in PLT.",
                	},
                new QueryArgument<NonNullGraphType<StringGraphType>>
                {
                	    Name = "privateKeyHex",
                	    Description = "A hex-encoded private key of the sender.  A made " +
                	        "transaction will be signed using this key.",
                	}
            ),
            resolve: context =>
            {
                Address recipient = new Address(context.GetArgument<string>("recipient"));
                string amount = context.GetArgument<string>("amount");
                string privateKeyHex = context.GetArgument<string>("privateKeyHex");

                PrivateKey privateKey = PrivateKey.FromString(privateKeyHex);
                var action = new Transfer(
                    recipient,
                    FungibleAssetValue.Parse(
                        Currencies.SingularityDrivesGold,
                        amount
                    )
                );

                return blockChain.MakeTransaction(
                    privateKey,
                    action,
                    ImmutableHashSet<Address>.Empty
                        .Add(privateKey.ToAddress())
                        .Add(recipient));
            }
        );
    }
}
