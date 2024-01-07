using System.Collections.Generic;

// public interface INetworkEndpoint<T> where T : EndpointParameters
public interface INetworkEndpoint
{
    string Address { get; }
    // void ApplyParameters(T parameters);

    // should also contain some endpoint metadata
    // EndpointMetadata
}

// idea - each endpoint is actually a class

public interface IArtworkController
{
    Artwork Artwork { get; }
}