// something that we can stick on a gameobject, 
// and it can make use of parameters to change 
// some aspects about the gameobject. The actual 
// way it does this depends on the concrete implementation

using System.Collections.Generic;

public interface IInsert
{
    string Id { get; }
    string Name { get; }
    InsertParameters GetParameters();
    // void UpdateParameters(string json);
}