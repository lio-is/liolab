using NUnit.Framework; 

using System.Numerics; 

using UnityEngine; 

using UnityEngine.TestTools; 

 

[TestFixture] 

public class EnemyTests 

{ 

    [Test] 

    public void EnemyStartsWithMaxHealth() 

    { 

        // Arrange 

        GameObject enemyObject = new GameObject(); 

        Enemy enemy = enemyObject.AddComponent<Enemy>(); 

 

        // Act 

 

        // Assert 

        Assert.AreEqual(enemy.MaxHitPoint, enemy.HitPoint); 

    } 

 

    [Test] 

    public void EnemyDeathGrantsXPAndShowsUI() 

    { 

        // Arrange 

        GameObject enemyObject = new GameObject(); 

        Enemy enemy = enemyObject.AddComponent<Enemy>(); 

        int initialXP = GameManager.instance.PlayerXP; 

 

        // Act 

        enemy.Death(); 

 

        // Assert 

        Assert.AreEqual(initialXP + enemy.XPValue, GameManager.instance.PlayerXP); 

        // You may want to add assertions to check if ShowText method is called or not 

    } 

 

    [Test] 

    public void EnemyCanRespawnAfterWaitingTime() 

    { 

        // Arrange 

        GameObject enemyObject = new GameObject(); 

        Enemy enemy = enemyObject.AddComponent<Enemy>(); 

        enemy.CanRespawn = true; 

        enemy.TimeToRespawn = 1f; 

 

        // Act 

        enemy.Death(); 

        Assert.IsFalse(enemy.IsAlive); // Enemy should be dead initially 

 

        // Wait for respawn time 

        WaitForSeconds wait = new WaitForSeconds(2f); 

        yield return wait; 

 

        // Assert 

        Assert.IsTrue(enemy.IsAlive); // Enemy should have respawned 

    } 

 

    [Test] 

    public void EnemyCannotRespawnIfFlagIsFalse() 

    { 

        // Arrange 

        GameObject enemyObject = new GameObject(); 

        Enemy enemy = enemyObject.AddComponent<Enemy>(); 

        enemy.CanRespawn = false; 

 

        // Act 

        enemy.Death(); 

 

        // Assert 

        Assert.IsFalse(enemy.IsAlive); // Enemy should be dead, not respawned 

    } 

 

    [Test] 

    public void EnemyChasesPlayerWithinTriggerLength() 

    { 

        // Arrange 

        GameObject enemyObject = new GameObject(); 

        Enemy enemy = enemyObject.AddComponent<Enemy>(); 

        GameObject playerObject = new GameObject("Player"); 

        playerObject.tag = "Fighter"; 

        playerObject.AddComponent<Player>(); 

        playerObject.transform.position = new Vector3(1, 0, 0); // Set player close to enemy 

        enemy.ChaseLength = 2f; 

        enemy.TriggerLength = 1f; 

 

        // Act 

        enemy.Update(); // Simulate the Update cycle 

 

        // Assert 

        Assert.IsTrue(enemy.IsChasing); 

    } 

 

    [Test] 

    public void EnemyStopsChasingOutsideChaseLength() 

    { 

        // Arrange 

        GameObject enemyObject = new GameObject(); 

        Enemy enemy = enemyObject.AddComponent<Enemy>(); 

        GameObject playerObject = new GameObject("Player"); 

        playerObject.tag = "Fighter"; 

        playerObject.AddComponent<Player>(); 

        playerObject.transform.position = new Vector3(3, 0, 0); // Set player far from enemy 

        enemy.ChaseLength = 2f; 

 

        // Act 

        enemy.Update(); // Simulate the Update cycle 

 

        // Assert 

        Assert.IsFalse(enemy.IsChasing); 

    } 

} 