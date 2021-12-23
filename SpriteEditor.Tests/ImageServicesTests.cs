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
                new Image(new uint[] { 0, 1, 2, 3 }, 2, 2),
                -2,
                new Image(new uint[] { 0, 1, 2, 3 }, 2, 2),
            };
            yield return new object[]
            {
                new Image(new uint[] { 0, 1, 2, 3 }, 2, 2),
                -1,
                new Image(new uint[] { 1, 0, 3, 2 }, 2, 2),
            };
            yield return new object[]
            {
                new Image(new uint[] { 0, 1, 2, 3 }, 2, 2),
                0,
                new Image(new uint[] { 0, 1, 2, 3 }, 2, 2),
            };
            yield return new object[]
            {
                new Image(new uint[] { 0, 1, 2, 3 }, 2, 2),
                1,
                new Image(new uint[] { 1, 0, 3, 2 }, 2, 2),
            };
            yield return new object[]
            {
                new Image(new uint[] { 0, 1, 2, 3 }, 2, 2),
                2,
                new Image(new uint[] { 0, 1, 2, 3 }, 2, 2),
            };
        }
    }

    [Theory]
    [MemberData(nameof(Data))]
    public void Test1(Image input, int translation, Image expected)
    {
        var sut = new ImageServices();

        var result = sut.MovePixels(new Vector(translation, 0), input);

        Assert.Equal(expected, result);
    }
}
