using NUnit.Framework; 

using System.Numerics; 

using UnityEngine; 

using UnityEngine.TestTools; 

using static System.Net.Mime.MediaTypeNames; 

 

[TestFixture] 

public class PlayerTests 

{ 

    [Test] 

    public void PlayerStartsWithMaxHealth() 

    { 

        // Arrange 

        GameObject playerObject = new GameObject(); 

        Player player = playerObject.AddComponent<Player>(); 

 

        // Act 

 

        // Assert 

        Assert.AreEqual(player.MaxHitPoint, player.HitPoint); 

    } 

 

    [Test] 

    public void PlayerRespawnRestoresHealthAndSetsAlive() 

    { 

        // Arrange 

        GameObject playerObject = new GameObject(); 

        Player player = playerObject.AddComponent<Player>(); 

        player.HitPoint = 0; 

        player.IsAlive = false; 

 

        // Act 

        player.Respawn(); 

 

        // Assert 

        Assert.AreEqual(player.MaxHitPoint, player.HitPoint); 

        Assert.IsTrue(player.IsAlive); 

    } 

 

    [Test] 

    public void PlayerDeathSetsAliveToFalseAndRotates() 

    { 

        // Arrange 

        GameObject playerObject = new GameObject(); 

        Player player = playerObject.AddComponent<Player>(); 

 

        // Act 

        player.Death(); 

 

        // Assert 

        Assert.IsFalse(player.IsAlive); 

        Assert.AreEqual(new Vector3(0, 0, 90), playerObject.transform.localEulerAngles); 

    } 

 

    [Test] 

    public void PlayerOnLevelUpIncreasesMaxHealth() 

    { 

        // Arrange 

        GameObject playerObject = new GameObject(); 

        Player player = playerObject.AddComponent<Player>(); 

        int initialMaxHealth = player.MaxHitPoint; 

 

        // Act 

        player.OnLevelUp(); 

 

        // Assert 

        Assert.AreEqual(initialMaxHealth + 10, player.MaxHitPoint); 

    } 

 

    [Test] 

    public void PlayerReceiveDamageReducesHealthAndIncreasesRage() 

    { 

        // Arrange 

        GameObject playerObject = new GameObject(); 

        Player player = playerObject.AddComponent<Player>(); 

        Damage damage = new Damage { damageAmount = 10, origin = Vector3.zero, pushForce = 5 }; 

 

        // Act 

        player.ReceiveDamage(damage); 

 

        // Assert 

        Assert.AreEqual(player.MaxHitPoint - 10, player.HitPoint); 

        Assert.AreEqual(10, player.Rage); 

    } 

} 