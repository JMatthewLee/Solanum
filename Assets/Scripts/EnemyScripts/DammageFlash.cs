using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DammageFlash : MonoBehaviour
{
    //https://www.youtube.com/watch?v=rq6yGh-piIU&ab_channel=SasquatchBStudios

    [ColorUsage(true,true)] //true to show alpha and true to show hdr
    [SerializeField] private Color hitFlashColour;
    [SerializeField] private float hitFlashTime;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Material flashMaterial;

    private Material instanceFlashMaterial;
    private Coroutine HitFlashCoroutine;

    private void Awake()
    {
        // Get the SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Create an instance of the flash material
        instanceFlashMaterial = new Material(flashMaterial);

        // Assign the custom flash material to the spriteRenderer
        spriteRenderer.material = instanceFlashMaterial;
    }

    public void CallHitFlash()
    {
        if (HitFlashCoroutine != null)
        {
            StopCoroutine(HitFlashCoroutine);
        }
        HitFlashCoroutine = StartCoroutine(HitFlash());
    }

    private IEnumerator HitFlash()
    {
        instanceFlashMaterial.SetColor("_FlashColour", hitFlashColour); // Set flash color

        float currentFlashAmount = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < hitFlashTime)
        {
            elapsedTime += Time.deltaTime;

            // Lerp flash amount from 1 to 0 over the duration of hitFlashTime
            currentFlashAmount = Mathf.Lerp(1f, 0f, elapsedTime / hitFlashTime);

            instanceFlashMaterial.SetFloat("_FlashAmount", currentFlashAmount);

            yield return null;
        }
    }

}