using JetBrains.Annotations;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;

namespace Common.Mod.Test.Stubs;

[UsedImplicitly(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Members)]
public class CoreApiStub : ICoreAPI
{
    private readonly string _tempPath;

#pragma warning disable CS8618
    public ILogger Logger { get; }
    public string[] CmdlArguments { get; }
    public IChatCommandApi ChatCommands { get; }
    public EnumAppSide Side { get; }
    public IEventAPI Event { get; }
    public IWorldAccessor World { get; }
    public IClassRegistryAPI ClassRegistry { get; }
    public INetworkAPI Network { get; }
    public IAssetManager Assets { get; }
    public IModLoader ModLoader { get; }
    public ITagRegistry<TagSet> CollectibleTagRegistry { get; }
    public ITagRegistry<TagSetFast> EntityTagRegistry { get; }
    public Dictionary<string, object> ObjectCache { get; }
    public string DataBasePath { get; }

    public CoreApiStub(string tempPath)
    {
        _tempPath = tempPath;
    }
#pragma warning restore CS8618

    public T RegisterRecipeRegistry<T>(string recipeRegistryCode) where T : RecipeRegistryBase
    {
        throw new NotImplementedException();
    }

    public void RegisterColorMap(ColorMap map)
    {
        throw new NotImplementedException();
    }

    public void RegisterEntity(string className, Type entity)
    {
        throw new NotImplementedException();
    }

    public void RegisterEntityBehaviorClass(string className, Type entityBehavior)
    {
        throw new NotImplementedException();
    }

    public void RegisterBlockClass(string className, Type blockType)
    {
        throw new NotImplementedException();
    }

    public void RegisterCropBehavior(string className, Type type)
    {
        throw new NotImplementedException();
    }

    public void RegisterBlockEntityClass(string className, Type blockentityType)
    {
        throw new NotImplementedException();
    }

    public void RegisterItemClass(string className, Type itemType)
    {
        throw new NotImplementedException();
    }

    public void RegisterCollectibleBehaviorClass(string className, Type blockBehaviorType)
    {
        throw new NotImplementedException();
    }

    public void RegisterBlockBehaviorClass(string className, Type blockBehaviorType)
    {
        throw new NotImplementedException();
    }

    public void RegisterBlockEntityBehaviorClass(string className, Type blockEntityBehaviorType)
    {
        throw new NotImplementedException();
    }

    public void RegisterMountable(string className, GetMountableDelegate mountableInstancer)
    {
        throw new NotImplementedException();
    }

    public string GetOrCreateDataPath(string foldername)
    {
        var path = Path.Combine(_tempPath, foldername);

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        return path;
    }

    public void StoreModConfig<T>(T jsonSerializeableData, string filename)
    {
        throw new NotImplementedException();
    }

    public void StoreModConfig(JsonObject jobj, string filename)
    {
        throw new NotImplementedException();
    }

    public T LoadModConfig<T>(string filename)
    {
        throw new NotImplementedException();
    }

    public JsonObject LoadModConfig(string filename)
    {
        throw new NotImplementedException();
    }

    public void RegisterEntityClass(string entityClassName, EntityProperties config)
    {
        throw new NotImplementedException();
    }
}
