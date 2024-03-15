using Dojo;
using Dojo.Starknet;
using dojo_bindings;

public class Recommendation : ModelInstance
{
    [ModelField("from")]
    public FieldElement from;

    [ModelField("to")]
    public FieldElement to;

    [ModelField("recommended")]
    public bool recommended;

}
