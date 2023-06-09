﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace LogProcessor.Tests;

public class LogFieldsUnitTests : TestWithStandardOutput
{
    public LogFieldsUnitTests(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public void CreatingFieldsObject()
    {
        var fields = LogFields.Of(new List<string> { "zero", "one", "two" });

        Assert.Equal(0, fields["zero"]);
        Assert.Equal(1, fields["one"]);
        Assert.Equal(2, fields["two"]);
        Assert.Equal(new List<string> { "zero", "one", "two" }, fields.Fields);

    }
    [Fact]
    public void CreatingFieldsWithNonDistinctNames()
    {
        // act
        static void createLogFields() =>
            LogFields.Of(new List<string> { "REPEAT", "REPEAT", "two" });

        // assert
        ArgumentException exception = Assert.Throws<ArgumentException>(createLogFields);
        Assert.Equal("[LogFields::Of][ERROR] List of field names has non-unique elements!", exception.Message);
    }
}
