using UnityEngine;

public class Rifle : MonoBehaviour
{


    public int damage;
    public int range;

    public int Attack(int distanceToTarget)
    {
        if (distanceToTarget <= range)
        {
            return damage;
        } 
        else
        {
            return 0;
        }
        
    }

    void Start()
    {
        
    }

    void Update()
    {
    
    }
}
