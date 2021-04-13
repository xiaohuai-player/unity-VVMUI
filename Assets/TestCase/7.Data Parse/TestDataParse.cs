using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using VVMUI.Core.Data;

public class Body
{
    public float Height;
    public float Weight;
}

public class BodyData : StructData
{
    public FloatData Height;
    public FloatData Weight;
}

public class Friend
{
    public string Name;
    public int Age;
    public Dictionary<string, float> Scores;
    public Body Body;
}

public class FriendData : StructData
{
    public StringData Name;
    public IntData Age;
    public DictionaryData<FloatData> Scores;
    public BodyData Body;
}

public class People
{
    public string Name;
    public int Age;
    public bool Party;
    public Dictionary<string, float> Scores;
    public Body Body;
    public System.Collections.Generic.List<Friend> Friends;
}

public class PeopleData : StructData
{
    public StringData Name;
    public IntData Age;
    public BoolData Party;
    public DictionaryData<FloatData> Scores;
    public BodyData Body;
    public ListData<FriendData> Friends;
}

public class ListDataBindingTest
{
    public void ParseTestPasses()
    {
        List<People> peoples = new List<People>(){
                new People (){
                    Name = "ceshi001",
                    Age = 6,
                    Party = false,
                    Scores = new Dictionary<string, float>(){
                        { "Language", 70.5f },
                        { "Math", 60.5f },
                        { "Art", 85.5f }
                    },
                    Body = new Body(){
                        Height = 171,
                        Weight = 75
                    },
                    Friends = new List<Friend>(){
                        new Friend () {
                            Name = "friend1",
                            Age = 10,
                            Scores = new Dictionary<string, float>(){
                                { "Language", 30.5f },
                                { "Math", 40.5f },
                                { "Art", 55.5f }
                            },
                            Body = new Body(){
                                Height = 151,
                                Weight = 35
                            }
                        },
                        new Friend () {
                            Name = "friend2",
                            Age = 13,
                            Scores = new Dictionary<string, float>(){
                                { "Language", 42.5f },
                                { "Math", 52.5f },
                                { "Art", 67.5f }
                            },
                            Body = new Body(){
                                Height = 191,
                                Weight = 85
                            }
                        }
                    }
                },
                new People (){
                    Name = "ceshi002",
                    Age = 26,
                    Party = false,
                    Scores = new Dictionary<string, float>(){
                        { "Language", 73.5f },
                        { "Math", 63.5f },
                        { "Art", 82.5f }
                    },
                    Body = new Body(){
                        Height = 131,
                        Weight = 25
                    },
                    Friends = new List<Friend>(){
                    }
                }
            };
        ListData<PeopleData> peoplesData = ListData.Parse<PeopleData>(peoples);
        Assert.That(peoplesData, Is.Not.Null, "ListData parse not null Failed");
        Assert.That(peoplesData.Count, Is.EqualTo(2), "ListData parse count Failed");
        Assert.That(peoplesData[0].Name.Get(), Is.EqualTo("ceshi001"), "Name equals Failed");
        Assert.That(peoplesData[1].Age.Get(), Is.EqualTo(26), "Age equals Failed");
        Assert.That(Mathf.Abs(peoplesData[0].Scores["Language"].Get() - 70.5f) < 0.001f, "Dictionary Failed");
        Assert.That(Mathf.Abs(peoplesData[1].Body.Height.Get() - 131f) < 0.001f, "Struct Failed");
        Assert.That(peoplesData[0].Friends[0].Name.Get(), Is.EqualTo("friend1"), "List Failed");
    }
}