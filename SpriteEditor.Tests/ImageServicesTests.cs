using SpriteEditor.Services;
using System.Collections.Generic;
using Xunit;

namespace SpriteEditor.Tests;

public class ImageServicesTests
{
    public static IEnumerable<object[]> Data
    {
        get
        {
            yield return new object[]
            {
                new uint[] { 0, 1, 2, 3 },
                -4,
                new uint[] { 0, 1, 2, 3 }
            };
            yield return new object[]
            {
                new uint[] { 0, 1, 2, 3 },
                -3,
                new uint[] { 3, 0, 1, 2 }
            };
            yield return new object[]
            {
                new uint[] { 0, 1, 2, 3 },
                -2,
                new uint[] { 2, 3, 0, 1 }
            };
            yield return new object[]
            {
                new uint[] { 0, 1, 2, 3 },
                -1,
                new uint[] { 1, 2, 3, 0 }
            };
            yield return new object[]
            {
                new uint[] { 0, 1, 2, 3 },
                0,
                new uint[] { 0, 1, 2, 3 }
            };
            yield return new object[]
            {
                new uint[] { 0, 1, 2, 3 },
                1,
                new uint[] { 3, 0, 1, 2 }
            };
            yield return new object[]
            {
                new uint[] { 0, 1, 2, 3 },
                2,
                new uint[] { 2, 3, 0, 1 }
            };
            yield return new object[]
            {
                new uint[] { 0, 1, 2, 3 },
                3,
                new uint[] { 1, 2, 3, 0 }
            };
            yield return new object[]
            {
                new uint[] { 0, 1, 2, 3 },
                4,
                new uint[] { 0, 1, 2, 3 }
            };
        }
    }

    [Theory]
    [MemberData(nameof(Data))]
    public void Test1(uint[] input, int translation, uint[] expected)
    {
        var sut = new ImageServices();

        var result = sut.MovePixelsHorizontally(translation, input);

        Assert.Equal(expected, result);
    }
}
