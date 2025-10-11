using Cake.Frosting;
using Common.Build.Core;

namespace Common.Mod.Cake;

public static class Program
{
    public static int Main(string[] args)
    {
        return new CakeHost()
            .UseContext<BuildContext>()
            .Run(args);
    }
}
