using NUnit.Framework; 

using System.Numerics; 

using UnityEngine; 

using static System.Net.Mime.MediaTypeNames; 

 

[TestFixture] 

public class FighterTests 

{ 

    [Test] 

    public void FighterStartsWithMaxHealth() 

    { 

        // Arrange 

        GameObject fighterObject = new GameObject(); 

        Fighter fighter = fighterObject.AddComponent<Fighter>(); 

 

        // Act 

 

        // Assert 

        Assert.AreEqual(fighter.MaxHitPoint, fighter.HitPoint); 

    } 

 

    [Test] 

    public void FighterTakesDamageReducesHealth() 

    { 

        // Arrange 

        GameObject fighterObject = new GameObject(); 

        Fighter fighter = fighterObject.AddComponent<Fighter>(); 

        Damage damage = new Damage { damageAmount = 5, origin = Vector3.zero, pushForce = 2 }; 

 

        // Act 

        fighter.ReceiveDamage(damage); 

 

        // Assert 

        Assert.AreEqual(fighter.MaxHitPoint - 5, fighter.HitPoint); 

    } 

 

    [Test] 

    public void FighterImmuneTimePreventsMultipleHits() 

    { 

        // Arrange 

        GameObject fighterObject = new GameObject(); 

        Fighter fighter = fighterObject.AddComponent<Fighter>(); 

        Damage damage = new Damage { damageAmount = 5, origin = Vector3.zero, pushForce = 2 }; 

 

        // Act 

        fighter.ReceiveDamage(damage); 

        float initialImmuneTime = fighter.LastImmune; 

        fighter.ReceiveDamage(damage); 

 

        // Assert 

        Assert.AreEqual(initialImmuneTime, fighter.LastImmune); 

        Assert.AreEqual(fighter.MaxHitPoint - 5, fighter.HitPoint); 

    } 

 

    [Test] 

    public void FighterDeathCallsDeathMethod() 

    { 

        // Arrange 

        GameObject fighterObject = new GameObject(); 

        Fighter fighter = fighterObject.AddComponent<Fighter>(); 

        bool deathMethodCalled = false; 

        fighter.DeathAction = () => deathMethodCalled = true; 

 

        // Act 

        fighter.Death(); 

 

        // Assert 

        Assert.IsTrue(deathMethodCalled); 

    } 

} 