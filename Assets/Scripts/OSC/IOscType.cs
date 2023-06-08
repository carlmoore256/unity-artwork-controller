using UnityEngine;
using OscJack;
using System;

// public abstract class OscType<T> { 
//     public abstract object Value  { get; protected set; }
//     public abstract void FromOscDataHandle(OscDataHandle data);

//     public enum DataType
//     {
//         None, Int, Float, String,
//         Vector2, Vector3, Vector4,
//         Vector2Int, Vector3Int
//     }

//     public static OscType CreateOscType(DataType dataType, OscDataHandle data)
//     {
//         switch (dataType)
//         {
//             case DataType.Float:
//                 var floatOscType = new OscFloat();
//                 floatOscType.FromOscDataHandle(data);
//                 return floatOscType;

//             case DataType.Int:
//                 var intOscType = new OscInt();
//                 intOscType.FromOscDataHandle(data);
//                 return intOscType;

//             // Continue for other data types

//             default:
//                 throw new ArgumentException($"Unsupported data type: {dataType}");
//         }
//     }
// }

// public class OscFloat : OscType
// {
//     public override float Value { get; protected set; }

//     public override void FromOscDataHandle(OscDataHandle data)
//     {
//         Value = data.GetElementAsFloat(0);
//     }
// }

// public class OscInt : OscType
// {
//     public int Value;
// }

// public class OscString : OscType
// {
//     public string Value;
// }

// public class OscVector2 : OscType
// {
//     public Vector2 Value;
// }

// public class OscVector3 : OscType
// {
//     public Vector3 Value;
// }

// public class OscVector2Int : OscType
// {
//     public Vector2Int Value;
// }

// public class OscVector3Int : OscType
// {
//     public Vector3Int Value;
// }
