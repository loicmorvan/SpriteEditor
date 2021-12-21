using System.Collections.Generic;
using Xunit;
using static SpriteEditor.Foundation.MathHelper;

namespace SpriteEditor.Tests;

public class MathHelperTests
{
    public static IEnumerable<object[]> Data
    {
        get
        {
            yield return new object[] { -3, 3, 0 };
            yield return new object[] { -2, 3, 1 };
            yield return new object[] { -1, 3, 2 };
            yield return new object[] { 0, 3, 0 };
            yield return new object[] { 1, 3, 1 };
            yield return new object[] { 2, 3, 2 };
            yield return new object[] { 3, 3, 0 };
        }
    }

    [Theory]
    [MemberData(nameof(Data))]
    public void Test(int value, int modulo, int expected)
    {
        var result = Mod(value, modulo);

        Assert.Equal(expected, result);
    }
}
