using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UnityEventInt : UnityEvent<int>
{
}

[System.Serializable]
public class UnityEventFloat : UnityEvent<float>
{
}

[System.Serializable]
public class UnityEventString : UnityEvent<string>
{
}

[System.Serializable]
public class UnityEventBool : UnityEvent<bool>
{
}

[System.Serializable]
public class UnityEventColor : UnityEvent<Color>
{
}

[System.Serializable]
public class UnityEventColorInt : UnityEvent<Color, int>
{
}

[System.Serializable]
public class UnityEventFloatInt : UnityEvent<float, int>
{
}