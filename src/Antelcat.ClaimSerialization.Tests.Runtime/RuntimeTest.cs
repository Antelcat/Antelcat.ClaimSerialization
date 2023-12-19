﻿using System.Text.Json;

namespace Antelcat.ClaimSerialization.Tests.Runtime;

public class RuntimeTest
{
    [Test]
    public void TestRuntime()
    {
        var claims = ClaimSerializer.Serialize(new IdentityModel
        {
            Roles = new SortedSet<IdentityModel.Role>
            {
                IdentityModel.Role.Admin,
                IdentityModel.Role.User,
                IdentityModel.Role.Guest
            }
        });
        var identity = ClaimSerializer.Deserialize<IdentityModel>(claims.ToList());
    }

    [Test]
    public void TestTypes()
    {
        JsonSerializer.Serialize(new object(),typeof(Type));
    }

    private static bool Predicate(Type type)
    {
        if (type is not { IsClass: true, IsGenericType: true, ContainsGenericParameters: true })
            return false;
        var names = type.Name.Split('`');
        if (names.Length != 2 ||
            !int.TryParse(type.Name.Split('`').Last(), out var count) || count != 1) return false;
        return EnumAllInterfaces(type)
            .Any(t => t.ContainsGenericParameters 
                      && t.GetGenericTypeDefinition() == typeof(ICollection<>));
    }
    
    private static Type[] GetAllInterfaces(Type type)
    {
        var interfaces = type.GetInterfaces();
        var ret        = new HashSet<Type>();
        if (interfaces.Length == 0) return interfaces;
        foreach (var @interface in interfaces.Concat(interfaces.SelectMany(GetAllInterfaces)))
        {
            ret.Add(@interface);
        }

        return ret.ToArray(); 
    }
    private static IEnumerable<Type> EnumAllInterfaces(Type type)
    {
        var interfaces = type.GetInterfaces();
        foreach (var @interface in interfaces.Concat(interfaces.SelectMany(EnumAllInterfaces)))
        {
            yield return @interface;
        }
    }

    [Test]
    [Req(Arg = "")]
    public void TestRef()
    {
        var a = (object)1;

        Func<object?, object?>     c = o => o;
        Console.WriteLine(a);
    }

}

public class ReqAttribute : Attribute
{
    public required string Arg { get; set; }
}
delegate void Setter<T>(ref T target, T value);
