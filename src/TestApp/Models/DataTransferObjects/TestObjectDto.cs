﻿namespace TestApp.Models.DataTransferObjects;

public sealed class TestObjectDto
{
    public long Id { get; set; }
    public bool Bool { get; set; }
    public byte Byte { get; set; }
    public char Char { get; set; }
    public DateTime DateTime { get; set; }
    public decimal Decimal { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public float Float { get; set; }
    public int Int { get; set; }
    public string LastName { get; set; }
    public long Long { get; set; }
    public string Nickname { get; set; }
    public short Short { get; set; }
    public List<TestObjectFieldDto>? Fields { get; set; }
    public List<TestObjectModificationDto> Modifications { get; set; }
    public List<TestObjectLinkDto> Links { get; set; }
    public TestObjectDto Child { get; set; }
}