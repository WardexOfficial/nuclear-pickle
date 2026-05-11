using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Content.Shared._Nuclear.Preferences.Loadouts;

/// <summary>serializer for backward compatibility groupBy (string → list)</summary>
public sealed class GroupBySerializer : 
    ITypeSerializer<List<string>, ValueDataNode>,
    ITypeSerializer<List<string>, SequenceDataNode>
{
    // Reading from string: "jumpsuit" → ["jumpsuit"]
    public List<string> Read(ISerializationManager serializationManager, ValueDataNode node,
        IDependencyCollection dependencies,
        Robust.Shared.Serialization.SerializationHookContext hookCtx,
        ISerializationContext? context = null,
        ISerializationManager.InstantiationDelegate<List<string>>? instanceProvider = null)
    {
        return new List<string> { node.Value };
    }

    public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node,
        IDependencyCollection dependencies, ISerializationContext? context = null)
    {
        return new ValidatedValueNode(node);
    }

    // Reading from list: [female, color] → ["female", "color"]
    public List<string> Read(ISerializationManager serializationManager, SequenceDataNode node,
        IDependencyCollection dependencies,
        Robust.Shared.Serialization.SerializationHookContext hookCtx,
        ISerializationContext? context = null,
        ISerializationManager.InstantiationDelegate<List<string>>? instanceProvider = null)
    {
        var list = new List<string>();
        foreach (var entry in node)
        {
            if (entry is ValueDataNode valueNode)
                list.Add(valueNode.Value);
        }
        return list;
    }

    public ValidationNode Validate(ISerializationManager serializationManager, SequenceDataNode node,
        IDependencyCollection dependencies, ISerializationContext? context = null)
    {
        return new ValidatedValueNode(node);
    }

    // Writing
    public DataNode Write(ISerializationManager serializationManager, List<string> value,
        IDependencyCollection dependencies, bool alwaysWrite = false,
        ISerializationContext? context = null)
    {
        return serializationManager.WriteValue(value, alwaysWrite, context, false);
    }
}