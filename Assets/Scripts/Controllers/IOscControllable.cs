using System.Collections.Generic;

public interface IOscControllable
{
    string OscAddress { get; }
    void RegisterEndpoints();
}

public interface IArtworkController
{
    Artwork Artwork { get; }
}