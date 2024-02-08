using System;

namespace Benchmark.Models.DataTransferObjects;

public sealed class TestObjectModificationDto
{
    public long Id { get; set; }

    public string Type { get; set; }

    public DateTime Date { get; set; }

    public string Author { get; set; }
}