using System.Collections.Generic;

public interface IOscControllable
{
    string OscAddress { get; }
    void RegisterEndpoints();
    void UnregisterEndpoints();
}

public interface IArtworkController
{
    Artwork Artwork { get; }
}