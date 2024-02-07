using Dojo;
using dojo_bindings;

public class Recommendation : ModelInstance
{
    [ModelField("from")]
    private dojo.FieldElement from;

    [ModelField("to")]
    private dojo.FieldElement to;

    [ModelField("recommended")]
    private bool recommended;

}
