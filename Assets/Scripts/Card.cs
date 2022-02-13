using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    public string text;
    public string modifierCondition = "";
    public Condition yesCondition;
    public Condition noCondition;
    public Person person;
    public bool mayRepeat;
    public bool used;

    public Card(string text, Condition yesCondition, Condition noCondition)
    {
        used = false;
        this.text = text;
        this.yesCondition = yesCondition;
        this.noCondition = noCondition;
    }

    public Card(string text, Condition yesCondition, Condition noCondition, string modifierCondition)
    {
        used = false;
        this.text = text;
        this.modifierCondition = modifierCondition;
        this.yesCondition = yesCondition;
        this.noCondition = noCondition;
    }

    public Card(string text, Condition yesCondition, Condition noCondition, Person person) : this(text, yesCondition, noCondition)
    {
        used = false;
        this.person = person;
    }

    public Card(string text, Condition yesCondition, Condition noCondition, string modifierCondition, Person person)
    {
        used = false;
        this.text = text;
        this.modifierCondition = modifierCondition;
        this.yesCondition = yesCondition;
        this.noCondition = noCondition;
        this.person = person;
    }

    public Card(string text, Condition yesCondition, Condition noCondition, Person person, bool mayRepeat) : this(text, yesCondition, noCondition, person)
    {
        used = false;
        this.mayRepeat = mayRepeat;
    }

    public Card(string text, Condition yesCondition, Condition noCondition, string modifierCondition, Person person, bool mayRepeat) : this(text, yesCondition, noCondition, modifierCondition, person)
    {
        used = false;
        this.mayRepeat = mayRepeat;
    }


}

public class Condition
{
    public float money;
    public float health;
    public float friends;
    public float income;
    public string modifier;

    public Condition(float money, float health, float friends, float income)
    {
        this.money = money;
        this.health = health;
        this.friends = friends;
        this.income = income;
        this.modifier = "";
    }

    public Condition(float money, float health, float friends, float income, string modifier)
    {
        this.money = money;
        this.health = health;
        this.friends = friends;
        this.income = income;
        this.modifier = modifier;
    }
}

public enum Person
{
    men,
    wife,
    doctor,
    daughter,
    colleague,
    cook,
    judge,
    friend
}