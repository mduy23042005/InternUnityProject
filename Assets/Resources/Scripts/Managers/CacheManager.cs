using System.Collections.Generic;

public class CacheManager
{
    // idClazz -> (nameClazz, appearance)
    public static Dictionary<int, Clazzes> clazzes = new Dictionary<int, Clazzes>();
    // idCharacter -> (nameCharacter, appearance, level, clazz)
    public static Dictionary<int, Characters> characters = new Dictionary<int, Characters>();
}