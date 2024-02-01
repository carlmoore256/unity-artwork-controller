using System.Collections.Generic;
using System.Linq;
using UnityEngine;


// main task is to run the base generator, then run the modifiers
public class MeshComposer
{   
    private readonly IMeshGenerator _baseGenerator;
    private List<IVertexModifier> _vertexModifiers;

    public MeshComposer(IMeshGenerator baseGenerator, IEnumerable<IVertexModifier> vertexModifiers)
    {
        _baseGenerator = baseGenerator;
        _vertexModifiers = vertexModifiers.ToList();

    }

    public MeshComposer(IMeshGenerator baseGenerator)
    {
        _baseGenerator = baseGenerator;
        _vertexModifiers = new List<IVertexModifier>();
    }
}