using System.Collections.Generic;

// public interface INetworkEndpoint<T> where T : EndpointParameters
public interface INetworkEndpoint
{
    string Address { get; }
    // void ApplyParameters(T parameters);

    // this way, endpoints can recursively register themselves
    void Register(string baseAddress); 
    void Unregister();

    // should also contain some endpoint metadata
    // EndpointMetadata
}

// idea - each endpoint is actually a class

public interface IArtworkController
{
    Artwork Artwork { get; }
}