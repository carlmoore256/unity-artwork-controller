using System;
using UnityEngine;


[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class ParameterDefinitionAttribute : Attribute
{
    public string Description { get; private set; }

    public ParameterDefinitionAttribute(string description)
    {
        Description = description;
    }
}

// public interface IControllerParameters
// {
//     [ParameterDefinition("The address of this controller")]
//     string Address { get; }
// }

public abstract class EndpointParameters
{
    // [ParameterDefinition("The address of this controller")]
    // public abstract string Address { get; protected set; }
}