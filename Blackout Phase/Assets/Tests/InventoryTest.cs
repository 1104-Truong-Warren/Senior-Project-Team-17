// Inventory Testing - Ellison
using NUnit.Framework;
using UnityEngine;

public class InventoryTest
{
    private Inventory testInventory;
    private Item testItem1;
    private Item testItem2;

    // setup for temporary test inventory manager and test item
    [SetUp]
    public void Setup()
    {
        // need a temp GameObject to hold Inventory for testing
        GameObject tempGameObject = new GameObject();
        testInventory = tempGameObject.AddComponent<Inventory>();

        // temp test items
        // Item is scriptable object so need to make instances of it
        testItem1 = ScriptableObject.CreateInstance<Item>();
        testItem2 = ScriptableObject.CreateInstance<Item>();
        testItem1.itemName = "Test Item 1";
        testItem2.itemName = "Test Item 2";
    }

    // cleanup to destroy temp GameObject
    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(testInventory.gameObject);
    }

    // first test: add to empty inventory
    // inventory should have 1 item and that item is testItem1
    [Test]
    public void AddSingleItemToInventoryTest()
    {
        testInventory.Add(testItem1);

        Assert.AreEqual(1, testInventory.items.Count);
        Assert.Contains(testItem1, testInventory.items);
    }

    // second test: remove item from inventory
    // inventory should have no items
    [Test]
    public void RemoveSingleItemFromInventoryTest()
    {
        testInventory.Add(testItem1);
        Assert.AreEqual(1, testInventory.items.Count);
        Assert.Contains(testItem1, testInventory.items);

        testInventory.Remove(testItem1);
        Assert.AreEqual(0, testInventory.items.Count);
        Assert.IsFalse(testInventory.items.Contains(testItem1));
    }

    // third test: add to a full inventory
    // inventory should still have 6 items and not add the new one
    [Test]
    public void AddItemToFullInventoryTest()
    {
        for (int i = 0; i < 6; i++)
        {
            testInventory.Add(testItem1);
        }
        
        testInventory.Add(testItem2);

        Assert.AreEqual(6, testInventory.items.Count);
        Assert.IsFalse(testInventory.items.Contains(testItem2));
    }


}
