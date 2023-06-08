using System.Net.NetworkInformation;
using System.Net;
using UnityEngine;

public class NetworkInfo : MonoBehaviour
{
    public string ipAddress;

    // Get the local IP address of the computer
    public static string GetLocalIPAddress()
    {
        string localIP = string.Empty;

        // Get all network interfaces
        NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

        // Iterate through each network interface
        foreach (NetworkInterface networkInterface in networkInterfaces)
        {
            // Check if it's an Ethernet or Wi-Fi interface
            if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
            {
                // Get the IP properties of the interface
                IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();

                // Get the unicast addresses assigned to the interface
                UnicastIPAddressInformationCollection unicastAddresses = ipProperties.UnicastAddresses;

                // Iterate through each unicast address
                foreach (UnicastIPAddressInformation address in unicastAddresses)
                {
                    // Check if it's an IPv4 address and not a loopback address
                    if (address.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork &&
                        !IPAddress.IsLoopback(address.Address))
                    {
                        localIP = address.Address.ToString();
                        break;
                    }
                }

                // Break the loop if we found an IP address
                if (!string.IsNullOrEmpty(localIP))
                    break;
            }
        }

        return localIP;
    }
}
