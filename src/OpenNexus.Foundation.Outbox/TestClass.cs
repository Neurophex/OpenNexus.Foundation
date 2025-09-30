using OpenNexus.Foundation.DDD.Core;
using OpenNexus.Foundation.DDD.Types;
using OpenNexus.Foundation.Primitives;

public class Test : Entity<int>
{
    private DomainProperty<string> _name;

    public Test(int id, string name) : base(id)
    {
        Result<DomainProperty<string>> result = DomainProperty<string>.Create(name, []);
    }
}