using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;

[CustomEditor(typeof(SVGImporter))]
public class SVGImporterEditor : Editor
{
    private static IArtworkController[] _artworkControllersCache = null;
    private List<bool> _toggleStates = new List<bool>();
    bool _clearedCache = true;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SVGImporter svgImporter = (SVGImporter)target;


        if (_artworkControllersCache == null)
        {
            _artworkControllersCache = GetAllArtworkControllers();
            _clearedCache = true;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Artwork Controllers", EditorStyles.boldLabel);

        
        // Display toggleable checkboxes for each IArtworkController script
        for (int i = 0; i < _artworkControllersCache.Length; i++)
        {
            if (_toggleStates.Count <= i)
            {
                _toggleStates.Add(false);
            }
            _toggleStates[i] = EditorGUILayout.Toggle(_artworkControllersCache[i].GetType().Name, _toggleStates[i]);
        }
        
        svgImporter.ClearArtworkControllers();
        
        for (int i = 0; i < _artworkControllersCache.Length; i++)
        {
            if (_toggleStates[i])
            {
                svgImporter.AddArtworkController(_artworkControllersCache[i]);
            }
        }

        if (GUILayout.Button("Load and Apply SVGs"))
        {
            svgImporter.LoadAndApplySVGs();
        }
    }

    private IArtworkController[] GetAllArtworkControllers()
    {
        List<IArtworkController> controllers = new List<IArtworkController>();
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (Assembly assembly in assemblies)
        {
            IEnumerable<Type> types = assembly.GetTypes()
                .Where(t => t.GetInterfaces().Contains(typeof(IArtworkController)) && !t.IsAbstract);
            foreach (Type type in types)
            {
                IArtworkController controller = Activator.CreateInstance(type) as IArtworkController;
                if (controller != null)
                {
                    controllers.Add(controller);
                }
            }
        }

        return controllers.ToArray();
    }

    private string[] GetArtworkControllerNames(IArtworkController[] artworkControllers)
    {
        string[] controllerNames = new string[artworkControllers.Length];
        for (int i = 0; i < artworkControllers.Length; i++)
        {
            controllerNames[i] = artworkControllers[i].GetType().Name;
        }
        return controllerNames;
    }

    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        _artworkControllersCache = null; // Invalidate the cache to update it on next inspection
    }
}
