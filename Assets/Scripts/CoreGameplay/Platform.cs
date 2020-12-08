using UnityEngine;

public class Platform : MonoBehaviour
{
    public PlatformMoving movingScript;
    public PlatformColliding collidingScript;
    public TextMesh progressText;
    public MeshRenderer renderer;

    public void initPlatform(Vector3 velocity,
        Vector3 fastVelocity,
        Vector3 posForFastTravel,
        bool fastSpeed,
        ObjectPool pool,
        bool disableScore = false,
        float progress = 0) 
    {
        movingScript.init(velocity, fastVelocity, posForFastTravel, fastSpeed, pool);
        collidingScript.init(velocity, disableScore);
        if(progress == 0) progressText.gameObject.SetActive(false);
        else
        {
            progressText.text = $"{(int) (progress * 100)}%";
            progressText.gameObject.SetActive(true);
        }
    }

    public void setColor(Color c)
    {
        renderer.material.color = c;
    }

    public void setNextPlatform(Transform transform)
    {
        collidingScript.setNextPlatform(transform);
    }
}
