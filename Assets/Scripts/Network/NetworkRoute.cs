using UnityEngine;

// find any INetworkControllable on the object, then register the routes
public class NetworkRoute : MonoBehaviour
{
    public string baseAddress => "/router"; // /artwork

    void Start()
    {
        // <INetworkControllable>().RegisterEndpoints();
    }

    // public void RegisterNetworkControllers()
    // {
    //     foreach (var component in gameObject.GetComponents<Component>())
    //     {
    //         var type = component.GetType();
    //         var interfaces = type.GetInterfaces();
    //         foreach (var @interface in interfaces)
    //         {
    //             if (
    //                 @interface.IsGenericType
    //                 && @interface.GetGenericTypeDefinition() == typeof(INetworkEndpoint<>)
    //             )
    //             {
    //                 RegisterEndpoint(component, @interface);
    //             }
    //         }
    //     }
    // }

    // private void RegisterEndpoint(Component component, Type interfaceType)
    // {
    //     var addressMethod = interfaceType.GetMethod("GetAddress");
    //     string address = (string)addressMethod.Invoke(component, null);

    //     // Now you have the address and the component, you can register the endpoint
    //     // For example:
    //     // networkHandler.Register(address, component);
    // }
}
