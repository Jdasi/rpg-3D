using System;

[Serializable]
public struct ModelConfig
{
    public TokenModel Prefab;

    // where the TokenModel is generic, any
    // modifications to the model should be here
    // e.g. skin color, clothing swaps, etc.
}
