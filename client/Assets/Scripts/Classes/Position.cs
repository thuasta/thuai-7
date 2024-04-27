using Newtonsoft.Json.Linq;
using Thubg.Messages;
using UnityEngine;

public class Position
{

    public Position(float xValue, float yValue)
    {
        x = xValue; y = yValue;
    }

    public float x { get; }
    public float y { get; }
}
