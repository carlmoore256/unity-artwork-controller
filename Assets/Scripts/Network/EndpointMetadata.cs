using System;

public class EndpointMetadata
{
    public string Address { get; set; }
    public string Description { get; set; }
    public Type DataType { get; set; }
    public float MinRange { get; set; }
    public float MaxRange { get; set; }

    // maybe an image
}

public class RangedValue
{
    public float Value { get; set; }
    public float MinRange { get; set; }
    public float MaxRange { get; set; }
}