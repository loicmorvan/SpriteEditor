using SpriteEditor.Services;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SpriteEditor.Tests;

public class ImageTests
{
    public static IEnumerable<object[]> Data
    {
        get
        {
            yield return new object[]
            {
                new Image(new byte[] { 0, 1, 2, 3 }, 2, 2),
                -2,
                new Image(new byte[] { 0, 1, 2, 3 }, 2, 2),
            };
            yield return new object[]
            {
                new Image(new byte[] { 0, 1, 2, 3 }, 2, 2),
                -1,
                new Image(new byte[] { 1, 0, 3, 2 }, 2, 2),
            };
            yield return new object[]
            {
                new Image(new byte[] { 0, 1, 2, 3 }, 2, 2),
                0,
                new Image(new byte[] { 0, 1, 2, 3 }, 2, 2),
            };
            yield return new object[]
            {
                new Image(new byte[] { 0, 1, 2, 3 }, 2, 2),
                1,
                new Image(new byte[] { 1, 0, 3, 2 }, 2, 2),
            };
            yield return new object[]
            {
                new Image(new byte[] { 0, 1, 2, 3 }, 2, 2),
                2,
                new Image(new byte[] { 0, 1, 2, 3 }, 2, 2),
            };
        }
    }

    [Theory]
    [MemberData(nameof(Data))]
    public void Test1(Image sut, int translation, Image expected)
    {
        var result = sut.MovePixels(new Vector(translation, 0));

        Assert.Equal(expected, result);
    }

    [Fact]
    public void SplitImageWorks()
    {
        var width = 60;
        var height = 60;
        var columns = 2;
        var rows = 2;
        var sut = new Image(Enumerable.Range(0, width * height).Select(x => (byte)x).ToArray(), width, height);

        var result = sut.Split(columns, rows);

        var splitWidth = width / columns;
        var splitHeight = height / rows;
        var expected = new[]
        {
            new Image(GeneratePixels(0, splitWidth, splitHeight, (uint)width), splitWidth, splitHeight),
            new Image(GeneratePixels(30, splitWidth, splitHeight, (uint)width), splitWidth, splitHeight),
            new Image(GeneratePixels(1800, splitWidth, splitHeight, (uint)width), splitWidth, splitHeight),
            new Image(GeneratePixels(1830, splitWidth, splitHeight, (uint)width), splitWidth, splitHeight)
        };

        Assert.Equal(expected, result);
    }

    private static byte[] GeneratePixels(byte start, int width, int height, uint lineStride)
    {
        var result = new byte[4 * width * height];
        for (int y = 0; y < height; y++)
        {
            var lineStart = start + (uint)y * lineStride;
            for (int x = 0; x < width; x++)
            {
                result[y * width + x] = lineStart;
                lineStart += 1;
            }
        }

        return result;
    }
}
