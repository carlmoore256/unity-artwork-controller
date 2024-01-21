// abstract away the idea of controlling a set of controllers from an artwork
// maybe the controllers plug into the artwork

// refer to the effects as plugins/inserts/effects

// plugin should not refer to artwork, nor should artwork refer to plugin
// separate these principles
// They both seem like high level modules
// Plugin could request a component iterator 

public abstract class Plugin {

}

public interface IArtwork
{
    string Name { get; }
    object GetParameters();

    void AddPlugin(Plugin plugin);


    // have something to set the commands?

    void ExecuteEffect(); // we could pass in a generic effect which 
    // is a command object
}