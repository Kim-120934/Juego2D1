using UnityEngine;

public class DestructibleWall : MonoBehaviour
{
   
    public void DestroyWall()
    {
        AudioManager.instance.PlaySFX(AudioManager.instance.wallBreakSFX);
        Destroy(gameObject);
    }
}

