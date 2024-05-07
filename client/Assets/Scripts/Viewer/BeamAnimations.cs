using UnityEngine;

public class BeamAnimations : MonoBehaviour
{
    public void Blink(float time)
    {
        gameObject.SetActive(true);
        Invoke(nameof(Hide), time);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}